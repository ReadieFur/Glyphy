//#define EXPOSE_UPDATE_EVENT

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Glyphy.Animation;
using Glyphy.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Glyphy.Platforms.Android.Services
{
    [Service(
        Label = "Glyphy Ambient Service",
        Exported = false
    )]
    public class AmbientLightingService : IntentService
    {
#if EXPOSE_UPDATE_EVENT
        private static ManualResetEventSlim configurationUpdatedResetEvent = new(false);

        public static void ConfigurationUpdated() => configurationUpdatedResetEvent.Set();
#endif

        //We should only ever have one running instance of this service so I am declaring the properties as static.
        private static readonly object threadLock = new();
        private static Thread? ambientLightingThread = null;
        private static CancellationTokenSource? ambientLightingThreadCancellationTokenSource;
        private static PowerManager? powerManager = null;
        private static NotificationManager? notificationManager = null;

        //Apparently this is set to be obsolete in the future? (And so OnStartCommand should be used instead), but it's not deprecated yet.
        protected override void OnHandleIntent(Intent? intent) {}

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent? intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            if (!Monitor.TryEnter(threadLock, 50))
                return StartCommandResult.RedeliverIntent;

            try
            {
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

                ambientLightingThreadCancellationTokenSource = new();
                ambientLightingThread = new Thread(AmbientLightingThread);
                ambientLightingThread.Start();

                return StartCommandResult.Sticky;
            }
            catch
            {
                return StartCommandResult.RedeliverIntent;
            }
            finally
            {
                Monitor.Exit(threadLock);
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            if (ambientLightingThread is null)
                return;

            if (ambientLightingThreadCancellationTokenSource is null)
                throw new NullReferenceException(nameof(ambientLightingThreadCancellationTokenSource));

            ambientLightingThreadCancellationTokenSource.Cancel();

#if false
            if (!ambientLightingThread.Join(5000))
                ambientLightingThread.Abort();
#else
            ambientLightingThread.Join();
#endif
        }

        private async void AmbientLightingThread()
        {
            while (!ambientLightingThreadCancellationTokenSource?.IsCancellationRequested ?? false)
            {
                Guid currentServiceAnimationID = Guid.Empty;

                try
                {
                    if (ambientLightingThreadCancellationTokenSource?.IsCancellationRequested ?? false) return;

                    SSettings settings = await Storage.Settings.GetCached();

                    if (powerManager is null
                        || notificationManager is null
                        || (powerManager.IsPowerSaveMode && !settings.IgnorePowerSaverMode) //Power saver mode.
                        //|| (notificationManager.CurrentInterruptionFilter == InterruptionFilter.Priority && !settings.IgnoreDoNotDisturb) //Do not disturb.
                    )
                    {
                        await Task.Delay(1000);
                        continue;
                    }

                    SAmbientServiceConfiguration configuration = await Storage.AmbientService.GetCached();
                    currentServiceAnimationID = configuration.AnimationID;

                    SAnimation? animation = await Storage.LoadAnimation(configuration.AnimationID);
                    if (animation is null)
                    {
                        await Task.Delay(1000);
                        continue;
                    }

                    if (ambientLightingThreadCancellationTokenSource?.IsCancellationRequested ?? false) return;

                    await AnimationRunner.StartAnimation(animation.Value);

                    bool skipRestartInterval = false;
                    while (AnimationRunner.IsRunning)
                    {
                        if (ambientLightingThreadCancellationTokenSource?.IsCancellationRequested ?? false) return;

#if EXPOSE_UPDATE_EVENT
                        if (configurationUpdatedResetEvent.IsSet)
                        {
                            await AnimationRunner.StopAnimation();
                            configurationUpdatedResetEvent.Reset();
                            skipRestartInterval = true;
                            break;
                        }
#endif

                        await Task.Delay(100);
                    }

                    if (ambientLightingThreadCancellationTokenSource?.IsCancellationRequested ?? false) return;

                    if (!skipRestartInterval)
                        await Task.Delay((int)(configuration.RestartInterval * 1000));
                }
                catch
                {
                    if (ambientLightingThreadCancellationTokenSource?.IsCancellationRequested ?? false) return;

                    await Task.Delay(1000);
                }
                finally
                {
                    if (AnimationRunner.ActiveAnimation is not null && AnimationRunner.ActiveAnimation.Value.Id == currentServiceAnimationID)
                    {
                        try { await AnimationRunner.StopAnimation(); }
                        catch {}
                    }
                }
            }
        }
    }
}
