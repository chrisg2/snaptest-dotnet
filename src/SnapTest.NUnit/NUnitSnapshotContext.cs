using NUnit.Framework;

namespace SnapTest.NUnit
{
    internal class NUnitSnapshotContext: SnapshotContext
    {
        public readonly SnapshotConstraint SnapshotConstraint;
        public global::NUnit.Framework.Constraints.ConstraintResult ConstraintResult;

        public NUnitSnapshotContext(string testName, SnapshotConstraint snapshotConstraint) : base(testName)
        {
            SnapshotConstraint = snapshotConstraint;
        }

        public override void Message(string message)
            => TestContext.Progress.WriteLine(message);
    }
}
