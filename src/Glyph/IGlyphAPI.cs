using Glyphy.Glyph.Indexes;

namespace Glyphy.Glyph;

public interface IGlyphAPI
{
    bool IsReady { get; }

    EPhoneType PhoneType { get; }

    Task<bool> WaitForReadyAsync(CancellationToken cancellationToken = default);

    void DrawFrame(IReadOnlyDictionary<SPhoneIndex, double> indexes);

    void SetIndex(SPhoneIndex index, double brightness);
}
