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
    }
}
