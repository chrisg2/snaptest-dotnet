using System;

using SnapTest.Middleware;

namespace SnapTest
{
    public class Snapshot: SnapshotMiddlewarePipeline
    {
        #region Methods
        public bool CompareTo(object actual, SnapshotContext context = null)
        {
            if (MiddlewarePipeline == null)
                throw new InvalidOperationException($"{nameof(CompareTo)} cannot be called without any middleware configured. Call Use(ISnapshotMiddleware) before calling CompareTo.");

            if (context == null)
                context = new SnapshotContext();

            context.Snapshot = this;
            context.Actual = actual;

            return MiddlewarePipeline.Process(context);
        }
        #endregion

        #region Overrides/New'ed methods
        public new Snapshot Use(ISnapshotMiddleware middleware)
        {
            base.Use(middleware);
            return this;
        }

        public new Snapshot Use<T>(Action<T> initializer = null) where T : ISnapshotMiddleware, new()
        {
            base.Use<T>(initializer);
            return this;
        }

        public new Snapshot Use(Func<SnapshotContext, bool> process)
        {
            base.Use(process);
            return this;
        }

        public new Snapshot Use(Action<SnapshotContext> process)
        {
            base.Use(process);
            return this;
        }
        #endregion
   }
}
