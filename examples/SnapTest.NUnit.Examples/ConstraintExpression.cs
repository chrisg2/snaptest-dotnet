using NUnit.Framework;
using SnapTest.NUnit;
using System.Linq;

namespace SnapTest.NUnit.Examples
{
    public partial class Tests
    {
        [Test]
        public void Can_use_constraint_expression()
        {
            var santasTimeZone
                = Model.Localities.All
                    .Where(_ => _.Landmarks.Contains("Santa's Workshop"))
                    .Select(_ => _.TimeZone)
                    .FirstOrDefault();

            Assert.That(santasTimeZone, Is.Null.And.MatchSnapshot());
        }
    }
}
