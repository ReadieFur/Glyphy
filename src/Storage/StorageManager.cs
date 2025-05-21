using Glyphy.Animation;
using Newtonsoft.Json;

namespace Glyphy.Storage
{
    internal partial class StorageManager : IStorageManager
    {
        #region Instance
        private static readonly object _INSTANCE_LOCK = new();

        private static StorageManager? _instance = null;
        //I am using this to make the API only initalize when called (as opposed to a static constructor).
        public static StorageManager Instance
        {
            get
            {
                //It is quicker to check if the instance is null before locking, at which point it will be ok to peform the check again.
                if (_instance is null)
                    lock (_INSTANCE_LOCK)
                        if (_instance is null)
                            _instance = new StorageManager();
                return _instance;
            }
        }
        #endregion

        public string InternalStoragePath => FileSystem.Current.AppDataDirectory;

        #region Animation
        private static readonly AnimationJsonConverter _animationJsonConverter = new();

        public async Task SaveAnimation(SAnimation animation)
        {
            if (!Directory.Exists(((IStorageManager)this).ExternalStoragePath))
                Directory.CreateDirectory(((IStorageManager)this).ExternalStoragePath);

            await File.WriteAllTextAsync(
                Path.Combine(((IStorageManager)this).ExternalStoragePath, animation.Id.ToString()) + ".json",
                JsonConvert.SerializeObject(animation, Formatting.Indented, _animationJsonConverter)
            );
        }

        public async Task<SAnimation> LoadAnimation(Guid id)
        {
            string path = Path.Combine(((IStorageManager)this).ExternalStoragePath, id.ToString()) + ".json";
            
            if (!File.Exists(path))
            {
                //TODO: If the file doesn't exist, attempt to load one of the presets.
                throw new FileNotFoundException();
            }

            return JsonConvert.DeserializeObject<SAnimation>(await File.ReadAllTextAsync(path), _animationJsonConverter);
        }

        public void DeleteAnimation(Guid id)
        {
            string path = Path.Combine(((IStorageManager)this).ExternalStoragePath, id.ToString()) + ".json";
            if (!File.Exists(path))
                File.Delete(path);
        }

        public IEnumerable<Guid> GetAnimationIDs()
        {
            //TODO: Implement presets.

            if (!Directory.Exists(((IStorageManager)this).ExternalStoragePath))
                yield break;

            foreach (string path in Directory.EnumerateFiles(((IStorageManager)this).ExternalStoragePath))
                if (Path.GetExtension(path).ToLower() == "json"
                    && Guid.TryParse(Path.GetFileNameWithoutExtension(path), out Guid id))
                    yield return id;
        }
        #endregion

        #region Settings
        private readonly object _SETTINGS_INSTANCE_LOCK = new();

        private Settings? _settings = null;
        //I am using this to make the API only initalize when called (as opposed to a static constructor).
        public Settings Settings
        {
            get
            {
                if (_settings is null)
                {
                    lock (_SETTINGS_INSTANCE_LOCK)
                    {
                        string path = Path.Combine(InternalStoragePath, "settings") + ".json";
                        try { _settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(path)); }
                        catch { }

                        if (_settings is null)
                            _settings = new();
                    }
                }
                return _settings;
            }
        }

        public async Task SaveSettings()
        {
            await File.WriteAllTextAsync(
                Path.Combine(Path.Combine(InternalStoragePath, "settings") + ".json"),
                JsonConvert.SerializeObject(Settings, Formatting.Indented)
            );
        }
        #endregion
    }
}
