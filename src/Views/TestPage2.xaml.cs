using CommunityToolkit.Maui.Alerts;
using Glyphy.Animation;
using Glyphy.Glyph;
using Glyphy.Glyph.Indexes;

namespace Glyphy.Views;

public partial class TestPage2 : ContentPage
{
    private readonly List<SAnimation> _animations = new();
    private SAnimation? _selectedAnimation = null;

	public TestPage2()
	{
		InitializeComponent();
        IsEnabled = false;

        BuildAnimations();
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

    private void BuildAnimations()
    {
        BuildFade();
    }

    private void BuildFade()
    {
        SAnimation animation = new()
        {
            PhoneType = Glyph.EPhoneType.PhoneOne,
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

        _animations.Add(animation);
    }
}
