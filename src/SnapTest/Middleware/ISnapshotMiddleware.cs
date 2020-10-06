namespace SnapTest.Middleware
{
    public interface ISnapshotMiddleware
    {
        ISnapshotMiddleware Use(ISnapshotMiddleware next);
        bool Process(SnapshotContext context);
    }
}
