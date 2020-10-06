using System;

namespace SnapTest.Middleware
{
    public class SnapshotMiddlewarePipeline
    {
        #region Properties
        public ISnapshotMiddleware MiddlewarePipeline { get; private set; }
        #endregion

        #region Methods
        public SnapshotMiddlewarePipeline Use(ISnapshotMiddleware middleware)
        {
            if (middleware == null)
                throw new ArgumentNullException(nameof(middleware));

            MiddlewarePipeline = middleware.Use(MiddlewarePipeline) ?? middleware;
            return this;
        }

        public SnapshotMiddlewarePipeline Use<T>(Action<T> initializer = null) where T : ISnapshotMiddleware, new()
        {
            var middleware = new T();
            if (initializer != null)
                initializer(middleware);

            return Use(middleware);
        }

        public SnapshotMiddlewarePipeline Use(Func<SnapshotContext, bool> process)
            => Use(new FunctionCallMiddleware(process));

        public SnapshotMiddlewarePipeline Use(Action<SnapshotContext> process)
            => Use(snapshot => { process(snapshot); return true; });
        #endregion
   }
}
