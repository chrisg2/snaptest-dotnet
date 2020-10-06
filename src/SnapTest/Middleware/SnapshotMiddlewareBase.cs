namespace SnapTest.Middleware
{
    public abstract class SnapshotMiddlewareBase : ISnapshotMiddleware
    {
        private ISnapshotMiddleware _next;

        public SnapshotMiddlewareBase() { }

        public ISnapshotMiddleware Use(ISnapshotMiddleware next)
        {
            if (next != null) {
                if (_next == null)
                    _next = next;
                else
                    _next.Use(next);
            }

            return this;
        }

        public bool ProcessNext(SnapshotContext context) { return _next == null ? true : _next.Process(context); }
        public abstract bool Process(SnapshotContext context);
    }
}
