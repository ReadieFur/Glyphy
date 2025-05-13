using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using AIntentService = Android.App.IntentService;

namespace Glyphy.Platforms.Android.Services
{
    [Service(
        Label = "Glyphy Ambient Service",
        Exported = false
    )]
    public class AmbientLightingService : AIntentService
    {
        private PowerManager? _powerManager = null;
        private NotificationManager? _notificationManager = null;
        private CancellationTokenSource? _cancellationTokenSource = null;
        private Thread? _serviceThread = null;

        //Apparently this is set to be obsolete in the future? (And so OnStartCommand should be used instead), but it's not deprecated yet.
        protected override void OnHandleIntent(Intent? intent) { }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent? intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            if (ApplicationContext is null
                || ApplicationContext.GetSystemService(PowerService) is not PowerManager powerManager
                || ApplicationContext.GetSystemService(NotificationService) is not NotificationManager notificationManager)
                throw new NullReferenceException();
            _powerManager = powerManager;
            _notificationManager = notificationManager;

            _cancellationTokenSource = new();
            _serviceThread = new(ServiceWorker);
            _serviceThread.Start(this);

            return StartCommandResult.Sticky;
        }

        public override void OnDestroy()
        {
            _cancellationTokenSource?.Cancel();
            _serviceThread?.Join();
        }

        private static void ServiceWorker(object? param)
        {
            if (param is not AmbientLightingService self
                || self._cancellationTokenSource is null)
                throw new NullReferenceException();

            CancellationToken ct = self._cancellationTokenSource.Token;

            while (ct.IsCancellationRequested)
            {
                //TODO.
                ct.WaitHandle.WaitOne(Timeout.Infinite);
            }
        }
    }
}
