using NUnit.Framework;

namespace SnapTest.NUnit.Tests
{
    public class SnapshotOutsideTests
    {
        bool setupMethodCalled = false;

        [OneTimeSetUp]
        public void SnapshotDoes_Match_throws_when_called_without_settings_outside_test_method()
        {
            Assert.Throws<SnapTestException>(() => SnapshotDoes.Match().ApplyTo("actual value"));
            Assert.Throws<SnapTestException>(() => SnapshotDoes.Match(_ => _.SnapshotName = "name").ApplyTo("actual value"));
            Assert.Throws<SnapTestException>(() => SnapshotDoes.Match(_ => _.SnapshotDirectoryPath = System.IO.Path.GetTempPath()).ApplyTo("actual value"));

            setupMethodCalled = true;
        }

        [Test]
        public void OneTimeSetup_method_must_have_been_executed()
            => Assert.That(setupMethodCalled, Is.True);
    }
}
