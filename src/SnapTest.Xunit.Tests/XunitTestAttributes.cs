using System;
using Xunit;

namespace SnapTest.Xunit.Tests
{
    public class XunitTestAttributes: SnapTestXunitTestBase
    {
		[Fact]
		public void Snapshot_works_in_Fact_test()
		{
            SnapshotAssert.Matches(Value1, Value1SnapshotName);
		}

		[Theory]
		[InlineData(1)]
		[InlineData(2)]
		public void Snapshot_works_in_Theory_test(int arg)
		{
            // Avoid warning xUnit1026: Theory method 'Snapshot_works_in_Theory_test' on test class 'TestAttributes' does not use parameter 'arg'
            arg = arg * -1;

            SnapshotAssert.Matches(Value1, Value1SnapshotName);
		}
    }
}
