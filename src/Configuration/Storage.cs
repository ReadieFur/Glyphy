using Glyphy.Animation;
using Glyphy.Configuration.NotificationConfiguration;
using Glyphy.Resources.Presets;
using Microsoft.Maui.Storage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Glyphy.Configuration
{
    //TODO: Store any NotificationService related items in memory for faster access times as well as keeping track of last write times for these values so they can be updated from the disk when necessary.
    public static class Storage
    {
        public static string BasePath => FileSystem.Current.AppDataDirectory;

        public static readonly CachedConfigurationWrapper<SNotificationConfigurationRoot> NotificationServiceSettings = new(Path.Combine(BasePath, "notification_service_configuration.json"));
        public static readonly CachedConfigurationWrapper<SSettings> Settings = new(Path.Combine(BasePath, "settings.json"));
        public static readonly CachedConfigurationWrapper<SAmbientServiceConfiguration> AmbientService = new(Path.Combine(BasePath, "ambient_service.json"));

        #region Animations
        private static string GetAnimationFilePath(Guid id) =>
            Path.Combine(BasePath, id.ToString() + ".json");

        public static async Task<bool> SaveAnimation(SAnimation animation)
        {
            try
            {
                //We don't need to check for duplicate files as they cannot exist in the first place, additionally we will be overwriting the file anyways.
                await File.WriteAllTextAsync(
                    GetAnimationFilePath(animation.Id),
                    JsonConvert.SerializeObject(animation));
            }
            catch
            {
                return false;
            }

            return true;
        }

        public static async Task<SAnimation?> LoadAnimation(Guid id)
        {
            try
            {
                string path = GetAnimationFilePath(id);

                if (!File.Exists(path))
                {
                    //If the file doesn't exist, attempt to load one of the presets.
                    SAnimation preset = Glyphs.Presets.FirstOrDefault(a => a.Id == id);
                    if (preset.Equals(default(SAnimation)))
                        return null;

                    return preset;
                }

                //TODO: Possibly sort data here (make sure the frames are in order).
                return JsonConvert.DeserializeObject<SAnimation>(await File.ReadAllTextAsync(path));
            }
            catch
            {
                return null;
            }
        }

        public static IEnumerable<Guid> GetAnimationIDs()
        {
            foreach (SAnimation preset in Glyphs.Presets)
                yield return preset.Id;

            foreach (string path in Directory.EnumerateFiles(BasePath))
            {
                string fileName = Path.GetFileNameWithoutExtension(path);
                if (Guid.TryParse(fileName, out Guid id))
                    yield return id;
            }
        }

        public static bool DeleteAnimation(Guid id)
        {
            try
            {
                string path = GetAnimationFilePath(id);

                if (!File.Exists(path))
                    return false;

                File.Delete(path);
            }
            catch
            {
                return false;
            }

            return true;
        }
        #endregion
    }
}
