using Glyphy.Glyph.Indexes;

namespace Glyphy.Glyph;

public partial class GlyphAPI : IGlyphAPI
{
    bool IGlyphAPI.IsReady => true;

    EPhoneType IGlyphAPI.PhoneType => EPhoneType.PhoneOne; //Simulate Phone (1) for desktop testing (because thats what I'm using).

    Task<bool> IGlyphAPI.WaitForReadyAsync(CancellationToken cancellationToken) => Task.FromResult(true);

    void IGlyphAPI.DrawFrame(IReadOnlyDictionary<SPhoneIndex, double> indexes) { }

    void IGlyphAPI.SetIndex(SPhoneIndex index, double brightness) { }
}
