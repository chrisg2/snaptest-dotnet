using System.Linq;

namespace SnapTest
{
    public partial class Snapshot
    {
        private static SnapshotValue ActualSnapshotValue(object actual, SnapshotSettings settings)
            => actual == null ? SnapshotValue.CreateNull() : FilterValue(SnapshotValue.FromObject(actual), settings);

        private static string Serialize(SnapshotValue value, SnapshotSettings settings)
            => value?.IsString ?? false
            ? value.ToString()
            : value.Serialize(settings.IndentJson);

        private static SnapshotValue FilterValue(SnapshotValue value, SnapshotSettings settings)
        {
            // Remove parts of the value which are explicitly excluded in settings.ExcludedPaths
            value.RemovePaths(settings.ExcludedPaths);

            // Select parts of the value which are identified by settings.SelectPath
            var selected = value.SelectTokens(settings.SelectPath).ToArray();

            switch (selected.Length) {
                case 0: return SnapshotValue.CreateNull();
                case 1: return selected.First();                // SelectPatch matched exactly 1 token - explode it and return it directly
                default: return SnapshotValue.ArrayToken(selected);  // SelectPatch matched >1 token - return them as an array
            }
        }
    }
}
