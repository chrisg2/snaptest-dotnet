namespace SnapTest.Xunit
{
    public static class SnapshotAssert
    {
        public static void Matches(object actual, string snapshotName = null, SnapshotSettingsBuilder<SnapshotSettings> settingsBuilder = null)
        {
            var settings = (settingsBuilder ?? SnapshotSettings.GetBuilder()).Build();
            settings.SnapshotComparer = new XunitSnapshotEqualityComparer();

            if (!string.IsNullOrEmpty(snapshotName))
                settings.SnapshotName = snapshotName;

            Snapshot.CompareTo(actual, settings); // XunitSnapshotEqualityComparer throws an appropriate exception if comparison fails
        }

        public static void Matches(object actual, SnapshotSettingsBuilder<SnapshotSettings> settingsBuilder)
            => Matches(actual, null, settingsBuilder);

        /// <summary>
        /// Extension method to allow fluent-style usage like "actual.ShouldMatchSnapshot()".
        /// </summary>
        /// <remarks>
        /// See https://github.com/xunit/samples.xunit/blob/main/AssertExtensions/ObjectAssertExtensions.cs for xUnit.net samples
        /// illustrating this style of usage.
        /// </remarks>
        /// <param name="actual"></param>
        /// <param name="snapshotName"></param>
        /// <param name="settingsBuilder"></param>
        public static void ShouldMatchSnapshot(this object actual, string snapshotName = null, SnapshotSettingsBuilder<SnapshotSettings> settingsBuilder = null)
            => Matches(actual, snapshotName, settingsBuilder);

        public static void ShouldMatchSnapshot(this object actual, SnapshotSettingsBuilder<SnapshotSettings> settingsBuilder)
            => Matches(actual, null, settingsBuilder);
    }
}
