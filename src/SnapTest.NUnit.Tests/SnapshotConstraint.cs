using NUnit.Framework;

namespace SnapTest.NUnit.Tests
{
    class SnapshotConstraintTests
    {
        [Test]
        public void Applying_SnapshotConstraint_constructed_with_name_does_not_affect_SnapshotName_from_SnapshotSettingsBuilder()
        {
            var b = SnapshotSettings.GetBuilder();
            b.WithSettings(_ => _.SnapshotName = "initial");
            new SnapshotConstraint("alternate name", b).ApplyTo(42);
            Assert.That(b.Build().SnapshotName, Is.EqualTo("initial"));
        }

        [Test]
        public void SnapshotConstraint_WithSettings_modifies_supplied_SnapshotSettingsBuilder()
        {
            var b = SnapshotSettings.GetBuilder();
            new SnapshotConstraint(b).WithSettings(_ => _.SnapshotName = "alternate");
            Assert.That(b.Build().SnapshotName, Is.EqualTo("alternate"));
        }
    }
}
