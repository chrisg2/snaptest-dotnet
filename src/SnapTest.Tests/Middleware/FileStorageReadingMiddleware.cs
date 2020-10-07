using NUnit.Framework;
using System;
using System.IO;

using SnapTest.Middleware;

namespace SnapTest.Tests
{
    public class FileStorageReadingMiddlewareTest
    {
        [TestCase(false, false, false)]
        [TestCase(false, true, false)]
        [TestCase(false, false, true)]
        [TestCase(false, true, true)]
        [TestCase(true, false, false)]
        [TestCase(true, true, false)]
        [TestCase(true, false, true)]
        [TestCase(true, true, true)]
        public void FileStorageReadingMiddleware_updates_context_as_expected(bool createFileForTest, bool forceSnapshotRefresh, bool createMissingSnapshots)
        {
            using var builder = new TempFileSnapshotBuilder();

            // Create snapshot file containing fileContent
            string fileContent = $"file content: {Guid.NewGuid().ToString()}"; // Only used if createFileForTest == true
            string actualValue = $"actual value: {Guid.NewGuid().ToString()}";

            if (createFileForTest)
                File.WriteAllText(builder.SnapshotFileName, fileContent);

            builder.WithFileStorageOptions(_ => {
                _.CreateMissingSnapshots = createMissingSnapshots;
                _.ForceSnapshotRefresh = forceSnapshotRefresh;
            });

            builder.UseFileStorageReadingMiddleware();
            var context = new SnapshotContextMock();
            Assume.That(context.MessageCalled, Is.False);
            bool result = builder.BuildAndCompareTo(actualValue, context);

            Assert.That(result, Is.True, "Snapshot middleware pipeline returned unexpected value");
            Assert.That(context.MessageCalled, Is.EqualTo(!createFileForTest && !forceSnapshotRefresh && !createMissingSnapshots));
            Assert.That(context.Actual, Is.EqualTo(actualValue), "context.Actual value is unexpected");
            Assert.That(context.ExpectedValueKnown, Is.EqualTo(createFileForTest || forceSnapshotRefresh || createMissingSnapshots), "context.ExpectedValueKnown value is unexpected");

            if (context.ExpectedValueKnown)
                Assert.That(context.Expected, Is.EqualTo(createFileForTest && !forceSnapshotRefresh ? fileContent : actualValue), "context.Expected value is unexpected");
        }

        private class SnapshotContextMock: SnapshotContext
        {
            public bool MessageCalled = false;
            public override void Message(string message) { MessageCalled = true; }
        }
   }
}
