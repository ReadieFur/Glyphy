using Glyphy.Animation;
using Glyphy.LED;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Glyphy.Resources.Presets
{
    public static class Glyphs
    {
        public static readonly IEnumerable<SAnimation> Presets = typeof(Glyphs)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(field => field.FieldType == typeof(SAnimation))
            .Select(field => (SAnimation)field.GetValue(null)!);

        public static readonly SAnimation oi = new()
        {
            Id = Guid.Parse("d8dd3f32-cdda-4eee-87c9-35aeb1050943"),
            Name = "Oi!",
            FrameRate = 30,
            Frames = new()
            {
                new()
                {
                    TransitionTime = 0,
                    Duration = 0.035f,
                    Values = new()
                    {
                        {
                            EAddressable.DOT,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.DOT,
                                Brightness = 1.0f
                            }
                        }
                    }
                },
                new()
                {
                    TransitionTime = 0,
                    Duration = 0.09f,
                    Values = new()
                },
                new()
                {
                    TransitionTime = 0,
                    Duration = 0.035f,
                    Values = new()
                    {
                        {
                            EAddressable.DOT,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.DOT,
                                Brightness = 1.0f
                            }
                        }
                    }
                },
                new()
                {
                    TransitionTime = 0,
                    Duration = 0,
                    Values = new()
                }
            }
        };
    }
}
