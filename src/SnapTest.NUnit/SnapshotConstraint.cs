using NUnit.Framework.Constraints;
using System;

namespace SnapTest.NUnit
{
    /// <summary>
    /// </summary>
    public class SnapshotConstraint : Constraint
    {
        #region Constructors
        public SnapshotConstraint(SnapshotSettingsBuilder settingsBuilder = null)
        {
            SettingsBuilder = settingsBuilder ?? new SnapshotSettingsBuilder();
        }

        public SnapshotConstraint(string testName) : this()
        {
            SettingsBuilder.WithSettings(_ => _.TestName = testName);
        }

        public SnapshotConstraint(Action<SnapshotSettings> settingsInitializer) : this()
        {
            if (settingsInitializer == null)
                throw new ArgumentNullException(nameof(settingsInitializer));

            SettingsBuilder.WithSettings(settingsInitializer);
        }
        #endregion

        #region Properties
        public SnapshotSettingsBuilder SettingsBuilder { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Shortcut method to call SettingsBuilder.WithSettings.
        /// </summary>
        /// <remarks>
        /// See <see cref="SnapshotSettingsBuilder.WithSettings"/> for further details.
        /// </remarks>
        /// <param name="settingsInitializer">
        /// An action to be called to initialize settings.
        /// A <see cref="SnapshotSettings"/> object is passed as the single parameter to the action.
        /// The action should set properties as appropriate on this object.
        /// </param>
        /// <returns>This SnapshotConstraint's <see cref="SettingsBuilder"/></returns>
        public SnapshotConstraint WithSettings(Action<SnapshotSettings> settingsInitializer)
        {
            SettingsBuilder.WithSettings(settingsInitializer);
            return this;
        }
        #endregion

        #region Overrides
        public override string Description
            => $"snapshotted value from {SettingsBuilder.Build().SnapshotFilePath}";

        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            var settings = SettingsBuilder.Build();

            if (string.IsNullOrEmpty(settings.SnapshotDirectory)) {
                throw new SnapTestException(
                    "The directory to hold snapshot files could not be determined from the current stack trace. " +
                    "You may need to explicitly specify a snapshot directory using the SnapshotConstraint's SettingsBuilder " +
                    "(for example: constraint.WithSettings(_ => _.SnapshotDirectory = \"...\"); ), " +
                    "or verify that the stack trace includes a method marked with one of the NUnit test method attributes such as [Test], [TestCase] etc. " +
                    "This error may occur if you perform a snapshot match within an async test helper child method."
                );
            }

            var comparer = new NUnitSnapshotComparer(this);
            settings.SnapshotComparer = comparer;

            bool result = Snapshot.CompareTo(actual, settings);

            return comparer.ConstraintResult ?? new ConstraintResult(this, actual, result);
        }
        #endregion
   }
}
