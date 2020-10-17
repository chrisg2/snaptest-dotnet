using NUnit.Framework;
using SnapTest.NUnit;
using System.Linq;

namespace SnapTest.NUnit.Examples
{
    public partial class Tests
    {
        private SnapshotSettingsBuilder commonBuilder;

        [SetUp]
        public void SetUp()
        {
            // Construct a builder to provide common settings shared by all tests in this fixture.
            // In this case the builder is configured in a test fixture SetUp method as we want
            // to use the same settings across multiple tests. The SnapshotSettingsBuilder could
            // also be created for an individual test if it did not need to be shared across tests.

            // Store snapshots in a snapshot group file named .snapshots/SettingsOverrides.json
            commonBuilder = new SnapshotSettingsBuilder().WithSettings(_ => {
                _.SnapshotName = "SettingsOverrides";
                _.DefaultSnapshotGroupKeyFromNUnitTestName = true;
                _.MismatchedActualExtension = ".actual.json";
                _.SnapshotExtension = ".json";
                _.SnapshotSubdirectory = ".snapshots";
            });
        }

        [Test]
        public void SnapshotDoesMatch_can_accept_builder()
        {
            var landmarks = Model.Localities.All.Select(_ => _.Landmarks).SelectMany(_ => _).OrderBy(_ => _);

            // Overide default settings by providing a SnapshotSettingsBuilder when calling SnapshotDoes.Match
            Assert.That(landmarks, SnapshotDoes.Match(commonBuilder));
        }


        [Test]
        public void WithSettings_can_be_called_on_SnapshotConstraint()
        {
            var localities = Model.Localities.All.OrderBy(_ => _.Name);

            // Settings can also be overridden by calling SnapshotConstraint.WithSettings
            Assert.That(localities, SnapshotDoes.Match(commonBuilder).WithSettings(_ =>
                _.IncludedPaths.Add("$..['Name','Coordinates']")));
        }

        [Test]
        public void SnapshotDoesMatch_can_accept_name_and_builder()
        {
            var firstLocalityName = Model.Localities.All.OrderBy(_ => _.Name).FirstOrDefault()?.Name;

            // The snapshot name defaults to the NUnit test name, but can be explicitly overridden when calling SnapshotDoes.Match
            Assert.That(firstLocalityName, SnapshotDoes.Match("SampleSnapshotName", commonBuilder));
        }

        [Test]
        public void SnapshotDoesMatch_can_accept_settings_initializer_action()
        {
            // An action taking a SnapshotSettings parameter can be passed to Match
            // to be called when settings are initialized.
            Assert.That(Model.Localities.All, SnapshotDoes.Match(_ => {
                _.IndentJson = false;
                _.ExcludedPaths.Add("$..TimeZone.CurrentTime");
            }));
        }
    }
}
