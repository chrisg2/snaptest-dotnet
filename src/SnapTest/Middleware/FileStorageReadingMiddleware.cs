using System;
using System.IO;

namespace SnapTest.Middleware
{
    public class FileStorageReadingMiddleware : FileStorageMiddlewareBase
    {
        private static bool shownRerunTip = false;

        public override bool Process(SnapshotContext context)
        {
            if (!ProcessNext(context))
                return false;

            string fullFilePath = Options.GetSnapshotFilePath(context.TestName);

            if (File.Exists(fullFilePath) && !Options.ForceSnapshotRefresh) {
                string expected = File.ReadAllText(fullFilePath);

                // Trim trailing new line if it exists
                context.Expected = (
                    expected.EndsWith(Environment.NewLine)
                    ? expected.Substring(0, expected.Length - Environment.NewLine.Length)
                    : expected
                );
            } else if (Options.CreateMissingSnapshots || Options.ForceSnapshotRefresh) {
                context.Expected = context.Actual;
            } else {
                context.Message($"WARNING: Test result snapshot file does not exist: {fullFilePath}");

                if (!shownRerunTip) {
                    shownRerunTip = true;

                    context.Message(
                        "===> Tip: Tests can be run with the following environment variable set to automatically create missing snapshot files based on actual results: "
                        + $"{FileStorageOptions.CreateMissingSnapshotsEnvironmentVariableName}=yes"
                    );
                }
            }

            return true;
        }
    }
}
