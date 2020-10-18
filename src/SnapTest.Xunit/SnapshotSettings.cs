using System.Linq;
using System.Reflection;
using Xunit;

namespace SnapTest.Xunit
{
    /// <summary>
    /// Settings used to control how snapshot processing is performed within the context of an xUnit.net test.
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
            MessageWriter = new XunitMessageWriter();
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
        protected override string DeriveSnapshotNameFromTestContext()
        {
            var method = FindTestMethodInStackTrace().Item1;
            var className = method.ReflectedType.Name;

            return DefaultSnapshotGroupKeyFromTestName ? className : $"{className}.{method.Name}";
        }

        /// <inheritdoc/>
        protected override string DeriveSnapshotGroupKeyFromTestContext()
            => FindTestMethodInStackTrace().Item1.Name;

        /// <inheritdoc/>
        protected override bool IsTestMethod(MethodBase method)
            =>  method.GetCustomAttributes<FactAttribute>().Any()
                || method.GetCustomAttributes<TheoryAttribute>().Any()
            ;
        #endregion
        #endregion
    }
}
