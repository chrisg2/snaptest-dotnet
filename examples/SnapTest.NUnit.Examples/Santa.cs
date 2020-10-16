using NUnit.Framework;
using SnapTest.NUnit;
using System.Linq;

namespace SnapTest.NUnit.Examples
{
    public class SantaTests
    {
        [Test]
        public void Santa_lives_at_the_NorthPole()
        {
            var santasHomeLocation
                = CityModel.Cities.AllCities
                    .Where(_ => _.Landmarks.Contains("Santa's Workshop"))
                    .Select(_ => _.Location)
                    .FirstOrDefault();

            Assert.That(santasHomeLocation, SnapshotDoes.Match());
        }

        [Test]
        public void Santa_has_no_time_zone()
        {
            var santasTimeZone
                = CityModel.Cities.AllCities
                    .Where(_ => _.Landmarks.Contains("Santa's Workshop"))
                    .Select(_ => _.TimeZone)
                    .FirstOrDefault();

            Assert.That(santasTimeZone, Is.Null.And.MatchSnapshot());
        }
    }
}
