namespace SnapTest.Middleware
{
    public abstract class FileStorageMiddlewareBase : SnapshotMiddlewareBase
    {
        public FileStorageOptions Options { get; } = new FileStorageOptions();
    }
}
