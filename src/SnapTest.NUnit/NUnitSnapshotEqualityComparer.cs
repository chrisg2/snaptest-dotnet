using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace SnapTest.NUnit
{
    /// <summary>
    /// <see cref="ISnapshotEqualityComparer"/> implementation that performs a snapshot comparison within the context of an NUnit test.
    /// </summary>
    internal class NUnitSnapshotEqualityComparer : SnapshotEqualityComparer
    {
        public ConstraintResult ConstraintResult;
        private readonly IConstraint constraint;
        private readonly SnapshotSettings settings;

        public NUnitSnapshotEqualityComparer(IConstraint constraint, SnapshotSettings settings)
        {
            this.constraint = constraint;
            this.settings = settings;
        }

        /// <summary>
        /// Compares an actual value to a snapshotted value. Details from the result of the comparison
        /// are stored in <see cref="ConstraintResult"/> for later access by <see cref="SnapshotConstraint.ApplyTo"/>.
        /// </summary>
        /// <seealso cref="SnapshotEqualityComparer.Equals"/>
        public override bool Equals(SnapshotValue actualValue, SnapshotValue snapshottedValue)
        {
            if (snapshottedValue == null) {
                ConstraintResult = new ConstraintResult(constraint, $"No snapshotted value available in {settings.SnapshotFilePath}", false);
                return false;
            }

            if (base.Equals(actualValue, snapshottedValue))
                ConstraintResult = new ConstraintResult(constraint, actualValue, true);
            else
                ConstraintResult = Is.EqualTo(snapshottedValue.Serialize(false)).ApplyTo(actualValue.Serialize(false));

            return ConstraintResult.Status == ConstraintStatus.Success;
        }
    }
}
