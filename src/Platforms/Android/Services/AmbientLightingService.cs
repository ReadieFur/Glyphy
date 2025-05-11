using Android.App;
using Android.Content;
using AIntentService = Android.App.IntentService;

namespace Glyphy.Platforms.Android.Services
{
    [Service(
        Label = "Glyphy Ambient Service",
        Exported = false
    )]
    public class AmbientLightingService : AIntentService
    {
        //Apparently this is set to be obsolete in the future? (And so OnStartCommand should be used instead), but it's not deprecated yet.
        protected override void OnHandleIntent(Intent? intent) { }
    }
}
