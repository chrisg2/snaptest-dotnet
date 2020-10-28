using NUnit.Framework;
using SnapTest.NUnit;
using System.Linq;

namespace SnapTest.NUnit.Examples
{
    [UseSnapshotGroup(SnapshotName = "LocalitiesSnapshotGroup")]
    public class SnapshotGroupTests
    {
        [Test]
        public void Santas_home_location()
        {
            var santasCoords
                = Model.Localities.All
                    .Where(_ => _.Landmarks.Contains("Santa's Workshop"))
                    .FirstOrDefault()
                    .Coordinates;

            Assert.That(santasCoords, SnapshotDoes.Match());
        }

        [Test]
        public void Sydney_location()
        {
            var sydneyCoords
                = Model.Localities.All
                    .Where(_ => _.Name == "Sydney")
                    .FirstOrDefault()
                    .Coordinates;

            Assert.That(sydneyCoords, SnapshotDoes.Match());
        }
    }
}
