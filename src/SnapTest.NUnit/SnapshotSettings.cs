using NUnit.Framework;
using System.Linq;
using System.Reflection;

namespace SnapTest.NUnit
{
    /// <summary>
    /// Settings used to control how snapshot processing is performed within the context of an NUnit test.
    /// </summary>
    /// <remarks>
    /// Instances of this class are typically obtained by first creating a <see cref="SnapshotSettingsBuilder&lt;SnapshotSettings&gt;"/>
    /// by calling <see cref="GetBuilder"/>, configuring the builder to set settings as appropriate, and then calling the builder's
    /// <see cref="SnapshotSettingsBuilder&lt;SnapshotSettings&gt;.Build"/> method.
    /// </remarks>
    public class SnapshotSettings: SnapshotTestFrameworkSettingsBase
    {
        #region Constructors
        /// <summary>
        /// This default constructor is private to avoid directly creating new objects.
        /// Use <see cref="Build"/> to create and initialize <see cref="SnapshotSettings"/> objects.
        /// </summary>
        private SnapshotSettings() { }
        #endregion

        #region Methods
        /// <summary>
        /// Creates a new <see cref="SnapshotSettingsBuilder&lt;SnapshotSettings&gt;"/> that can be used to create and initialize <see cref="SnapshotSettings"/> objects
        /// as needed when a snapshot match operation is performed.
        /// </summary>
        public static SnapshotSettingsBuilder<SnapshotSettings> GetBuilder()
            => new SnapshotSettingsBuilder<SnapshotSettings>(Build);

        /// <summary>
        /// Create a new <see cref="SnapshotSettings"/> object for snapshot operations with the NUnit testing framework.
        /// </summary>
        private static SnapshotSettings Build()
            => new SnapshotSettings() { MessageWriter = new NUnitMessageWriter() };

        #region Base class method overrides
        /// <inheritdoc/>
        protected override string DeriveSnapshotNameFromTestContext()
        {
            var tc = TestContext.CurrentContext;

            if (tc.Test == null || tc.Test.Name == null || tc.Test.ClassName == null)
                return null;

            var className = tc.Test.ClassName.Split('.').LastOrDefault();

            return DefaultSnapshotGroupKeyFromTestName ? className : $"{className}.{tc.Test.Name}";
        }

        /// <inheritdoc/>
        protected override string DeriveSnapshotGroupKeyFromTestContext()
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
