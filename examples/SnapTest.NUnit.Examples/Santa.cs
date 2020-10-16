using NUnit.Framework;
using SnapTest.NUnit;
using System.Linq;

namespace SnapTest.Examples
{
    public class SantaTests
    {
        [Test]
        public void Santa_lives_at_the_NorthPole()
        {
            var santasHomeLocation = CityModel.Cities.AllCities.Where(_ => _.Landmarks.Contains("Santa's Workshop")).Select(_ => _.Location).FirstOrDefault();
            Assert.That(santasHomeLocation, SnapshotDoes.Match());
        }
    }
}
