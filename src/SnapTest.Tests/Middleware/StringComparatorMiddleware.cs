using NUnit.Framework;

using SnapTest.Middleware;

namespace SnapTest.Tests
{
    public class StringComparatorMiddlwareTest
    {
        [Test]
        public void Comparison_of_unset_values_is_false()
        {
            var context = new SnapshotContext() { };
            Assert.That(new StringComparatorMiddleware().Process(context), Is.False);
        }

        [TestCase(null, null, true)]
        [TestCase("a", "a", true)]
        [TestCase("a", "b", false)]
        [TestCase(null, "b", false)]
        [TestCase("a", null, false)]
        [TestCase("42", 42, true)]
        [TestCase(42, 42, true)]
        [TestCase("24", 42, false)]
        [TestCase(24, 42, false)]
        public void Comparison_gives_expected_result(object actual, object expected, bool matchExpected)
        {
            var context = new SnapshotContext() { Actual = actual, Expected = expected };
            Assert.That(new StringComparatorMiddleware().Process(context), Is.EqualTo(matchExpected));
        }
   }
}
