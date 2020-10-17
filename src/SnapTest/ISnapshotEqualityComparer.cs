namespace SnapTest
{
    /// <summary>
    /// The <c>ISnapshotEqualityComparer</c> interface is used to compare an actual value to a snapshotted value for equality.
    /// </summary>
    public interface ISnapshotEqualityComparer
    {
        /// <summary>
        /// Compares an actual value to a snapshotted value for equality.
        /// </summary>
        /// <remarks>
        /// An <see cref="ISnapshotEqualityComparer"/> can be specified for snapshot processing in the <see cref="SnapshotSettings.SnapshotComparer"/> property
        /// of the <see cref="SnapshotSettings"/> object passed to <see cref="Snapshot.CompareTo"/>.
        ///
        /// If no comparer is explicitly configured (the default), <see cref="SnapshotEqualityComparer.Default"/> is used for comparison.
        /// This default comparer performs a deep equality comparison of <paramref name="actualValue"/> and <paramref name="snapshottedValue"/>.
        /// If <paramref name="snapshottedValue"/> is null then the comparison returns false.
        /// </remarks>
        /// <param name="actualValue">The actual value to compare against the snapshotted value.</param>
        /// <param name="snapshottedValue">The snapshotted value to compare against the actual value.</param>
        /// <returns>true if <paramref name="actualValue"/> compares as being equal to <paramref name="snapshottedValue"/>; otherwise false.</returns>
        bool Equals(SnapshotValue actualValue, SnapshotValue snapshottedValue);
    }
}
