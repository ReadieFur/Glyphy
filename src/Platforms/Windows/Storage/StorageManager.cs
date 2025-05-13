namespace Glyphy.Storage
{
    internal partial class StorageManager : IStorageManager
    {
        public string ExternalStoragePath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Glyphy");
    }
}
