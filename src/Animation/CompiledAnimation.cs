using Android.Animation;
using AndroidX.ConstraintLayout.Motion.Widget;
using Glyphy.LED;
using Glyphy.LED.Zones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Android.Views.Choreographer;

namespace Glyphy.Animation
{
    internal class CompiledAnimation
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public double FrameInterval { get; private set; } = 0;
        public int[,] Frames { get; private set; } = new int[0, 0];

        private CompiledAnimation() {}

        //Temporary solution, precompile the animation. Pros: less cpu overhead between frames, Cons: higher initial CPU and larger memory footprint.
        public static CompiledAnimation CompileAnimation(SAnimation animation, EPhoneType targetDevice)
        {
            CompiledAnimation comp = new CompiledAnimation
            {
                Id = animation.Id,
                FrameInterval = 1000.0 / animation.FrameRate
            };

            //First group all keyframes by LED.
            Dictionary<string, List<SKeyframe>> groupedKeyframes = animation.Keyframes
                .GroupBy(kf => kf.Led)
                .ToDictionary(
                    g => g.Key, //Create a dictionary where keys are mapped to LEDs.
                    g => g.OrderBy(kf => kf.Timestamp).ToList() //Order the nested list with the LEDs keyframes in order of the timestamp (smallest to largest).
                );

            //Get the duration and frame count of the animation.
            long durationMs = animation.Keyframes.Max(kf => kf.Timestamp);
            int frameCount = (int)Math.Ceiling(durationMs / 1000.0 * animation.FrameRate);

            //Calculate intermediate frames.
            Dictionary<string, Queue<int>> ledFrames = new();

            foreach (var ledGroup in groupedKeyframes)
            {
                string ledId = ledGroup.Key;
                Queue<int> framesForLed = new();

                //Calculate each frame for this LED.
                for (int frameIndex = 0; frameIndex < frameCount; frameIndex++)
                {
                    long frameTime = (long)(frameIndex * comp.FrameInterval);

                    SKeyframe? previousKeyframe = null, nextKeyframe = null;

                    foreach (var keyframe in ledGroup.Value)
                    {
                        if (keyframe.Timestamp <= frameTime)
                        {
                            previousKeyframe = keyframe;
                        }
                        else
                        {
                            nextKeyframe = keyframe;
                            break;
                        }
                    }

                    float brightness = 0f;

                    if (previousKeyframe is not null && nextKeyframe is not null)
                    {
                        brightness = Helpers.Interpolate(previousKeyframe.Value, nextKeyframe.Value, frameTime);
                    }
                    else if (previousKeyframe is not null)
                    {
                        brightness = previousKeyframe.Value.Brightness; //No next keyframe, use previous.
                    }
                    else if (nextKeyframe is not null)
                    {
                        brightness = nextKeyframe.Value.Brightness; //No previous keyframe, use next.
                    }

                    int clampedBrightness = Math.Clamp((int)(brightness * 4095), 0, 4095);

                    framesForLed.Enqueue(clampedBrightness);
                }

                ledFrames[ledId] = framesForLed;
            }

            //Initialize the compiled frame buffer.
            int numberOfZones = ZoneMapper.GetZoneTypeForDevice(API.Instance.PhoneType).GetEnumValues().Cast<ushort>().Max() + 1; //Enum values + 1 is always the zone count.
            comp.Frames = new int[frameCount, numberOfZones];

            return comp;
        }
    }
}
