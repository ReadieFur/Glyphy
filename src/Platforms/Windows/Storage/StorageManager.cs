namespace Glyphy.Storage
{
    internal partial class StorageManager : IStorageManager
    {
        string IStorageManager.ExternalStoragePath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Glyphy");
    }
}
