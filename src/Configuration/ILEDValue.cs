using Glyphy.LED;

namespace Glyphy.Configuration
{
    public interface ILEDValue
    {
        public EInterpolationType InterpolationType { get; set; }
        public EAddressable Led { get; set; }
        public uint Brightness { get; set; }
    }
}
