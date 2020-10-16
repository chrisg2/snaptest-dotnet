using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SnapTest.NUnit
{
    /// <summary>
    /// Settings used to control how snapshot processing is performed within the context of an NUnit test.
    /// Instancse of the <c>SnapshotSettings</c> class are typically constructed using a <see cref="SnapshotSettingsBuilder"/>.
    /// </summary>
    public class SnapshotSettings: SnapTest.SnapshotSettings
    {
        #region Constructors
        /// <summary>
        /// This default constructor is private to avoid directly creating new objects.
        /// Use <see cref="Build"/> to create and initialize <see cref="SnapshotSettings"/> objects.
        /// </summary>
        private SnapshotSettings() { }
        #endregion

        #region Properties
        /// <summary>
        /// Subdirectory name under the directory containing the NUnit test source file to store snapshot files in. Defaults to <c>"_snapshots"</c>.
        /// This value is appended to the <c>SnapshotDirectoryPath</c> when <see cref="SnapshotSettingsBuilder.Build"/> determines
        /// the default snapshot directory path to use. If <c>SnapshotDirectoryPath</c> has otherwise been explicitly set then
        /// the value of this property is ignored.
        /// </summary>
        public string SnapshotSubdirectory { get; set; } = "_snapshots";

        /// <summary>
        /// Flag indicating whether to use the NUnit test name (taken from <c><see cref="TestContext.CurrentContext"/>.Test.TestName</c>)
        /// as the default <c>SnapshotGroup</c>. If <c>SnapshotGroup</c> has otherwise been set to a non-null value then the value
        /// of this property is ignored.
        /// </summary>
        public bool DefaultSnapshotGroupFromNUnitTestName { get; set; } = false;
        #endregion

        #region Methods
        /// <summary>
        /// Create a new <see cref="SnapshotSettings"/> object, invoke initialization actions for it, and
        /// apply default settings.
        /// </summary>
        internal static SnapshotSettings Build(IEnumerable<Action<SnapshotSettings>> settingsInitializers)
        {
            var s = new SnapshotSettings() { MessageWriter = new NUnitMessageWriter() };

            foreach (var initializer in settingsInitializers)
                initializer(s);

            // Set various properties that have not otherwise already been set to calculated default values.

            if (string.IsNullOrWhiteSpace(s.SnapshotName))
                s.SnapshotName = s.DeriveSnapshotNameFromTestContext();

            if (string.IsNullOrWhiteSpace(s.SnapshotGroup))
                s.SnapshotGroup = s.DeriveSnapshotGroupFromTestContext();

            if (string.IsNullOrWhiteSpace(s.SnapshotDirectoryPath))
                s.SnapshotDirectoryPath = Path.Combine(GetSnapshotDirectoryPathFromStackTrace(), s.SnapshotSubdirectory ?? string.Empty);

            return s;
        }

        #region Private helper methods
        private string DeriveSnapshotNameFromTestContext()
        {
            var tc = TestContext.CurrentContext;

            if (tc.Test == null || tc.Test.Name == null || tc.Test.ClassName == null) {
                throw new SnapTestException(
                    "SnapshotName can only be dynamically determined when accessed from while an NUnit test method is executing. " +
                    "To access SnapshotName at other times, you may need to explicitly specify a name when creating the SnapshotConstraint."
                );
            }

            var classNameParts = tc.Test.ClassName.Split('.');
            var className = classNameParts[classNameParts.Length - 1];

            return DefaultSnapshotGroupFromNUnitTestName ? className : $"{className}.{tc.Test.Name}";
        }

        private string DeriveSnapshotGroupFromTestContext()
            => DefaultSnapshotGroupFromNUnitTestName ? TestContext.CurrentContext.Test.Name : null;

        private static string GetSnapshotDirectoryPathFromStackTrace()
        {
            var d = (
                from frame in new StackTrace(1, true).GetFrames()
                let method = frame.GetMethod()
                where method != null
                let syncMethod = (IsAsyncMethod(method) ? FindAsynchMethodBase(method) : method)
                where IsTestMethod(syncMethod)
                select Path.GetDirectoryName(frame.GetFileName())
            ).FirstOrDefault();

            if (string.IsNullOrEmpty(d)) {
                throw new SnapTestException(
                    "The directory to hold snapshot files could not be determined from the current stack trace. " +
                    "You may need to explicitly specify a snapshot directory using the SnapshotConstraint's SettingsBuilder." +
                    "For example: constraint.WithSettings(s => s.SnapshotDirectoryPath = \"...\"); Alternatively, verify that " +
                    "that the stack trace includes a method marked with one of the NUnit test method attributes such as [Test], [TestCase] etc. " +
                    "This error may occur if you have built your test assembly without debugging information, " +
                    "or perform a snapshot match within an async test helper child method."
                );
            }
            return d;
        }

        private static bool IsTestMethod(MethodBase method)
            =>  method.GetCustomAttributes<TestAttribute>().Any()
                || method.GetCustomAttributes<TestCaseAttribute>().Any()
                || method.GetCustomAttributes<TestCaseSourceAttribute>().Any()
                || method.GetCustomAttributes<TheoryAttribute>().Any()
            ;

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
        #endregion
    }
}
