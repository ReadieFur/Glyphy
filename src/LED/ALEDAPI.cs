using System.Threading.Tasks;

namespace Glyphy.LED
{
    /// <summary>
    /// An abstract class that contains API methods reachable on all platforms.
    /// </summary>
    public abstract class ALEDAPI
    {
        public abstract uint MaxBrightness { get; }

        public abstract Task<uint> GetBrightness(EGroup ledGroup);

        public abstract Task<uint> GetBrightness(EAddressable addressableLED);

        public abstract Task SetBrightness(EGroup ledGroup, uint brightness);

        public abstract Task SetBrightness(EAddressable addressableLED, uint brightness);
    }
}
