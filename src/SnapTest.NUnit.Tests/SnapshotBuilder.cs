using NUnit.Framework;
using System.Collections.Generic;
using System.IO;

namespace SnapTest.NUnit.Tests
{
    public class SnapshotBuilderBase
    {
        [Test]
        public void Does_MatchSnapshot_can_be_used_with_builder()
        {
            var builder = new SnapshotBuilder().WithFileStorageOptions(_ => _.Extension = ".txt");
            Assert.That("actual output", Does.MatchSnapshot("actual_output", builder));
        }

        [Test]
        public void Does_Not_MatchSnapshot_can_be_used_with_builder()
        {
            var builder = new SnapshotBuilder().WithFileStorageOptions(_ => _.Extension = ".txt");
            Assert.That("different actual output", Does.Not.MatchSnapshot("actual_output", builder));
        }

        #region Tests verifying behavior of SnapshotBuilder.BuildFileStorageOptions().SnapshotDirectory
        [Test]
        public void Default_SnapshotDirectory_in_Test_ends_with_source_directory()
            => Assert.That(new SnapshotBuilder().BuildFileStorageOptions().SnapshotDirectory, Does.EndWith(Path.Combine(nameof(SnapTest.NUnit.Tests), "_snapshots")));

        [TestCase]
        public void Default_SnapshotDirectory_in_TestCase_ends_with_source_directory()
            => Assert.That(new SnapshotBuilder().BuildFileStorageOptions().SnapshotDirectory, Does.EndWith(Path.Combine(nameof(SnapTest.NUnit.Tests), "_snapshots")));

        [TestCaseSource(nameof(SimpleTestCaseSource))]
        public void Default_SnapshotDirectory_in_TestCaseSource_ends_with_source_directory(string _)
            => Assert.That(new SnapshotBuilder().BuildFileStorageOptions().SnapshotDirectory, Does.EndWith(Path.Combine(nameof(SnapTest.NUnit.Tests), "_snapshots")));

        public static IEnumerable<object> SimpleTestCaseSource() { yield return "a value"; }

        [Theory]
        public void Default_SnapshotDirectory_in_Theory_ends_with_source_directory()
            => Assert.That(new SnapshotBuilder().BuildFileStorageOptions().SnapshotDirectory, Does.EndWith(Path.Combine(nameof(SnapTest.NUnit.Tests), "_snapshots")));

        [Test]
        public void SnapshotDirectory_tail_can_be_set()
        {
            var builder = new SnapshotBuilder();
            builder.SnapshotDirectoryTail = "tail";
            Assert.That(builder.BuildFileStorageOptions().SnapshotDirectory, Does.EndWith(Path.Combine(nameof(SnapTest.NUnit.Tests), "tail")));
        }

        [Test]
        public void SnapshotDirectory_tail_can_be_blank()
        {
            var builder = new SnapshotBuilder() { SnapshotDirectoryTail = string.Empty };
            Assert.That(builder.BuildFileStorageOptions().SnapshotDirectory, Does.EndWith(nameof(SnapTest.NUnit.Tests)));
        }

        [Test]
        public void SnapshotDirectory_tail_can_be_null()
        {
            var builder = new SnapshotBuilder() { SnapshotDirectoryTail = null };
            Assert.That(builder.BuildFileStorageOptions().SnapshotDirectory, Does.EndWith(nameof(SnapTest.NUnit.Tests)));
        }
        #endregion
    }
}
