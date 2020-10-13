namespace SnapTest
{
    public class SnapshotComparer: ISnapshotComparer
    {
        public virtual bool Compare(SnapshotValue actualValue, SnapshotValue snapshottedValue)
            => snapshottedValue != null && SnapshotValue.DeepEquals(actualValue, snapshottedValue);

        public static readonly SnapshotComparer Default = new SnapshotComparer();
    }
}
