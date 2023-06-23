using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Glyphy.Animation;
using Java.Util.Concurrent;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Glyphy.Platforms.Android
{
    //Do I even need this to be a foreground service as I don't need it to be realtime and I can set the option to keep it alive. I'd have to test this over time though in the future.
    //https://betterprogramming.pub/what-is-foreground-service-in-android-3487d9719ab6
    [Service(
        Label = "Glyphy Ambient Service",
        Exported = false
    )]
    public class ForegroundService : Service
    {
        public static void StopService()
        {
            throw new NotImplementedException();
        }

        private CancellationTokenSource? cancellationTokenSource;
        private Task? longRunningTask;

        public override IBinder? OnBind(Intent? intent) =>
            null;

        public override StartCommandResult OnStartCommand(Intent? intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            //Create a notification for the foreground service (keeps the foreground service active and responsive).
#pragma warning disable CA1416 // Validate platform compatibility
            var notification = new Notification.Builder(this, "glyphy")
                .SetContentTitle("Glyphy Ambient Service")
                .SetContentText("Glyphy ambient service is running.")
                .SetSmallIcon(Resource.Drawable.ic_mtrl_checked_circle) //TODO: Change icon.
                .SetVisibility(NotificationVisibility.Secret)
                .Build();
#pragma warning restore CA1416 // Validate platform compatibility
            
            cancellationTokenSource = new();
            longRunningTask = RunService();

            //Start the service in the foreground.
            StartForeground(1, notification);

            //Return the appropriate StartCommandResult value.
            return StartCommandResult.Sticky;
        }

        public override void OnDestroy()
        {
            cancellationTokenSource?.Cancel();

            //Stop the foreground service.
            StopForeground(true);

            if (longRunningTask is not null)
                longRunningTask = null;

            base.OnDestroy();
        }

        private Task RunService()
        {
            return Task.Run(async () =>
            {
                while (true)
                {
                    if (cancellationTokenSource is null || cancellationTokenSource.IsCancellationRequested)
                        break;

                    try
                    {
                        await AnimationRunner.StartAnimation(Glyphy.Resources.Presets.Glyphs.SQUIGGLE, cancellationTokenSource.Token);
                        await AnimationRunner.WaitForCompletion(cancellationTokenSource.Token);
                        await Task.Delay(1000, cancellationTokenSource.Token);
                    }
                    catch (CancellationException)
                    {
                        await AnimationRunner.StopAnimation();
                    }
                    catch {}
                }
            });
        }
    }
}
