using NUnit.Framework;
using System;
using System.IO;

namespace SnapTest.Tests
{
    public class FileStorageWritingMiddlewareTest
    {
        public static System.Collections.Generic.IEnumerable<object[]> AllCreateAndForceCombinations()
            =>  new object[][] {
                new object[]{ null, false, false },
                new object[]{ null, false, true },
                new object[]{ null, true, false },
                new object[]{ null, true, true },
                new object[]{ "starting content", false, false },
                new object[]{ "starting content", false, true },
                new object[]{ "starting content", true, false },
                new object[]{ "starting content", true, true },
            };

        private (SnapshotContextMock, bool) RunNakedFileStorageWritingMiddlewarePipeline(
            TempFileSnapshotBuilder builder, string startingSnapshotFileContent, bool createMissingSnapshots, bool forceSnapshotRefresh
        )
        {
            if (startingSnapshotFileContent != null)
                File.WriteAllText(builder.SnapshotFileName, startingSnapshotFileContent);
            else
                Assume.That(builder.SnapshotFileName, Does.Not.Exist);

            string actualValue = $"actual: {Guid.NewGuid().ToString()}";

            builder.WithFileStorageOptions(_ => _.CreateMissingSnapshots = createMissingSnapshots);
            builder.WithFileStorageOptions(_ => _.ForceSnapshotRefresh = forceSnapshotRefresh);

            builder.UseFileStorageWritingMiddleware();

            var context = new SnapshotContextMock();
            return (context, builder.BuildAndCompareTo(actualValue, context));
        }

        [TestCaseSource(nameof(AllCreateAndForceCombinations))]
        public void Naked_FileStorageWritingMiddleware_processing_result_is_true(string startingSnapshotFileContent, bool createMissingSnapshots, bool forceSnapshotRefresh)
        {
            using var builder = new TempFileSnapshotBuilder();
            var (context, result) = RunNakedFileStorageWritingMiddlewarePipeline(builder, startingSnapshotFileContent, createMissingSnapshots, forceSnapshotRefresh);
            Assert.That(result, Is.True);
        }

        [TestCaseSource(nameof(AllCreateAndForceCombinations))]
        public void Naked_FileStorageWritingMiddleware_emits_message(string startingSnapshotFileContent, bool createMissingSnapshots, bool forceSnapshotRefresh)
        {
            using var builder = new TempFileSnapshotBuilder();
            var (context, result) = RunNakedFileStorageWritingMiddlewarePipeline(builder, startingSnapshotFileContent, createMissingSnapshots, forceSnapshotRefresh);
            Assert.That(context.MessageCalled, Is.EqualTo(startingSnapshotFileContent == null || forceSnapshotRefresh));
        }

        [TestCaseSource(nameof(AllCreateAndForceCombinations))]
        public void Naked_FileStorageWritingMiddleware_snapshot_exists_when_expected(string startingSnapshotFileContent, bool createMissingSnapshots, bool forceSnapshotRefresh)
        {
            using var builder = new TempFileSnapshotBuilder();
            var (context, result) = RunNakedFileStorageWritingMiddlewarePipeline(builder, startingSnapshotFileContent, createMissingSnapshots, forceSnapshotRefresh);
            Assert.That(builder.SnapshotFileName, startingSnapshotFileContent != null || createMissingSnapshots || forceSnapshotRefresh ? Does.Exist : Does.Not.Exist);
        }

        [TestCaseSource(nameof(AllCreateAndForceCombinations))]
        public void Naked_FileStorageWritingMiddleware_snapshot_content_set_correctly(string startingSnapshotFileContent, bool createMissingSnapshots, bool forceSnapshotRefresh)
        {
            if (startingSnapshotFileContent == null && !createMissingSnapshots && !forceSnapshotRefresh)
                return; // Snapshot doesn't get created for this combination

            using var builder = new TempFileSnapshotBuilder();
            var (context, result) = RunNakedFileStorageWritingMiddlewarePipeline(builder, startingSnapshotFileContent, createMissingSnapshots, forceSnapshotRefresh);
            Assert.That(
                File.ReadAllText(builder.SnapshotFileName),
                Is.EqualTo(forceSnapshotRefresh || startingSnapshotFileContent == null ? context.Actual : startingSnapshotFileContent)
            );
        }

        [TestCaseSource(nameof(AllCreateAndForceCombinations))]
        public void Naked_FileStorageWritingMiddleware_creates_mismatch_snapshot_when_expected(string startingSnapshotFileContent, bool createMissingSnapshots, bool forceSnapshotRefresh)
        {
            using var builder = new TempFileSnapshotBuilder();
            var (context, result) = RunNakedFileStorageWritingMiddlewarePipeline(builder, startingSnapshotFileContent, createMissingSnapshots, forceSnapshotRefresh);
            Assert.That(
                builder.MismatchedActualSnapshotFileName,
                startingSnapshotFileContent != null || createMissingSnapshots || forceSnapshotRefresh ? Does.Not.Exist : Does.Exist
            );
        }

        [TestCaseSource(nameof(AllCreateAndForceCombinations))]
        public void Naked_FileStorageWritingMiddleware_mismatch_snapshot_content_set_correctly(string startingSnapshotFileContent, bool createMissingSnapshots, bool forceSnapshotRefresh)
        {
            if (startingSnapshotFileContent != null || createMissingSnapshots || forceSnapshotRefresh)
                return; // Snapshot doesn't get created for this combination

            using var builder = new TempFileSnapshotBuilder();
            var (context, result) = RunNakedFileStorageWritingMiddlewarePipeline(builder, startingSnapshotFileContent, createMissingSnapshots, forceSnapshotRefresh);
            Assert.That(File.ReadAllText(builder.MismatchedActualSnapshotFileName), Is.EqualTo(context.Actual));
        }

        [Test]
        public void Snapshot_file_is_not_created_when_middleware_fails()
        {
            using var builder = new TempFileSnapshotBuilder();

            builder.WithFileStorageOptions(_ => _.ForceSnapshotRefresh = true);
            builder.WithFileStorageOptions(_ => _.CreateMissingSnapshots = true);

            builder
                .Use(_ => false) // First middleware returns false - which should abort further processing
                .UseFileStorageWritingMiddleware();

            builder.BuildAndCompareTo("actual value");

            Assert.That(builder.SnapshotFileName, Does.Not.Exist, "Snapshot file created unexpectedly");
            Assert.That(File.ReadAllText(builder.MismatchedActualSnapshotFileName), Is.EqualTo("actual value"), "Snapshot mismatch actual file contents don't match expected value");
        }

        [TestCase(null)]
        [TestCase(45)]
        public void Storing_non_string_data_is_not_currently_supported(object actualValue)
        {
            using var builder = new TempFileSnapshotBuilder();

            builder.WithFileStorageOptions(_ => _.ForceSnapshotRefresh = true);

            builder.UseFileStorageWritingMiddleware();
            Assert.Throws<NotImplementedException>(() => builder.BuildAndCompareTo(actualValue));
            Assert.That(builder.SnapshotFileName, Does.Not.Exist);
            Assert.That(builder.MismatchedActualSnapshotFileName, Does.Not.Exist);
        }

        private class SnapshotContextMock: SnapshotContext
        {
            public bool MessageCalled = false;
            public override void Message(string message) { MessageCalled = true; }
        }
   }
}
