using System;

namespace SnapTest.Middleware
{
    public class StringComparatorMiddleware : SnapshotMiddlewareBase
    {
        public override bool Process(SnapshotContext context)
        {
            if (!ProcessNext(context))
                return false;

            return
                context.ExpectedValueKnown &&
                string.Compare(context.Actual?.ToString() ?? string.Empty, context.Expected?.ToString() ?? string.Empty, StringComparison.InvariantCultureIgnoreCase) == 0;
        }
    }
}
