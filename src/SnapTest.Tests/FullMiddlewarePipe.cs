using NUnit.Framework;
using System;
using System.IO;

namespace SnapTest.Tests
{
    public class FullMiddlewarePipeTest
    {
        [TestCaseSource(nameof(SnapshotTestCases))]
        public void Snapshot_writes_expected_value_to_file(object actualValue, string expectedSnapshottedString)
        {
            using var builder = new TempFileSnapshotBuilder();
            Assume.That(builder.SnapshotFileName, Does.Not.Exist);

            // Run snapshot to create snapshot file based on actualValue and check results
            builder.WithFileStorageOptions(_ => _.ForceSnapshotRefresh = true);
            Assert.That(builder.BuildAndCompareTo(actualValue), Is.True);
            Assert.That(File.ReadAllText(builder.SnapshotFileName), Is.EqualTo(expectedSnapshottedString + Environment.NewLine), "Snapshot file created with unexpected content");
            Assert.That(builder.MismatchedActualSnapshotFileName, Does.Not.Exist);
        }

        [TestCaseSource(nameof(SnapshotTestCases))]
        public void Snapshot_compare_succeeds(object actualValue, string expectedSnapshottedString)
        {
            using var builder = new TempFileSnapshotBuilder();
            Assume.That(builder.SnapshotFileName, Does.Not.Exist);

            File.WriteAllText(builder.SnapshotFileName, expectedSnapshottedString + Environment.NewLine);

            // Run snapshot to compare to snapshot file created above and check results
            Assert.That(builder.BuildAndCompareTo(actualValue), Is.True);
            Assert.That(builder.MismatchedActualSnapshotFileName, Does.Not.Exist);
        }

        [TestCaseSource(nameof(SnapshotTestCases))]
        public void non_existent_snapshot_file_does_not_match(object actualValue, string expectedSnapshottedString)
        {
            using var builder = new TempFileSnapshotBuilder();
            Assert.That(builder.BuildAndCompareTo(actualValue), Is.False);
            Assert.That(File.ReadAllText(builder.MismatchedActualSnapshotFileName), Is.EqualTo(expectedSnapshottedString + Environment.NewLine));
        }

        public static System.Collections.Generic.IEnumerable<object[]> SnapshotTestCases()
        {
            yield return new object[]{ null, "null" };
            yield return new object[]{ "simple string", "simple string" };
            yield return new object[]{ "Non-ANSI string ¤¥£¢©۝", "Non-ANSI string ¤¥£¢©۝" };
            yield return new object[]{ Environment.NewLine, Environment.NewLine };
            yield return new object[]{ 42, "42" };
            yield return new object[]{ new { anItem = 42 }, "{\n  \"anItem\": 42\n}".Replace("\n", Environment.NewLine) };
            yield return new object[]{ new { aItem = "string", bItem = new { b1Item = 5 } }, "{\n  \"aItem\": \"string\",\n  \"bItem\": {\n    \"b1Item\": 5\n  }\n}".Replace("\n", Environment.NewLine) };

            var guid = System.Guid.NewGuid();
            yield return new object[]{ guid, $"\"{guid.ToString()}\"" };
        }
    }
}
