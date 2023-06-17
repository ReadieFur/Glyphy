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
        PowerManager powerManager = null!;
        NotificationManager notificationManager = null!;

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

        //This won't be called before OnListenerConnected so we don't need to null check the managers.
        public override void OnNotificationPosted(global::Android.Service.Notification.StatusBarNotification? sbn)
        {
            base.OnNotificationPosted(sbn);

            //I am building for Android 12.0+ so I don't need to validate the platform compatibility here.
#pragma warning disable CA1416 // Validate platform compatibility
            if (powerManager.IsPowerSaveMode || notificationManager.CurrentInterruptionFilter == InterruptionFilter.Priority)
                return;
#pragma warning restore CA1416

#if DEBUG && true
            //TESTING:
            try
            {
                if (!AnimationRunner.IsRunning)
                    AnimationRunner.StartAnimation(Glyphy.Resources.Presets.Glyphs.GUIRO);
            }
            catch
            {
                //This try/catch/rethrow is just for me to remember to structure it like this for production.
                throw;
            }
#endif
        }
    }
}
