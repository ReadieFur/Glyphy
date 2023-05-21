using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Glyphy.Configuration
{
    public static class Configuration
    {
        private static string BasePath => FileSystem.Current.AppDataDirectory;

        private static string GetFilePath(Guid id) =>
            Path.Combine(BasePath, id.ToString(), ".json");

        public static async Task<bool> SaveAnimation(SAnimation animation)
        {
            try
            {
                //We don't need to check for duplicate files as they cannot exist in the first place, additionally we will be overwriting the file anyways.
                await File.WriteAllTextAsync(
                    GetFilePath(animation.Id),
                    JsonSerializer.Serialize(animation));
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
                    return null;

                //TODO: Possibly sort data here.
                return JsonSerializer.Deserialize<SAnimation>(await File.ReadAllTextAsync(path));
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
    }
}
