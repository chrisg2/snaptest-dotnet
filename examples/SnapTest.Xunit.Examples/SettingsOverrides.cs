using SnapTest.Xunit;
using System.Linq;
using Xunit;

namespace SnapTest.Xunit.Examples
{
    public partial class Tests
    {
        private SnapshotSettingsBuilder GetSettingsBuilder()
        {
            // Construct a SnapshotSettingsBuilder to provide common settings shared by all tests in this fixture.
            // The settings store snapshots in a snapshot group file named .snapshots/SettingsOverrides.json

            return SnapshotSettings.GetBuilder().WithSettings(_ => {
                _.SnapshotName = "SettingsOverrides";
                _.DefaultSnapshotGroupKeyFromXunitTestMethod = true;
                _.MismatchedActualExtension = ".actual.json";
                _.SnapshotExtension = ".json";
                _.SnapshotSubdirectory = ".snapshots";
            });
        }

        [Fact]
        public void Matches_can_accept_settings()
        {
            var landmarks = Model.Localities.All.Select(_ => _.Landmarks).SelectMany(_ => _).OrderBy(_ => _);

            // Overide default settings by providing a SnapshotSettings when calling SnapshotAssert.Matches
            SnapshotAssert.Matches(landmarks, GetSettingsBuilder());
        }

        [Fact]
        public void Matches_can_accept_name_and_settings()
        {
            var firstLocalityName = Model.Localities.All.OrderBy(_ => _.Name).FirstOrDefault()?.Name;

            // The snapshot name defaults to the xUnit.net test method name, but can be explicitly overridden when calling SnapshotAssert.Matches
            SnapshotAssert.Matches(firstLocalityName, "SampleSnapshotName", GetSettingsBuilder());
        }
    }
}
