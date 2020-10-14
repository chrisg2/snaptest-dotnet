using System;
using System.Collections.Generic;

namespace SnapTest.NUnit
{
    /// <summary>
    /// The <c>SnapshotSettingsBuilder</c> class is used to construct and initialize <see cref="SnapshotSettings"/> objects
    /// for <see cref="SnapshotConstraint"/>s. <see cref="WithSettings"/> can be called on instances of this class to
    /// register actions which will be called immediately after a new <see cref="SnapshotSettings"/> object is created in order
    /// to set property values in the <see cref="SnapshotSettings"/> object.
    /// </summary>
    public class SnapshotSettingsBuilder
    {
        private List<Action<SnapshotSettings>> settingsInitializers = new List<Action<SnapshotSettings>>();

        /// <summary>
        /// Create a new <see cref="SnapshotSettings"/> object, invoke initialization actions for it, and
        /// apply default settings.
        /// </summary>
        /// <returns>
        /// The created and initialized <see cref="SnapshotSettings"/> object.
        /// </returns>
        public SnapshotSettings Build()
            => SnapshotSettings.Build(settingsInitializers);

        /// <summary>
        /// Register an action to be called to set properties on a <see cref="SnapshotSettings"/> object when <see cref="Build"/> is called.
        /// </summary>
        /// <param name="settingsInitializer">
        /// The action to be called to initialize settings when <see cref="Build"/> is called.
        /// A <see cref="SnapshotSettings"/> object is passed as the single parameter to the action.
        /// The action should set properties as appropriate on this object.
        /// </param>
        /// <returns>This <see cref="SnapshotSettingsBuilder"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="settingsInitializer"/> is null.
        /// </exception>
        public SnapshotSettingsBuilder WithSettings(Action<SnapshotSettings> settingsInitializer)
        {
            if (settingsInitializer == null)
                throw new ArgumentNullException(nameof(settingsInitializer));

            settingsInitializers.Add(settingsInitializer);
            return this;
        }
    }
}
