using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace SnapTest.NUnit
{
    /// <summary>
    /// <see cref="ISnapshotComparer"/> implementation that performs a snapshot comparison within the context of an NUnit test.
    /// </summary>
    internal class NUnitSnapshotComparer : SnapshotComparer
    {
        public ConstraintResult ConstraintResult;
        private readonly IConstraint constraint;
        private readonly SnapshotSettings settings;

        public NUnitSnapshotComparer(IConstraint constraint, SnapshotSettings settings)
        {
            this.constraint = constraint;
            this.settings = settings;
        }

        /// <summary>
        /// Compares an actual value to a snapshotted value. Details from the result of the comparison
        /// are stored in <see cref="ConstraintResult"/> for later access by <see cref="SnapshotConstraint.ApplyTo"/>.
        /// </summary>
        /// <seealso cref="SnapshotComparer.Compare"/>
        public override bool Compare(SnapshotValue actualValue, SnapshotValue snapshottedValue)
        {
            if (snapshottedValue == null) {
                ConstraintResult = new ConstraintResult(constraint, $"No snapshotted value available in {settings.SnapshotFilePath}", false);
                return false;
            }

            if (base.Compare(actualValue, snapshottedValue))
                ConstraintResult = new ConstraintResult(constraint, actualValue, true);
            else
                ConstraintResult = Is.EqualTo(snapshottedValue.Serialize(false)).ApplyTo(actualValue.Serialize(false));

            return ConstraintResult.Status == ConstraintStatus.Success;
        }
    }
}
