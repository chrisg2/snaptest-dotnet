using NUnit.Framework;

namespace SnapTest.Tests
{
    public class SnapshotSettingsBuilderTest
    {
        [Test(ExpectedResult = "explicit")]
        public string SnapshotName_takes_explicitly_set_value()
            => new SnapshotSettingsBuilder<SnapshotSettings>(() => new SnapshotSettings()).WithSettings(_ => _.SnapshotName = "explicit")
                .Build()
                .SnapshotName;

        [Test(ExpectedResult = "explicit")]
        public string SnapshotDirectoryPath_takes_explicitly_set_value()
            => new SnapshotSettingsBuilder<SnapshotSettings>(() => new SnapshotSettings()).WithSettings(_ => _.SnapshotDirectoryPath = "explicit")
                .Build()
                .SnapshotDirectoryPath;
    }
}
