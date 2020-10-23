using NUnit.Framework;

namespace SnapTest.Tests
{
    public class SnapshotSettingsBuilderTest
    {
        [Test]
        public void SnapshotName_takes_explicitly_set_value()
        {
            var builder = new SnapshotSettingsBuilder<SnapshotSettings>(() => new SnapshotSettings()).WithSettings(_ =>
                _.SnapshotName = "explicit");
            Assert.That(builder.Build().SnapshotName, Is.EqualTo("explicit"));
        }

        [Test]
        public void SnapshotDirectoryPath_takes_explicitly_set_value()
        {
            var builder = new SnapshotSettingsBuilder<SnapshotSettings>(() => new SnapshotSettings()).WithSettings(_ =>
                _.SnapshotDirectoryPath = "explicit");
            Assert.That(builder.Build().SnapshotDirectoryPath, Is.EqualTo("explicit"));
        }


    }
}
