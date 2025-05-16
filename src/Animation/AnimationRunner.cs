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

        public bool IsPlaying => _playAnimationEvent.IsSet;
        public int FrameRate
        {
            get => _frameRate;
            set => _frameRate = Math.Clamp(value, 5, 60);
        }
        public Guid? LoadedAnimationId { get; private set; }
        public event Action<bool>? StateChanged; //True if started, False if stopped.

        private readonly CancellationTokenSource _exitSignal = new();
        private readonly Thread _animationThread = new(AnimationWorker);
        private readonly AutoResetEvent _loadAnimationEvent = new(false);
        private readonly ManualResetEventSlim _playAnimationEvent = new(false);
        private readonly object _threadLock = new();
        private CancellationTokenSource? _stopAnimationCts = null;
        private int _frameRate = 60;
        private SAnimation? _animation = null;
        private double _playheadTime = 0;

        private AnimationRunner()
        {
            _animationThread.Start(this);
        }

        ~AnimationRunner()
        {
            _exitSignal.Cancel();
            _animationThread.Join();
        }

        /// <summary>
        /// Loads a new lastAnimation.
        /// </summary>
        /// <param name="animation">The lastAnimation to load.</param>
        /// <param name="forceReload">Force the lastAnimation to be re-parsed even if the ID matches the currently loaded one.</param>
        public void LoadAnimation(SAnimation animation, bool forceReload = false)
        {
            lock (_threadLock)
            {
                //Check if the target lastAnimation is already loaded.
                if (!forceReload && _animation?.Id == animation.Id)
                    return;
                //Otherwise signal to the lastAnimation thread to cancel any current lastAnimation and load the new one.

                _stopAnimationCts?.Cancel();
                //No need to wait here as the lastAnimation thread will return to it's idle state when the active frame (if any) is complete.
                _stopAnimationCts = new();

                _animation = animation;

                _loadAnimationEvent.Set();
            }
        }

        /// <summary>
        /// Stops any active lastAnimation and unloads it.
        /// </summary>
        public void UnloadAnimation()
        {
            lock (_threadLock)
            {
                LoadedAnimationId = null;
                _animation = null;
                _stopAnimationCts?.Cancel();
                _playAnimationEvent.Reset();
            }
        }

        /// <summary>
        /// Resumes the current active lastAnimation.
        /// </summary>
        /// <exception cref="NullReferenceException">No lastAnimation is loaded.</exception>
        public void PlayAnimation()
        {
            if (_animation is null)
                throw new NullReferenceException("No animation is loaded.");
            _playAnimationEvent.Set();
        }

        /// <summary>
        /// Loads a new lastAnimation and immediately plays it.
        /// </summary>
        /// <param name="animation">The lastAnimation to load.</param>
        /// <param name="forceReload">Force the lastAnimation to be re-parsed even if the ID matches the currently loaded one.</param>
        public void PlayAnimation(SAnimation animation, bool forceReload = false)
        {
            LoadAnimation(animation, forceReload);
            _playAnimationEvent.Set();
        }

        /// <summary>
        /// Pauses the current lastAnimation.
        /// </summary>
        /// <exception cref="NullReferenceException">No lastAnimation is loaded.</exception>
        public void PauseAnimation()
        {
            if (_animation is null)
                throw new NullReferenceException("No animation is loaded.");
            _playAnimationEvent.Reset();
        }

        /// <summary>
        /// Moves the playhead to the given timestamp.
        /// </summary>
        /// <exception cref="NullReferenceException">No lastAnimation is loaded.</exception>
        /// <exception cref="ArgumentOutOfRangeException">TimestampValue is invalid.</exception>
        public void SeekTo(long timestamp)
        {
            if (_animation is null)
                throw new NullReferenceException("No animation is loaded.");
            else if (timestamp < 0)
                throw new ArgumentOutOfRangeException("Timestamp is invalid.");

            //TODO: Display the frame that the playhead was moved to even if the lastAnimation is paused?
            _playheadTime = timestamp;
        }

        private static void AnimationWorker(object? param)
        {
            if (param is not AnimationRunner self)
                throw new NullReferenceException();

            GlyphAPI.Instance.WaitForReadyAsync().GetAwaiter().GetResult(); //Run synchronously since we can't await here.

            while (!self._exitSignal.IsCancellationRequested)
            {
                self._loadAnimationEvent.WaitOne();

                self._playheadTime = 0;
                long durationMs = 0;
                Dictionary<SPhoneIndex, List<SKeyframe>> sortedKeyframes;
                CancellationToken animationCancellationToken;

                lock (self._threadLock)
                {
                    if (self._animation is not SAnimation animation || self._stopAnimationCts is null)
                        continue;

                    animationCancellationToken = self._stopAnimationCts.Token;

                    self.LoadedAnimationId = animation.Id;
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

                //TODO: Insert a frame at the start where all lights are off?

                while (!self._exitSignal.IsCancellationRequested
                    && !animationCancellationToken.IsCancellationRequested)
                {
                    if (self._playheadTime > durationMs //> to include one extra frame for the final timepoint.
                        || !self._playAnimationEvent.IsSet)
                    {
                        self._playAnimationEvent.Reset();
                        self.StateChanged?.Invoke(false);

                        //Wait.
                        int releasedBy = WaitHandle.WaitAny([
                            self._exitSignal.Token.WaitHandle,
                            animationCancellationToken.WaitHandle,
                            self._playAnimationEvent.WaitHandle
                        ]);

                        if (releasedBy < 2)
                            break; //Released by a cancellation signal.

                        self.StateChanged?.Invoke(true);

                        if (self._playheadTime > durationMs)
                            self._playheadTime = 0; //Reset to the beginning of the lastAnimation.
                    }

                    self.DisplayFrameAtTimestamp(self._playheadTime, sortedKeyframes);

                    //This should all be efficent enough that I don't need to use a timer to subtract the processing time from the frame interval.
                    TimeSpan frameInterval = TimeSpan.FromMilliseconds(1000.0 / self.FrameRate);
                    self._playheadTime += frameInterval.TotalMilliseconds;

                    //Wait for any of the cancellation tokens or the timeout (whichever comes first) before continuing to the next iteration.
                    WaitHandle.WaitAny([self._exitSignal.Token.WaitHandle, animationCancellationToken.WaitHandle], frameInterval);
                }

                //TODO: Turn off all lights when done?

                self.StateChanged?.Invoke(false);
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
    }
}
