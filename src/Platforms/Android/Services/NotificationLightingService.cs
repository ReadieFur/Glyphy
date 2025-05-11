using Android.App;
using ANotificationListenerService = Android.Service.Notification.NotificationListenerService;
using Android.Content;
using Android;
using FlagFilterType = Android.Service.Notification.FlagFilterType;

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
    }
}
