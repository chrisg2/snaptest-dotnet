using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace SnapTest
{
    public partial class Snapshot
    {
        #region Fields
        private static bool shownRerunTip = false;
        private static bool shownMismatchReviewTip = false;
        private static bool shownNewFileReviewTip = false;
        #endregion

        #region File reading & writing methods
        private static JToken ReadSnapshotFromFile(SnapshotSettings settings, bool readAsString)
        {
            string snapshotFilePath = settings.SnapshotFilePath;

            // If snapshot file exists, and snapshot from that file will be used in further processing...
            if (File.Exists(snapshotFilePath) && (!settings.ForceSnapshotRefresh || settings.SnapshotGroup != null)) {
                var fileContents = File.ReadAllText(snapshotFilePath);
                if (readAsString) {
                    // Trim a trailing Environment.NewLine
                    return JValue.FromObject(fileContents.EndsWith(Environment.NewLine) ? fileContents.Substring(0, fileContents.Length - Environment.NewLine.Length) : fileContents);
                }

                try {
                    return JToken.Parse(fileContents);
                }
                catch (Exception ex) {
                    throw new SnapTestParseException($"Unable to read and parse contents of file as JSON: {snapshotFilePath}", ex);
                }
            }

            if (!settings.CreateMissingSnapshots && !settings.ForceSnapshotRefresh) {
                Message($"WARNING: Test result snapshot file does not exist: {snapshotFilePath}", settings);

                if (!shownRerunTip) {
                    shownRerunTip = true;

                    Message(
                        "===> Tip: Tests can be run with the following environment variable set to automatically create missing snapshot files based on actual results: "
                        + $"{SnapshotSettings.CreateMissingSnapshotsEnvironmentVariableName}=yes",
                        settings
                    );
                }
            }

            return null;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="readAsString"></param>
        /// <returns>
        /// A pair of JToken values: (snapshotted value to compare to the actual value, complete snapshot group).
        /// These values are the null if no snapshot is available.
        /// Otherwise if a snapshot group is not used then the values will be the same.
        /// Otherwise a snapshot group is used: the first item is the specific snapshotted value to compare (if specified in the group),
        /// while the second item is the full snapshot group.
        /// </returns>
        private static (JToken, JToken) GetSnapshottedValue(SnapshotSettings settings, bool readAsString)
        {
            var snapshot = ReadSnapshotFromFile(settings, readAsString);

            if (settings.SnapshotGroup == null || snapshot == null)
                return (snapshot, snapshot);

            if (!(snapshot is JObject o))
                throw new SnapTestParseException($"File does not contain JSON object representing a snapshot group: {settings.SnapshotFilePath}");

            return (o.Property(settings.SnapshotGroup)?.Value, snapshot);
        }

        private static void WriteSnapshotIfRequired(bool comparisonResult, JToken actualJson, JToken snapshottedValue, JToken completeSnapshot, SnapshotSettings settings)
        {
            string mismatchFilePath = settings.MismatchedActualFilePath;

            // Delete any pre-existing mismatched actual file from a previous run to keep a clean state
            if (File.Exists(mismatchFilePath))
                File.Delete(mismatchFilePath);

            // Write actual value to either mismatchFilePath or snapshotFilePath if/as appropriate for the settings we have

            string fileToWrite = null;
            if (settings.ForceSnapshotRefresh || (settings.CreateMissingSnapshots && snapshottedValue == null)) {
                fileToWrite = settings.SnapshotFilePath;

                Message($"Created or refreshed snapshot file at {fileToWrite}", settings);
                if (!shownNewFileReviewTip) {
                    shownNewFileReviewTip = true;
                    Message("===> Tip: Review the content of created and refreshed snapshot files to ensure they reflect expected output.", settings);
                }
            } else if (!comparisonResult) {
                fileToWrite = mismatchFilePath;

                Message($"Created snapshot actual mismatched output file at {mismatchFilePath}", settings);
                if (!shownMismatchReviewTip) {
                    shownMismatchReviewTip = true;
                    Message("===> Tip: Review the content of mismatched output files to and use them to update snapshot files as appropriate.", settings);
                }
            }

            if (fileToWrite != null) {
                if (settings.SnapshotGroup == null) {
                    WriteValueToFile(Serialize(actualJson, settings), fileToWrite);
                } else {
                    JObject jsonToWrite;
                    if (fileToWrite == mismatchFilePath) {
                        jsonToWrite = new JObject();
                        jsonToWrite.Add(settings.SnapshotGroup, actualJson);
                    }
                    else {
                        jsonToWrite = completeSnapshot as JObject ?? new JObject();
                        if (snapshottedValue != null)
                            snapshottedValue.Replace(actualJson);
                        else
                            jsonToWrite.Add(settings.SnapshotGroup, actualJson);
                    }

                    WriteValueToFile(Serialize(jsonToWrite, settings), fileToWrite);
                }
            }
        }

        private static void WriteValueToFile(string value, string filePath)
        {
            string fullDirectory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(fullDirectory))
                Directory.CreateDirectory(fullDirectory);

            File.WriteAllText(filePath, value.ToString()+Environment.NewLine);
        }
        #endregion
    }
}
