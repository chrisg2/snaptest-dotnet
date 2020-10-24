using SnapTest.Xunit;
using System.Linq;
using Xunit;

namespace SnapTest.Xunit.Examples
{
    public partial class Tests
    {
        [Fact]
        public void Can_use_simple_Matches_call()
        {
            var santasHomeLocation
                = Model.Localities.All
                    .Where(_ => _.Landmarks.Contains("Santa's Workshop"))
                    .FirstOrDefault();

            SnapshotAssert.Matches(santasHomeLocation);
        }

        [Fact]
        public void Can_use_fluent_ShouldMatchSnapshot_call()
        {
            var santasHomeLocation
                = Model.Localities.All
                    .Where(_ => _.Landmarks.Contains("Santa's Workshop"))
                    .FirstOrDefault();

            santasHomeLocation.ShouldMatchSnapshot();
        }

        [Fact]
        public void Each_field_can_be_asserted()
        {
            // Alternate implementation of the Can_use_simple_Matches_call & Can_use_fluent_ShouldMatchSnapshot_call without using a snapshot
            var santasHomeLocation
                = Model.Localities.All
                    .Where(_ => _.Landmarks.Contains("Santa's Workshop"))
                    .FirstOrDefault();

            Assert.Equal(90.0, santasHomeLocation.Coordinates.Latitude);
            Assert.Equal(0.0, santasHomeLocation.Coordinates.Longitude);
            Assert.Single(santasHomeLocation.Landmarks);
            Assert.Equal("North Pole", santasHomeLocation.Name);
            Assert.Null(santasHomeLocation.TimeZone);
        }
    }
}
