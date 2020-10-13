using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace SnapTest.NUnit
{
    internal class NUnitSnapshotComparer : SnapshotComparer
    {
        public ConstraintResult ConstraintResult;
        private readonly IConstraint _constraint;

        public NUnitSnapshotComparer(IConstraint constraint) { _constraint = constraint; }

        public override bool Compare(SnapshotValue actualValue, SnapshotValue snapshottedValue)
        {
            bool result;
            if (snapshottedValue == null) {
                ConstraintResult = new ConstraintResult(_constraint, $"No snapshotted value available", false);
                result = false;
            } else {
                result = base.Compare(actualValue, snapshottedValue);

                if (result)
                    ConstraintResult = new ConstraintResult(_constraint, actualValue, true);
                else {
                    // Check result using an NUnit Is.EqualTo constraint.
                    // The constraint result is recorded in ConstraintResult to be returned later by SnapshotConstraint.ApplyTo.

                    ConstraintResult = Is.EqualTo(snapshottedValue.Serialize(false)).ApplyTo(actualValue.Serialize(false));
                }
            }

            return ConstraintResult.Status == ConstraintStatus.Success;
        }
    }
}
