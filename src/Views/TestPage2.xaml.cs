using CommunityToolkit.Maui.Alerts;
using Glyphy.Animation;
using Glyphy.Glyph;
using Glyphy.Glyph.Indexes;
using System.Reflection;

namespace Glyphy.Views;

public partial class TestPage2 : ContentPage
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    private class AnimationProperty : Attribute {}

    private readonly List<SAnimation> _animations = new();
    private SAnimation? _selectedAnimation = null;

	public TestPage2()
	{
		InitializeComponent();
        IsEnabled = false;

        //Populate animations dynamically (because im lazy).
        foreach (MethodInfo method in typeof(TestPage2).GetMethods(BindingFlags.Static | BindingFlags.NonPublic))
            if (method.GetCustomAttribute<AnimationProperty>() is not null
                && method.ReturnType == typeof(SAnimation)
                && method.GetParameters().Length == 0)
                _animations.Add((SAnimation)method.Invoke(null, null)!);
        foreach (var animation in _animations)
            AnimationPicker.Items.Add(animation.Name);

        AnimationRunner.Instance.StateChanged += AnimationRunner_StateChanged;
        Loaded += TestPage2_Loaded;
	}

    private async void TestPage2_Loaded(object? sender, EventArgs e)
    {
        if (!await GlyphAPI.Instance.WaitForReadyAsync())
            await Toast.Make("GlyphAPI failed to start.", CommunityToolkit.Maui.Core.ToastDuration.Long).Show();
        else
            IsEnabled = true;
    }

    private void AnimationPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (AnimationPicker.SelectedItem is not string name)
            return;

        SAnimation animation = _animations.Find(a => a.Name == name);
        if (_selectedAnimation is not null && _selectedAnimation.Value.Id == animation.Id)
            return;

        _selectedAnimation = animation;

        if (EnabledSwitch.IsToggled)
            AnimationRunner.Instance.PlayAnimation(animation, true);
    }

    private void AnimationRunner_StateChanged(bool isRunning)
    {
        if (!isRunning && LoopingSwitch.IsToggled && EnabledSwitch.IsToggled && _selectedAnimation is SAnimation animation)
        {
            AnimationRunner.Instance.PlayAnimation();
            return;
        }
        else if (!isRunning && EnabledSwitch.IsToggled)
        {
            Dispatcher.Dispatch(() => EnabledSwitch.IsToggled = false);
        }
    }

    private void Enabled_Toggled(object sender, ToggledEventArgs e)
    {
        if (e.Value)
        {
            if (_selectedAnimation is not SAnimation animation)
            {
                EnabledSwitch.IsToggled = false;
                return;
            }

            AnimationRunner.Instance.PlayAnimation(animation, true);
        }
        else
        {
            AnimationRunner.Instance.UnloadAnimation();
        }
    }

    private void BrightnessMultiplierSlider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        AnimationRunner.Instance.BrightnessMultiplier = e.NewValue;
    }

#pragma warning disable IDE0051 //Remove unused private members.
    [AnimationProperty]
    private static SAnimation Fade()
    {
        SAnimation animation = new()
        {
            PhoneType = EPhoneType.PhoneOne,
            Name = "Fade"
        };

        List<SKeyframe> frameBuffer = [
            new SKeyframe
            {
                Timestamp = 0,
                Brightness = 0,
                Interpolation = EInterpolationType.Smooth
            },
            new SKeyframe
            {
                Timestamp = 900,
                Brightness = 1,
                Interpolation = EInterpolationType.Smooth
            },
            new SKeyframe
            {
                Timestamp = 1800,
                Brightness = 0,
                Interpolation = EInterpolationType.Smooth
            }
        ];

        List<SPhoneIndex> leds =
        [
            EPhoneOne.E1,
            EPhoneOne.D1_1,
            EPhoneOne.D1_2,
            EPhoneOne.D1_3,
            EPhoneOne.D1_4,
            EPhoneOne.D1_5,
            EPhoneOne.D1_6,
            EPhoneOne.D1_7,
            EPhoneOne.D1_8,
            EPhoneOne.C2,
            EPhoneOne.C1,
            EPhoneOne.C4,
            EPhoneOne.C3,
            EPhoneOne.A1,
            EPhoneOne.B1,
        ];

        foreach (SPhoneIndex led in leds)
        {
            animation.Keyframes[led].AddRange(frameBuffer);

            //Offset the led times a little (making copies of the data).
            List<SKeyframe> newFrameBuffer = new();
            for (int i = 0; i < frameBuffer.Count; i++)
                newFrameBuffer.Add(frameBuffer[i] with { Timestamp = frameBuffer[i].Timestamp + 75 });
            frameBuffer = newFrameBuffer;
        }

        return animation;
    }

    [AnimationProperty]
    private static SAnimation FadeSlow()
    {
        SAnimation animation = new()
        {
            PhoneType = EPhoneType.PhoneOne,
            Name = "Fade Slow"
        };

        List<SKeyframe> frameBuffer = [
            new SKeyframe
            {
                Timestamp = 0,
                Brightness = 0,
                Interpolation = EInterpolationType.Smooth
            },
            new SKeyframe
            {
                Timestamp = 1200,
                Brightness = 1,
                Interpolation = EInterpolationType.Smooth
            },
            new SKeyframe
            {
                Timestamp = 2400,
                Brightness = 0,
                Interpolation = EInterpolationType.Smooth
            }
        ];

        List<SPhoneIndex> leds =
        [
            EPhoneOne.E1,
            EPhoneOne.D1_1,
            EPhoneOne.D1_2,
            EPhoneOne.D1_3,
            EPhoneOne.D1_4,
            EPhoneOne.D1_5,
            EPhoneOne.D1_6,
            EPhoneOne.D1_7,
            EPhoneOne.D1_8,
            EPhoneOne.C2,
            EPhoneOne.C1,
            EPhoneOne.C4,
            EPhoneOne.C3,
            EPhoneOne.A1,
            EPhoneOne.B1,
        ];

        foreach (SPhoneIndex led in leds)
        {
            animation.Keyframes[led].AddRange(frameBuffer);

            //Offset the led times a little (making copies of the data).
            List<SKeyframe> newFrameBuffer = new();
            for (int i = 0; i < frameBuffer.Count; i++)
                newFrameBuffer.Add(frameBuffer[i] with { Timestamp = frameBuffer[i].Timestamp + 175 });
            frameBuffer = newFrameBuffer;
        }

        return animation;
    }

    [AnimationProperty]
    private static SAnimation Ambient()
    {
        SAnimation animation = new()
        {
            PhoneType = EPhoneType.PhoneOne,
            Name = "Ambient"
        };

        animation.Keyframes[EPhoneOne.A1] = new()
        {
            new SKeyframe
            {
                Timestamp = 0,
                Brightness = 1,
                Interpolation = EInterpolationType.None
            },
            new SKeyframe
            {
                Timestamp = 3000,
                Brightness = 0,
                Interpolation = EInterpolationType.Smooth
            },
            new SKeyframe
            {
                Timestamp = 8000,
                Brightness = 0,
                Interpolation = EInterpolationType.Smooth
            },
            new SKeyframe
            {
                Timestamp = 11000,
                Brightness = 1,
                Interpolation = EInterpolationType.Smooth
            },
        };
        animation.Keyframes[EPhoneOne.C3].AddRange(
        [
            new SKeyframe
            {
                Timestamp = 0,
                Brightness = 0,
                Interpolation = EInterpolationType.None
            },
            new SKeyframe
            {
                Timestamp = 3000,
                Brightness = 1,
                Interpolation = EInterpolationType.Smooth
            },
            new SKeyframe
            {
                Timestamp = 6000,
                Brightness = 0,
                Interpolation = EInterpolationType.Smooth
            },
            new SKeyframe
            {
                Timestamp = 6500,
                Brightness = 0,
                Interpolation = EInterpolationType.None
            },
            new SKeyframe
            {
                Timestamp = 8000,
                Brightness = 1,
                Interpolation = EInterpolationType.Smooth
            },
            new SKeyframe
            {
                Timestamp = 9500,
                Brightness = 0,
                Interpolation = EInterpolationType.Smooth
            }
        ]);
        animation.Keyframes[EPhoneOne.C4].AddRange(
        [
            new SKeyframe
            {
                Timestamp = 0,
                Brightness = 0,
                Interpolation = EInterpolationType.None
            },
            new SKeyframe
            {
                Timestamp = 3000,
                Brightness = 1,
                Interpolation = EInterpolationType.Smooth
            },
            new SKeyframe
            {
                Timestamp = 6000,
                Brightness = 0,
                Interpolation = EInterpolationType.Smooth
            },
            new SKeyframe
            {
                Timestamp = 6500 + 100,
                Brightness = 0,
                Interpolation = EInterpolationType.Smooth
            },
            new SKeyframe
            {
                Timestamp = 8000 + 100,
                Brightness = 1,
                Interpolation = EInterpolationType.Smooth
            },
            new SKeyframe
            {
                Timestamp = 9500 + 100,
                Brightness = 0,
                Interpolation = EInterpolationType.Smooth
            }
        ]);
        animation.Keyframes[EPhoneOne.C1].AddRange(
        [
            new SKeyframe
            {
                Timestamp = 0,
                Brightness = 0,
                Interpolation = EInterpolationType.None
            },
            new SKeyframe
            {
                Timestamp = 3000,
                Brightness = 1,
                Interpolation = EInterpolationType.Smooth
            },
            new SKeyframe
            {
                Timestamp = 6000,
                Brightness = 0,
                Interpolation = EInterpolationType.Smooth
            },
            new SKeyframe
            {
                Timestamp = 6500 + 200,
                Brightness = 0,
                Interpolation = EInterpolationType.Smooth
            },
            new SKeyframe
            {
                Timestamp = 8000 + 200,
                Brightness = 1,
                Interpolation = EInterpolationType.Smooth
            },
            new SKeyframe
            {
                Timestamp = 9500 + 200,
                Brightness = 0,
                Interpolation = EInterpolationType.Smooth
            }
        ]);
        animation.Keyframes[EPhoneOne.C2].AddRange(
        [
            new SKeyframe
            {
                Timestamp = 0,
                Brightness = 0,
                Interpolation = EInterpolationType.None
            },
            new SKeyframe
            {
                Timestamp = 3000,
                Brightness = 1,
                Interpolation = EInterpolationType.Smooth
            },
            new SKeyframe
            {
                Timestamp = 6000,
                Brightness = 0,
                Interpolation = EInterpolationType.Smooth
            },
            new SKeyframe
            {
                Timestamp = 6500 + 300,
                Brightness = 0,
                Interpolation = EInterpolationType.Smooth
            },
            new SKeyframe
            {
                Timestamp = 8000 + 300,
                Brightness = 1,
                Interpolation = EInterpolationType.Smooth
            },
            new SKeyframe
            {
                Timestamp = 9500 + 300,
                Brightness = 0,
                Interpolation = EInterpolationType.Smooth
            }
        ]);
        animation.Keyframes[EPhoneOne.B1] = new()
        {
            new SKeyframe
            {
                Timestamp = 0,
                Brightness = 1,
                Interpolation = EInterpolationType.None
            },
            new SKeyframe
            {
                Timestamp = 50,
                Brightness = 0,
                Interpolation = EInterpolationType.None
            },
            new SKeyframe
            {
                Timestamp = 100,
                Brightness = 1,
                Interpolation = EInterpolationType.None
            },
            new SKeyframe
            {
                Timestamp = 150,
                Brightness = 0,
                Interpolation = EInterpolationType.None
            },
            //
            new SKeyframe
            {
                Timestamp = 2000,
                Brightness = 1,
                Interpolation = EInterpolationType.None
            },
            new SKeyframe
            {
                Timestamp = 2050,
                Brightness = 0,
                Interpolation = EInterpolationType.None
            },
            new SKeyframe
            {
                Timestamp = 2100,
                Brightness = 1,
                Interpolation = EInterpolationType.None
            },
            new SKeyframe
            {
                Timestamp = 2150,
                Brightness = 0,
                Interpolation = EInterpolationType.None
            },
            //
            new SKeyframe
            {
                Timestamp = 4000,
                Brightness = 0,
                Interpolation = EInterpolationType.None
            },
            new SKeyframe
            {
                Timestamp = 7000,
                Brightness = 1,
                Interpolation = EInterpolationType.Smooth
            },
            new SKeyframe
            {
                Timestamp = 10000,
                Brightness = 0,
                Interpolation = EInterpolationType.Smooth
            },
        };
        List<SKeyframe> barLedBuffer1 = [
            new SKeyframe
            {
                Timestamp = 3000,
                Brightness = 0,
                Interpolation = EInterpolationType.Smooth
            },
            new SKeyframe
            {
                Timestamp = 5000,
                Brightness = 1,
                Interpolation = EInterpolationType.Smooth
            },
            new SKeyframe
            {
                Timestamp = 7000,
                Brightness = 0,
                Interpolation = EInterpolationType.Smooth
            }
        ];
        List<SPhoneIndex> barLeds =
        [
            EPhoneOne.E1,
            EPhoneOne.D1_1,
            EPhoneOne.D1_2,
            EPhoneOne.D1_3,
            EPhoneOne.D1_4,
            EPhoneOne.D1_5,
            EPhoneOne.D1_6,
            EPhoneOne.D1_7,
            EPhoneOne.D1_8,
        ];
        foreach (SPhoneIndex led in barLeds)
        {
            animation.Keyframes[led].AddRange(barLedBuffer1);
            List<SKeyframe> newFrameBuffer = new();
            for (int i = 0; i < barLedBuffer1.Count; i++)
                newFrameBuffer.Add(barLedBuffer1[i] with { Timestamp = barLedBuffer1[i].Timestamp + 100 });
            barLedBuffer1 = newFrameBuffer;
        }
        barLeds.Reverse();
        List<SKeyframe> barLedBuffer2 = [
            new SKeyframe
            {
                Timestamp = 8000,
                Brightness = 0,
                Interpolation = EInterpolationType.Smooth
            },
            new SKeyframe
            {
                Timestamp = 9000,
                Brightness = 1,
                Interpolation = EInterpolationType.Smooth
            },
            new SKeyframe
            {
                Timestamp = 10000,
                Brightness = 0,
                Interpolation = EInterpolationType.Smooth
            }
        ];
        foreach (SPhoneIndex led in barLeds)
        {
            animation.Keyframes[led].AddRange(barLedBuffer2);
            List<SKeyframe> newFrameBuffer = new();
            for (int i = 0; i < barLedBuffer2.Count; i++)
                newFrameBuffer.Add(barLedBuffer2[i] with { Timestamp = barLedBuffer2[i].Timestamp + 75 });
            barLedBuffer2 = newFrameBuffer;
        }

        return animation;
    }

    [AnimationProperty]
    private static SAnimation All()
    {
        SAnimation animation = new()
        {
            PhoneType = EPhoneType.PhoneOne,
            Name = "All"
        };

        animation.Keyframes[EPhoneOne.A1] =
            animation.Keyframes[EPhoneOne.B1] =
            animation.Keyframes[EPhoneOne.C1] =
            animation.Keyframes[EPhoneOne.C2] =
            animation.Keyframes[EPhoneOne.C3] =
            animation.Keyframes[EPhoneOne.C4] =
            animation.Keyframes[EPhoneOne.D1_1] =
            animation.Keyframes[EPhoneOne.D1_2] =
            animation.Keyframes[EPhoneOne.D1_3] =
            animation.Keyframes[EPhoneOne.D1_4] =
            animation.Keyframes[EPhoneOne.D1_5] =
            animation.Keyframes[EPhoneOne.D1_6] =
            animation.Keyframes[EPhoneOne.D1_7] =
            animation.Keyframes[EPhoneOne.D1_8] =
            animation.Keyframes[EPhoneOne.E1] =
            new()
            {
                new SKeyframe
                {
                    Timestamp = 0,
                    Brightness = 1,
                    Interpolation = EInterpolationType.None
                }
            };

        return animation;
    }

    //[AnimationProperty]
    //private static SAnimation Rave()
    //{
    //    SAnimation animation = new()
    //    {
    //        PhoneType = EPhoneType.PhoneOne,
    //        Name = "Rave"
    //    };

    //    return animation;
    //}
#pragma warning restore IDE0051
}
