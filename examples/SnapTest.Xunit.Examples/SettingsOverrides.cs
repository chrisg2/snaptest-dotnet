using SnapTest.Xunit;
using System.Linq;
using Xunit;

namespace SnapTest.Xunit.Examples
{
    public partial class Tests
    {
        private SnapshotSettingsBuilder<SnapshotSettings> GetSettingsBuilder()
        {
            // Construct a SnapshotSettingsBuilder to provide common settings shared by all tests in this fixture.
            // The settings store snapshots in a snapshot group file named .snapshots/SettingsOverrides.json

            return SnapshotSettings.GetBuilder().WithSettings(_ => {
                _.SnapshotName = "SettingsOverrides";
                _.DefaultSnapshotGroupKeyFromTestName = true;
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

            // Or using fluent style:
            // landmarks.ShouldMatchSnapshot(GetSettingsBuilder());
        }

        [Fact]
        public void Matches_can_accept_name_and_settings()
        {
            var firstLocalityName = Model.Localities.All.OrderBy(_ => _.Name).FirstOrDefault()?.Name;

            // The snapshot name defaults to the xUnit.net test method name, but can be explicitly overridden when calling SnapshotAssert.Matches
            SnapshotAssert.Matches(firstLocalityName, "SampleSnapshotName", GetSettingsBuilder());

            // Or using fluent style:
            // firstLocalityName.ShouldMatchSnapshot("SampleSnapshotName", GetSettingsBuilder());
        }

        [Fact]
        public void Matches_can_accept_settings_initializer_action()
        {
            var sydneyCoordinates = Model.Localities.All.Where(_ => _.Name == "Sydney").FirstOrDefault()?.Coordinates;

            SnapshotAssert.Matches(sydneyCoordinates, _ => {
                _.SnapshotName = "SnapshotNameFromSettingsInitializer";
                _.IndentJson = false;
            });
        }
    }
}
