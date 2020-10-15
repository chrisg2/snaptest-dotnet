using NUnit.Framework;
using SnapTest.NUnit;

namespace SnapTest.Examples
{
    public class ChristmasTests
    {
        [Test]
        public void Santa_lives_at_the_NorthPole()
        {
            Assert.That(Santa.HomeLocation, SnapshotDoes.Match());
        }
    }

    public class LatLong {
        public float Latitude;
        public float Longitude;
    }

    public class Santa
    {
        public static readonly LatLong HomeLocation = new LatLong() { Latitude = 90, Longitude = 0 };
    }
}
