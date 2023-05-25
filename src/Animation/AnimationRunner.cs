using Glyphy.LED;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Glyphy.Animation
{
    public class AnimationRunner
    {
        public static bool IsRunning => ActiveAnimation is not null;
        public static SAnimation? ActiveAnimation { get; private set; }

        public static event Action<SAnimation?>? OnStateChanged;

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
                if (cancellationToken is not null && cancellationToken.Value.IsCancellationRequested)
                    return Task.FromCanceled(cancellationToken.Value);
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
                if (cancellationToken is not null && cancellationToken.Value.IsCancellationRequested)
                    return;
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

                    //TODO: Optimise these operations.
                    foreach (SFrame frame in ActiveAnimation!.Value.Frames)
                    {
                        TimeSpan frameTime = TimeSpan.FromMicroseconds(1000 / ActiveAnimation.Value.FrameRate);

                        //Get starting values.
                        Dictionary<EAddressable, float> startValues = new();
                        foreach (KeyValuePair<EAddressable, SLEDValue> kvp in frame.Values)
                            startValues.Add(kvp.Key, await API.Instance.GetBrightness(kvp.Key));

                        //Transition to the frame.
                        //TODO: Change this to a time based solution as it will be more accurate.
                        float transitionFrames = ActiveAnimation.Value.FrameRate * frame.TransitionTime;
                        for (float i = 0; i < transitionFrames; i++)
                        {
                            //Check each frame if the animation has been cancelled.
                            cancellationTokenSource.Token.ThrowIfCancellationRequested();

                            foreach (KeyValuePair<EAddressable, SLEDValue> kvp in frame.Values)
                            {
                                float start = startValues[kvp.Key];
                                float end = frame.Values[kvp.Key].Brightness;
                                float deltaT = i / transitionFrames;
                                float transitionBrightness = kvp.Value.InterpolationType switch
                                {
                                    EInterpolationType.NONE => kvp.Value.Brightness,
                                    EInterpolationType.LINEAR => Interpolation.Linear(start, end, deltaT),
                                    EInterpolationType.SMOOTH_STEP => Interpolation.Smoothstep(start, end, deltaT),
                                    EInterpolationType.SMOOTHER_STEP => Interpolation.SmootherStep(start, end, deltaT),
                                    _ => throw new NotImplementedException(), //Shouldn't be reached.
                                };

                                //I don't want to wait on this becuase it could cause the animation to stutter, however the downside to not waiting is we may skip frames.
                                _ = API.Instance.SetBrightness(kvp.Key, transitionBrightness);
                            }

                            await Task.Delay(frameTime);
                        }

                        //I could just wait here but the reason for updating at the given interval is to set the led value incase it gets changed externally.
                        //TODO: Detect if the user is in a low-power mode and decrease the number of updates or disable the updates all together.
                        for (double i = 0; i < ActiveAnimation.Value.FrameRate * frame.Duration; i++)
                        {
                            foreach (KeyValuePair<EAddressable, SLEDValue> kvp in frame.Values)
                                _ = API.Instance.SetBrightness(kvp.Key, kvp.Value.Brightness);
                            await Task.Delay(frameTime);
                        }
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
    }
}
