namespace Glyphy.Glyph;

public partial class GlyphAPI : IGlyphAPI
{
    bool IGlyphAPI.IsReady => true;

    EPhoneType IGlyphAPI.PhoneType => EPhoneType.Unknown;

    Task<bool> IGlyphAPI.WaitForReadyAsync(CancellationToken cancellationToken) => Task.FromResult(true);

    void IGlyphAPI.DrawFrame(IReadOnlyDictionary<ushort, double> source) { }
    void IGlyphAPI.DrawFrame(IReadOnlyDictionary<string, double> source) { }

    void IGlyphAPI.SetIndex(ushort id, double brightness) { }
    void IGlyphAPI.SetIndex(string key, double brightness) { }
}
