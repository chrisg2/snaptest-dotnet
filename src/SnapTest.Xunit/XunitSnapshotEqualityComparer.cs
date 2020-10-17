namespace SnapTest.Xunit
{
    /// <summary>
    /// <see cref="ISnapshotEqualityComparer"/> implementation that performs a snapshot comparison within the context of an xUnit.net test.
    /// </summary>
    internal class XunitSnapshotEqualityComparer : SnapshotEqualityComparer
    {
        // TODO: Work out if this class is actually needed to do anything more than the base SnapshotEqualityComparer class

        /// <inheritdoc/>
        public override bool Equals(SnapshotValue actualValue, SnapshotValue snapshottedValue)
            => base.Equals(actualValue, snapshottedValue);
    }
}
