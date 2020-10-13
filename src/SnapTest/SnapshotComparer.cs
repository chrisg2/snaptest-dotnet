using Newtonsoft.Json.Linq;

namespace SnapTest
{
    public class SnapshotComparer: ISnapshotComparer
    {
        public virtual bool Compare(JToken actualValue, JToken snapshottedValue)
            => snapshottedValue != null && JToken.DeepEquals(actualValue, snapshottedValue);

        public static readonly SnapshotComparer Default = new SnapshotComparer();
    }
}
