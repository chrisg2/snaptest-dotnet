using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SnapTest
{
    /// <summary>
    /// Settings used to control how snapshot processing is performed within the context of test being executed by a test
    /// framework (such as NUnit or xUnit.net).
    /// </summary>
    public abstract class SnapshotTestFrameworkSettingsBase: SnapshotSettings
    {
        #region Properties
        /// <summary>
        /// Subdirectory name under the directory containing the test source file to store snapshot files in. Defaults to <c>"_snapshots"</c>.
        /// </summary>
        /// <remarks>
        /// <para>If <see cref="SnapshotSettings.SnapshotDirectoryPath"/> is not explicitly set when a <see cref="SnapshotTestFrameworkSettingsBase"/>
        /// object is built, then <c>SnapshotDirectoryPath</c> defaults to the directory containing the test source file is combined with the
        /// <c>SnapshotSubdirectory</c>.</para>
        ///
        /// <para>The value of this property is not used if <c>SnapshotDirectoryPath</c> is explicitly set.</para>
        /// </remarks>
        public string SnapshotSubdirectory { get; set; } = "_snapshots";

        /// <summary>
        /// Flag indicating whether to use the test name as the default for <see cref="SnapshotSettings.SnapshotGroupKey"/>.
        /// If <c>SnapshotGroupKey</c> has otherwise been set to a non-null value then the value of this property is ignored.
        /// </summary>
        public bool DefaultSnapshotGroupKeyFromTestName { get; set; } = false;
        #endregion

        #region Virtual and abstract methods
        /// <inheritdoc/>
        public override void ApplyDefaults()
        {
            // TODO: Call FindTestMethodInStackTrace() *once* and pass result in to the following steps

            // Set various properties that have not otherwise already been set to calculated default values.

            if (string.IsNullOrWhiteSpace(SnapshotName)) {
                SnapshotName = DeriveSnapshotNameFromTestContext() ??
                    throw new SnapTestException(
                        "SnapshotName can only be dynamically determined when accessed while an NUnit test method is executing. " +
                        "To access SnapshotName at other times, you may need to explicitly specify a name when creating the SnapshotConstraint."
                    );
            }

            if (string.IsNullOrWhiteSpace(SnapshotGroupKey) && DefaultSnapshotGroupKeyFromTestName)
                SnapshotGroupKey = DeriveSnapshotGroupKeyFromTestContext();

            if (string.IsNullOrWhiteSpace(SnapshotDirectoryPath)) {
                var d = Path.GetDirectoryName(FindTestMethodInStackTrace().Item2.GetFileName()) ??
                    throw new SnapTestException(
                        "The directory to hold snapshot files could not be determined from the current stack trace. " +
                        "Verify that the stack trace includes a method that is identified as a test method by the test framework, " +
                        "the test assembly has has been built with debugging information, and an async test helpder child method is not being executed. " +
                        "If these conditions cannot be met you may need to explicitly specify a snapshot directory using a settings builder." +
                        "For example: settingsBuilder.WithSettings(s => s.SnapshotDirectoryPath = \"...\"); "
                    );

                SnapshotDirectoryPath = Path.Combine(d, SnapshotSubdirectory ?? string.Empty);
            }
        }

        protected abstract string DeriveSnapshotNameFromTestContext();

        protected abstract string DeriveSnapshotGroupKeyFromTestContext();

        protected virtual (MethodBase, StackFrame) FindTestMethodInStackTrace()
            => (
                from frame in new StackTrace(1, true).GetFrames()
                let m = frame.GetMethod()
                where m != null
                let syncMethod = (IsAsyncMethod(m) ? FindAsynchMethodBase(m) : m)
                where IsTestMethod(syncMethod)
                select (syncMethod, frame)
            ).FirstOrDefault();

        protected abstract bool IsTestMethod(MethodBase method);
        #endregion

        #region Private helper methods
        private static bool IsAsyncMethod(MemberInfo method)
            => typeof(IAsyncStateMachine).IsAssignableFrom(method.DeclaringType);

        private static MethodBase FindAsynchMethodBase(MemberInfo method)
        {
            Type methodDeclaringType = method.DeclaringType;
            Type classDeclaringType = methodDeclaringType?.DeclaringType;

            if (classDeclaringType == null)
                return null;

            return (
                from methodInfo in classDeclaringType.GetMethods()
                let stateMachineAttribute = methodInfo.GetCustomAttribute<AsyncStateMachineAttribute>()
                where stateMachineAttribute != null && stateMachineAttribute.StateMachineType == methodDeclaringType
                select methodInfo
            ).SingleOrDefault();
        }
        #endregion
    }
}
