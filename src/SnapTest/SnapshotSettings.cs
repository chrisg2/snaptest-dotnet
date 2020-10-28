using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace SnapTest
{
    /// <summary>
    /// Settings used to control how snapshot processing is performed.
    /// </summary>
    /// <remarks>
    ///
    /// <para>An instance of the <c>SnapshotSettings</c> class may be provided as a parameter when calling</para>
    /// <see cref="Snapshot.MatchTo"/>.
    /// </remarks>
    public partial class SnapshotSettings
    {
        #region Internal fields
        private string _snapshotGroupKey;
        private readonly List<string> _includedPaths = new List<string>();
        private readonly List<string> _excludedPaths = new List<string>();
        #endregion

        #region Settings related to snapshot identification
        /// <summary>
        /// The name of the snapshot. The name is commonly the same as a test name, and this value is used to construct
        /// <see cref="SnapshotFilePath"/> and <see cref="MismatchedActualFilePath"/>.
        /// </summary>
        public string SnapshotName { get; set; }

        /// <summary>
        /// A key used to identify the particular snapshot to use out of a group of snapshots identified by <see cref="SnapshotName"/>.
        /// </summary>
        /// <remarks>
        /// <para>Normally a single snapshot file stores a single snapshotted value. However if <c>SnapshotGroupKey</c> is set to a non-null
        /// value then the snapshot file can contain multiple snapshot values in JSON format, each one identified by a different key.
        /// <c>SnapshotGroupKey</c> is set to a non-null value to identify the key to use for a particular snapshot operation.</para>
        ///
        /// <para>Whitespace is trimmed from the start and end of any value supplied when <c>SnapshotGroupKey</c> is set.</para>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if an attempt is made to set <c>SnapshotGroupKey</c> to a non-null value that does not contain any non-whitespace characters.
        /// </exception>
        public string SnapshotGroupKey {
            get {
                return _snapshotGroupKey;
            }
            set {
                if (value != null && string.IsNullOrWhiteSpace(value))
                    throw new ArgumentOutOfRangeException(nameof(value), "SnapshotGroupKey must either be null, or contain at least one non-whitespace character");

                _snapshotGroupKey = value?.Trim();
            }
        }
        #endregion

        #region Settings related to configuring how individual fields are treated during snapshot matching
        /// <summary>
        /// Gets a <see cref="SnapshotField"/> object representing the elements in a compound snapshotted object that match
        /// the specified JSON Path. Members on the <see cref="SnapshotField"/> can subsequently be accessed to control how
        /// elements identified by the JSON Path are treated during snapshot processing.
        /// </summary>
        /// <param name="jsonPath">The JSON Path. For  more information about and examples of JSON Path syntax, see https://goessner.net/articles/JsonPath/ .</param>
        /// <remarks>
        /// See <see cref="SnapshotField"/> for more information about configuring snapshot matching behaviors for specific elements.
        /// </remarks>
        /// <returns>A <see cref="SnapshotField"/> object for the specified <paramref name="jsonPath"/>.</returns>
        /// <seealso cref="SnapshotField"/>
        public SnapshotField Field(string jsonPath)
            => new SnapshotField(this, jsonPath);

        /// <summary>
        /// JSON Paths that have been configured to be included in a snapshot match by calling <see cref="SnapshotField.Include"/>.
        /// </summary>
        /// <seealso cref="SnapshotField.Include"/>
        protected internal IEnumerable<string> IncludedPaths
            => _includedPaths;

        /// <summary>
        /// JSON Paths that have been configured to be excluded from a snapshot match by calling <see cref="SnapshotField.Exclude"/>.
        /// </summary>
        /// <seealso cref="SnapshotField.Exclude"/>
        protected internal IEnumerable<string> ExcludedPaths
            => _excludedPaths;
        #endregion

        #region Settings related to snapshot files
        /// <summary>
        /// Name of an environment variable that can be set to cause missing snapshots to be created based on actual values provided
        /// when a snapshot match operation is performed. The <see cref="CreateMissingSnapshots"/> property defaults to true if this environment
        /// variable is set to any non-empty value.
        /// </summary>
        /// <seealso cref="CreateMissingSnapshots"/>
        /// <seealso cref="RefreshSnapshotsEnvironmentVariableName"/>
        public const string CreateMissingSnapshotsEnvironmentVariableName = "SNAPTEST_CREATE_MISSING_SNAPSHOTS";

        /// <summary>
        /// Flag indicating whether missing snapshots should be created based on actual values provided
        /// when a snapshot match operation is performed. Defaults to to true if environment variable identified by
        /// <see cref="CreateMissingSnapshotsEnvironmentVariableName"/> is set to any non-empty value;
        /// otherwise defaults to false.
        /// </summary>
        /// <seealso cref="CreateMissingSnapshotsEnvironmentVariableName"/>
        /// <seealso cref="ForceSnapshotRefresh"/>
        public bool CreateMissingSnapshots { get; set; } = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable(CreateMissingSnapshotsEnvironmentVariableName));

        /// <summary>
        /// Name of an environment variable that can be set to cause snapshot files to be forcibly refreshed to reflect actual values provided
        /// for snapshot matches. The <see cref="ForceSnapshotRefresh"/> property defaults to true if this environment
        /// variable is set to any non-empty value.
        /// </summary>
        /// <seealso cref="CreateMissingSnapshotsEnvironmentVariableName"/>
        /// <seealso cref="ForceSnapshotRefresh"/>
        public const string RefreshSnapshotsEnvironmentVariableName = "SNAPTEST_REFRESH";

        /// <summary>
        /// Flag indicating whether snapshot files should be forcibly refreshed to reflect actual values provided
        /// for snapshot matches. Defaults to to true if environment variable identified by
        /// <see cref="RefreshSnapshotsEnvironmentVariableName"/> is set to any non-empty value;
        /// otherwise defaults to false.
        /// </summary>
        /// <seealso cref="CreateMissingSnapshots"/>
        /// <seealso cref="RefreshSnapshotsEnvironmentVariableName"/>
        public bool ForceSnapshotRefresh { get; set; } = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable(RefreshSnapshotsEnvironmentVariableName));

        /// <summary>
        /// Flag used to control whether serialized JSON for an actual value saved to a snapshot file when either <see cref="ForceSnapshotRefresh"/> or <see cref="CreateMissingSnapshots"/> are true has new lines and indentation (true, which is the default),
        /// or has no indentation and appears on a single line (`false`).
        /// </summary>
        public bool IndentJson { get; set; } = true;

        /// <summary>
        /// Path to the directory in which snapshot files are stored. If not set, the current working directory is used.
        /// </summary>
        /// <seealso cref="SnapshotExtension"/>
        /// <seealso cref="SnapshotFilePath"/>
        /// <seealso cref="MismatchedActualFilePath"/>
        public string SnapshotDirectoryPath { get; set; }

        /// <summary>
        /// The extension to append as a suffix to snapshot filenames, including a ".". Default value is ".txt".
        /// </summary>
        /// <seealso cref="SnapshotDirectoryPath"/>
        /// <seealso cref="SnapshotFilePath"/>
        /// <seealso cref="MismatchedActualExtension"/>
        public string SnapshotExtension { get; set; } = ".txt";

        /// <summary>
        /// The extension to append as a suffix to mismatched actual filenames, including a ".". Default value is ".txt.actual".
        /// </summary>
        /// <seealso cref="SnapshotDirectoryPath"/>
        /// <seealso cref="MismatchedActualFilePath"/>
        /// <seealso cref="SnapshotExtension"/>
        public string MismatchedActualExtension { get; set; } = ".txt.actual";

        private static readonly Regex badFilenameCharacters = new Regex(@"[/|:*?\\\""<>]");

        private string GetSnapshotFilePathWithExtension(string baseName, string extension)
        {
            if (string.IsNullOrWhiteSpace(baseName))
                throw new ArgumentOutOfRangeException(nameof(baseName), "SnapshotName must be specified in order to determine snapshot file name");

            return Path.Combine(SnapshotDirectoryPath ?? string.Empty, badFilenameCharacters.Replace(baseName + extension, "_"));
        }

        /// <summary>
        /// Gets the file path of the snapshot file, which is determined based on the values of the <see cref="SnapshotDirectoryPath"/>,
        /// <see cref="SnapshotName"/> and <see cref="SnapshotExtension"/> properties.
        /// </summary>
        /// <remarks>
        /// Any characters in <see cref="SnapshotName"/> or <see cref="SnapshotExtension"/> which are generally not allowed to be used
        /// in filenames on either Windows or common UNIX-like filesystems are replaced with <c>"_"</c>.
        /// </remarks>
        public string SnapshotFilePath
            => GetSnapshotFilePathWithExtension(SnapshotName, SnapshotExtension);

        /// <summary>
        /// Gets the file path of the mismatched actual file, which is determined based on the values of the <see cref="SnapshotDirectoryPath"/>,
        /// <see cref="SnapshotGroupKey"/>, <see cref="SnapshotName"/> and <see cref="MismatchedActualExtension"/> properties.
        /// </summary>
        /// <remarks>
        /// Any characters in <see cref="SnapshotName"/>, <see cref="SnapshotGroupKey"/> or <see cref="MismatchedActualExtension"/> which are generally
        /// not allowed to be used in filenames on either Windows or common UNIX-like filesystems are replaced with <c>"_"</c>.
        /// </remarks>
        public string MismatchedActualFilePath
            => GetSnapshotFilePathWithExtension(SnapshotName + (SnapshotGroupKey == null ? string.Empty : ("." + SnapshotGroupKey)), MismatchedActualExtension);
        #endregion

        #region Properties providing interfaces to help control snapshot behaviors
        /// <summary>
        /// An object impementing the <see cref="ISnapshotEqualityComparer"/> interface to be used for comparing an actual value to a snapshotted value.
        /// If this property is not explicitly set when performing a snapshot match operation, the default <see cref="SnapshotEqualityComparer.Default"/> is used.
        /// </summary>
        public ISnapshotEqualityComparer SnapshotComparer { get; set; }

        /// <summary>
        /// An object impementing the <see cref="IMessageWriter"/> interface to be used for emitting informational messages during snapshot processing.
        /// If this property is not explicitly set when performing a snapshot match operation, information messages are not emitted.
        /// </summary>
        public IMessageWriter MessageWriter { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Apply default settings to this settings object.
        /// </summary>
        /// <remarks>
        /// This method can be overriden in classes derived from <see cref="SnapshotSettings"/> to set default settings values when a settings object is being built.
        /// This method is called by <see cref="SnapshotSettingsBuilder&lt;SnapshotSettings&gt;"/> after settings initializer actions that are registered with the builder have been invoked.
        /// </remarks>
        public virtual void ApplyDefaults()
        {
            // Empty
        }
        #endregion
    }
}
