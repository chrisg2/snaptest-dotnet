using SnapTest.Xunit;
using System.Linq;
using Xunit;

namespace SnapTest.Xunit.Examples
{
    public partial class Tests
    {
        [Fact]
        public void Can_include_and_exclude_fields()
        {
            var sydney = Model.Localities.All.Where(c => c.Name == "Sydney").FirstOrDefault();

            var builder = SnapshotSettings.GetBuilder().WithSettings(_ => {
                // Include only the TimeZone field in the snapshot
                _.Field("TimeZone").Include();
                // Exclude the current time from the snapshot as it changes from moment to moment
                _.Field("TimeZone.CurrentTime").Exclude();
            });

            SnapshotAssert.Matches(sydney, builder);
        }
    }
}
