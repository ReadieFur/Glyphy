namespace Glyphy.Glyph
{
    internal static class GlyphHelpers
    {
        private const int GLYPH_MIN_BRIGHTNESS = 0;
        private const int GLYPH_MAX_BRIGHTNESS = 4096;
        private const double INTERNAL_MIN_BRIGHTNESS = 0;
        private const double INTERNAL_MAX_BRIGHTNESS = 1;

        /// <summary>
        /// Convert a brightness from the range used by this API to the one used by the Glyph interface.
        /// </summary>
        public static int InternalToExternalBrightness(double brightness)
        {
            return (int)Math.Clamp(
                Helpers.ConvertNumberRange(brightness, INTERNAL_MIN_BRIGHTNESS, INTERNAL_MAX_BRIGHTNESS, GLYPH_MIN_BRIGHTNESS, GLYPH_MAX_BRIGHTNESS),
                GLYPH_MIN_BRIGHTNESS,
                GLYPH_MAX_BRIGHTNESS);
        }

        /// <summary>
        /// Convert a brightness from the range used by this API to the one used by the Glyph interface.
        /// </summary>
        public static double ExternalToInternalBrightness(int brightness)
        {
            return Math.Clamp(
                Helpers.ConvertNumberRange(brightness, GLYPH_MIN_BRIGHTNESS, GLYPH_MAX_BRIGHTNESS, INTERNAL_MIN_BRIGHTNESS, INTERNAL_MAX_BRIGHTNESS),
                INTERNAL_MIN_BRIGHTNESS,
                INTERNAL_MAX_BRIGHTNESS);
        }
    }
}
