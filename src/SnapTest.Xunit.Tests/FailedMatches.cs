using System;
using System.IO;
using Xunit;

namespace SnapTest.Xunit.Tests
{
    public class FailedMatchTests : SnapTestXunitTestBase
    {
		[Fact]
		public void Failed_match_throws_Xunit_EqualException()
		{
            var builder = SnapshotSettings.GetBuilder().WithSettings(_ => _.ForceSnapshotRefresh = _.CreateMissingSnapshots = false);

            Assert.Throws<global::Xunit.Sdk.EqualException>(() =>
                SnapshotAssert.Matches(Guid.NewGuid(), Value1SnapshotName, builder));
		}

		[Fact]
		public void Failed_match_produces_mismatch_actual_file()
		{
            var builder = SnapshotSettings.GetBuilder().WithSettings(_ => _.ForceSnapshotRefresh = _.CreateMissingSnapshots = false);
            var settings = builder.Build();

            var mismatchedActualFilePath = settings.MismatchedActualFilePath;
            if (File.Exists(mismatchedActualFilePath))
                File.Delete(mismatchedActualFilePath);

            try {
                SnapshotAssert.Matches(Guid.NewGuid(), builder);
            }
            catch {
                // Ignore any exception
            }

            Assert.True(File.Exists(mismatchedActualFilePath));
		}

        [Fact]
        public void Match_with_no_snapshot_fails() // There should be no snapshot stored for this test
        {
            Assert.Throws<global::Xunit.Sdk.XunitException>(() =>
                SnapshotAssert.Matches(Guid.NewGuid(), _ => _.ForceSnapshotRefresh = _.CreateMissingSnapshots = false ));
        }
    }
}
