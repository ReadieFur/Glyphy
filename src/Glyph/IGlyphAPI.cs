namespace Glyphy.Glyph;

public interface IGlyphAPI
{
    bool IsReady { get; }

    EPhoneType PhoneType { get; }

    Task<bool> WaitForReadyAsync(CancellationToken cancellationToken = default);

    void DrawFrame<TZone>(IReadOnlyDictionary<TZone, double> source) where TZone : struct, Enum;

    void SetZone<TZone>(TZone zone, double brightness) where TZone : struct, Enum;
}
