//#define ENABLE_ON_TAP

using Glyphy.LED;
using Glyphy.Misc;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace Glyphy.Controls;

public partial class Glyph : ContentView
{
#if ENABLE_ON_TAP
	public event EventHandler? CameraTapped;
	public event EventHandler? DiagonalTapped;
	public event EventHandler? RecordingLEDTapped;
	public event EventHandler? CenterTopLeftTapped;
	public event EventHandler? CenterTopRightTapped;
	public event EventHandler? CenterBottomLeftTapped;
	public event EventHandler? CenterBottomRightTapped;
	public event EventHandler? LineTapped;
	public event EventHandler? DotTapped;
#endif

	//I have no reason in bothering making these bindable.
	public Brush BaseColour { get => GlyphBase.Fill; set => GlyphBase.Fill = value; }
	public Brush CameraColour { get => GlyphCamera.Stroke; set => GlyphCamera.Stroke = value; }
	public Brush DiagonalColour { get => GlyphDiagonal.Stroke; set => GlyphDiagonal.Stroke = value; }
	public Brush RecordingLEDColour { get => GlyphRecordingLED.Fill; set => GlyphRecordingLED.Fill = value; }
	public Brush CenterTopLeftColour { get => GlyphCenterTopLeft.Stroke; set => GlyphCenterTopLeft.Stroke = value; }
	public Brush CenterTopRightColour { get => GlyphCenterTopRight.Stroke; set => GlyphCenterTopRight.Stroke = value; }
	public Brush CenterBottomLeftColour { get => GlyphCenterBottomLeft.Stroke; set => GlyphCenterBottomLeft.Stroke = value; }
	public Brush CenterBottomRightColour { get => GlyphCenterBottomRight.Stroke; set => GlyphCenterBottomRight.Stroke = value; }
	public Color Line1Colour { get => GlyphLine1.Color; set => GlyphLine1.Color = value; }
    public Color Line2Colour { get => GlyphLine2.Color; set => GlyphLine2.Color = value; }
    public Color Line3Colour { get => GlyphLine3.Color; set => GlyphLine3.Color = value; }
    public Color Line4Colour { get => GlyphLine4.Color; set => GlyphLine4.Color = value; }
    public Color Line5Colour { get => GlyphLine5.Color; set => GlyphLine5.Color = value; }
    public Color Line6Colour { get => GlyphLine6.Color; set => GlyphLine6.Color = value; }
    public Color Line7Colour { get => GlyphLine7.Color; set => GlyphLine7.Color = value; }
    public Color Line8Colour { get => GlyphLine8.Color; set => GlyphLine8.Color = value; }
    public Brush DotColour { get => GlyphDot.Fill; set => GlyphDot.Fill = value; }

	public Glyph()
	{
		InitializeComponent();
	}

#if ENABLE_ON_TAP
	private bool FallsIntoBounts(TappedEventArgs tap, Path path)
	{
        Point? point = tap.GetPosition(GlyphContainer);
        if (point is null)
            return false;

        RectF bounds = path.GetPath().Bounds;
		if (bounds.Width == 0)
		{
            bounds.Width = (int)path.StrokeThickness * 2;
			//Subtract half as the thickness is on both sides of the line. So we move from |*| to *|| where * is the point of origin and | is the thickness part.
			//In my head the above made sense though in practice I found subtracting double the whole amount was correct (Though I'm also hoping that this isn't because my path is "upside-down", but then again I don't know if it is).
			bounds.X -= bounds.Width * 2;
        }
        if (bounds.Height == 0)
		{
            bounds.Height = (int)path.StrokeThickness;
			//Add half to counteract half of the next Y-= operation.
			bounds.Y += bounds.Height / 2;
        }
        bounds.Y -= bounds.Height;

		return bounds.Contains((float)point.Value.X, (float)point.Value.Y);
	}
#endif

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
#if ENABLE_ON_TAP
		if (FallsIntoBounts(e, GlyphCamera))
			CameraTapped?.Invoke(this, EventArgs.Empty);
		else if (FallsIntoBounts(e, GlyphDiagonal))
			DiagonalTapped?.Invoke(this, EventArgs.Empty);
		else if (FallsIntoBounts(e, GlyphRecordingLED))
			RecordingLEDTapped?.Invoke(this, EventArgs.Empty);
		else if (FallsIntoBounts(e, GlyphCenterTopLeft))
			CenterTopLeftTapped?.Invoke(this, EventArgs.Empty);
		else if (FallsIntoBounts(e, GlyphCenterTopRight))
			CenterTopRightTapped?.Invoke(this, EventArgs.Empty);
		else if (FallsIntoBounts(e, GlyphCenterBottomLeft))
			CenterBottomLeftTapped?.Invoke(this, EventArgs.Empty);
		else if (FallsIntoBounts(e, GlyphCenterBottomRight))
			CenterBottomRightTapped?.Invoke(this, EventArgs.Empty);
		else if (FallsIntoBounts(e, GlyphLine))
			LineTapped?.Invoke(this, EventArgs.Empty);
		else if (FallsIntoBounts(e, GlyphDot))
			DotTapped?.Invoke(this, EventArgs.Empty);
#endif
    }

    public void UpdatePreview(EAddressable led, double brightness)
    {
        Color colour;
        if (led == EAddressable.RECORDING_LED)
            colour = Color.FromHsla(0, 1, Helpers.ConvertNumberRange(brightness, 0, 100, 0.25, 0.6));
        else
            colour = Color.FromHsla(0, 0, Helpers.ConvertNumberRange(brightness, 0, 100, 0.25, 1));

        switch (led)
        {
            case EAddressable.CAMERA:
                CameraColour = colour;
                break;
            case EAddressable.DIAGONAL:
                DiagonalColour = colour;
                break;
            case EAddressable.RECORDING_LED:
                RecordingLEDColour = colour;
                break;
            case EAddressable.CENTER_TOP_LEFT:
                CenterTopLeftColour = colour;
                break;
            case EAddressable.CENTER_TOP_RIGHT:
                CenterTopRightColour = colour;
                break;
            case EAddressable.CENTER_BOTTOM_LEFT:
                CenterBottomLeftColour = colour;
                break;
            case EAddressable.CENTER_BOTTOM_RIGHT:
                CenterBottomRightColour = colour;
                break;
            case EAddressable.LINE_1:
                Line1Colour = colour;
                break;
            case EAddressable.LINE_2:
                Line2Colour = colour;
                break;
            case EAddressable.LINE_3:
                Line3Colour = colour;
                break;
            case EAddressable.LINE_4:
                Line4Colour = colour;
                break;
            case EAddressable.LINE_5:
                Line5Colour = colour;
                break;
            case EAddressable.LINE_6:
                Line6Colour = colour;
                break;
            case EAddressable.LINE_7:
                Line7Colour = colour;
                break;
            case EAddressable.LINE_8:
                Line8Colour = colour;
                break;
            case EAddressable.DOT:
                DotColour = colour;
                break;
        }
    }
}
