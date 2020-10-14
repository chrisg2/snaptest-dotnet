using NUnit.Framework;

namespace SnapTest.NUnit.Tests
{
    class SnapshotConstraintTests
    {
        [Test]
        public void Applying_SnapshotConstraint_constructed_with_name_does_not_affect_TestName_from_SnapshotSettingsBuilder()
        {
            var b = new SnapshotSettingsBuilder();
            b.WithSettings(_ => _.TestName = "initial");
            new SnapshotConstraint("alternate name", b).ApplyTo(42);
            Assert.That(b.Build().TestName, Is.EqualTo("initial"));
        }

        [Test]
        public void SnapshotConstraint_WithSettings_modifies_supplied_SnapshotSettingsBuilder()
        {
            var b = new SnapshotSettingsBuilder();
            new SnapshotConstraint(b).WithSettings(_ => _.TestName = "alternate");
            Assert.That(b.Build().TestName, Is.EqualTo("alternate"));
        }
    }
}
