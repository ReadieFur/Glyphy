using Newtonsoft.Json;

namespace Glyphy.Configuration
{
    public interface IConfigurationBase
    {
        //Annoyingly attributes are not inherited by subclasses, however it's not a huge deal as we construct the subclasses by this interface.
        [JsonProperty("version")]
        float Version { get; set; }
    }
}
