using Glyphy.Glyph;
using Glyphy.Glyph.Indexes;

namespace Glyphy.Animation
{
    public class AnimationRunner
    {
        #region Instance
        private static readonly object _INSTANCE_LOCK = new();

        private static AnimationRunner? _instance = null;
        //I am using this to make the API only initalize when called (as opposed to a static constructor).
        public static AnimationRunner Instance
        {
            get
            {
                //It is quicker to check if the instance is null before locking, at which point it will be ok to peform the check again.
                if (_instance is null)
                    lock (_INSTANCE_LOCK)
                        if (_instance is null)
                            _instance = new();
                return _instance;
            }
        }
        #endregion

        public bool IsPlaying => ActiveAnimationId is not null;
        public int FrameRate
        {
            get => _frameRate;
            set => _frameRate = Math.Clamp(value, 5, 60);
        }
        public Guid? ActiveAnimationId { get; private set; }
        public event Action<bool> StateChanged; //True if started, False if stopped.

        private readonly CancellationTokenSource _exitSignal = new();
        private readonly Thread _animationThread = new(AnimationWorker);
        private readonly AutoResetEvent _processAnimationEvent = new(false);
        private readonly object _threadLock = new();
        private CancellationTokenSource? _stopAnimationCts = null;
        private int _frameRate = 60;
        private SAnimation? _animation = null;

        private AnimationRunner()
        {
            _animationThread.Start(this);
        }

        ~AnimationRunner()
        {
            _exitSignal.Cancel();
            _animationThread.Join();
        }

        private static void AnimationWorker(object? param)
        {
            if (param is not AnimationRunner self)
                throw new ArgumentException(nameof(param));

            GlyphAPI.Instance.WaitForReadyAsync().GetAwaiter().GetResult(); //Run synchronously since we can't await here.

            while (!self._exitSignal.IsCancellationRequested)
            {
                self._processAnimationEvent.WaitOne();

                long durationMs = 0;
                Dictionary<SPhoneIndex, List<SKeyframe>> sortedKeyframes;
                CancellationToken animationCancellationToken;

                lock (self._threadLock)
                {
                    if (self._animation is not SAnimation animation || self._stopAnimationCts is null)
                        continue;

                    animationCancellationToken = self._stopAnimationCts.Token;

                    self.ActiveAnimationId = animation.Id;
                    self.StateChanged?.Invoke(true);

                    sortedKeyframes = animation.Keyframes;
                    foreach (var kvp in sortedKeyframes)
                    {
                        //Order the keyframes by their timestamps (uses SKeyframe.IComparable).
                        kvp.Value.Sort();

                        long localMax = kvp.Value.Max(kf => kf.Timestamp);
                        if (localMax > durationMs)
                            durationMs = localMax;
                    }
                }

                double t = 0;
                while (!self._exitSignal.IsCancellationRequested
                    && !animationCancellationToken.IsCancellationRequested
                    && t <= durationMs) //<= to include one extra frame for the final timepoint.
                {
                    /*TODO: For future playback seeking.
                    if (t > durationMs)
                    {
                        //Pause here.
                    }
                    else if (t < 0)
                    {
                    }*/

                    TimeSpan frameInterval = TimeSpan.FromMilliseconds(1000.0 / self.FrameRate);

                    self.DisplayFrameAtTimestamp(t, sortedKeyframes);

                    t += frameInterval.TotalMilliseconds;

                    //Wait for any of the cancellation tokens or the timeout (whichever comes first) before continuing to the next iteration.
                    WaitHandle.WaitAny([self._exitSignal.Token.WaitHandle, animationCancellationToken.WaitHandle], frameInterval);
                }

                self.StateChanged?.Invoke(false);
                self.ActiveAnimationId = null;
                self._animation = null;
            }
        }

        private void DisplayFrameAtTimestamp(double timestamp, Dictionary<SPhoneIndex, List<SKeyframe>> sortedKeyframes)
        {
            Dictionary<SPhoneIndex, double> frameValues = new();

            foreach (var kvp in sortedKeyframes)
            {
                SKeyframe? previousKeyframe = null, nextKeyframe = null;

                foreach (SKeyframe keyframe in kvp.Value)
                {
                    if (keyframe.Timestamp <= timestamp)
                    {
                        previousKeyframe = keyframe;
                    }
                    else
                    {
                        nextKeyframe = keyframe;
                        break;
                    }
                }

                double brightness = 0;

                if (previousKeyframe is not null && nextKeyframe is not null)
                {
                    brightness = AnimationHelpers.Interpolate(previousKeyframe.Value, nextKeyframe.Value, timestamp);
                }
                else if (previousKeyframe is not null)
                {
                    brightness = previousKeyframe.Value.Brightness; //No next keyframe, use previous.
                }
                else if (nextKeyframe is not null)
                {
                    brightness = nextKeyframe.Value.Brightness; //No previous keyframe, use next.
                }

                frameValues.Add(kvp.Key, brightness);
            }

            GlyphAPI.Instance.DrawFrame(frameValues);
        }

        public void PlayAnimation(SAnimation animation)
        {
            lock (_threadLock)
            {
                _stopAnimationCts?.Cancel();
                //No need to wait here as the animation thread will return to it's idle state when the active frame (if any) is complete.
                _stopAnimationCts = new();

                _animation = animation;

                _processAnimationEvent.Set();
            }
        }

        public void StopAnimation()
        {
            lock (_threadLock)
                _stopAnimationCts?.Cancel();
        }
    }
}
