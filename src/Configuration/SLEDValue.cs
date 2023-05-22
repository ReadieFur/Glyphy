using Glyphy.LED;

namespace Glyphy.Configuration
{
    public struct SLEDValue
    {
        public EInterpolationType InterpolationType { get; set; }
        public EAddressable Led { get; set; }
        public double Brightness { get; set; }
    }
}
