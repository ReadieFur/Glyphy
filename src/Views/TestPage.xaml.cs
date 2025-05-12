using Glyphy.Animation;
using Glyphy.Glyph;
using Glyphy.Glyph.Indexes;
using Newtonsoft.Json;
using Timer = System.Timers.Timer;

namespace Glyphy.Views;

public partial class TestPage : ContentPage
{
    private const int _FRAME_RATE = 60;
    private readonly double increment = Helpers.ConvertNumberRange(1000 / _FRAME_RATE, 0, 1000, 0, 1);
    private Timer _loopTask = new();
    private double i = 0;

    public TestPage()
	{
		InitializeComponent();

        _loopTask.Elapsed += _loopTask_Elapsed;
        _loopTask.Interval = 1000 / _FRAME_RATE;

        TestButton.Text = "Enable (Not Running)";

        TestJson();
    }

    private async void TestButton_Clicked(object sender, EventArgs e)
	{
        await TestLed();
    }

    private void TestJson()
    {
        SAnimation animation = new();
        animation.PhoneType = EPhoneType.PhoneOne;
        animation.Keyframes.Add(new SKeyframe
        {
            Led = EPhoneOne.A1,
            Timestamp = 300,
            Brightness = 0.6,
            Interpolation = EInterpolationType.Bezier,
            InTangent = new(0.5, 0.5)
        });

        string json = JsonConvert.SerializeObject(animation, Formatting.Indented);

        SAnimation animation2 = JsonConvert.DeserializeObject<SAnimation>(json);
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
}
