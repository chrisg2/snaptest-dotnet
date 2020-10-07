using System.Text.RegularExpressions;

namespace SnapTest.Middleware
{
    public class FileStorageOptions
    {
        public const string CreateMissingSnapshotsEnvironmentVariableName = "SNAPTEST_CREATE_MISSING_SNAPSHOTS";
        public const string RefreshSnapshotsEnvironmentVariableName = "SNAPTEST_REFRESH";

        public bool CreateMissingSnapshots { get; set; } = !string.IsNullOrEmpty(System.Environment.GetEnvironmentVariable(CreateMissingSnapshotsEnvironmentVariableName));

        public bool ForceSnapshotRefresh { get; set; } = !string.IsNullOrEmpty(System.Environment.GetEnvironmentVariable(RefreshSnapshotsEnvironmentVariableName));

        public string SnapshotDirectory { get; set; }

        /// <summary>
        /// The extension to append as a suffix to snapshot filenames, including a ".".
        /// </summary>
        public string Extension { get; set; } = ".txt";

        private static readonly Regex badFilenameCharacters = new Regex(@"[/|:*?\\\""<>]");

        public string GetSnapshotFilePath(string testName)
            => System.IO.Path.Combine(SnapshotDirectory ?? string.Empty, badFilenameCharacters.Replace(testName + Extension, "_"));
    }
}
