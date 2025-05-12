namespace Glyphy.Glyph;

public partial class GlyphAPI : IGlyphAPI
{
    bool IGlyphAPI.IsReady => true;

    EPhoneType IGlyphAPI.PhoneType => EPhoneType.Unknown;

    Task IGlyphAPI.WaitForReadyAsync() => Task.CompletedTask;

    void IGlyphAPI.DrawFrame<TZone>(IReadOnlyDictionary<TZone, double> source) { }

    void IGlyphAPI.SetZone<TZone>(TZone zone, double brightness) { }
}
