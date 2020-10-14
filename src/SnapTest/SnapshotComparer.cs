namespace SnapTest
{
    /// <summary>
    /// A default implementation of the <see cref="ISnapshotComparer"/> interface to compare an actual value to a snapshotted value for equality.
    /// </summary>
    /// <remarks>
    /// The comparison performed by <c>SnapshotComparer</c> performs a deep equality comparison of an actual value against a snapshotted value.
    /// The comparison returns false if the snapshotted value is null.
    /// </remarks>
    public class SnapshotComparer: ISnapshotComparer
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <remarks>
        /// The default constructor is protected so that instances of the <see cref="SnapshotComparer"/> class cannot be directly created.
        /// Use <see cref="Default"/> to access a default instance of <see cref="SnapshotComparer"/>.
        /// </remarks>
        protected SnapshotComparer() { }

        public virtual bool Compare(SnapshotValue actualValue, SnapshotValue snapshottedValue)
            => snapshottedValue != null && SnapshotValue.DeepEquals(actualValue, snapshottedValue);

        /// <summary>
        /// A static default <see cref="SnapshotComparer"/> that can be used for comparing snapshots.
        /// </summary>
        public static readonly SnapshotComparer Default = new SnapshotComparer();
    }
}
