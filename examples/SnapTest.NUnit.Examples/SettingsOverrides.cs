using NUnit.Framework;
using SnapTest.NUnit;
using System.Linq;

namespace SnapTest.NUnit.Examples
{
    public class SettingsOverridesTests
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
                _.DefaultSnapshotGroupFromNUnitTestName = true;
                _.MismatchedActualExtension = ".actual.json";
                _.SnapshotExtension = ".json";
                _.SnapshotSubdirectory = ".snapshots";
            });
        }

        [Test]
        public void No_landmarks_have_been_added_or_lost()
        {
            var landmarks = CityModel.Cities.AllCities.Select(_ => _.Landmarks).SelectMany(_ => _).OrderBy(_ => _);

            // Overide default settings by providing a SnapshotSettingsBuilder when calling SnapshotDoes.Match
            Assert.That(landmarks, SnapshotDoes.Match(commonBuilder));
        }

        [Test]
        public void Cities_have_not_moved()
        {
            var cities = CityModel.Cities.AllCities.OrderBy(_ => _.Name);

            // Settings can also be overridden by calling SnapshotConstraint.WithSettings
            Assert.That(cities, SnapshotDoes.Match(commonBuilder).WithSettings(_ =>
                _.IncludedPaths.Add("$..['Name','Location']")));
        }

        [Test]
        public void First_city_name_has_not_moved()
        {
            var firstCityName = CityModel.Cities.AllCities.OrderBy(_ => _.Name).FirstOrDefault()?.Name;

            // The snapshot name defaults to the NUnit test name, but can be explicitly overridden when calling SnapshotDoes.Match
            Assert.That(firstCityName, SnapshotDoes.Match("First city name", commonBuilder));
        }

        [Test]
        public void City_snapshot_json_can_be_flattened()
        {
            // An action taking a SnapshotSettings parameter can be passed to Match
            // to be called when settings are initialized.
            Assert.That(CityModel.Cities.AllCities, SnapshotDoes.Match(_ => {
                _.IndentJson = false;
                _.ExcludedPaths.Add("$..TimeZone.CurrentTime");
            }));
        }
    }
}