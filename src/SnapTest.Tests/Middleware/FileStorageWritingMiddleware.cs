using NUnit.Framework;
using System;
using System.IO;

namespace SnapTest.Tests
{
    public class FileStorageWritingMiddlewareTest
    {
        [TestCase(false, false)]
        [TestCase(true, false)]
        [TestCase(false, true)]
        [TestCase(true, true)]
        public void Snapshot_file_is_created_when_expected(bool createMissingSnapshots, bool forceSnapshotRefresh)
        {
            using var builder = new TempFileSnapshotBuilder();

            Assume.That(builder.SnapshotFileName, Does.Not.Exist);

            string actualValue = $"actual: {Guid.NewGuid().ToString()}";

            builder.WithFileStorageOptions(_ => _.CreateMissingSnapshots = createMissingSnapshots);
            builder.WithFileStorageOptions(_ => _.ForceSnapshotRefresh = forceSnapshotRefresh);

            builder.UseFileStorageWritingMiddleware();

            var context = new SnapshotContextMock();
            bool result = builder.BuildAndCompareTo(actualValue, context);

            Assert.That(result, Is.True, "Snapshot middleware pipeline returned unexpected value");
            if (!createMissingSnapshots && !forceSnapshotRefresh) {
                Assert.That(builder.SnapshotFileName, Does.Not.Exist, "Snapshot file created unexpectedly");
                Assert.That(context.MessageCalled, Is.False, "SnapshotContext.Message was called unexpectedly");
            } else {
                Assert.That(builder.SnapshotFileName, Does.Exist, "Snapshot file not created");
                Assert.That(context.MessageCalled, Is.True, "SnapshotContext.Message was not called as expected");
                Assert.That(File.ReadAllText(builder.SnapshotFileName), Is.EqualTo(actualValue), "Bad snapshot file contents");
            }
        }

        [TestCase(false, false)]
        [TestCase(true, false)]
        [TestCase(false, true)]
        [TestCase(true, true)]
        public void Snapshot_file_is_updated_when_expected(bool createMissingSnapshots, bool forceSnapshotRefresh)
        {
            using var builder = new TempFileSnapshotBuilder();

            // Create snapshot file containing originalValue
            string originalValue = $"original: {Guid.NewGuid().ToString()}";
            File.WriteAllText(builder.SnapshotFileName, originalValue);

            string updatedValue = $"updated: {Guid.NewGuid().ToString()}";
            Assume.That(updatedValue, Is.Not.EqualTo(originalValue));

            builder.WithFileStorageOptions(_ => {
                _.CreateMissingSnapshots = createMissingSnapshots;
                _.ForceSnapshotRefresh = forceSnapshotRefresh;
            });

            // Do a snapshot comparison with CreateMissingSnapshots and ForceSnapshotRefresh parameters as specified
            builder.UseFileStorageWritingMiddleware();

            var context = new SnapshotContextMock();
            builder.BuildAndCompareTo(updatedValue, context);

            // Verify outcomes
            Assert.That(File.ReadAllText(builder.SnapshotFileName), Is.EqualTo(forceSnapshotRefresh ? updatedValue : originalValue), "Snapshot file contents don't match expected value");
            Assert.That(context.MessageCalled, Is.EqualTo(forceSnapshotRefresh), "SnapshotContext.Message was not called as expected");
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

            builder.BuildAndCompareTo(null);

            Assert.That(builder.SnapshotFileName, Does.Not.Exist);
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
        }

        private class SnapshotContextMock: SnapshotContext
        {
            public bool MessageCalled = false;
            public override void Message(string message) { MessageCalled = true; }
        }
   }
}
