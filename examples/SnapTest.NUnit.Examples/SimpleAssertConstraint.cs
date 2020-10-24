using NUnit.Framework;
using SnapTest.NUnit;
using System.Linq;

namespace SnapTest.NUnit.Examples
{
    public partial class Tests
    {
        [Test]
        public void Can_use_simple_Assert_constraint()
        {
            var santasHomeLocation
                = Model.Localities.All
                    .Where(_ => _.Landmarks.Contains("Santa's Workshop"))
                    .FirstOrDefault();

            Assert.That(santasHomeLocation, SnapshotDoes.Match());
        }

        [Test]
        public void Each_field_can_be_asserted()
        {
            // Alternate implementation of the Can_use_simple_Assert_constraint without using a snapshot
            var santasHomeLocation
                = Model.Localities.All
                    .Where(_ => _.Landmarks.Contains("Santa's Workshop"))
                    .FirstOrDefault();

            Assert.That(santasHomeLocation.Coordinates.Latitude, Is.EqualTo(90.0));
            Assert.That(santasHomeLocation.Coordinates.Longitude, Is.EqualTo(0.0));
            Assert.That(santasHomeLocation.Landmarks.Length, Is.EqualTo(1));
            Assert.That(santasHomeLocation.Name, Is.EqualTo("North Pole"));
            Assert.That(santasHomeLocation.TimeZone, Is.Null);
        }
    }
}
