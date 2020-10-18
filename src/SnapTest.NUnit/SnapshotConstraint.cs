using NUnit.Framework;
using NUnit.Framework.Constraints;
using System;

namespace SnapTest.NUnit
{
    /// <summary>
    /// NUnit <see cref="Constraint"/> that compares an actual value against an expected value that is stored
    /// in a snapshot file.
    /// </summary>
    public class SnapshotConstraint : Constraint
    {
        #region Constructors
        /// <summary>
        /// Construct a <see cref="SnapshotConstraint"/> that will use settings from <paramref name="settingsBuilder"/> (if supplied),
        /// or a default <see cref="SnapshotSettingsBuilder&lt;SnapshotSettings&gt;"/> (if not supplied).
        /// </summary>
        /// <param name="settingsBuilder">A builder to create a <see cref="SnapshotSettings"/> object when needed
        /// to perform a snapshot comparison. This value (if supplied) is used to initialize <see cref="SettingsBuilder"/>.</param>
        public SnapshotConstraint(SnapshotSettingsBuilder<SnapshotSettings> settingsBuilder = null)
        {
            SettingsBuilder = settingsBuilder ?? SnapshotSettings.GetBuilder();
        }

        /// <summary>
        /// Construct a <see cref="SnapshotConstraint"/> that will use the specified snapshot name, along with settings from
        /// <paramref name="settingsBuilder"/> (if supplied) or a default <see cref="SnapshotSettingsBuilder&lt;SnapshotSettings&gt;"/> (if not supplied).
        /// </summary>
        /// <param name="snapshotName">The name of the snapshot, used to initialize <see cref="SnapshotName"/>.</param>
        /// <param name="settingsBuilder">A builder to create a <see cref="SnapshotSettings"/> object when needed
        /// to perform a snapshot comparison. This value (if supplied) is used to initialize <see cref="SettingsBuilder"/>.</param>
        public SnapshotConstraint(string snapshotName, SnapshotSettingsBuilder<SnapshotSettings> settingsBuilder = null) : this(settingsBuilder)
        {
            // Do not use WithSettings on a caller-supplied SnapshotSettingsBuilder<SnapshotSettings> here, as
            // that would change the operationg of the caller's builder which could be unexpected.
            // Instead the snapshotName is stored in property to be used to explicitly override the SnapshotName
            // in built SnapshotSettings when needed.

            SnapshotName = snapshotName;
        }

        /// <summary>
        /// Construct a <see cref="SnapshotConstraint"/> that will use the specified specified action to initialize
        /// <see cref="SnapshotSettings"/> objects when they are created as needed.
        /// </summary>
        /// <param name="settingsInitializer">An action to be called to set values in <see cref="SnapshotSettings"/> objects created
        /// by this <see cref="SnapshotConstraint"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="settingsInitializer"/> is null.
        /// </exception>
        public SnapshotConstraint(Action<SnapshotSettings> settingsInitializer) : this()
        {
            if (settingsInitializer == null)
                throw new ArgumentNullException(nameof(settingsInitializer));

            WithSettings(settingsInitializer);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The <see cref="SnapshotSettingsBuilder&lt;SnapshotSettings&gt;"/> used by this <see cref="SnapshotConstraint"/> to create <see cref="SnapshotSettings"/>
        /// objects as needed to perform snapshot comparisons or other operations.
        /// </summary>
        public SnapshotSettingsBuilder<SnapshotSettings> SettingsBuilder { get; }

        /// <summary>
        /// The snapshot name to use when this <see cref="SnapshotConstraint"/> performs a comparison.
        /// </summary>
        /// <remarks>
        /// If this property is null then the snapshot name determined by the <see cref="SettingsBuilder"/> is used. The default snapshot name
        /// from <see cref="SettingsBuilder"/> is determined from details in NUnit's <see cref="TestContext.CurrentContext"/> object:
        /// <list type="bullet">
        /// <item>If the <see cref="SnapshotTestFrameworkSettingsBase.DefaultSnapshotGroupKeyFromTestName"/> setting being used for the snapshot comparison is false (the default),
        /// the snapshot name defaults to <c>{ClassName}.{TestName}</c>.</item>
        /// <item>Otherwise, the snapshot name defaults to <c>{ClassName}</c> (and the snapshot group defaults to <c>{TestName}</c>).</item>
        /// </list>
        /// </remarks>
        public string SnapshotName { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Shortcut method to call <see cref="SnapshotSettingsBuilder&lt;SnapshotSettings&gt;.WithSettings"/> on <see cref="SettingsBuilder"/>.
        /// </summary>
        /// <param name="settingsInitializer">
        /// An action to be called to initialize <see cref="SnapshotSettings"/> property values when a new settings object
        /// is created by this <see cref="SnapshotConstraint"/>.
        /// </param>
        /// <remarks>
        /// Be aware: If a <see cref="SnapshotSettingsBuilder&lt;SnapshotSettings&gt;"/> was specified as a parameter when the
        /// <see cref="SnapshotConstraint"/> was constructed then calling this method will cause that
        /// <see cref="SnapshotSettingsBuilder&lt;SnapshotSettings&gt;"/> to be modified.
        /// </remarks>
        /// <returns>This <see cref="SnapshotConstraint"/>.</returns>
        public SnapshotConstraint WithSettings(Action<SnapshotSettings> settingsInitializer)
        {
            SettingsBuilder.WithSettings(settingsInitializer);
            return this;
        }

        private SnapshotSettings BuildSettings()
        {
            var settings = SettingsBuilder.Build();

            if (!string.IsNullOrEmpty(SnapshotName))
                settings.SnapshotName = SnapshotName;

            return settings;
        }

        #region Overrides
        /// <summary>
        /// The Description of what this constraint tests, for use in messages.
        /// </summary>
        public override string Description
            => $"snapshotted value from {BuildSettings().SnapshotFilePath}";

        /// <summary>
        /// Applies the constraint to an actual value by comparing the actual value to the snapshotted value, and returns a <see cref="ConstraintResult"/>.
        /// </summary>
        /// <param name="actual">The value to be tested.</param>
        /// <returns>A <see cref="ConstraintResult"/> representing the result of applying the constraint.</returns>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            var settings = BuildSettings();
            var comparer = new NUnitSnapshotEqualityComparer(this, settings);
            settings.SnapshotComparer = comparer;

            bool result = Snapshot.MatchTo(actual, settings);

            return comparer.ConstraintResult ?? new ConstraintResult(this, actual, result);
        }
        #endregion
        #endregion
   }
}
