using Android.App;
using ANotificationListenerService = Android.Service.Notification.NotificationListenerService;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Glyphy.Animation;
using Glyphy.Configuration;
using Android.OS;
using Android.Content;
using System.Diagnostics;
using Android;
using FlagFilterType = Android.Service.Notification.FlagFilterType;
using Android.Runtime;

//https://developer.android.com/reference/android/service/notification/NotificationListenerService
namespace Glyphy.Platforms.Android.Services
{
    //Have notification animations be disabled while the app is focused.
    [Service(
        Label = "Notification Lighting Service",
        Exported = false,
        Permission = Manifest.Permission.BindNotificationListenerService
    )]
    [IntentFilter(new string[] { "android.service.notification.NotificationListenerService" })]
    //Set the required meta-data to signal that we only want to receive notifications that appear to the user (otherwise we get all notifications such as when the device rotation is changed).
    [MetaData(name: MetaDataDefaultFilterTypes, Value = nameof(FlagFilterType.Conversations) + "|" + nameof(FlagFilterType.Alerting))]
    [MetaData(name: MetaDataDisabledFilterTypes, Value = nameof(FlagFilterType.Ongoing) + "|" + nameof(FlagFilterType.Silent))]
    public class NotificationLightingService : ANotificationListenerService
    {
        public const string DEFAULT_KEY = "default";

        public static NotificationLightingService? Instance { get; private set; } = null!;

        public static bool IsRunning => Instance is not null;

        //These won't be null during a running instance.
        private PowerManager powerManager = null!;
        private NotificationManager notificationManager = null!;

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent? intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            //Signal that if the system kills the service, it should be restarted.
            return StartCommandResult.Sticky;

        }

        public override void OnListenerConnected()
        {
            base.OnListenerConnected();

            if (ApplicationContext is null)
                throw new NullReferenceException("ApplicationContext is null.");

            if (ApplicationContext.GetSystemService(PowerService) is PowerManager _powerManager)
                powerManager = _powerManager;
            else
                throw new NullReferenceException("PowerManager is null.");

            if (ApplicationContext.GetSystemService(NotificationService) is NotificationManager _notificationManager)
                notificationManager = _notificationManager;
            else
                throw new NullReferenceException("NotificationManager is null.");

            Instance = this;
        }

        public override void OnListenerDisconnected()
        {
            Instance = null;

#if ANDROID24_0_OR_GREATER
            base.OnListenerDisconnected();
#endif

            powerManager = null!;
            notificationManager = null!;
        }

        //This won't be called before OnListenerConnected so we don't need to null check the managers.
        public override void OnNotificationPosted(global::Android.Service.Notification.StatusBarNotification? sbn)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            base.OnNotificationPosted(sbn);

            Task.Run(async () =>
            {
                SSettings cachedSettings = await Storage.GetCachedSettings();

                //I am building for Android 12.0+ so I don't need to validate the platform compatibility here.
#pragma warning disable CA1416 // Validate platform compatibility
                //TODO: Ignore notifications from this application (as for now I will only be posting the long-running foreground service notification).
                if (sbn is null
                    || (powerManager.IsPowerSaveMode && !cachedSettings.IgnorePowerSaverMode) //Power saver mode.
                    || (notificationManager.CurrentInterruptionFilter == InterruptionFilter.Priority && !cachedSettings.IgnoreDoNotDisturb) //Do not disturb.
                    || (sbn.Notification?.Flags & NotificationFlags.Insistent) != 0) //Is notification is set to be silent.
                    return;
#pragma warning restore CA1416

                try
                {
                    IReadOnlyDictionary<string, Guid> cachedNotificationConfiguration = await Storage.GetCachedNotificationServiceConfiguration();

                    Guid cachedAnimationID;
                    if (sbn?.PackageName is not null && cachedNotificationConfiguration.ContainsKey(sbn.PackageName))
                        cachedAnimationID = cachedNotificationConfiguration[sbn.PackageName];
                    else if (cachedNotificationConfiguration.ContainsKey(DEFAULT_KEY))
                        cachedAnimationID = cachedNotificationConfiguration[DEFAULT_KEY];
                    else
                        return;

                    SAnimation? animation = await Storage.LoadAnimation(cachedAnimationID);
                    if (animation is null)
                        return;

                    //Wait for x milliseconds since the stopwatch started as it seems this event fires much faster than tho notification sound does.
                    int timeToWait = 500 - (int)stopwatch.ElapsedMilliseconds;
                    if (timeToWait > 0)
                        await Task.Delay(timeToWait);

                    if (AnimationRunner.GetQueuedInterrupts > 0)
                        return;

                    await AnimationRunner.AddInterruptAnimation(animation.Value);
                }
                catch {}
            });
        }
    }
}
