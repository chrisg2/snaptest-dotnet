using System;

namespace SnapTest.NUnit
{
    /// <inheritdoc/>
    public class SnapshotSettingsBuilder: SnapTest.SnapshotSettingsBuilder<SnapshotSettings>
    {
        /// <inheritdoc/>
        public SnapshotSettingsBuilder(SettingsFactory settingsFactory) : base(settingsFactory) { }

        /// <summary>
        /// TODO: Document
        /// </summary>
        /// <param name="settingsInitializer"></param>
        /// <returns></returns>
        public new SnapshotSettingsBuilder WithSettings(Action<SnapshotSettings> settingsInitializer)
        {
            base.WithSettings(settingsInitializer);
            return this;
        }
    }
}
