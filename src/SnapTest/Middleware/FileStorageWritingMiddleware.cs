using System;
using System.IO;

namespace SnapTest.Middleware
{
    public class FileStorageWritingMiddleware : FileStorageMiddlewareBase
    {
        // Flags to track showing each tip once per process execution
        private static bool shownMismatchReviewTip = false;
        private static bool shownNewFileReviewTip = false;

        public override bool Process(SnapshotContext context)
        {
            bool ok = ProcessNext(context);

            string snapshotFilePath = Options.GetSnapshotFilePath(context.TestName);
            string mismatchFilePath = Options.GetMismatedActualFilePath(context.TestName);

            // Delete any pre-existing mismatched actual file from a previous run to keep a clean state
            if (File.Exists(mismatchFilePath))
                File.Delete(mismatchFilePath);

            if (!ok || (!Options.ForceSnapshotRefresh && !Options.CreateMissingSnapshots && !File.Exists(snapshotFilePath))) {
                WriteValue(context.Actual, mismatchFilePath);

                context.Message($"Created snapshot actual mismatched output file at {mismatchFilePath}");
                if (!shownMismatchReviewTip) {
                    shownMismatchReviewTip = true;
                    context.Message("===> Tip: Review the content of mismatched output files to and use them to update snapshot files as appropriate.");
                }
            } else if (Options.ForceSnapshotRefresh || (Options.CreateMissingSnapshots && !File.Exists(snapshotFilePath))) {
                WriteValue(context.Actual, snapshotFilePath);

                context.Message($"Created or refreshed snapshot file at {snapshotFilePath}");
                if (!shownNewFileReviewTip) {
                    shownNewFileReviewTip = true;
                    context.Message("===> Tip: Review the content of newly created snapshot files to ensure they reflect expected output.");
                }
            }

            return ok;
        }

        private static void WriteValue(object value, string filePath)
        {
            if (!(value is string)) {
                throw new NotImplementedException(
                    "The snapshot middleware pipeline has produced a " +
                    $"{(value == null ? "null value" : $"value of type {value.GetType()}")} for writing to file storage, " +
                    "but FileStorageWritingMiddleware currently only supports writing non-null string data. " +
                    "This may indicate a problem with your middleware pipeline."
                );
            }

            string fullDirectory = Path.GetDirectoryName(filePath);
            if (!System.IO.Directory.Exists(fullDirectory))
                System.IO.Directory.CreateDirectory(fullDirectory);

            File.WriteAllText(filePath, value.ToString());
        }
    }
}
