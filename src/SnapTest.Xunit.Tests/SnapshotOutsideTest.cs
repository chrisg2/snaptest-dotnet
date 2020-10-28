using Xunit;

namespace SnapTest.Xunit.Tests
{
    public class SnapshotOutsideTests
    {
        bool setupMethodCalled = false;

        public SnapshotOutsideTests()
        {
            SnapshotAssert_Matches_throws_when_called_without_settings_outside_test_method();
            SnapshotAssert_Matches_throws_when_called_with_SnapshotName_set_outside_test_method();
            SnapshotAssert_Matches_throws_when_called_with_SnapshotDirectoryPath_set_outside_test_method();

            setupMethodCalled = true;
        }

        private void SnapshotAssert_Matches_throws_when_called_without_settings_outside_test_method()
            => Assert.Throws<SnapTestException>(() => SnapshotAssert.Matches("actual value"));

        private void SnapshotAssert_Matches_throws_when_called_with_SnapshotName_set_outside_test_method()
            => Assert.Throws<SnapTestException>(() => SnapshotAssert.Matches("actual value", _ => _.SnapshotName = "name"));

        private void SnapshotAssert_Matches_throws_when_called_with_SnapshotDirectoryPath_set_outside_test_method()
            => Assert.Throws<SnapTestException>(() => SnapshotAssert.Matches("actual value", _ => _.SnapshotDirectoryPath = System.IO.Path.GetTempPath()));

        [Fact]
        public void Constructor_must_have_been_executed()
            => Assert.True(setupMethodCalled);
    }
}
