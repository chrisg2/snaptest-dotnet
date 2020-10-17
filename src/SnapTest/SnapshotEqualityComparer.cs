namespace SnapTest
{
    /// <summary>
    /// A default implementation of the <see cref="ISnapshotEqualityComparer"/> interface to compare an actual value to a snapshotted value for equality.
    /// </summary>
    /// <remarks>
    /// The comparison performed by <c>SnapshotEqualityComparer</c> performs a deep equality comparison of an actual value against a snapshotted value.
    /// The comparison returns false if the snapshotted value is null.
    /// </remarks>
    public class SnapshotEqualityComparer: ISnapshotEqualityComparer
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <remarks>
        /// The default constructor is protected so that instances of the <see cref="SnapshotEqualityComparer"/> class cannot be directly created.
        /// Use <see cref="Default"/> to access a default instance of <see cref="SnapshotEqualityComparer"/>.
        /// </remarks>
        protected SnapshotEqualityComparer() { }

        /// <inheritdoc/>
        public virtual bool Equals(SnapshotValue actualValue, SnapshotValue snapshottedValue)
            => snapshottedValue != null && SnapshotValue.DeepEquals(actualValue, snapshottedValue);

        /// <summary>
        /// A static default <see cref="SnapshotEqualityComparer"/> that can be used for comparing snapshots.
        /// </summary>
        public static readonly SnapshotEqualityComparer Default = new SnapshotEqualityComparer();
    }
}
