using Glyphy.Glyph.Indexes;
using Glyphy.Misc;

namespace Glyphy.Glyph;

public partial class GlyphAPI : IGlyphAPI
{
    #region Constants
    public const double GLYPH_MIN_BRIGHTNESS = 0;
    public const double GLYPH_MAX_BRIGHTNESS = 1;
    private const int KETCHUM_MIN_BRIGHTNESS = 0;
    private const int KETCHUM_MAX_BRIGHTNESS = 4096;
    #endregion

    #region Instance
    private static readonly object _INSTANCE_LOCK = new();

    private static IGlyphAPI? _instance = null;
    //I am using this to make the API only initalize when called (as opposed to a static constructor).
    public static IGlyphAPI Instance
    {
        get
        {
            //It is quicker to check if the instance is null before locking, at which point it will be ok to peform the check again.
            if (_instance is null)
                lock (_INSTANCE_LOCK)
                    if (_instance is null)
                        _instance = new GlyphAPI();
            return _instance;
        }
    }
    #endregion

    #region Helpers
    /// <summary>
    /// Convert a brightness from the range used by this API to the one used by the Glyph interface.
    /// </summary>
    protected static int GlyphToKetchumBrightness(double brightness)
    {
        return (int)Math.Clamp(
            MathHelpers.ConvertNumberRange(brightness, GLYPH_MIN_BRIGHTNESS, GLYPH_MAX_BRIGHTNESS, KETCHUM_MIN_BRIGHTNESS, KETCHUM_MAX_BRIGHTNESS),
            KETCHUM_MIN_BRIGHTNESS,
            KETCHUM_MAX_BRIGHTNESS);
    }

    /// <summary>
    /// Create an array of brightnesses ordered for the given phone type to be passed to the Ketchum API.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="phoneType"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    protected static int[] CreateFrame(EPhoneType phoneType, IReadOnlyDictionary<SPhoneIndex, double> values)
    {
        //Create default map.
        Dictionary<ushort, double> indexMap = IndexMapper.GetMapping(phoneType).IdxToKey.ToDictionary(kvp => kvp.Key, _ => 0.0);

        //Insert values.
        foreach (var kvp in values)
            indexMap[kvp.Key] = kvp.Value; //Allow to throw given an invalid key.

        //Return frame for device.
        return indexMap
            .OrderBy(kvp => kvp.Key) //Order by light ID.
            .Select(kvp => GlyphToKetchumBrightness(kvp.Value)) //Convert brightness range.
            .ToArray();
    }
    #endregion
}
