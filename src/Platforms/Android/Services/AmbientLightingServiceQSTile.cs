using Android.App;
using Android.Content;
using Android.Service.QuickSettings;
using Glyphy.Configuration;
using System.Threading.Tasks;

namespace Glyphy.Platforms.Android.Services
{
    [Service(
        Label = "Glyphy Ambient QS",
        Icon = "@drawable/ic_widgets_black_24dp",
        Exported = true, //https://stackoverflow.com/questions/74769099/tile-in-quicksettings-not-working-after-updating-sdk-to-31
        Permission = global::Android.Manifest.Permission.BindQuickSettingsTile
    )]
    [IntentFilter(new[] { "android.service.quicksettings.action.QS_TILE" })]
    [MetaData(MetaDataActiveTile, Value = "true")]
    [MetaData(MetaDataToggleableTile, Value = "true")]
    public class AmbientLightingServiceQSTile : TileService
    {
        public override void OnCreate()
        {
            base.OnCreate();
        }

        public override void OnStartListening()
        {
            base.OnStartListening();

            Tile? tile = QsTile;
            if (tile is null)
                return;

            tile.State = Helpers.IsServiceRunning<AmbientLightingService>() ? TileState.Active : TileState.Inactive;
            tile.UpdateTile();
        }

        public override void OnClick()
        {
            base.OnClick();

            Tile? tile = QsTile;
            if (tile is null)
                return;

            if (tile.State == TileState.Active)
                Helpers.StopService<AmbientLightingService>(MainActivity.AMBIENT_SERVICE_IS_FOREGROUND);
            else
                Helpers.StartService<AmbientLightingService>(MainActivity.AMBIENT_SERVICE_IS_FOREGROUND);

            tile.State = Helpers.IsServiceRunning<AmbientLightingService>() ? TileState.Active : TileState.Inactive;
            tile.UpdateTile();

            Task.Run(async () =>
            {
                SAmbientServiceConfiguration ambientServiceConfiguration = await Storage.AmbientService.GetCached();
                ambientServiceConfiguration.Enabled = tile.State == TileState.Active;
                await Storage.AmbientService.Save(ambientServiceConfiguration);
            });
        }
    }
}
