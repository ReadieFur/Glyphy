namespace Glyphy.Glyph;

public interface IGlyphAPI
{
    bool IsReady { get; }

    EPhoneType PhoneType { get; }

    Task<bool> WaitForReadyAsync(CancellationToken cancellationToken = default);

    void DrawFrame(IReadOnlyDictionary<ushort, double> source);
    void DrawFrame(IReadOnlyDictionary<string, double> source);

    void SetIndex(ushort id, double brightness);
    void SetIndex(string key, double brightness);
}
