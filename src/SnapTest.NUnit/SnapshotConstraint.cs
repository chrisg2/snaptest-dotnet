using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace SnapTest.NUnit
{
    /// <summary>
    /// </summary>
    public class SnapshotConstraint : Constraint
    {
        #region Static and instance fields
        /// <summary>
        /// Field to store the snapshot name.
        /// If this is null, the test name is dynamically determined from the NUnit TestContext.
        /// </summary>
        private string _testName;

        private SnapshotBuilderBase _snapshotBuilder;
        #endregion

        #region Constructors
        public SnapshotConstraint(string testName = null, SnapshotBuilderBase snapshotBuilder = null)
        {
            _testName = testName;
            _snapshotBuilder = snapshotBuilder ?? new SnapshotBuilder();
        }

        public SnapshotConstraint(SnapshotBuilderBase snapshotBuilder) : this(null, snapshotBuilder) { }
        #endregion

        #region Properties
        public string TestName
        {
            get {
                if (_testName != null)
                    return _testName; // Use explicitly set test name

                // Otherwise fall back to using dynamically determined test name
                var tc = TestContext.CurrentContext;

                if (tc.Test == null || tc.Test.Name == null || tc.Test.ClassName == null) {
                    throw new SnapTestException(
                        "TestName can only be dynamically determined when accessed from while an NUnit test method is executing. " +
                        "To access TestName at other times, you may need to explicitly specify a name when creating the SnapshotConstraint."
                    );
                }

                var classNameParts = tc.Test.ClassName.Split('.');
                return $"{classNameParts[classNameParts.Length - 1]}.{tc.Test.Name}";
            }

            set {
                _testName = value;
            }
        }
        #endregion

        #region Overrides
        public override string Description
            => $"snapshotted value from {_snapshotBuilder.BuildFileStorageOptions().GetSnapshotFilePath(TestName)}";

        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            if (string.IsNullOrEmpty(_snapshotBuilder.BuildFileStorageOptions().SnapshotDirectory)) {
                throw new SnapTestException(
                    "The directory to hold snapshot files could not be determined from the current stack trace. " +
                    "You may need to explicitly specify a source directory when creating the SnapshotConstraint, " + // TODO: Add example of how?
                    "or verify that the stack trace includes a method marked with one of the NUnit test method attributes such as [Test], [TestCase] etc. " +
                    "This error may occur if you perform a snapshot match within an async test helper child method."
                );
            }

            var context = new NUnitSnapshotContext(TestName, this);

            bool result = _snapshotBuilder.Build().CompareTo(actual, context);

            return context.ConstraintResult ?? new ConstraintResult(this, actual, result);
        }
        #endregion
   }
}
