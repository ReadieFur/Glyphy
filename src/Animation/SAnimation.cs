using Glyphy.Glyph;
using Glyphy.Glyph.Indexes;
using Glyphy.Misc;

namespace Glyphy.Animation
{
    public struct SAnimation
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = "Untitled";
        public EPhoneType PhoneType { get; set; } = EPhoneType.Unknown;
        public AutoDictionaryInitializer<SPhoneIndex, List<SKeyframe>> Keyframes { get; set; } = new();

        public SAnimation() { }

        //public static bool operator ==(SAnimation left, SAnimation right) => left.Id == right.Id;
        //public static bool operator !=(SAnimation left, SAnimation right) => left.Id != right.Id;
    }
}
