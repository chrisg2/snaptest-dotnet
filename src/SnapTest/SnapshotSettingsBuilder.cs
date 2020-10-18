using System;
using System.Collections.Generic;

namespace SnapTest
{
    /// <summary>
    /// The <c>SnapshotSettingsBuilder</c> class is used to construct and initialize <see cref="SnapTest.SnapshotSettings"/> objects.
    /// <see cref="WithSettings"/> can be called on instances of this class to
    /// register actions which will be called immediately after a new <see cref="SnapshotSettings"/> object is created in order
    /// to set property values in the <see cref="SnapshotSettings"/> object.
    /// </summary>
    public class SnapshotSettingsBuilder<SnapshotSettingsT> where SnapshotSettingsT : SnapshotSettings
    {
        public delegate SnapshotSettingsT SettingsFactory();

        private SettingsFactory settingsFactory;
        private List<Action<SnapshotSettingsT>> settingsInitializers = new List<Action<SnapshotSettingsT>>();

        /// <summary>
        /// Constructors a <c>SnapshotSettingBuilder</c> with the specified factory that can be called
        /// to instantiate a new <c>SnapshotSettingsT</c> object when required.
        /// </summary>
        /// <param name="settingsFactory">A factory delegate that can be called to instantiate a new <c>SnapshotSettingsT</c> object when required.</param>
        public SnapshotSettingsBuilder(SettingsFactory settingsFactory)
        {
            this.settingsFactory = settingsFactory;
        }

        /// <summary>
        /// Create a new <see cref="SnapshotSettings"/> object, invoke initialization actions for it, and
        /// apply default settings.
        /// </summary>
        /// <returns>
        /// The created and initialized <see cref="SnapshotSettings"/> object.
        /// </returns>
        public SnapshotSettingsT Build()
        {
            var s = settingsFactory();

            if (settingsInitializers != null) {
                foreach (var initializer in settingsInitializers)
                    initializer(s);
            }

            s.ApplyDefaults();

            return s;
        }

        /// <summary>
        /// Register an action to be called to set properties on a <see cref="SnapshotSettings"/> object when <see cref="Build"/> is called.
        /// </summary>
        /// <param name="settingsInitializer">
        /// The action to be called to initialize settings when <see cref="Build"/> is called.
        /// A <c>SnapshotSettingsT</c> object is passed as the single parameter to the action.
        /// The action should set properties as appropriate on this object.
        /// </param>
        /// <returns>This <c>SnapshotSettingsBuilder</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="settingsInitializer"/> is null.
        /// </exception>
        public SnapshotSettingsBuilder<SnapshotSettingsT> WithSettings(Action<SnapshotSettingsT> settingsInitializer)
        {
            if (settingsInitializer == null)
                throw new ArgumentNullException(nameof(settingsInitializer));

            settingsInitializers.Add(settingsInitializer);
            return this;
        }
    }
}
