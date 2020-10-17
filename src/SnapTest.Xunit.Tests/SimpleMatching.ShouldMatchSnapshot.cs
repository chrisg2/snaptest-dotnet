using System;
using Xunit;

namespace SnapTest.Xunit.Tests
{
    public class SimpleMatching_ShouldMatchSnapshot: SnapTestXunitTestBase
    {
		[Fact]
		public void Can_be_called()
		{
            Value1.ShouldMatchSnapshot();
		}

        [Fact]
        public void Can_be_used_with_snapshot_name()
        {
            Value1.ShouldMatchSnapshot(Value1SnapshotName);
        }

		[Fact]
		public void Can_be_used_with_settings_builder()
		{
            Value1.ShouldMatchSnapshot(GetBuilderUsingSnapshotGroup());
		}

		[Fact]
		public void Can_be_used_with_snapshot_name_and_settings_builder()
		{
            Value2.ShouldMatchSnapshot(Value2SnapshotName, GetBuilderUsingSnapshotGroup());
		}
    }
}
