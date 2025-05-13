using Environment = Android.OS.Environment;
using JFile = Java.IO.File;

namespace Glyphy.Storage
{
    internal partial class StorageManager : IStorageManager
    {
        string IStorageManager.ExternalStoragePath => _externalStoragePath;

        private readonly string _externalStoragePath;

        public StorageManager()
        {
            JFile? javaPath = Environment.GetExternalStoragePublicDirectory(Environment.DirectoryDocuments);
            if (javaPath is null)
                throw new NullReferenceException();
            _externalStoragePath = Path.Combine(javaPath.AbsolutePath, "Glyphy");
        }
    }
}
