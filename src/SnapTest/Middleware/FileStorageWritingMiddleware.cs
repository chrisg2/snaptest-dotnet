using System;
using System.IO;

namespace SnapTest.Middleware
{
    public class FileStorageWritingMiddleware : FileStorageMiddlewareBase
    {
        private static bool shownReviewTip = false;

        public override bool Process(SnapshotContext context)
        {
            bool ok = ProcessNext(context);

            string fullFilePath = Options.GetSnapshotFilePath(context.TestName);

            // Determine whether the actual result should be saved to the snapshot
            if (ok && (Options.ForceSnapshotRefresh || (Options.CreateMissingSnapshots && !File.Exists(fullFilePath)))) {
                if (!(context.Actual is string)) {
                    throw new NotImplementedException(
                        "The snapshot middleware pipeline has produced a " +
                        $"{(context.Actual == null ? "null value" : $"value of type {context.Actual.GetType()}")} for writing to file storage, " +
                        "but FileStorageWritingMiddleware currently only supports writing string data. This may indicate a problem with your middleware pipeline."
                    );
                }

                string fullDirectory = Path.GetDirectoryName(fullFilePath);
                if (!System.IO.Directory.Exists(fullDirectory))
                    System.IO.Directory.CreateDirectory(fullDirectory);

                File.WriteAllText(fullFilePath, context.Actual.ToString());

                context.Message($"Created or refreshed snapshot file at {fullFilePath}");
                if (!shownReviewTip) { // Show this tip once per process execution
                    shownReviewTip = true;
                    context.Message("===> Tip: Review the content of newly created snapshot files to ensure they reflect expected output.");
                }
            }

            return ok;
        }
    }
}
