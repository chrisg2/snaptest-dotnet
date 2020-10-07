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
        public string SnapshotExtension { get; set; } = ".txt";
        public string MismatchedActualExtension { get; set; } = ".txt.actual";

        private static readonly Regex badFilenameCharacters = new Regex(@"[/|:*?\\\""<>]");

        private string GetFilePathWithExtension(string testName, string extension)
            => System.IO.Path.Combine(SnapshotDirectory ?? string.Empty, badFilenameCharacters.Replace(testName + extension, "_"));

        public string GetSnapshotFilePath(string testName) => GetFilePathWithExtension(testName, SnapshotExtension);

        public string GetMismatedActualFilePath(string testName) => GetFilePathWithExtension(testName, MismatchedActualExtension);
    }
}
