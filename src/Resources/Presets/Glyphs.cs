using Glyphy.Animation;
using Glyphy.LED;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Glyphy.Resources.Presets
{
    //The following Glyphs are interpreted by me scrubbing through a screen recording of the original system Glyphs. As a result their timings may not be 100% accurate.
    public static class Glyphs
    {
        public static readonly IEnumerable<SAnimation> Presets = typeof(Glyphs)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(field => field.FieldType == typeof(SAnimation))
            .Select(field => (SAnimation)field.GetValue(null)!);

        public static readonly SAnimation OI = new()
        {
            Id = Guid.Parse("d8dd3f32-cdda-4eee-87c9-35aeb1050943"),
            Name = "Oi!",
            FrameRate = 30,
            Frames = new()
            {
                new()
                {
                    TransitionTime = 0,
                    Duration = 0.1f,
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
                    Duration = 0.1f,
                    Values = new()
                },
                new()
                {
                    TransitionTime = 0,
                    Duration = 0.1f,
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

        public static readonly SAnimation BULB_ONE = new()
        {
            Id = Guid.Parse("584069bc-ccad-4714-992e-a0653c4a282a"),
            Name = "Bulb One",
            FrameRate = 30,
            Frames = new()
            {
                new()
                {
                    TransitionTime = 0,
                    Duration = 0.065f,
                    Values = new()
                    {
                        {
                            EAddressable.CENTER_TOP_LEFT,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.CENTER_TOP_LEFT,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.CENTER_TOP_RIGHT,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.CENTER_TOP_RIGHT,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.CENTER_BOTTOM_LEFT,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.CENTER_BOTTOM_LEFT,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.CENTER_BOTTOM_RIGHT,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.CENTER_BOTTOM_RIGHT,
                                Brightness = 1.0f
                            }
                        }
                    }
                },
                new()
                {
                    TransitionTime = 0,
                    Duration = 0.23f,
                    Values = new()
                    {
                        {
                            EAddressable.LINE_1,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.LINE_1,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.LINE_2,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.LINE_2,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.LINE_3,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.LINE_3,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.LINE_4,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.LINE_4,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.LINE_5,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.LINE_5,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.LINE_6,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.LINE_6,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.LINE_7,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.LINE_7,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.LINE_8,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.LINE_8,
                                Brightness = 1.0f
                            }
                        }
                    }
                },
                new()
                {
                    TransitionTime = 0,
                    Duration = 0.365f,
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
                    TransitionTime = 0.23f,
                    Duration = 0f,
                    Values = new()
                    {
                        {
                            EAddressable.DOT,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.DOT,
                                Brightness = 0f
                            }
                        }
                    }
                },
                new()
                {
                    TransitionTime = 0.23f,
                    Duration = 0f,
                    Values = new()
                }
            }
        };

        public static readonly SAnimation BULB_TWO = new()
        {
            Id = Guid.Parse("8c1acb79-af35-4a94-81f8-738335ed956f"),
            Name = "Bulb Two",
            FrameRate = 30,
            Frames = new()
            {
                new()
                {
                    TransitionTime = 0.3f,
                    Duration = 0,
                    Values = new()
                    {
                        {
                            EAddressable.CAMERA,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.CAMERA,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.DIAGONAL,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.DIAGONAL,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.CENTER_TOP_LEFT,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.CENTER_TOP_LEFT,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.CENTER_TOP_RIGHT,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.CENTER_TOP_RIGHT,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.CENTER_BOTTOM_LEFT,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.CENTER_BOTTOM_LEFT,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.CENTER_BOTTOM_RIGHT,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.CENTER_BOTTOM_RIGHT,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.LINE_1,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.LINE_1,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.LINE_2,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.LINE_2,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.LINE_3,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.LINE_3,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.LINE_4,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.LINE_4,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.LINE_5,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.LINE_5,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.LINE_6,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.LINE_6,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.LINE_7,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.LINE_7,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.LINE_8,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.LINE_8,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.DOT,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.DOT,
                                Brightness = 1.0f
                            }
                        }
                    }
                },
                new()
                {
                    TransitionTime = 0,
                    Duration = 0.43f,
                    Values = new()
                    {
                        {
                            EAddressable.DIAGONAL,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.DIAGONAL,
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

        public static readonly SAnimation GUIRO = new()
        {
            Id = Guid.Parse("80fded9d-a1aa-4beb-b654-76ef49d06568"),
            Name = "Guiro",
            FrameRate = 75, //Requires a higher framerate due to the fast transitions (if we go lower we may have frames that run too short or too long).
            Frames = new()
            {
                new()
                {
                    TransitionTime = 0,
                    Duration = 0.04f,
                    Values = new()
                    {
                        {
                            EAddressable.CAMERA,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.CAMERA,
                                Brightness = 1.0f
                            }
                        }
                    }
                },
                new()
                {
                    TransitionTime = 0,
                    Duration = 0.04f,
                    Values = new()
                    {
                        {
                            EAddressable.CENTER_TOP_LEFT,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.CENTER_TOP_LEFT,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.CENTER_TOP_RIGHT,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.CENTER_TOP_RIGHT,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.CENTER_BOTTOM_LEFT,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.CENTER_BOTTOM_LEFT,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.CENTER_BOTTOM_RIGHT,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.CENTER_BOTTOM_RIGHT,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.LINE_1,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.LINE_1,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.LINE_2,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.LINE_2,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.LINE_3,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.LINE_3,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.LINE_4,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.LINE_4,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.LINE_5,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.LINE_5,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.LINE_6,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.LINE_6,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.LINE_7,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.LINE_7,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.LINE_8,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.LINE_8,
                                Brightness = 1.0f
                            }
                        }
                    }
                },
                new()
                {
                    TransitionTime = 0,
                    Duration = 0.04f,
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
                    Duration = 0.15f,
                    Values = new()
                },
                new()
                {
                    TransitionTime = 0,
                    Duration = 0.04f,
                    Values = new()
                    {
                        {
                            EAddressable.CAMERA,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.CAMERA,
                                Brightness = 1.0f
                            }
                        }
                    }
                },
                new()
                {
                    TransitionTime = 0,
                    Duration = 0.04f,
                    Values = new()
                    {
                        {
                            EAddressable.CENTER_TOP_LEFT,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.CENTER_TOP_LEFT,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.CENTER_TOP_RIGHT,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.CENTER_TOP_RIGHT,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.CENTER_BOTTOM_LEFT,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.CENTER_BOTTOM_LEFT,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.CENTER_BOTTOM_RIGHT,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.CENTER_BOTTOM_RIGHT,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.LINE_1,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.LINE_1,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.LINE_2,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.LINE_2,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.LINE_3,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.LINE_3,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.LINE_4,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.LINE_4,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.LINE_5,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.LINE_5,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.LINE_6,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.LINE_6,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.LINE_7,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.LINE_7,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.LINE_8,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.LINE_8,
                                Brightness = 1.0f
                            }
                        }
                    }
                },
                new()
                {
                    TransitionTime = 0,
                    Duration = 0.04f,
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

        public static readonly SAnimation VOLLEY = new()
        {
            Id = Guid.Parse("502a0795-0d3d-42d4-be4e-0ec28163fe27"),
            Name = "Volley",
            FrameRate = 30,
            Frames = new()
            {
                new()
                {
                    TransitionTime = 0,
                    Duration = 0,
                    Values = new()
                    {
                        {
                            EAddressable.LINE_1,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.LINE_1,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.LINE_2,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.LINE_2,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.LINE_3,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.LINE_3,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.LINE_4,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.LINE_4,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.LINE_5,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.LINE_5,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.LINE_6,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.LINE_6,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.LINE_7,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.LINE_7,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.LINE_8,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.LINE_8,
                                Brightness = 1.0f
                            }
                        }
                    }
                },
                new()
                {
                    TransitionTime = 0.2f,
                    Duration = 0,
                    Values = new()
                    {
                        {
                            EAddressable.LINE_1,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.LINE_1,
                                Brightness = 0
                            }
                        },
                        {
                            EAddressable.LINE_2,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.LINE_2,
                                Brightness = 0
                            }
                        },
                        {
                            EAddressable.LINE_3,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.LINE_3,
                                Brightness = 0
                            }
                        },
                        {
                            EAddressable.LINE_4,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.LINE_4,
                                Brightness = 0
                            }
                        },
                        {
                            EAddressable.LINE_5,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.LINE_5,
                                Brightness = 0
                            }
                        },
                        {
                            EAddressable.LINE_6,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.LINE_6,
                                Brightness = 0
                            }
                        },
                        {
                            EAddressable.LINE_7,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.LINE_7,
                                Brightness = 0
                            }
                        },
                        {
                            EAddressable.LINE_8,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.LINE_8,
                                Brightness = 0
                            }
                        }
                    }
                }
            }
        };

        public static readonly SAnimation SQUIGGLE = new()
        {
            Id = Guid.Parse("0c62bcf9-ea90-47af-9ce2-2cfa4e54e698"),
            Name = "Squiggle",
            FrameRate = 75,
            Frames = new()
            {
                new()
                {
                    TransitionTime = 0,
                    Duration = 0.025f,
                    Values = new()
                    {
                        {
                            EAddressable.CAMERA,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.CAMERA,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.LINE_1,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.LINE_1,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.LINE_2,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.LINE_2,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.LINE_3,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.LINE_3,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.LINE_4,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.LINE_4,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.LINE_5,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.LINE_5,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.LINE_6,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.LINE_6,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.LINE_7,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.LINE_7,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.LINE_8,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.LINE_8,
                                Brightness = 1.0f
                            }
                        }
                    }
                },
                //TODO: I think the above may have a fast fade out in the original animation.
                new()
                {
                    TransitionTime = 0,
                    Duration = 0.025f,
                    Values = new()
                },
                new()
                {
                    TransitionTime = 0,
                    Duration = 0.025f,
                    Values = new()
                    {
                        {
                            EAddressable.CAMERA,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.CAMERA,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.LINE_1,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.LINE_1,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.LINE_2,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.LINE_2,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.LINE_3,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.LINE_3,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.LINE_4,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.LINE_4,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.LINE_5,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.LINE_5,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.LINE_6,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.LINE_6,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.LINE_7,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.LINE_7,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.LINE_8,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.LINE_8,
                                Brightness = 1.0f
                            }
                        }
                    }
                },
                new()
                {
                    TransitionTime = 0,
                    Duration = 0.025f,
                    Values = new()
                },
                new()
                {
                    TransitionTime = 0,
                    Duration = 0.02f,
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
                    Duration = 0.025f,
                    Values = new()
                },
                new()
                {
                    TransitionTime = 0,
                    Duration = 0.025f,
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

        //This animation needs updating as some subtile transitions are missing around the middle of the animation.
        public static readonly SAnimation ISOLATOR = new()
        {
            Id = Guid.Parse("8d32b1fa-6ba9-42a0-9000-193987e302f0"),
            Name = "Isolator",
            FrameRate = 30,
            Frames = new()
            {
                new()
                {
                    TransitionTime = 0.05f,
                    Duration = 0,
                    Values = new()
                    {
                        {
                            EAddressable.CAMERA,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.CAMERA,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.DIAGONAL,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.DIAGONAL,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.CENTER_TOP_LEFT,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.CENTER_TOP_LEFT,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.CENTER_TOP_RIGHT,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.CENTER_TOP_RIGHT,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.CENTER_BOTTOM_LEFT,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.CENTER_BOTTOM_LEFT,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.CENTER_BOTTOM_RIGHT,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.CENTER_BOTTOM_RIGHT,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.LINE_1,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.LINE_1,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.LINE_2,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.LINE_2,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.LINE_3,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.LINE_3,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.LINE_4,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.LINE_4,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.LINE_5,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.LINE_5,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.LINE_6,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.LINE_6,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.LINE_7,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.LINE_7,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.LINE_8,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.LINE_8,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.DOT,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.DOT,
                                Brightness = 1.0f
                            }
                        }
                    }
                },
                //TODO: Fix this part of the animation to better match the original.
#region FIX
                new()
                {
                    TransitionTime = 0.47f,
                    //TransitionTime = 0.2f,
                    Duration = 0,
                    Values = new()
                    {
                        {
                            EAddressable.CAMERA,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.CAMERA,
                                Brightness = 0.5f
                            }
                        },
                        {
                            EAddressable.DIAGONAL,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.DIAGONAL,
                                Brightness = 0.5f
                            }
                        },
                        {
                            EAddressable.CENTER_TOP_LEFT,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.CENTER_TOP_LEFT,
                                Brightness = 0.5f
                            }
                        },
                        {
                            EAddressable.CENTER_TOP_RIGHT,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.CENTER_TOP_RIGHT,
                                Brightness = 0.5f
                            }
                        },
                        {
                            EAddressable.CENTER_BOTTOM_LEFT,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.CENTER_BOTTOM_LEFT,
                                Brightness = 0.5f
                            }
                        },
                        {
                            EAddressable.CENTER_BOTTOM_RIGHT,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.CENTER_BOTTOM_RIGHT,
                                Brightness = 0.5f
                            }
                        },
                        {
                            EAddressable.LINE_1,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.LINE_1,
                                Brightness = 0.5f
                            }
                        },
                        {
                            EAddressable.LINE_2,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.LINE_2,
                                Brightness = 0.5f
                            }
                        },
                        {
                            EAddressable.LINE_3,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.LINE_3,
                                Brightness = 0.5f
                            }
                        },
                        {
                            EAddressable.LINE_4,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.LINE_4,
                                Brightness = 0.5f
                            }
                        },
                        {
                            EAddressable.LINE_5,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.LINE_5,
                                Brightness = 0.5f
                            }
                        },
                        {
                            EAddressable.LINE_6,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.LINE_6,
                                Brightness = 0.5f
                            }
                        },
                        {
                            EAddressable.LINE_7,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.LINE_7,
                                Brightness = 0.5f
                            }
                        },
                        {
                            EAddressable.LINE_8,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.LINE_8,
                                Brightness = 0.5f
                            }
                        },
                        {
                            EAddressable.DOT,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.DOT,
                                Brightness = 0.5f
                            }
                        }
                    }
                },
                /*new()
                {
                    TransitionTime = 0.1f,
                    Duration = 0,
                    Values = new()
                    {
                        {
                            EAddressable.CAMERA,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.CAMERA,
                                Brightness = 0.85f
                            }
                        },
                        {
                            EAddressable.DIAGONAL,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.DIAGONAL,
                                Brightness = 0.85f
                            }
                        },
                        {
                            EAddressable.CENTER_TOP_LEFT,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.CENTER_TOP_LEFT,
                                Brightness = 0.85f
                            }
                        },
                        {
                            EAddressable.CENTER_TOP_RIGHT,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.CENTER_TOP_RIGHT,
                                Brightness = 0.85f
                            }
                        },
                        {
                            EAddressable.CENTER_BOTTOM_LEFT,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.CENTER_BOTTOM_LEFT,
                                Brightness = 0.85f
                            }
                        },
                        {
                            EAddressable.CENTER_BOTTOM_RIGHT,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.CENTER_BOTTOM_RIGHT,
                                Brightness = 0.85f
                            }
                        },
                        {
                            EAddressable.LINE_1,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.LINE_1,
                                Brightness = 0.85f
                            }
                        },
                        {
                            EAddressable.LINE_2,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.LINE_2,
                                Brightness = 0.85f
                            }
                        },
                        {
                            EAddressable.LINE_3,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.LINE_3,
                                Brightness = 0.85f
                            }
                        },
                        {
                            EAddressable.LINE_4,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.LINE_4,
                                Brightness = 0.85f
                            }
                        },
                        {
                            EAddressable.LINE_5,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.LINE_5,
                                Brightness = 0.85f
                            }
                        },
                        {
                            EAddressable.LINE_6,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.LINE_6,
                                Brightness = 0.85f
                            }
                        },
                        {
                            EAddressable.LINE_7,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.LINE_7,
                                Brightness = 0.85f
                            }
                        },
                        {
                            EAddressable.LINE_8,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.LINE_8,
                                Brightness = 0.85f
                            }
                        },
                        {
                            EAddressable.DOT,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.DOT,
                                Brightness = 0.85f
                            }
                        }
                    }
                },*/
#endregion
                new()
                {
                    TransitionTime = 0,
                    Duration = 0,
                    Values = new()
                    {
                        {
                            EAddressable.DIAGONAL,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.DIAGONAL,
                                Brightness = 0.5f
                            }
                        },
                        {
                            EAddressable.CENTER_TOP_LEFT,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.CENTER_TOP_LEFT,
                                Brightness = 0.5f
                            }
                        },
                        {
                            EAddressable.CENTER_TOP_RIGHT,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.CENTER_TOP_RIGHT,
                                Brightness = 0.5f
                            }
                        },
                        {
                            EAddressable.CENTER_BOTTOM_LEFT,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.CENTER_BOTTOM_LEFT,
                                Brightness = 0.5f
                            }
                        },
                        {
                            EAddressable.CENTER_BOTTOM_RIGHT,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.CENTER_BOTTOM_RIGHT,
                                Brightness = 0.5f
                            }
                        },
                        {
                            EAddressable.LINE_1,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.LINE_1,
                                Brightness = 0.5f
                            }
                        },
                        {
                            EAddressable.LINE_2,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.LINE_2,
                                Brightness = 0.5f
                            }
                        },
                        {
                            EAddressable.LINE_3,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.LINE_3,
                                Brightness = 0.5f
                            }
                        },
                        {
                            EAddressable.LINE_4,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.LINE_4,
                                Brightness = 0.5f
                            }
                        },
                        {
                            EAddressable.LINE_5,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.LINE_5,
                                Brightness = 0.5f
                            }
                        },
                        {
                            EAddressable.LINE_6,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.LINE_6,
                                Brightness = 0.5f
                            }
                        },
                        {
                            EAddressable.LINE_7,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.LINE_7,
                                Brightness = 0.5f
                            }
                        },
                        {
                            EAddressable.LINE_8,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.LINE_8,
                                Brightness = 0.5f
                            }
                        },
                        {
                            EAddressable.DOT,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.DOT,
                                Brightness = 0.5f
                            }
                        }
                    }
                },
                new()
                {
                    TransitionTime = 0.1f,
                    Duration = 0,
                    Values = new()
                    {
                        {
                            EAddressable.DIAGONAL,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.DIAGONAL,
                                Brightness = 0.2f
                            }
                        },
                        {
                            EAddressable.CENTER_TOP_LEFT,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.CENTER_TOP_LEFT,
                                Brightness = 0.2f
                            }
                        },
                        {
                            EAddressable.CENTER_TOP_RIGHT,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.CENTER_TOP_RIGHT,
                                Brightness = 0.2f
                            }
                        },
                        {
                            EAddressable.CENTER_BOTTOM_LEFT,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.CENTER_BOTTOM_LEFT,
                                Brightness = 0.2f
                            }
                        },
                        {
                            EAddressable.CENTER_BOTTOM_RIGHT,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.CENTER_BOTTOM_RIGHT,
                                Brightness = 0.2f
                            }
                        },
                        {
                            EAddressable.LINE_1,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.LINE_1,
                                Brightness = 0.2f
                            }
                        },
                        {
                            EAddressable.LINE_2,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.LINE_2,
                                Brightness = 0.2f
                            }
                        },
                        {
                            EAddressable.LINE_3,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.LINE_3,
                                Brightness = 0.2f
                            }
                        },
                        {
                            EAddressable.LINE_4,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.LINE_4,
                                Brightness = 0.2f
                            }
                        },
                        {
                            EAddressable.LINE_5,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.LINE_5,
                                Brightness = 0.2f
                            }
                        },
                        {
                            EAddressable.LINE_6,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.LINE_6,
                                Brightness = 0.2f
                            }
                        },
                        {
                            EAddressable.LINE_7,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.LINE_7,
                                Brightness = 0.2f
                            }
                        },
                        {
                            EAddressable.LINE_8,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.LINE_8,
                                Brightness = 0.2f
                            }
                        },
                        {
                            EAddressable.DOT,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.DOT,
                                Brightness = 0.2f
                            }
                        }
                    }
                },
                new()
                {
                    TransitionTime = 0,
                    Duration = 0.04f,
                    Values = new()
                    {
                        {
                            EAddressable.CENTER_TOP_LEFT,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.CENTER_TOP_LEFT,
                                Brightness = 0.2f
                            }
                        },
                        {
                            EAddressable.CENTER_TOP_RIGHT,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.CENTER_TOP_RIGHT,
                                Brightness = 0.2f
                            }
                        },
                        {
                            EAddressable.CENTER_BOTTOM_LEFT,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.CENTER_BOTTOM_LEFT,
                                Brightness = 0.2f
                            }
                        },
                        {
                            EAddressable.CENTER_BOTTOM_RIGHT,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.CENTER_BOTTOM_RIGHT,
                                Brightness = 0.2f
                            }
                        },
                        {
                            EAddressable.DOT,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.DOT,
                                Brightness = 0.2f
                            }
                        }
                    }
                },
                new()
                {
                    TransitionTime = 0,
                    Duration = 0.02f,
                    Values = new()
                },
                new()
                {
                    TransitionTime = 0,
                    Duration = 0,
                    Values = new()
                    {
                        {
                            EAddressable.CENTER_TOP_LEFT,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.CENTER_TOP_LEFT,
                                Brightness = 0.2f
                            }
                        },
                        {
                            EAddressable.CENTER_TOP_RIGHT,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.CENTER_TOP_RIGHT,
                                Brightness = 0.2f
                            }
                        },
                        {
                            EAddressable.CENTER_BOTTOM_LEFT,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.CENTER_BOTTOM_LEFT,
                                Brightness = 0.2f
                            }
                        },
                        {
                            EAddressable.CENTER_BOTTOM_RIGHT,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.CENTER_BOTTOM_RIGHT,
                                Brightness = 0.2f
                            }
                        }
                    }
                },
                new()
                {
                    TransitionTime = 0.3f,
                    Duration = 0,
                    Values = new()
                    {
                        {
                            EAddressable.CENTER_TOP_LEFT,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.CENTER_TOP_LEFT,
                                Brightness = 0.5f
                            }
                        },
                        {
                            EAddressable.CENTER_TOP_RIGHT,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.CENTER_TOP_RIGHT,
                                Brightness = 0.5f
                            }
                        },
                        {
                            EAddressable.CENTER_BOTTOM_LEFT,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.CENTER_BOTTOM_LEFT,
                                Brightness = 0.5f
                            }
                        },
                        {
                            EAddressable.CENTER_BOTTOM_RIGHT,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.CENTER_BOTTOM_RIGHT,
                                Brightness = 0.5f
                            }
                        },
                        {
                            EAddressable.DOT,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.DOT,
                                Brightness = 0.5f
                            }
                        }
                    }
                },
                new()
                {
                    TransitionTime = 0.3f,
                    Duration = 0,
                    Values = new()
                    {
                        {
                            EAddressable.CENTER_TOP_LEFT,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.CENTER_TOP_LEFT,
                                Brightness = 0.2f
                            }
                        },
                        {
                            EAddressable.CENTER_TOP_RIGHT,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.CENTER_TOP_RIGHT,
                                Brightness = 0.2f
                            }
                        },
                        {
                            EAddressable.CENTER_BOTTOM_LEFT,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.CENTER_BOTTOM_LEFT,
                                Brightness = 0.2f
                            }
                        },
                        {
                            EAddressable.CENTER_BOTTOM_RIGHT,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.CENTER_BOTTOM_RIGHT,
                                Brightness = 0.2f
                            }
                        }
                    }
                },
                new()
                {
                    TransitionTime = 0,
                    Duration = 0.10f,
                    Values = new()
                },
                new()
                {
                    TransitionTime = 0,
                    Duration = 0,
                    Values = new()
                    {
                        {
                            EAddressable.DOT,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.DOT,
                                Brightness = 1
                            }
                        }
                    }
                },
                new()
                {
                    TransitionTime = 0.1f,
                    Duration = 0,
                    Values = new()
                    {
                        {
                            EAddressable.DOT,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.DOT,
                                Brightness = 0
                            }
                        }
                    }
                }
            }
        };

        public static readonly SAnimation GAMMA = new()
        {
            Id = Guid.Parse("80fc14b5-eec2-4838-82cd-c8c43b6c2cb7"),
            Name = "Gamma",
            FrameRate = 75,
            Frames = new()
            {
                new()
                {
                    TransitionTime = 0,
                    Duration = 0.025f,
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
                    Duration = 0.025f,
                    Values = new()
                    {
                        {
                            EAddressable.LINE_1,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.LINE_1,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.LINE_2,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.LINE_2,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.LINE_3,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.LINE_3,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.LINE_4,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.LINE_4,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.LINE_5,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.LINE_5,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.LINE_6,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.LINE_6,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.LINE_7,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.LINE_7,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.LINE_8,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.LINE_8,
                                Brightness = 1.0f
                            }
                        }
                    }
                },
                new()
                {
                    TransitionTime = 0,
                    Duration = 0.025f,
                    Values = new()
                    {
                        {
                            EAddressable.DIAGONAL,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.DIAGONAL,
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

        //TODO: This animation needs speeding up and the diagonal LED to have interpolation.
        public static readonly SAnimation BEAK = new()
        {
            Id = Guid.Parse("14cd19ce-c92e-43eb-9c7c-6f5caf6385af"),
            Name = "Beak",
            FrameRate = 30,
            Frames = new()
            {
                new()
                {
                    TransitionTime = 0,
                    Duration = 0.07f,
                    Values = new()
                    {
                        {
                            EAddressable.CAMERA,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.CAMERA,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.DIAGONAL,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.DIAGONAL,
                                Brightness = 1.0f
                            }
                        }
                    }
                },
                new()
                {
                    TransitionTime = 0,
                    Duration = 0.07f,
                    Values = new()
                    {
                        {
                            EAddressable.DIAGONAL,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.DIAGONAL,
                                Brightness = 1.0f
                            }
                        }
                    }
                },
                new()
                {
                    TransitionTime = 0,
                    Duration = 0.04f,
                    Values = new()
                    {
                        {
                            EAddressable.DIAGONAL,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.DIAGONAL,
                                Brightness = 1.0f
                            }
                        },
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
                    TransitionTime = 0.13f,
                    Duration = 0,
                    Values = new()
                    {
                        {
                            EAddressable.DIAGONAL,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.DIAGONAL,
                                Brightness = 0.75f
                            }
                        }
                    }
                },
                new()
                {
                    TransitionTime = 0,
                    Duration = 0.05f,
                    Values = new()
                    {
                        {
                            EAddressable.DIAGONAL,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.DIAGONAL,
                                Brightness = 0.75f
                            }
                        },
                        {
                            EAddressable.LINE_1,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.LINE_1,
                                Brightness = 0.75f
                            }
                        },
                        {
                            EAddressable.LINE_2,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.LINE_2,
                                Brightness = 0.75f
                            }
                        },
                        {
                            EAddressable.LINE_3,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.LINE_3,
                                Brightness = 0.75f
                            }
                        },
                        {
                            EAddressable.LINE_4,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.LINE_4,
                                Brightness = 0.75f
                            }
                        },
                        {
                            EAddressable.LINE_5,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.LINE_5,
                                Brightness = 0.75f
                            }
                        },
                        {
                            EAddressable.LINE_6,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.LINE_6,
                                Brightness = 0.75f
                            }
                        },
                        {
                            EAddressable.LINE_7,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.LINE_7,
                                Brightness = 0.75f
                            }
                        },
                        {
                            EAddressable.LINE_8,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.LINE_8,
                                Brightness = 0.75f
                            }
                        }
                    }
                },
                new()
                {
                    TransitionTime = 0.02f,
                    Duration = 0,
                    Values = new()
                    {
                        {
                            EAddressable.DIAGONAL,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.DIAGONAL,
                                Brightness = 0.85f
                            }
                        }
                    }
                },
                new()
                {
                    TransitionTime = 0.07f,
                    Duration = 0,
                    Values = new()
                    {
                        {
                            EAddressable.DIAGONAL,
                            new()
                            {
                                InterpolationType = EInterpolationType.SMOOTH_STEP,
                                Led = EAddressable.DIAGONAL,
                                Brightness = 0
                            }
                        }
                    }
                },
            }
        };

        public static readonly SAnimation NOPE = new()
        {
            Id = Guid.Parse("9b5d1574-0dfd-4df4-a85f-d7eb5a53e9e8"),
            Name = "Nope",
            FrameRate = 75,
            Frames = new()
            {
                new()
                {
                    TransitionTime = 0,
                    Duration = 0.03f,
                    Values = new()
                    {
                        {
                            EAddressable.CAMERA,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.CAMERA,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.CENTER_TOP_LEFT,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.CENTER_TOP_LEFT,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.CENTER_TOP_RIGHT,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.CENTER_TOP_RIGHT,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.CENTER_BOTTOM_LEFT,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.CENTER_BOTTOM_LEFT,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.CENTER_BOTTOM_RIGHT,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.CENTER_BOTTOM_RIGHT,
                                Brightness = 1.0f
                            }
                        }
                    }
                },
                new()
                {
                    TransitionTime = 0,
                    Duration = 0.03f,
                    Values = new()
                },
                new()
                {
                    TransitionTime = 0,
                    Duration = 0.03f,
                    Values = new()
                    {
                        {
                            EAddressable.CAMERA,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.CAMERA,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.CENTER_TOP_LEFT,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.CENTER_TOP_LEFT,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.CENTER_TOP_RIGHT,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.CENTER_TOP_RIGHT,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.CENTER_BOTTOM_LEFT,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.CENTER_BOTTOM_LEFT,
                                Brightness = 1.0f
                            }
                        },
                        {
                            EAddressable.CENTER_BOTTOM_RIGHT,
                            new()
                            {
                                InterpolationType = EInterpolationType.NONE,
                                Led = EAddressable.CENTER_BOTTOM_RIGHT,
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
