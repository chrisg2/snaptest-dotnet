using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace SnapTest
{
    public class SnapshotSettings
    {
        #region Settings related to snapshot/test identification
        public string TestName { get; set; }

        /// <remarks>
        /// Snapshot groups do not work entirely cleanly with mismatched actual snapshot files: when a mismatch occurs,
        /// a separate mismatch file is created for each snapshot group. It is up to you to manually merge the relevant
        /// expected values from each mismatch actual snapshot file into the master snapshot file.
        /// </remarks>
        public string SnapshotGroup {
            get {
                return _snapshotGroup;
            }
            set {
                if (value != null && string.IsNullOrWhiteSpace(value))
                    throw new ArgumentOutOfRangeException(nameof(value), "SnapshotGroup must either be null, or contain at least one non-whitespace character");

                _snapshotGroup = value?.Trim();
            }
        }
        private string _snapshotGroup;
        #endregion

        #region Sttings related to Json serialization of value
        public bool IndentJson { get; set; } = true;

        /// <summary>
        ///
        /// </summary>
        /// <remarks>
        /// This value is ignored when processing simple primitive or string values.
        /// </remarks>
        public string SelectPath { get; set; } = null;
        public IList<string> ExcludedPaths { get; } = new List<string>();
        #endregion

        #region Settings related to snapshot files
        public const string CreateMissingSnapshotsEnvironmentVariableName = "SNAPTEST_CREATE_MISSING_SNAPSHOTS";
        public const string RefreshSnapshotsEnvironmentVariableName = "SNAPTEST_REFRESH";

        public bool CreateMissingSnapshots { get; set; } = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable(CreateMissingSnapshotsEnvironmentVariableName));

        public bool ForceSnapshotRefresh { get; set; } = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable(RefreshSnapshotsEnvironmentVariableName));

        public string SnapshotDirectory { get; set; }

        /// <summary>
        /// The extension to append as a suffix to snapshot filenames, including a ".".
        /// </summary>
        public string SnapshotExtension { get; set; } = ".txt";
        public string MismatchedActualExtension { get; set; } = ".txt.actual";

        private static readonly Regex badFilenameCharacters = new Regex(@"[/|:*?\\\""<>]");

        private string GetFilePathWithExtension(string baseName, string extension)
        {
            if (string.IsNullOrWhiteSpace(baseName))
                throw new ArgumentOutOfRangeException(nameof(baseName), "Test name must be specified in order to determine snapshot file name");

            return Path.Combine(SnapshotDirectory ?? string.Empty, badFilenameCharacters.Replace(baseName + extension, "_"));
        }

        public string SnapshotFilePath
            => GetFilePathWithExtension(TestName, SnapshotExtension);

        public string MismatchedActualFilePath
            => GetFilePathWithExtension(TestName + (SnapshotGroup == null ? string.Empty : ("." + SnapshotGroup)), MismatchedActualExtension);
        #endregion

        #region Properties providing interfaces to help control snapshot behaviors
        public ISnapshotComparer SnapshotComparer { get; set; }
        public IMessageWriter MessageWriter { get; set; }
        #endregion
    }
}
