namespace SnapTest
{
    public interface ISnapshotComparer
    {
        /// <summary>
        /// Compares an actual value to a snapshotted value.
        /// </summary>
        /// <remarks>
        /// An ISnapshotComparer can be specified for snapshot processing in the <see cref="SnapshotSettings.SnapshotComparer"/> property
        /// of the <see cref="SnapshotSettings"/> object passed to <see cref="Snapshot.CompareTo"/>.
        ///
        /// No comparer is explicitly configured by default, in which case a default <see cref="SnapshotComparer"/> is used for comparison.
        /// This default comparer performs a deep equality comparison of actualValue and snapshottedValue.
        /// If snapshottedValue is null then the comparison returns false.
        /// </remarks>
        /// <param name="actualValue">The actual value to compare against the snapshot.</param>
        /// <param name="snapshottedValue">The snapshotted value to compare against the actual value.</param>
        /// <returns>true if actualValue compares equal to the snapshottedValue; otherwise false.</returns>
        bool Compare(SnapshotValue actualValue, SnapshotValue snapshottedValue);
    }
}
