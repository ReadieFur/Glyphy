using Android.App;
using ANotificationListenerService = Android.Service.Notification.NotificationListenerService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glyphy.Animation;
using Glyphy.Configuration;
using Android.OS;
using Android.Content;
using System.Diagnostics;

//https://developer.android.com/reference/android/service/notification/NotificationListenerService
namespace Glyphy.Platforms.Android
{
    //Have notification animations be disabled while the app is focused.
    [Service(
        Label = "Notification Listener Service",
        Exported = false,
        Permission = "android.permission.BIND_NOTIFICATION_LISTENER_SERVICE"
    )]
    [IntentFilter(new string[] { "android.service.notification.NotificationListenerService" })]
    public class NotificationListenerService : ANotificationListenerService
    {
        public const string DEFAULT_KEY = "default";

        public static NotificationListenerService? Instance { get; private set; } = null!;

        public static bool IsRunning => Instance is not null;

        //These won't be null during a running instance.
        private PowerManager powerManager = null!;
        private NotificationManager notificationManager = null!;

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

            base.OnListenerDisconnected();

            powerManager = null!;
            notificationManager = null!;
        }

        //This won't be called before OnListenerConnected so we don't need to null check the managers.
        public override void OnNotificationPosted(global::Android.Service.Notification.StatusBarNotification? sbn)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            base.OnNotificationPosted(sbn);

#if DEBUG && true
            Task.Run(async () =>
            {
                SSettings cachedSettings = await Storage.GetCachedSettings();

                //I am building for Android 12.0+ so I don't need to validate the platform compatibility here.
#pragma warning disable CA1416 // Validate platform compatibility
                //Make this an option that the user can disable.
                if ((powerManager.IsPowerSaveMode && !cachedSettings.IgnorePowerSaverMode)
                    || (notificationManager.CurrentInterruptionFilter == InterruptionFilter.Priority && !cachedSettings.IgnoreDoNotDisturb))
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

                    if (AnimationRunner.IsRunning)
                        return;

                    await AnimationRunner.StartAnimation(animation.Value);
                }
                catch {}
            });
#endif
        }
    }
}
