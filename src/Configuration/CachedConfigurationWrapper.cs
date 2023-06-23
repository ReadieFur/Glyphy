using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Glyphy.Configuration
{
    //TODO: Add a version check using the interface and call the respective update method if there is a major version mismatch.
    public class CachedConfigurationWrapper<TSetting> where TSetting : struct, IConfigurationBase //, new()
    {
        private readonly string ItemStoragePath;
        private TSetting? _item = default;

        public CachedConfigurationWrapper(string settingsPath) =>
            ItemStoragePath = settingsPath;

        public async Task<bool> Save(TSetting item)
        {
            try
            {
                _item = item;

                await File.WriteAllTextAsync(
                    ItemStoragePath,
                    JsonConvert.SerializeObject(_item));

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<TSetting?> Load()
        {
            try
            {
                if (!File.Exists(ItemStoragePath))
                    _item = new();
                else
                    _item = JsonConvert.DeserializeObject<TSetting>(await File.ReadAllTextAsync(ItemStoragePath));

                return _item;
            }
            catch
            {
                return default;
            }
        }

        public async Task<TSetting> GetCached()
        {
            if (_item is not null)
                return _item.Value;

            return await Load() ?? throw new NullReferenceException("Failed to load settings.");
        }
    }
}
