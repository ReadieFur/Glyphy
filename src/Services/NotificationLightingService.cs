#define ADD_DELAY //This can sometimes be useful as the notification event can fire before the notification sound plays. TODO: Find a more reliable, dynamic, way to do this.

using Android.App;
using ANotificationListenerService = Android.Service.Notification.NotificationListenerService;
using System;
using System.Threading.Tasks;
using Glyphy.Animation;
using Glyphy.Configuration;
using Android.OS;
using Android.Content;
using System.Diagnostics;
using Android;
using FlagFilterType = Android.Service.Notification.FlagFilterType;
using Android.Runtime;
using Glyphy.Configuration.NotificationConfiguration;

//https://developer.android.com/reference/android/service/notification/NotificationListenerService
namespace Glyphy.Services
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

        //These shouldn't be null during an active instance.
        private PowerManager? powerManager = null;
        private NotificationManager? notificationManager = null;

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
        }

        public override void OnListenerDisconnected()
        {
#if ANDROID24_0_OR_GREATER
            base.OnListenerDisconnected();
#endif

            powerManager = null!;
            notificationManager = null!;
        }

        //This won't be called before OnListenerConnected so we don't need to null check the managers.
        public override void OnNotificationPosted(Android.Service.Notification.StatusBarNotification? sbn)
        {
#if ADD_DELAY
            Stopwatch stopwatch = Stopwatch.StartNew();
#endif

            base.OnNotificationPosted(sbn);

            Task.Run(async () =>
            {
                SSettings cachedSettings = await Storage.Settings.GetCached();

                //I am building for Android 12.0+ so I don't need to validate the platform compatibility here.
#pragma warning disable CA1416 // Validate platform compatibility
                //TODO: Ignore notifications from this application (as for now I will only be posting the long-running foreground service notification).
                if (sbn is null
                    || powerManager is null
                    || notificationManager is null
                    || powerManager.IsPowerSaveMode && !cachedSettings.IgnorePowerSaverMode //Power saver mode.
                    || notificationManager.CurrentInterruptionFilter == InterruptionFilter.Priority && !cachedSettings.IgnoreDoNotDisturb //Do not disturb.
                    || (sbn.Notification?.Flags & NotificationFlags.Insistent) != 0) //Is notification is set to be silent.
                    return;
#pragma warning restore CA1416

                try
                {
                    SNotificationConfigurationRoot notificationServiceConfiguration = await Storage.NotificationServiceSettings.GetCached();

                    Guid cachedAnimationID;
                    if (sbn?.PackageName is not null && notificationServiceConfiguration.Configuration.ContainsKey(sbn.PackageName))
                        cachedAnimationID = notificationServiceConfiguration.Configuration[sbn.PackageName].AnimationID;
                    else if (notificationServiceConfiguration.Configuration.ContainsKey(DEFAULT_KEY))
                        cachedAnimationID = notificationServiceConfiguration.Configuration[DEFAULT_KEY].AnimationID;
                    else
                        return;

                    SAnimation? animation = await Storage.LoadAnimation(cachedAnimationID);
                    if (animation is null)
                        return;

#if ADD_DELAY
                    //Wait for x milliseconds since the stopwatch started as it seems this event fires much faster than tho notification sound does.
                    int timeToWait = 500 - (int)stopwatch.ElapsedMilliseconds;
                    if (timeToWait > 0)
                        await Task.Delay(timeToWait);
#endif

                    if (AnimationRunner.GetQueuedInterrupts > 0)
                        return;

                    await AnimationRunner.AddInterruptAnimation(animation.Value);
                }
                catch {}
            });
        }
    }
}
