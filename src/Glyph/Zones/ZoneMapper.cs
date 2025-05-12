namespace Glyphy.Glyph.Zones
{
    internal static class ZoneMapper
    {
        /// <summary>
        /// Converts an input zone map to the desired platform.
        /// </summary>
        /// <typeparam name="TTargetMapping">The zone map to convert to.</typeparam>
        /// <typeparam name="TIn">The zone map to convert from, either a string or variant of EPhoneZones.</typeparam>
        /// <param name="source">The zone map to convert from.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static Dictionary<TTargetMapping, int> ZoneToZone<TTargetMapping, TIn>(IReadOnlyDictionary<TIn, int> source) where TTargetMapping : struct, Enum where TIn : class, Enum
        {
            Type tOutType = typeof(TTargetMapping);
            if (!IsEnumZone<TTargetMapping>())
                throw new ArgumentException("Unsupported TTargetMapping type");

            Type tInType = typeof(TIn);
            if (tInType != typeof(string) &&
                IsEnumZone<TIn>())
                throw new ArgumentException("Unsupported TIn type");

            Dictionary<TTargetMapping, int> outMapping = new();

            foreach (var kvp in source)
            {
                string? key = tInType.IsEnum ? tInType.GetEnumName(kvp.Key) : kvp.Key as string;

                if (!Enum.TryParse(key, true, out TTargetMapping outKey))
                    continue;

                outMapping.Add(outKey, kvp.Value);
            }

            return outMapping;
        }

        /// <summary>
        /// Converts a typed zone to an LED array.
        /// </summary>
        /// <typeparam name="TZone">A variant of EPhoneZones</typeparam>
        /// <returns>A list of LED brightness values to be passed to GlyphManager.SetFrameColors()</returns>
        public static int[] ZoneToArray<TZone>(IReadOnlyDictionary<TZone, double> source) where TZone : struct, Enum
        {
            Type tZoneType = typeof(TZone);
            if (!IsEnumZone<TZone>())
                throw new ArgumentException("Unsupported TZoneType type");

            Dictionary<TZone, double> copy = new(source);

            foreach (TZone value in tZoneType.GetEnumValues())
                if (!copy.ContainsKey(value))
                    copy.Add(value, 0);

            return copy
                .OrderBy(kvp => (ushort)(object)kvp.Key)
                .Select(kvp => GlyphHelpers.InternalToExternalBrightness(kvp.Value))
                .ToArray();
        }

        /// <summary>
        /// Attempts to sterilize an LED array to a given platforms zone mapping.
        /// </summary>
        /// <typeparam name="TZone">The zone platform to sterilize to.</typeparam>
        /// <param name="source">The input LED array.</param>
        /// <returns>The specified platforms zone mapping.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static Dictionary<TZone, double> ArrayToZone<TZone>(int[] source) where TZone : struct, Enum
        {
            Type tZoneType = typeof(TZone);
            if (!IsEnumZone<TZone>())
                throw new ArgumentException("Unsupported TZoneType type");

            if (source.Length != tZoneType.GetEnumValues().Length)
                throw new ArgumentOutOfRangeException("Source length does not match the expected length for the given zone type.");

            Dictionary<TZone, double> mapping = new();
            for (ushort i = 0; i < source.Length; i++)
                mapping.Add((TZone)Enum.ToObject(typeof(TZone), i), GlyphHelpers.ExternalToInternalBrightness(source[i]));
            return mapping;
        }

        /// <summary>
        /// Checks if the enum type is a variant of EPhoneZone.
        /// </summary>
        /// <returns>True if TZone is a variant of EPhoneZone, otherwise false.</returns>
        public static bool IsEnumZone<TZone>() where TZone : Enum
        {
            Type tZoneType = typeof(TZone);
            return tZoneType == typeof(EPhoneOne) ||
                tZoneType != typeof(EPhoneTwo) ||
                tZoneType != typeof(EPhoneTwoA) ||
                tZoneType != typeof(EPhoneThreeAZones);
        }

        /// <summary>
        /// Gets the zone mapping for a given device.
        /// </summary>
        /// <param name="phoneType">The phone model.</param>
        /// <returns>A variant of typeof(EPhoneZone)</returns>
        /// <exception cref="ArgumentOutOfRangeException">Throws if the given phone model is invalid.</exception>
        public static Type GetZoneTypeForDevice(EPhoneType phoneType)
        {
            switch (phoneType)
            {
                case EPhoneType.PhoneOne:
                    return typeof(EPhoneOne);
                case EPhoneType.PhoneTwo:
                    return typeof(EPhoneTwo);
                case EPhoneType.PhoneTwoA:
                case EPhoneType.PhoneTwoAPlus:
                    return typeof(EPhoneTwoA);
                case EPhoneType.PhoneThreeA:
                    //case EPhoneType.PhoneThreeAPro: //This uses the same identifier as the regular 3a so this clause must be skipped.
                    return typeof(EPhoneThreeAZones);
                default:
                    throw new ArgumentOutOfRangeException(nameof(phoneType));
            }
        }

        /// <summary>
        /// Convert a zone to it's underlying Glyph ID.
        /// </summary>
        public static ushort ZoneToID<TZone>(TZone zone) where TZone : struct, Enum
        {
            Type tZoneType = typeof(TZone);
            if (!IsEnumZone<TZone>())
                throw new ArgumentException("Unsupported TZoneType type");

            if (Convert.ChangeType(zone, Enum.GetUnderlyingType(tZoneType)) is not ushort id)
                throw new InvalidCastException();

            return id;
        }

        /// <summary>
        /// Convert an ID to a zone.
        /// </summary>
        /// <typeparam name="TZone">The target zone.</typeparam>
        public static TZone IDToZone<TZone>(ushort id) where TZone : struct, Enum => (TZone)Enum.ToObject(typeof(TZone), id);
    }
}
