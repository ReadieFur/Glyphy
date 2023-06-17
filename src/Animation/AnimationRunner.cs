using Glyphy.LED;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Glyphy.Animation
{
    public class AnimationRunner
    {
        public static bool IsRunning => ActiveAnimation is not null;
        public static SAnimation? ActiveAnimation { get; private set; }

        public static event Action<SAnimation?>? OnStateChanged;
        public static event Action<IReadOnlyList<SLEDValue>>? OnRunFrame;

        private static readonly object _lock = new();
        private static CancellationTokenSource cancellationTokenSource = null!;
        private static Task? runner = null;

        /// <summary>
        /// </summary>
        /// <param name="animation"></param>
        /// <param name="cancellationToken">Used to stop waiting for the lock.</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static Task StartAnimation(SAnimation animation, CancellationToken? cancellationToken = null)
        {
            while (!Monitor.TryEnter(_lock, TimeSpan.FromMilliseconds(100)))
            {
                if (cancellationToken is not null)
                    cancellationToken.Value.ThrowIfCancellationRequested();
            }

            try
            {
                if (IsRunning)
                    throw new Exception("An animation is already running. Stop the existing animation first.");

                cancellationTokenSource = new();

                ActiveAnimation = animation;
                runner = RunnerTask();
            }
            finally
            {
                Monitor.Exit(_lock);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// </summary>
        /// <param name="cancellationToken">Used to stop waiting for the lock.</param>
        /// <returns></returns>
        public static async Task StopAnimation(CancellationToken? cancellationToken = null)
        {
            while (!Monitor.TryEnter(_lock, TimeSpan.FromMilliseconds(100)))
            {
                if (cancellationToken is not null)
                    cancellationToken.Value.ThrowIfCancellationRequested();
            }

            try
            {
                if (!IsRunning)
                    return;

                cancellationTokenSource.Cancel();

                if (runner is not null)
                    await runner;
            }
            finally
            {
                Monitor.Exit(_lock);
            }
        }

        public static async Task WaitForCompletion(CancellationToken? cancellationToken = null)
        {
            while (IsRunning)
            {
                if (cancellationToken is not null && cancellationToken.Value.IsCancellationRequested)
                    return;

                await Task.Delay(100);
            }
        }

        private static Task RunnerTask()
        {
            return Task.Run(async () =>
            {
                try
                {
                    OnStateChanged?.Invoke(ActiveAnimation);

                    //Removing this can allow for errors but also allows for he user to enter custom configurations that are outside the bounds of the UI restrictions.
                    //ActiveAnimation!.Value.Normalize();

                    foreach (SFrame frame in ActiveAnimation!.Value.Frames)
                    {
                        TimeSpan targetFrameTime = TimeSpan.FromMilliseconds(1000 / ActiveAnimation.Value.FrameRate);

                        //Get starting values.
                        Dictionary<EAddressable, float> startValues = new();
                        foreach (KeyValuePair<EAddressable, SLEDValue> kvp in frame.Values)
                            startValues.Add(kvp.Key, await API.Instance.GetBrightness(kvp.Key));

                        //Transition to the frame.
                        TimeSpan transitionEndTime = TimeSpan.FromSeconds(frame.TransitionTime);
                        Stopwatch transitionStopwatch = new();
                        transitionStopwatch.Start();
                        while (transitionStopwatch.Elapsed < transitionEndTime)
                        {
                            //Check each frame if the animation has been cancelled (checking at this stage will increase CPU usage but also responsivness).
                            cancellationTokenSource.Token.ThrowIfCancellationRequested();

                            //Calculating this here as opposed to in the next loop is ever so slightly less accurate but as a result is more efficient. I will deem this fine as we don't need to be super accurate.
                            float deltaT = transitionStopwatch.ElapsedMilliseconds / (float)transitionEndTime.TotalMilliseconds;

                            List<SLEDValue> ledTransitionStateValues = new();

                            foreach (EAddressable ledKey in Enum.GetValues<EAddressable>())
                            {
                                if (!frame.Values.TryGetValue(ledKey, out SLEDValue ledData))
                                {
                                    _ = API.Instance.SetBrightness(ledKey, 0);
                                    ledTransitionStateValues.Add(new() { Led = ledKey });
                                    continue;
                                }

                                //I feel like this stage can be more efficent if I understood the math more.
                                float transitionBrightness;
                                if (ledData.InterpolationType == EInterpolationType.NONE)
                                {
                                    transitionBrightness = ledData.Brightness;
                                }
                                else
                                {
                                    float transitionState = ledData.InterpolationType switch
                                    {
                                        EInterpolationType.LINEAR => Interpolation.Linear(deltaT),
                                        EInterpolationType.SMOOTH_STEP => Interpolation.Smoothstep(deltaT),
                                        EInterpolationType.SMOOTHER_STEP => Interpolation.SmootherStep(deltaT),
                                        _ => throw new NotImplementedException(), //Shouldn't be reached.
                                    };
                                    transitionBrightness = Misc.Helpers.ConvertNumberRange(transitionState, 0f, 1f, startValues[ledKey], ledData.Brightness);
                                }

                                //I don't want to wait on this becuase it could cause the animation to stutter, however the downside to not waiting is we may skip frames.
                                _ = API.Instance.SetBrightness(ledKey, transitionBrightness);

                                ledTransitionStateValues.Add(new() { Led = ledKey, Brightness = transitionBrightness });
                            }

                            OnRunFrame?.Invoke(ledTransitionStateValues);

                            TimeSpan sleepTime = TimeSpan.FromMilliseconds(targetFrameTime.TotalMilliseconds - (transitionStopwatch.ElapsedMilliseconds % targetFrameTime.TotalMilliseconds));
                            if (sleepTime > TimeSpan.Zero)
                                await Task.Delay(sleepTime);
                        }

                        //Run a UI update here incase there no transition time was processed.
                        List<SLEDValue> ledFrameFinalValues = new();
                        foreach (EAddressable ledKey in Enum.GetValues<EAddressable>())
                            ledFrameFinalValues.Add(frame.Values.TryGetValue(ledKey, out SLEDValue ledValue) ? ledValue : new() { Led = ledKey });
                        OnRunFrame?.Invoke(ledFrameFinalValues);

                        //I could just wait here but the reason for updating at the given interval is to set the led value incase it gets changed externally.
                        //TODO: Detect if the user is in a low-power mode and decrease the number of updates or disable the updates all together.
                        TimeSpan durationEndTime = TimeSpan.FromSeconds(frame.Duration);
                        Stopwatch durationStopwatch = new();
                        durationStopwatch.Start();
                        //I am using a do-while loop here so that if the duration is set to 0, the frame will still be displayed.
                        do
                        {
                            //Check each frame if the animation has been cancelled.
                            cancellationTokenSource.Token.ThrowIfCancellationRequested();

                            foreach (SLEDValue ledValue in ledFrameFinalValues)
                                _ = API.Instance.SetBrightness(ledValue.Led, ledValue.Brightness);

                            //I shouldn't need to call the UI event here as this update is only for changed that may have occured on the hardware side.

                            TimeSpan sleepTime = TimeSpan.FromMilliseconds(targetFrameTime.TotalMilliseconds - (durationStopwatch.ElapsedMilliseconds % targetFrameTime.TotalMilliseconds));
                            if (sleepTime > TimeSpan.Zero)
                                await Task.Delay(sleepTime);
                        }
                        while (durationStopwatch.Elapsed < durationEndTime);
                    }
                }
                catch (Exception) {}
                finally
                {
                    //Clean up.
                    //Lock here? Though I think that could cause a deadlock.
                    runner = null;
                    ActiveAnimation = null;
                    OnStateChanged?.Invoke(null);
                }
            });
        }

        //TODO: Create a RunFrame method that can be called externally to run a single frame.
    }
}
