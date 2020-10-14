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
    public class SnapshotSettingsBuilder
    {
        private List<Action<SnapshotSettings>> settingsInitializers = new List<Action<SnapshotSettings>>();

        public SnapshotSettings Build()
        {
            var s = new SnapshotSettings() {
                MessageWriter = new NUnitMessageWriter()
            };

            settingsInitializers.ForEach(_ => _(s));

            if (string.IsNullOrWhiteSpace(s.TestName))
                s.TestName = GetTestNameFromTestContext(s);

            if (string.IsNullOrWhiteSpace(s.SnapshotGroup))
                s.SnapshotGroup = GetSnapshotGroupFromTestContext(s);

            if (string.IsNullOrWhiteSpace(s.SnapshotDirectory)) {
                var d = GetSnapshotDirectoryFromStackTrace();
                if (!string.IsNullOrEmpty(d))
                    s.SnapshotDirectory = Path.Combine(d, s.SnapshotDirectoryTail ?? string.Empty);
            }

            return s;
        }

        /// <summary>
        /// Register an action to be called to set properties on a <see cref="SnapshotSettings"/> object when <see cref="Build"/> is called.
        /// </summary>
        /// <param name="settingsInitializer">
        /// The action to be called to initialize settings when <see cref="Build"/> is called.
        /// A <see cref="SnapshotSettings"/> object is passed as the single parameter to the action.
        /// The action should set properties as appropriate on this object.
        /// </param>
        /// <returns>This SettingsBuilder</returns>
        public SnapshotSettingsBuilder WithSettings(Action<SnapshotSettings> settingsInitializer)
        {
            if (settingsInitializer == null)
                throw new ArgumentNullException(nameof(settingsInitializer));

            settingsInitializers.Add(settingsInitializer);
            return this;
        }

        #region Helper methods
        private static string GetTestNameFromTestContext(SnapshotSettings settings)
        {
            var tc = TestContext.CurrentContext;

            if (tc.Test == null || tc.Test.Name == null || tc.Test.ClassName == null) {
                throw new SnapTestException(
                    "TestName can only be dynamically determined when accessed from while an NUnit test method is executing. " +
                    "To access TestName at other times, you may need to explicitly specify a name when creating the SnapshotConstraint."
                );
            }

            var classNameParts = tc.Test.ClassName.Split('.');
            var className = classNameParts[classNameParts.Length - 1];
            return settings.DefaultSnapshotGroupFromNUnitTestName ? className : $"{className}.{tc.Test.Name}";
        }

        private static string GetSnapshotGroupFromTestContext(SnapshotSettings settings)
            => settings.DefaultSnapshotGroupFromNUnitTestName ? TestContext.CurrentContext.Test.Name : null;

        private static string GetSnapshotDirectoryFromStackTrace()
            => (
                from frame in new StackTrace(1, true).GetFrames()
                let method = frame.GetMethod()
                where method != null
                let syncMethod = (IsAsyncMethod(method) ? FindAsynchMethodBase(method) : method)
                where IsTestMethod(syncMethod)
                select Path.GetDirectoryName(frame.GetFileName())
            ).FirstOrDefault() ?? string.Empty;

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
    }
}
