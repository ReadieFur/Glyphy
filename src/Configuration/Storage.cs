using Glyphy.Animation;
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
    public static class Storage
    {
        private static string BasePath => FileSystem.Current.AppDataDirectory;

        private static string GetFilePath(Guid id) =>
            Path.Combine(BasePath, id.ToString() + ".json");

        public static async Task<bool> SaveAnimation(SAnimation animation)
        {
            try
            {
                //We don't need to check for duplicate files as they cannot exist in the first place, additionally we will be overwriting the file anyways.
                await File.WriteAllTextAsync(
                    GetFilePath(animation.Id),
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
                string path = GetFilePath(id);

                if (!File.Exists(path))
                {
                    //If the file dosen't exist, attempt to load one of the presets.
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
                string path = GetFilePath(id);

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
    }
}
