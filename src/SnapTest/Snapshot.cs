using System;

namespace SnapTest
{
    /// <summary>
    /// Core class providing methods to compare an actual value against a snapshotted value.
    /// </summary>
    public static partial class Snapshot
    {
        #region Public methods
        /// <summary>
        /// Compares an actual value to the value stored in a snapshot.
        /// </summary>
        /// <remarks>
        /// A snapshot file and mismatch file may be created or updated based on the values of
        /// <see cref="SnapshotSettings.ForceSnapshotRefresh"/> and <see cref="SnapshotSettings.CreateMissingSnapshots"/>,
        /// whether or not a snapshot currently exists, and whether the actual value matches the snapshot (if it exists). In the
        /// case that these settings result in the snapshot being created or updated, the comparison result is true despite
        /// what value the snapshot currently shows.
        /// </remarks>
        /// <param name="actual">Actual value to be compared.</param>
        /// <param name="settings">Settings controlling the details of how and where the snapshot is stored and updated.</param>
        /// <returns>true if the actual value matches the snapshot, otherwise false.</returns>
        /// <exception cref="SnapTestParseException">
        /// Thrown if:
        /// - A snapshot file contains content that is not valid JSON.
        /// - A snapshot file contains JSON that is not an object when an object is expected.
        /// - A value in settings.ExcludedPaths results in the entire actual value being excluded from snapshotting.
        /// </exception>
        public static bool CompareTo(object actual, SnapshotSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            bool snapshottedValueIsSimpleString = (settings.SnapshotGroupKey == null && (actual is string || actual is Guid));
            var (snapshottedValue, completeSnapshotGroup) = GetSnapshottedValue(settings, snapshottedValueIsSimpleString);

            var actualSnapshotValue = ActualSnapshotValue(actual, settings);

            bool comparisonResult =
                settings.ForceSnapshotRefresh
                || (settings.CreateMissingSnapshots && snapshottedValue == null)
                || (settings.SnapshotComparer ?? SnapshotComparer.Default).Compare(actualSnapshotValue, snapshottedValue);

            WriteSnapshotIfRequired(comparisonResult, actualSnapshotValue, snapshottedValue, completeSnapshotGroup, settings);

            return comparisonResult;
        }
        #endregion

        #region Helper methods
        private static void Message(string message, SnapshotSettings settings)
        {
            if (settings.MessageWriter != null)
                settings.MessageWriter.Write(message);
        }
        #endregion
    }
}
