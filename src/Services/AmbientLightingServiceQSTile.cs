using Android.App;
using Android.Content;
using Android.Service.QuickSettings;
using Glyphy.Animation;
using Glyphy.Configuration;
using System.Threading.Tasks;
using Helpers = Glyphy.Misc.Helpers;

namespace Glyphy.Services
{
    //TODO: Figure out what property to set to disable the ">" icon on the right side of the tile.
    [Service(
        Label = "Glyphy Ambient Service",
        Icon = "@drawable/ic_widgets_black_24dp",
        Exported = true, //https://stackoverflow.com/questions/74769099/tile-in-quicksettings-not-working-after-updating-sdk-to-31
        Permission = Android.Manifest.Permission.BindQuickSettingsTile
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

            _ = Task.Run(UpdateTile);
        }

        public override void OnClick()
        {
            base.OnClick();

            _ = Task.Run(async () =>
            {
                if ((QsTile?.State ?? TileState.Unavailable) == TileState.Active)
                    Helpers.StopService<AmbientLightingService>(MainActivity.AMBIENT_SERVICE_IS_FOREGROUND);
                else
                    Helpers.StartService<AmbientLightingService>(MainActivity.AMBIENT_SERVICE_IS_FOREGROUND);

                await UpdateTile();

                SAmbientServiceConfiguration ambientServiceConfiguration = await Storage.AmbientService.GetCached();
                ambientServiceConfiguration.Enabled = (QsTile?.State ?? TileState.Unavailable) == TileState.Active; //Always use the tile's most recent state.
                await Storage.AmbientService.Save(ambientServiceConfiguration);
            });
        }

        private async Task UpdateTile()
        {
            Tile? tile = QsTile;
            if (tile is null)
                return;

            bool isServiceRunning = Helpers.IsServiceRunning<AmbientLightingService>();

            tile.State = isServiceRunning ? TileState.Active : TileState.Inactive;

            if (isServiceRunning)
            {
                SAmbientServiceConfiguration ambientServiceConfiguration = await Storage.AmbientService.GetCached();
                SAnimation? animation = await Storage.LoadAnimation(ambientServiceConfiguration.AnimationID);
                tile.Subtitle = animation?.Name ?? "None";
            }
            else
            {
                tile.Subtitle = "Disabled";
            }

            tile.UpdateTile();
        }
    }
}
