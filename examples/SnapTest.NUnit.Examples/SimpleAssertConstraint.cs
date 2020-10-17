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
                    .Select(_ => _.Coordinates)
                    .FirstOrDefault();

            Assert.That(santasHomeLocation, SnapshotDoes.Match());
        }
    }
}
