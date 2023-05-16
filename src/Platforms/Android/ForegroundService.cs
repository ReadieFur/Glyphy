using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glyphy.Platforms.Android
{
    [Service]
    internal class ForegroundService : Service
    {
        public override IBinder? OnBind(Intent? intent)
        {
            return null;
        }

        public override StartCommandResult OnStartCommand(Intent? intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            // Create a notification for the foreground service
            var notification = new Notification.Builder(this, "glyphy")
                .SetContentTitle("Glyphy")
                .SetContentText("Glyphy is running.")
                .SetSmallIcon(Resource.Drawable.ic_mtrl_checked_circle)
                .SetVisibility(NotificationVisibility.Secret)
                .Build();

            // Start the service in the foreground
            StartForeground(1, notification);

            // Return the appropriate StartCommandResult value
            return StartCommandResult.Sticky;
        }

        public override void OnDestroy()
        {
            // Stop the foreground service
            StopForeground(true);

            base.OnDestroy();
        }
    }
}
