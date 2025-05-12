namespace Glyphy.Glyph.Indexes
{
    internal static class IndexMapper
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
