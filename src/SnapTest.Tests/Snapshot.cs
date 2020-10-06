using NUnit.Framework;
using System;

using SnapTest.Middleware;

namespace SnapTest.Tests
{
    public class SnapshotTest
    {
        [Test]
        public void Snapshot_can_be_default_constructed()
            => Assert.DoesNotThrow(() => new Snapshot());

        [Test]
        public void Attempting_to_invoke_CompareTo_without_middleware_throws()
            => Assert.Throws<InvalidOperationException>(() => new Snapshot().CompareTo(""));

        [TestCase("value 1", "value 1")]
        [TestCase("value 1", "another value")]
        public void Snapshot_pipeline_executes_in_order(string actualValue, string injectedExpectedValue)
        {
            bool firstMiddlewareInvoked = false;
            bool lastMiddlewareInvoked = false;

            bool compareToResult = new Snapshot()
                .Use(_ => firstMiddlewareInvoked = true)
                .Use(_ => Assert.That(lastMiddlewareInvoked, Is.False))
                .Use(_ => Assert.That(_.Actual, Is.EqualTo(actualValue)))
                .Use(_ => Assert.That(_.ExpectedValueKnown, Is.False))
                .Use(_ => _.Expected = injectedExpectedValue)
                .Use(_ => Assert.That(_.ExpectedValueKnown, Is.True))
                .Use<StringComparatorMiddleware>()
                .Use(_ => lastMiddlewareInvoked = true)
                .CompareTo(actualValue)
            ;

            Assert.That(compareToResult, Is.EqualTo(actualValue == injectedExpectedValue), "Incorrect CompareTo result");
            Assert.That(firstMiddlewareInvoked, Is.True, "First middleware was not invoked");
            Assert.That(lastMiddlewareInvoked, Is.EqualTo(actualValue == injectedExpectedValue), "Last middleware was/was not invoked as expected");
        }
   }
}
