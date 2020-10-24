using Xunit;
using Xunit.Sdk;

namespace SnapTest.Xunit
{
    /// <summary>
    /// <see cref="ISnapshotEqualityComparer"/> implementation that performs a snapshot value comparison within the context of an xUnit.net test.
    /// </summary>
    internal class XunitSnapshotEqualityComparer : SnapshotEqualityComparer
    {
        /// <inheritdoc/>
        public override bool Equals(SnapshotValue actualValue, SnapshotValue snapshottedValue, SnapTest.SnapshotSettings settings)
        {
            if (!base.Equals(actualValue, snapshottedValue, settings)) {
                var snapshottedValueSerialized = snapshottedValue.Serialize(false);
                var actualValueSerialized = actualValue.Serialize(false);

                Assert.Equal(snapshottedValueSerialized, actualValueSerialized);

                throw new EqualException(snapshottedValueSerialized, actualValueSerialized); // Just in case Assert.Equal didn't throw...
            }

            return true;
        }
    }
}
