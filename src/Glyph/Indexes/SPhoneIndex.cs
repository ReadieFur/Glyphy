namespace Glyphy.Glyph.Indexes
{
    public readonly struct SPhoneIndex
    {
        public EPhoneType PhoneType { get; init; } = EPhoneType.Unknown;
        public string Key { get; init; } = string.Empty;
        public ushort Idx { get; init; } = 0;

        public SPhoneIndex(EPhoneType phoneType, string key)
        {
            PhoneType = phoneType;
            Key = key;
            if (!IndexMapper.GetMapping(phoneType).KeyToIdx.TryGetValue(key, out ushort idx))
                throw new ArgumentException($"'{key}' is not a valid key for '{phoneType}'.");
            Idx = idx;
        }

        private SPhoneIndex(EPhoneType phoneType, ushort idx)
        {
            PhoneType = phoneType;
            Key = IndexMapper.GetMapping(phoneType).IdxToKey[idx]; //Should never fail.
            Idx = idx;
        }

        public static implicit operator string(SPhoneIndex self) => self.Key;
        public static implicit operator ushort(SPhoneIndex self) => self.Idx;
        public static implicit operator int(SPhoneIndex self) => self.Idx;
        //I could alternativly attempt to map the index over if the types don't match as opposed to throwing.
        public static implicit operator EPhoneOne(SPhoneIndex self) => self.PhoneType == EPhoneType.PhoneOne ? (EPhoneOne)self : throw new InvalidCastException(); 
        public static implicit operator EPhoneTwo(SPhoneIndex self) => self.PhoneType == EPhoneType.PhoneTwo ? (EPhoneTwo)self : throw new InvalidCastException();
        public static implicit operator EPhoneTwoA(SPhoneIndex self) => self.PhoneType == EPhoneType.PhoneTwoA ? (EPhoneTwoA)self : throw new InvalidCastException();
        public static implicit operator EPhoneThreeA(SPhoneIndex self) => self.PhoneType == EPhoneType.PhoneThreeA ? (EPhoneThreeA)self : throw new InvalidCastException();

        public static implicit operator SPhoneIndex(EPhoneOne self) => new(EPhoneType.PhoneOne, (ushort)self);
        public static implicit operator SPhoneIndex(EPhoneTwo self) => new(EPhoneType.PhoneTwo, (ushort)self);
        public static implicit operator SPhoneIndex(EPhoneTwoA self) => new(EPhoneType.PhoneTwoA, (ushort)self);
        public static implicit operator SPhoneIndex(EPhoneThreeA self) => new(EPhoneType.PhoneThreeA, (ushort)self);
    }
}
