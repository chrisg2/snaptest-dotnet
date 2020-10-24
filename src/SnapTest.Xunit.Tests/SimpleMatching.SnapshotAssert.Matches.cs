using Xunit;

namespace SnapTest.Xunit.Tests
{
    public class SimpleMatching_SnapshotAssert_Matches: SnapTestXunitTestBase
    {
		[Fact]
		public void Can_be_called()
		{
            SnapshotAssert.Matches(Value1);
		}

        [Fact]
        public void Can_be_used_with_snapshot_name()
        {
            SnapshotAssert.Matches(Value1, Value1SnapshotName);
        }

		[Fact]
		public void Can_be_used_with_settings_builder()
		{
            SnapshotAssert.Matches(Value1, GetBuilderUsingSnapshotGroup());
		}

		[Fact]
		public void Can_be_used_with_snapshot_name_and_settings_builder()
		{
            SnapshotAssert.Matches(Value2, Value2SnapshotName, GetBuilderUsingSnapshotGroup());
		}

		[Fact]
		public void Can_be_used_with_settings_initializer_action()
		{
            SnapshotAssert.Matches(Value1, _ => _.SnapshotName = Value1SnapshotName);
		}
    }
}
