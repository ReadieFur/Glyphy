using System.Threading.Tasks;

namespace Glyphy.LED
{
    /// <summary>
    /// An abstract class that contains API methods reachable on all platforms.
    /// </summary>
    public abstract class ALEDAPI
    {
        //I am using floats becuase I don't need 64 bits of precision.
        public abstract Task<float> GetBrightness(EGroup ledGroup);

        public abstract Task<float> GetBrightness(EAddressable addressableLED);

        public abstract Task SetBrightness(EGroup ledGroup, float brightness);

        public abstract Task SetBrightness(EAddressable addressableLED, float brightness);
    }
}
