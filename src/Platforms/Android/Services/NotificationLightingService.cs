using Android.App;
using ANotificationListenerService = Android.Service.Notification.NotificationListenerService;
using Android.Content;
using Android;
using FlagFilterType = Android.Service.Notification.FlagFilterType;
using Android.Runtime;
using Android.OS;
using Glyphy.Animation;

namespace Glyphy.Platforms.Android.Services
{
    [Service(
        Label = "Notification Lighting Service",
        Exported = false,
        Permission = Manifest.Permission.BindNotificationListenerService
    )]
    [IntentFilter(["android.service.notification.NotificationListenerService"])]
    //Set the required meta-data to signal that we only want to receive notifications that appear to the user (otherwise we get all notifications such as when the device rotation is changed).
    [MetaData(name: MetaDataDefaultFilterTypes, Value = nameof(FlagFilterType.Conversations) + "|" + nameof(FlagFilterType.Alerting))]
    [MetaData(name: MetaDataDisabledFilterTypes, Value = nameof(FlagFilterType.Ongoing) + "|" + nameof(FlagFilterType.Silent))]
    public class NotificationLightingService : ANotificationListenerService
    {
        //These shouldn't be null during an active instance.
        private PowerManager? _powerManager = null;
        private NotificationManager? _notificationManager = null;

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent? intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            //Signal that if the system kills the service, it should be restarted.
            return StartCommandResult.Sticky;
        }

        public override void OnListenerConnected()
        {
            if (ApplicationContext is null
                || ApplicationContext.GetSystemService(PowerService) is not PowerManager powerManager
                || ApplicationContext.GetSystemService(NotificationService) is not NotificationManager notificationManager)
                throw new NullReferenceException();
            _powerManager = powerManager;
            _notificationManager = notificationManager;
        }

        public override void OnListenerDisconnected()
        {
            _powerManager = null;
            _notificationManager = null;
        }

        public override void OnNotificationPosted(global::Android.Service.Notification.StatusBarNotification? sbn)
        {
            //Stopwatch stopwatch = Stopwatch.StartNew();

            if ((sbn!.Notification?.Flags & NotificationFlags.Insistent) != 0) //Is notification is set to be silent.
                return;

            /** Notification Glyph delay:
             * This can sometimes be useful as the notification event can fire before the notification sound plays.
             * TODO: Find a more reliable, dynamic, way to do this.
             * Wait for 500ms (seems like a good delay time in testing).
             */
            /*int timeToWait = 500 - (int)stopwatch.ElapsedMilliseconds;
            if (timeToWait > 0)
                Task.Delay(timeToWait).GetAwaiter().GetResult();*/

            //TODO: Get animation to be played.
            //AnimationRunner.Instance.PlayAnimation();
        }
    }
}
