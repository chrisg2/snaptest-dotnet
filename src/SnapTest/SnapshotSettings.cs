using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace SnapTest
{
    /// <summary>
    /// Settings used to control how snapshot processing is performed.
    /// An instance of the <c>SnapshotSettings</c> class may be provided as a parameter when calling
    /// <see cref="Snapshot.CompareTo"/>.
    /// </summary>
    public class SnapshotSettings
    {
        #region Internal fields
        private string _snapshotGroup;
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
        /// <para>Normally a single snapshot file stores a single snapshotted value. However if <c>SnapshotGroup</c> is set to a non-null
        /// value then the snapshot file can contain multiple snapshot values in JSON format, each one identified by a different key.
        /// <c>SnapshotGroup</c> is set to a non-null value to identify the key to use for a particular snapshot operation.</para>
        ///
        /// <para>Snapshot groups make working with mismatched actual snapshot files somewhat more complicated: when a mismatch occurs,
        /// a separate mismatch file is created for each snapshot group. It is up to you to manually merge the relevant
        /// expected values from each mismatch actual snapshot file into the master snapshot file.</para>
        ///
        /// <para>Whitespace is trimmed from the start and end of any value supplied when <c>SnapshotGroup</c> is set.</para>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if an attempt is made to set <c>SnapshotGroup</c> to a non-null value that does not contain any non-whitespace characters.
        /// </exception>
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
        #endregion

        #region Settings related to Json serialization of values
        /// <summary>
        /// Flag used to control whether JSON that is generated by a snapshot operation has new lines and indentation (true, which is the default),
        /// or whether JSON has no indentation and appears on a single line (false).
        /// </summary>
        public bool IndentJson { get; set; } = true;

        /// <summary>
        /// JSON Path expression that is used to select the element(s) of a compound object to compare to a snapshot.
        /// </summary>
        /// <remarks>
        /// <para>When a compound object (that is, an object that is not a primitive type, string, or similar) is compared
        /// to a snapshot, it is sometimes helpful to select particular elements of the object to be compared. <c>SelectPath</c>
        /// can be set to a JSON Path expression in this situations to select appropriate element(s).</para>
        ///
        /// <para>As an example, consider the following classes:</para>
        /// <code language="c#">
        /// public class Address
        /// {
        ///     public string Street;
        ///     public string Postcode;
        /// }
        ///
        /// public class Garden
        /// {
        ///     public string Name;
        ///     public Address Address;
        ///     public IEnumerable&lt;string&gt; Trees;
        /// }
        /// </code>
        ///
        /// <para>Given a <c>Garden</c> object to compare to a snapshot, the <c>SelectPath</c> could be set to the following JSON Path
        /// expressions to select particular elements of the object for comparison:</para>
        /// <list type="bullet">
        /// <item><term><c>$</c></term> <description>Selects the entire object. This gives the same result as if <c>SelectPath</c> is null.</description></item>
        /// <item><term><c>Name</c></term> <description>Selects just the <c>Garden.Name</c> field.</description></item>
        /// <item><term><c>Address.Street</c></term> <description>Selects just the <c>Garden.Address.Street</c> field.</description></item>
        /// <item><term><c>Trees[0,1]</c></term> <description>Selects the first and second elements of the <c>Garden.Trees</c> enumeration.</description></item>
        /// <item><term><c>$['Name','Address']</c></term> <description>Selects both the <c>Garden.Name</c> and <c>Garden.Address</c> fields.</description></item>
        /// </list>
        ///
        /// <para>For more information about and examples of JSON Path syntax, see https://goessner.net/articles/JsonPath/.</para>
        ///
        /// <para>This value is ignored when processing simple primitive or string values.</para>
        /// </remarks>
        /// <seealso cref="ExcludedPaths"/>
        public string SelectPath { get; set; } = null;

        /// <summary>
        /// A list of JSON Paths identifying element(s) in a compound object provided to be compared to a snapshot that should be excluded from the comparison.
        /// </summary>
        /// <remarks>
        /// <para>When a compound object (that is, an object that is not a primitive type, string, or similar) is compared to a snapshot,
        /// it is sometimes helpful to exclude particular elements of the object from the comparison. JSON Path values can be added to the
        /// <c>ExcludedPaths</c> list to identify such elements.</para>
        ///
        /// <para>See <see cref="SelectPath"/> for more information about and examples of JSON Paths.</para>
        /// </remarks>
        /// <seealso cref="SelectPath"/>
        /// <exception cref="SnapTestParseException">
        /// Thrown if a a value in the list of <c>ExcludedPaths</c> results in the entire object being excluded from the snapshot comparison.
        /// </exception>
        public IList<string> ExcludedPaths { get; } = new List<string>();
        #endregion

        #region Settings related to snapshot files
        /// <summary>
        /// Name of an environment variable that can be set to cause missing snapshots to be created based on actual values provided
        /// when a snapshot is compared. The <see cref="CreateMissingSnapshots"/> property defaults to true if this environment
        /// variable is set to any non-empty value.
        /// </summary>
        /// <seealso cref="CreateMissingSnapshots"/>
        /// <seealso cref="RefreshSnapshotsEnvironmentVariableName"/>
        public const string CreateMissingSnapshotsEnvironmentVariableName = "SNAPTEST_CREATE_MISSING_SNAPSHOTS";

        /// <summary>
        /// Flag indicating whether missing snapshots should be created based on actual values provided
        /// when a snapshot is compared. Defaults to to true if environment variable identified by
        /// <see cref="CreateMissingSnapshotsEnvironmentVariableName"/> is set to any non-empty value;
        /// otherwise defaults to false.
        /// </summary>
        /// <seealso cref="CreateMissingSnapshotsEnvironmentVariableName"/>
        /// <seealso cref="ForceSnapshotRefresh"/>
        public bool CreateMissingSnapshots { get; set; } = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable(CreateMissingSnapshotsEnvironmentVariableName));

        /// <summary>
        /// Name of an environment variable that can be set to cause snapshot files to be forcibly refreshed to reflect actual values provided
        /// for snapshot comparisons. The <see cref="ForceSnapshotRefresh"/> property defaults to true if this environment
        /// variable is set to any non-empty value.
        /// </summary>
        /// <seealso cref="CreateMissingSnapshotsEnvironmentVariableName"/>
        /// <seealso cref="ForceSnapshotRefresh"/>
        public const string RefreshSnapshotsEnvironmentVariableName = "SNAPTEST_REFRESH";

        /// <summary>
        /// Flag indicating whether snapshot files should be forcibly refreshed to reflect actual values provided
        /// for snapshot comparisons. Defaults to to true if environment variable identified by
        /// <see cref="RefreshSnapshotsEnvironmentVariableName"/> is set to any non-empty value;
        /// otherwise defaults to false.
        /// </summary>
        /// <seealso cref="CreateMissingSnapshots"/>
        /// <seealso cref="RefreshSnapshotsEnvironmentVariableName"/>
        public bool ForceSnapshotRefresh { get; set; } = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable(RefreshSnapshotsEnvironmentVariableName));

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
        /// The extension to append as a suffix to mismatched actual snapshot filenames, including a ".". Default value is ".txt.actual".
        /// </summary>
        /// <seealso cref="SnapshotDirectoryPath"/>
        /// <seealso cref="MismatchedActualFilePath"/>
        /// <seealso cref="SnapshotExtension"/>
        public string MismatchedActualExtension { get; set; } = ".txt.actual";

        private static readonly Regex badFilenameCharacters = new Regex(@"[/|:*?\\\""<>]");

        private string GetSnapshotFilePathWithExtension(string baseName, string extension)
        {
            if (string.IsNullOrWhiteSpace(baseName))
                throw new ArgumentOutOfRangeException(nameof(baseName), "Test name must be specified in order to determine snapshot file name");

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
        /// Gets the file path of the mismatched actual snapshot file, which is determined based on the values of the <see cref="SnapshotDirectoryPath"/>,
        /// <see cref="SnapshotGroup"/>, <see cref="SnapshotName"/> and <see cref="MismatchedActualExtension"/> properties.
        /// </summary>
        /// <remarks>
        /// Any characters in <see cref="SnapshotName"/>, <see cref="SnapshotGroup"/> or <see cref="MismatchedActualExtension"/> which are generally
        /// not allowed to be used in filenames on either Windows or common UNIX-like filesystems are replaced with <c>"_"</c>.
        /// </remarks>
        public string MismatchedActualFilePath
            => GetSnapshotFilePathWithExtension(SnapshotName + (SnapshotGroup == null ? string.Empty : ("." + SnapshotGroup)), MismatchedActualExtension);
        #endregion

        #region Properties providing interfaces to help control snapshot behaviors
        /// <summary>
        /// An object impementing the <see cref="ISnapshotComparer"/> interface to be used for comparing an actual value to a snapshotted value.
        /// if this property is not explicitly set when performing a snapshot comparison, the default <see cref="SnapshotComparer.Default"/> is used.
        /// </summary>
        public ISnapshotComparer SnapshotComparer { get; set; }

        /// <summary>
        /// An object impementing the <see cref="IMessageWriter"/> interface to be used for emitting informational messages during snapshot processing.
        /// if this property is not explicitly set when performing a snapshot comparison, information messages are not emitted.
        /// </summary>
        public IMessageWriter MessageWriter { get; set; }
        #endregion
    }
}