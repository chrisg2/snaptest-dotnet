using System;

namespace SnapTest
{
    /// <summary>
    /// Core class providing methods to match an actual value against a snapshotted value.
    /// </summary>
    public static partial class Snapshot
    {
        #region Public methods
        /// <summary>
        /// Compares an actual value to the value stored in a snapshot.
        /// </summary>
        /// <remarks>
        /// A snapshot file and mismatched actual file may be created or updated based on the values of
        /// <see cref="SnapshotSettings.ForceSnapshotRefresh"/> and <see cref="SnapshotSettings.CreateMissingSnapshots"/>,
        /// whether or not a snapshot currently exists, and whether the actual value matches the snapshot (if it exists). In the
        /// case that these settings result in the snapshot file being created or updated, the match result is true regardless
        /// what value the snapshot file currently shows.
        /// </remarks>
        /// <param name="actual">Actual value to be matched.</param>
        /// <param name="settings">Settings controlling the details of how and where the snapshot is stored and updated.</param>
        /// <returns>true if the actual value matches the snapshotted value, otherwise false.</returns>
        /// <exception cref="SnapTestParseException">
        /// Thrown if:
        /// <list type="bullet">
        /// <item>A snapshot file contains content that is not valid JSON when JSON content is expected.</item>
        /// <item>A snapshot file contains JSON that is not an object when an object is expected.</item>
        /// <item>A field identified to be excluded from the snapshot match by calling <see cref="SnapshotSettings.SnapshotField.Exclude"/> results in the
        /// entire actual value being excluded from the match.</item>
        /// </list>
        /// </exception>
        public static bool MatchTo(object actual, SnapshotSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            bool snapshottedValueIsSimpleString = (settings.SnapshotGroupKey == null && (actual is string || actual is Guid));
            var (snapshottedValue, completeSnapshotGroup) = GetSnapshottedValue(settings, snapshottedValueIsSimpleString);

            var actualSnapshotValue = ActualSnapshotValue(actual, settings);

            bool comparisonResult = false;
            try {
                return comparisonResult =
                    settings.ForceSnapshotRefresh
                    || (settings.CreateMissingSnapshots && snapshottedValue == null)
                    || (settings.SnapshotComparer ?? SnapshotEqualityComparer.Default).Equals(actualSnapshotValue, snapshottedValue, settings);
            }
            finally {
                WriteSnapshotIfRequired(comparisonResult, actualSnapshotValue, snapshottedValue, completeSnapshotGroup, settings);
            }
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
