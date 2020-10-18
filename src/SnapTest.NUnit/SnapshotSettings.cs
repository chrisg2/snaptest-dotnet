using NUnit.Framework;
using System.Linq;
using System.Reflection;

namespace SnapTest.NUnit
{
    /// <summary>
    /// Settings used to control how snapshot processing is performed within the context of an NUnit test.
    /// </summary>
    /// <remarks>
    /// Instances of this class are typically obtained by calling <see cref="GetBuilder"/> to get a
    /// <see cref="SnapshotSettingsBuilder&lt;SnapshotSettings&gt;"/>, configuring the builder to set settings values as
    /// appropriate, and then calling the builder's <see cref="SnapshotSettingsBuilder&lt;SnapshotSettings&gt;.Build"/> method.
    /// </remarks>
    public class SnapshotSettings: SnapshotTestFrameworkSettingsBase
    {
        #region Constructors
        /// <summary>
        /// This default constructor is private to avoid other code directly creating new objects.
        /// </summary>
        /// <remarks>
        /// Use <see cref="GetBuilder"/> to get a builder that can be used to create <see cref="SnapshotSettings"/> objects.
        /// </remarks>
        private SnapshotSettings()
        {
            MessageWriter = new NUnitMessageWriter();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Creates a new <see cref="SnapshotSettingsBuilder&lt;SnapshotSettings&gt;"/> that can be used to create
        /// and initialize <see cref="SnapshotSettings"/> objects as needed when a snapshot match operation is performed.
        /// </summary>
        public static SnapshotSettingsBuilder<SnapshotSettings> GetBuilder()
            => new SnapshotSettingsBuilder<SnapshotSettings>(() => new SnapshotSettings());

        #region Base class method overrides
        /// <inheritdoc/>
        protected override string DeriveSnapshotNameFromTestContext(MethodBase _)
        {
            var tc = TestContext.CurrentContext;

            if (tc.Test == null || tc.Test.Name == null || tc.Test.ClassName == null)
                return null;

            var className = tc.Test.ClassName.Split('.').LastOrDefault();

            return DefaultSnapshotGroupKeyFromTestName ? className : $"{className}.{tc.Test.Name}";
        }

        /// <inheritdoc/>
        protected override string DeriveSnapshotGroupKeyFromTestContext(MethodBase _)
            => TestContext.CurrentContext.Test.Name;

        /// <inheritdoc/>
        protected override bool IsTestMethod(MethodBase method)
            =>  method.GetCustomAttributes<TestAttribute>().Any()
                || method.GetCustomAttributes<TestCaseAttribute>().Any()
                || method.GetCustomAttributes<TestCaseSourceAttribute>().Any()
                || method.GetCustomAttributes<TheoryAttribute>().Any()
            ;
        #endregion
        #endregion
    }
}
