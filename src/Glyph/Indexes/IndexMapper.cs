namespace Glyphy.Glyph.Indexes
{
    public static class IndexMapper
    {
        public record ReadonlyBiMap<T1, T2>(
            IReadOnlyDictionary<T1, T2> KeyToIdx,
            IReadOnlyDictionary<T2, T1> IdxToKey
        );

        //Faster than querying the enum values for each call.
        //Using this structure compared to just a plain ushort[] is fine because internally the values are stored as an array.
        private static readonly Dictionary<EPhoneType, ReadonlyBiMap<string, ushort>> _mappingsCache = new();

        public static ReadonlyBiMap<string, ushort> GetMapping(EPhoneType phoneType)
        {
            if (!_mappingsCache.ContainsKey(phoneType))
            {
                Dictionary<string, ushort> keyToIdx = new();
                Dictionary<ushort, string> idxToKey = new();

                Type enumType = GetIndexTypeForPhone(phoneType);
                Array values = Enum.GetValuesAsUnderlyingType(enumType);
                foreach (ushort value in values)
                {
                    if (Enum.GetName(enumType, value) is string key)
                    {
                        keyToIdx[key] = value;
                        idxToKey[value] = key;
                    }
                }

                _mappingsCache[phoneType] = new(keyToIdx, idxToKey);
            }

            return _mappingsCache[phoneType];
        }

        private static int[] IndexesToFrameInternal<T>(EPhoneType phoneType, IReadOnlyDictionary<T, double> values)
        {
            Dictionary<ushort, double> indexMap = new();

            if (values is IReadOnlyDictionary<ushort, double> uValues)
            {
                foreach (ushort idx in GetMapping(phoneType).IdxToKey.Keys)
                {
                    double value = 0; //TODO: Test if -1 leaves a Glyph unchanged.
                    uValues.TryGetValue(idx, out value);
                    indexMap[idx] = value;
                }
            }
            else if (values is IReadOnlyDictionary<string, double> sValues)
            {
                foreach (var kvp in GetMapping(phoneType).KeyToIdx)
                {
                    double value = 0;
                    sValues.TryGetValue(kvp.Key, out value);
                    indexMap[kvp.Value] = value;
                }
            }
            else
            {
                //Shouldn't happen.
                throw new InvalidDataException();
            }

            //TODO: Check if there is an invalid mapping? (May be a waste of CPU time).

            return indexMap
                .OrderBy(kvp => kvp.Key) //Order by light ID.
                .Select(kvp => GlyphHelpers.InternalToExternalBrightness(kvp.Value))
                .ToArray();
        }
        /// <summary>
        /// Converts a collection of indexes (by id) and brightness values to an LED array.
        /// </summary>
        internal static int[] IndexesToFrame(EPhoneType phoneType, IReadOnlyDictionary<ushort, double> values) => IndexesToFrameInternal(phoneType, values);
        /// <summary>
        /// Converts a collection of indexes (by name) and brightness values to an LED array.
        /// </summary>
        internal static int[] IndexesToFrame(EPhoneType phoneType, IReadOnlyDictionary<string, double> values) => IndexesToFrameInternal(phoneType, values);

        /// <summary>
        /// Gets the indexing type for a given device.
        /// </summary>
        /// <param name="phoneType">The phone model.</param>
        /// <returns>A variant of typeof(EPhoneIndex)</returns>
        /// <exception cref="ArgumentOutOfRangeException">Throws if the given phone model is invalid.</exception>
        private static Type GetIndexTypeForPhone(EPhoneType phoneType)
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
                    return typeof(EPhoneThreeA);
                default:
                    throw new ArgumentOutOfRangeException(nameof(phoneType));
            }
        }
    }
}
