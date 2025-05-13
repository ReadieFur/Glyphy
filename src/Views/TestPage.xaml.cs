using Glyphy.Animation;
using Glyphy.Glyph;
using Glyphy.Glyph.Indexes;
using Glyphy.Misc;
using Glyphy.Storage;
using Newtonsoft.Json;
using Timer = System.Timers.Timer;

namespace Glyphy.Views;

public partial class TestPage : ContentPage
{
    private const int _FRAME_RATE = 60;
    private readonly double increment = MathHelpers.ConvertNumberRange(1000 / _FRAME_RATE, 0, 1000, 0, 1);
    private Timer _loopTask = new();
    private double i = 0;

    public TestPage()
	{
		InitializeComponent();

        _loopTask.Elapsed += _loopTask_Elapsed;
        _loopTask.Interval = 1000 / _FRAME_RATE;

        TestButton.Text = "Enable (Not Running)";

        AnimationRunner.Instance.StateChanged += isRunning => Dispatcher.Dispatch(() => TestButton.Text = isRunning ? "Disable (Running...)" : "Enable (Not Running)");

        //TestJson();
    }

    private void TestButton_Clicked(object sender, EventArgs e)
	{
        //TestLed();
        TestAnimation();
    }

    private void TestJson()
    {
        SAnimation animation = new();
        animation.PhoneType = EPhoneType.PhoneOne;
        animation.Keyframes[EPhoneOne.A1].AddRange([
            new SKeyframe
            {
                Timestamp = 500,
                Brightness = 0.9,
                Interpolation = EInterpolationType.Bezier,
                InTangent = new(0.5, 0.5)
            },
            new SKeyframe
            {
                Timestamp = 300,
                Brightness = 0.6,
                Interpolation = EInterpolationType.Bezier,
                OutTangent = new(0.5, 0.5)
            },
        ]);

        string json = JsonConvert.SerializeObject(animation, Formatting.Indented, new AnimationJsonConverter());

        SAnimation animation2 = JsonConvert.DeserializeObject<SAnimation>(json, new AnimationJsonConverter());
    }

    private async Task TestLed()
    {
        //Start the API.
        if (!await GlyphAPI.Instance.WaitForReadyAsync())
            throw new Exception();

        //Testing on PhoneOne.
        if (GlyphAPI.Instance.PhoneType != EPhoneType.PhoneOne)
            throw new Exception();

        if (_loopTask.Enabled)
        {
            _loopTask.Stop();
            i = 0;

            GlyphAPI.Instance.DrawFrame(new Dictionary<SPhoneIndex, double>()); //Turn off all lights.

            TestButton.Text = "Enable (Not Running)";

            return;
        }

        TestButton.Text = "Disable (Running...)";

        _loopTask.Start();
    }

    private void _loopTask_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        i += increment;
        if (i > 1)
            i = 0;

        GlyphAPI.Instance.DrawFrame(new Dictionary<SPhoneIndex, double>
        {
            { EPhoneOne.A1, i }
        });
    }

    private void TestAnimation()
    {
        if (AnimationRunner.Instance.IsPlaying)
        {
            AnimationRunner.Instance.StopAnimation();
            return;
        }

        SAnimation animation = new();
        animation.PhoneType = EPhoneType.PhoneOne;

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

        AnimationRunner.Instance.PlayAnimation(animation);
    }
}
