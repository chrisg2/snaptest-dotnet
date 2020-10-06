using NUnit.Framework;
using System;

namespace SnapTest.Tests
{
    public class SnapshotContextTest
    {
        [Test]
        public void Expected_throws_if_not_set()
        {
            var context = new SnapshotContext();
            Assume.That(context.ExpectedValueKnown, Is.False);
            Assert.Throws<InvalidOperationException>(() => { var _ = context.Expected; });
        }

        [Test]
        public void Expected_can_be_accessed_after_set()
        {
            const string setValue = "set value";
            var context = new SnapshotContext() { Expected = setValue };
            Assume.That(context.ExpectedValueKnown, Is.True);
            Assert.That(context.Expected, Is.EqualTo(setValue));
        }
   }
}
