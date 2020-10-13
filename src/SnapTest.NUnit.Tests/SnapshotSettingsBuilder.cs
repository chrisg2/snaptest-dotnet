using NUnit.Framework;
using System.Collections.Generic;
using System.IO;

namespace SnapTest.NUnit.Tests
{
    public class SnapshotSettingsBuilderTest
    {
        [Test]
        public void Does_MatchSnapshot_can_be_used_with_builder()
        {
            var builder = new SnapshotSettingsBuilder().WithSettings(_ => _.SnapshotExtension = ".txt");
            Assert.That("actual output", Does.MatchSnapshot(builder));
        }

        [Test]
        public void Does_Not_MatchSnapshot_can_be_used_with_builder()
        {
            var builder = new SnapshotSettingsBuilder().WithSettings(_ => _.SnapshotExtension = ".txt");
            Assert.That("different actual output", Does.Not.MatchSnapshot(builder));
        }

        #region Tests verifying behavior of SnapshotSettingsBuilder.Build().SnapshotDirectory
        [Test]
        public void Default_SnapshotDirectory_in_Test_ends_with_source_directory()
            => Assert.That(new SnapshotSettingsBuilder().Build().SnapshotDirectory, Does.EndWith(Path.Combine(nameof(SnapTest.NUnit.Tests), "_snapshots")));

        [TestCase]
        public void Default_SnapshotDirectory_in_TestCase_ends_with_source_directory()
            => Assert.That(new SnapshotSettingsBuilder().Build().SnapshotDirectory, Does.EndWith(Path.Combine(nameof(SnapTest.NUnit.Tests), "_snapshots")));

        [TestCaseSource(nameof(SimpleTestCaseSource))]
        public void Default_SnapshotDirectory_in_TestCaseSource_ends_with_source_directory(string _)
            => Assert.That(new SnapshotSettingsBuilder().Build().SnapshotDirectory, Does.EndWith(Path.Combine(nameof(SnapTest.NUnit.Tests), "_snapshots")));

        public static IEnumerable<object> SimpleTestCaseSource() { yield return "a value"; }

        [Theory]
        public void Default_SnapshotDirectory_in_Theory_ends_with_source_directory()
            => Assert.That(new SnapshotSettingsBuilder().Build().SnapshotDirectory, Does.EndWith(Path.Combine(nameof(SnapTest.NUnit.Tests), "_snapshots")));

        [Test]
        public void SnapshotDirectory_tail_can_be_set()
        {
            var builder = new SnapshotSettingsBuilder();
            builder.WithSettings(_ => _.SnapshotDirectoryTail = "tail");
            Assert.That(builder.Build().SnapshotDirectory, Does.EndWith(Path.Combine(nameof(SnapTest.NUnit.Tests), "tail")));
        }

        [Test]
        public void SnapshotDirectory_tail_can_be_blank()
        {
            var builder = new SnapshotSettingsBuilder();
            builder.WithSettings(_ => _.SnapshotDirectoryTail = string.Empty);
            Assert.That(builder.Build().SnapshotDirectory, Does.EndWith(nameof(SnapTest.NUnit.Tests)));
        }

        [Test]
        public void SnapshotDirectory_tail_can_be_null()
        {
            var builder = new SnapshotSettingsBuilder();
            builder.WithSettings(_ => _.SnapshotDirectoryTail = null);
            Assert.That(builder.Build().SnapshotDirectory, Does.EndWith(nameof(SnapTest.NUnit.Tests)));
        }
        #endregion
    }
}
