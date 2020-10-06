using System;

namespace SnapTest.Middleware
{
    public class FunctionCallMiddleware : SnapshotMiddlewareBase
    {
        private readonly Func<SnapshotContext, bool> _process;

        public FunctionCallMiddleware(Func<SnapshotContext, bool> process) { _process = process; }

        public override bool Process(SnapshotContext context)
            => ProcessNext(context) && (_process == null || _process(context));
    }
}
