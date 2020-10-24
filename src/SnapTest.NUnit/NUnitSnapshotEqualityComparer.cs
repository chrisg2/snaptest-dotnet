using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace SnapTest.NUnit
{
    /// <summary>
    /// <see cref="ISnapshotEqualityComparer"/> implementation that performs a snapshot value comparison within the context of an NUnit test.
    /// </summary>
    internal class NUnitSnapshotEqualityComparer : SnapshotEqualityComparer
    {
        #region Fields
        public ConstraintResult ConstraintResult;
        private readonly IConstraint constraint;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor that initializes an <see cref="NUnitSnapshotEqualityComparer"/> to work with the specified
        /// <see cref="IConstraint"/> and <see cref="SnapshotSettings"/>.
        /// </summary>
        /// <param name="constraint">The NUnit constraint the comparer is to be used with.</param>
        internal NUnitSnapshotEqualityComparer(IConstraint constraint)
        {
            this.constraint = constraint;
        }
        #endregion

        #region Method overrides
        /// <summary>
        /// Compares an actual value to a snapshotted value. Details from the result of the comparison
        /// are stored in <see cref="ConstraintResult"/> for later access by <see cref="SnapshotConstraint.ApplyTo"/>.
        /// </summary>
        /// <seealso cref="SnapshotEqualityComparer.Equals"/>
        public override bool Equals(SnapshotValue actualValue, SnapshotValue snapshottedValue, SnapTest.SnapshotSettings settings)
        {
            if (snapshottedValue == null) {
                ConstraintResult = new ConstraintResult(constraint, $"No snapshotted value available in {settings.SnapshotFilePath}", false);
                return false;
            }

            if (base.Equals(actualValue, snapshottedValue, settings))
                ConstraintResult = new ConstraintResult(constraint, actualValue, true);
            else
                ConstraintResult = Is.EqualTo(snapshottedValue.Serialize(false)).ApplyTo(actualValue.Serialize(false));

            return ConstraintResult.Status == ConstraintStatus.Success;
        }
        #endregion
    }
}
