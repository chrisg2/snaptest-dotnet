using NUnit.Framework;
using SnapTest.NUnit;
using System.Linq;

namespace SnapTest.NUnit.Examples
{
    public partial class Tests
    {
        [Test]
        public void Can_include_and_exclude_fields()
        {
            var sydney = Model.Localities.All.Where(c => c.Name == "Sydney").FirstOrDefault();

            var builder = SnapshotSettings.GetBuilder().WithSettings(_ => {
                // Include only the TimeZone field in the snapshot
                _.Field("TimeZone").Include();
                // Exclude the current time from the snapshot as it changes from moment to moment
                _.Field("TimeZone.CurrentTime").Exclude();
            });

            Assert.That(sydney, SnapshotDoes.Match(builder));
        }
    }
}
