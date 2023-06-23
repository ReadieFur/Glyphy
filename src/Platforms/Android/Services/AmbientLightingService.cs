using Android.App;
using Android.Content;

namespace Glyphy.Platforms.Android.Services
{
    [Service(
        Label = "Glyphy Ambient Service",
        Exported = false
    )]
    public class AmbientLightingService : IntentService
    {
        protected override void OnHandleIntent(Intent? intent)
        {
        }
    }
}
