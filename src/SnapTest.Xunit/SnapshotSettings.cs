using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Xunit;

// TODO: Move common parts shared with SnapTest.NUnit.SnapshotSettings into shared baseclass

// TODO: Do we need to do a "build" of SnapshotSettings, or just use normal object construction for xUnit? An argument for a builder
// may be that SnapshotAssert.Matches modifies settings values, so we don't want passed in settings to get mutated

namespace SnapTest.Xunit
{
    /// <summary>
    /// Settings used to control how snapshot processing is performed within the context of an xUnit.net test.
    /// Instances of the <c>SnapshotSettings</c> class are typically constructed using a <see cref="SnapshotSettingsBuilder"/>.
    /// </summary>
    public class SnapshotSettings: SnapTest.SnapshotSettings
    {
        #region Constructors
        /// <summary>
        /// TODO: Update these comments copied from SnapTest.NUnit:
        /// This default constructor is private to avoid directly creating new objects.
        /// Use <see cref="Build"/> to create and initialize <see cref="SnapshotSettings"/> objects.
        /// </summary>
        private SnapshotSettings() { }
        #endregion

        #region Properties
        /// <summary>
        /// Subdirectory name under the directory containing the xUnit.net test source file to store snapshot files in. Defaults to <c>"_snapshots"</c>.
        /// This value is appended to the <c>SnapshotDirectoryPath</c> when <see cref="SnapshotSettingsBuilder.Build"/> determines
        /// the default snapshot directory path to use. If <c>SnapshotDirectoryPath</c> has otherwise been explicitly set then
        /// the value of this property is ignored.
        /// </summary>
        public string SnapshotSubdirectory { get; set; } = "_snapshots";

        /// <summary>
        /// Flag indicating whether to use the xUnit.net test method name as the default <c>SnapshotGroupKey</c>.
        /// If <c>SnapshotGroupKey</c> has otherwise been set to a non-null value then the value
        /// of this property is ignored.
        /// </summary>
        public bool DefaultSnapshotGroupKeyFromXunitTestMethod { get; set; } = false;
        #endregion

        #region Methods
        // TODO: Document method
        public static SnapshotSettingsBuilder GetBuilder()
            => new SnapshotSettingsBuilder(Build);

        /// <summary>
        /// Create a new <see cref="SnapshotSettings"/> object, invoke initialization actions for it, and
        /// apply default settings.
        /// </summary>
        private static SnapshotSettings Build(IEnumerable<Action<SnapshotSettings>> settingsInitializers = null)
        {
            var s = new SnapshotSettings() { MessageWriter = new XunitMessageWriter() };

            if (settingsInitializers != null) {
                foreach (var initializer in settingsInitializers)
                    initializer(s);
            }

            // Set various properties that have not otherwise already been set to calculated default values.

            if (string.IsNullOrWhiteSpace(s.SnapshotName))
                s.SnapshotName = s.DeriveSnapshotNameFromTestContext();

            if (string.IsNullOrWhiteSpace(s.SnapshotGroupKey))
                s.SnapshotGroupKey = s.DeriveSnapshotGroupKeyFromTestContext();

            if (string.IsNullOrWhiteSpace(s.SnapshotDirectoryPath))
                s.SnapshotDirectoryPath = Path.Combine(GetSnapshotDirectoryPathFromStackTrace(), s.SnapshotSubdirectory ?? string.Empty);

            return s;
        }

        #region Private helper methods
        private static (MethodBase, StackFrame) FindTestMethodInStackTrace()
        {
            var testMethod = (
                from frame in new StackTrace(1, true).GetFrames()
                let m = frame.GetMethod()
                where m != null
                let syncMethod = (IsAsyncMethod(m) ? FindAsynchMethodBase(m) : m)
                where IsTestMethod(syncMethod)
                select new { Method = syncMethod, Frame = frame }
            ).FirstOrDefault();

            if (testMethod == null) {
                throw new SnapTestException(
                    "SnapshotName can only be dynamically determined when accessed while an xUnit.net test method is executing. " +
                    "To access SnapshotName at other times, you may need to explicitly specify a name." // TODO: How?
                );
            }

            return (testMethod.Method, testMethod.Frame);
        }

        private string DeriveSnapshotNameFromTestContext()
        {
            var method = FindTestMethodInStackTrace().Item1;
            var className = method.ReflectedType.Name;

            return DefaultSnapshotGroupKeyFromXunitTestMethod ? className : $"{className}.{method.Name}";
        }

        private string DeriveSnapshotGroupKeyFromTestContext()
            => DefaultSnapshotGroupKeyFromXunitTestMethod ? FindTestMethodInStackTrace().Item1.Name : null;

        private static string GetSnapshotDirectoryPathFromStackTrace()
        {
            var d = Path.GetDirectoryName(FindTestMethodInStackTrace().Item2.GetFileName());

            if (string.IsNullOrEmpty(d)) {
                throw new SnapTestException(
                    "The directory to hold snapshot files could not be determined from the current stack trace. " +
                    "You may need to explicitly specify a snapshot directory using the SnapshotConstraint's SettingsBuilder." +
                    // TODO: Update messaging for xUnit
                    "For example: constraint.WithSettings(s => s.SnapshotDirectoryPath = \"...\"); Alternatively, verify that " +
                    "that the stack trace includes a method marked with one of the xUnit.net test method attributes such as [Test], [TestCase] etc. " +
                    "This error may occur if you have built your test assembly without debugging information, " +
                    "or perform a snapshot match within an async test helper child method."
                );
            }
            return d;
        }

        private static bool IsTestMethod(MethodBase method)
            =>  method.GetCustomAttributes<FactAttribute>().Any()
                || method.GetCustomAttributes<TheoryAttribute>().Any()
                // TODO: Are there any other attributes to look for?
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
