namespace SnapTest
{
    /// <summary>
    /// The <c>ISnapshotComparer</c> interface is used to compare an actual value to a snapshotted value for equality.
    /// </summary>
    public interface ISnapshotComparer
    {
        /// <summary>
        /// Compares an actual value to a snapshotted value.
        /// </summary>
        /// <remarks>
        /// An <see cref="ISnapshotComparer"/> can be specified for snapshot processing in the <see cref="SnapshotSettings.SnapshotComparer"/> property
        /// of the <see cref="SnapshotSettings"/> object passed to <see cref="Snapshot.CompareTo"/>.
        ///
        /// No comparer is explicitly configured by default, in which case <see cref="SnapshotComparer.Default"/> is used for comparison.
        /// This default comparer performs a deep equality comparison of <paramref name="actualValue"/> and <paramref name="snapshottedValue"/>.
        /// If <paramref name="snapshottedValue"/> is null then the comparison returns false.
        /// </remarks>
        /// <param name="actualValue">The actual value to compare against the snapshotted value.</param>
        /// <param name="snapshottedValue">The snapshotted value to compare against the actual value.</param>
        /// <returns>true if <paramref name="actualValue"/> compares as being equal to <paramref name="snapshottedValue"/>; otherwise false.</returns>
        bool Compare(SnapshotValue actualValue, SnapshotValue snapshottedValue);
    }
}
