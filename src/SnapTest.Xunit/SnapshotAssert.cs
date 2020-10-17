﻿using Xunit.Sdk;

namespace SnapTest.Xunit
{
    public static class SnapshotAssert
    {
        public static void Matches(object actual, string snapshotName = null, SnapshotSettingsBuilder settingsBuilder = null)
        {
            var settings = (settingsBuilder ?? SnapshotSettings.GetBuilder()).Build();
            settings.SnapshotComparer = new XunitSnapshotEqualityComparer();

            if (!string.IsNullOrEmpty(snapshotName))
                settings.SnapshotName = snapshotName;

            var result = Snapshot.CompareTo(actual, settings);

            // TODO: Work out how to transparently pass back details of a comparison failure

            if (!result)
                throw new EqualException("Snapshotted value", actual);
        }

        public static void Matches(object actual, SnapshotSettingsBuilder settingsBuilder)
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
        /// <param name="settings"></param>
        public static void ShouldMatchSnapshot(this object actual, string snapshotName = null, SnapshotSettingsBuilder settingsBuilder = null)
            => Matches(actual, snapshotName, settingsBuilder);

        public static void ShouldMatchSnapshot(this object actual, SnapshotSettingsBuilder settingsBuilder)
            => Matches(actual, null, settingsBuilder);
    }
}