using System;
using System.IO;
using NUnit.Framework;

namespace SnapTest.NUnit.Tests
{
    public class FailedMatchTests
    {
		[Test]
		public void Does_Not_MatchSnapshot_succeeds()
		{
            Assert.That(Guid.NewGuid(), Does.Not.MatchSnapshot().WithSettings(_ => _.ForceSnapshotRefresh = _.CreateMissingSnapshots = false));
		}

		[Test]
		public void Failed_match_produces_mismatch_actual_file()
		{
            var builder = SnapshotSettings.GetBuilder();
            var settings = builder.Build();

            var mismatchedActualFilePath = settings.MismatchedActualFilePath;
            if (File.Exists(mismatchedActualFilePath))
                File.Delete(mismatchedActualFilePath);

            SnapshotDoes.Match(builder)
                .WithSettings(_ => _.ForceSnapshotRefresh = _.CreateMissingSnapshots = false)
                .ApplyTo(Guid.NewGuid());

            Assert.That(mismatchedActualFilePath, Does.Exist);
		}
    }
}
