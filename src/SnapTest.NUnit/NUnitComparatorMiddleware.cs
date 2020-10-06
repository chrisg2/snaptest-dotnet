using NUnit.Framework;
using NUnit.Framework.Constraints;

using SnapTest.Middleware;

namespace SnapTest.NUnit
{
    public class NUnitComparatorMiddleware : SnapshotMiddlewareBase
    {
        public override bool Process(SnapshotContext context)
        {
            if (!ProcessNext(context))
                return false;

            NUnitSnapshotContext nUnitContext = context as NUnitSnapshotContext;

            if (!context.ExpectedValueKnown) {
                nUnitContext.ConstraintResult = new ConstraintResult(nUnitContext.SnapshotConstraint, $"No snapshotted value available to compare to: {nUnitContext.Actual}", false);
            } else {
                // Check result using an NUnit Is.EqualTo constraint.
                // The constraint result is recorded in the context to be returned later by ApplyTo.

                nUnitContext.ConstraintResult = Is.EqualTo(nUnitContext.Expected).ApplyTo(nUnitContext.Actual);
            }

            return nUnitContext.ConstraintResult.Status == ConstraintStatus.Success;
        }
    }
}
