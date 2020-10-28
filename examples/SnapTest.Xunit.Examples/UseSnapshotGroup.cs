using SnapTest.Xunit;
using System.Linq;
using Xunit;

namespace SnapTest.Xunit.Examples
{
    [UseSnapshotGroup(SnapshotName = "LocalitiesSnapshotGroup")]
    public class SnapshotGroupTests
    {
        [Fact]
        public void Santas_home_location()
        {
            var santasCoords
                = Model.Localities.All
                    .Where(_ => _.Landmarks.Contains("Santa's Workshop"))
                    .FirstOrDefault()
                    .Coordinates;

            santasCoords.ShouldMatchSnapshot();
        }

        [Fact]
        public void Sydney_location()
        {
            var sydneyCoords
                = Model.Localities.All
                    .Where(_ => _.Name == "Sydney")
                    .FirstOrDefault()
                    .Coordinates;

            sydneyCoords.ShouldMatchSnapshot();
        }
    }
}
