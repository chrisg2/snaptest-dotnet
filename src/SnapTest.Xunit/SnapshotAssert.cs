namespace SnapTest.Xunit
{
    public static class SnapshotAssert
    {
        /// <summary>
        /// Verifies that an actual value matches a snapshotted value, optionally with a snapshot name and settings produced by the specified settings builder.
        /// </summary>
        /// <param name="actual">The actual value to match against the snapshotted value.</param>
        /// <param name="snapshotName">The snapshot name. If not specified, the snapshot name from the settings builder is used, which defaults to the test method name.</param>
        /// <param name="settingsBuilder">A builder that produces <see cref="SnapshotSettings"/> objects used to control how the snapshot match operation is performed.</param>
        /// <exception cref="global::Xunit.Sdk.EqualException">
        /// Thrown when the <paramref name="actual"/> value does not match the snapshotted value.
        /// </exception>
        public static void Matches(object actual, string snapshotName = null, SnapshotSettingsBuilder<SnapshotSettings> settingsBuilder = null)
        {
            var settings = (settingsBuilder ?? SnapshotSettings.GetBuilder()).Build();

            settings.SnapshotComparer = new XunitSnapshotEqualityComparer();

            if (!string.IsNullOrEmpty(snapshotName))
                settings.SnapshotName = snapshotName;

            Snapshot.MatchTo(actual, settings); // XunitSnapshotEqualityComparer throws an appropriate exception if match fails
        }

        /// <summary>
        /// Verifies that an actual value matches a snapshotted value, using settings produced by the specified settings builder.
        /// </summary>
        /// <param name="actual">The actual value to match against the snapshotted value.</param>
        /// <param name="settingsBuilder">A builder that produces <see cref="SnapshotSettings"/> objects used to control how the snapshot match operation is performed.</param>
        /// <exception cref="global::Xunit.Sdk.EqualException">
        /// Thrown when the <paramref name="actual"/> value does not match the snapshotted value.
        /// </exception>
        public static void Matches(object actual, SnapshotSettingsBuilder<SnapshotSettings> settingsBuilder)
            => Matches(actual, null, settingsBuilder);

        /// <summary>
        /// Verifies that an actual value matches a snapshotted value using a fluent-style expression,
        /// optionally with a snapshot name and settings produced by the specified settings builder.
        /// </summary>
        /// <param name="actual">The actual value to match against the snapshotted value.</param>
        /// <param name="snapshotName">The snapshot name. If not specified, the snapshot name from the settings builder is used, which defaults to the test method name.</param>
        /// <param name="settingsBuilder">A builder that produces <see cref="SnapshotSettings"/> objects used to control how the snapshot match operation is performed.</param>
        /// <exception cref="global::Xunit.Sdk.EqualException">
        /// Thrown when the <paramref name="actual"/> value does not match the snapshotted value.
        /// </exception>
        public static void ShouldMatchSnapshot(this object actual, string snapshotName = null, SnapshotSettingsBuilder<SnapshotSettings> settingsBuilder = null)
            // See https://github.com/xunit/samples.xunit/blob/main/AssertExtensions/ObjectAssertExtensions.cs for
            // xUnit.net samples illustrating this style of extension method.
            => Matches(actual, snapshotName, settingsBuilder);

        /// <summary>
        /// Verifies that an actual value matches a snapshotted value using a fluent-style expression,
        /// using settings produced by the specified settings builder.
        /// </summary>
        /// <param name="actual">The actual value to match against the snapshotted value.</param>
        /// <param name="settingsBuilder">A builder that produces <see cref="SnapshotSettings"/> objects used to control how the snapshot match operation is performed.</param>
        /// <exception cref="global::Xunit.Sdk.EqualException">
        /// Thrown when the <paramref name="actual"/> value does not match the snapshotted value.
        /// </exception>
        public static void ShouldMatchSnapshot(this object actual, SnapshotSettingsBuilder<SnapshotSettings> settingsBuilder)
            => Matches(actual, null, settingsBuilder);
    }
}
