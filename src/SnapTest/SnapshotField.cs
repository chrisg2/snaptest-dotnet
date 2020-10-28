namespace SnapTest
{
    partial class SnapshotSettings
    {
        /// <summary>
        /// <see cref="SnapshotField"/> objects are used to configure how elements of a compound object that is matched to a snapshotted
        /// value are treated.
        /// </summary>
        /// <remarks>
        /// <para>When a compound object (that is, an object that is <em>not</em> a primitive type, string, or similar) is matched against
        /// a snapshotted value, it is sometimes helpful to control how matching occurs for specific elements in te object. Fields identifing such elements
        /// can be configured by obtaining a <see cref="SnapshotField"/> object by calling <see cref="SnapshotSettings.Field"/>, and then
        /// calling methods on that object.</para>
        ///
        /// <para>Fields are identified using JSON Paths. To illustrate this, consider the following classes:</para>
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
        /// <para>Given a <c>Garden</c> object to match against a snapshot, the following JSON Paths could be used to identify particular elements of the
        /// object for comparison:</para>
        /// <list type="bullet">
        /// <item><term><c>$</c></term> <description>Identifies the entire object.</description></item>
        /// <item><term><c>Name</c></term> <description>Identifies just the <c>Garden.Name</c> field.</description></item>
        /// <item><term><c>Address.Street</c></term> <description>Identifies just the <c>Garden.Address.Street</c> field.</description></item>
        /// <item><term><c>Trees[0,1]</c></term> <description>Identifies the first and second elements of the <c>Garden.Trees</c> enumeration.</description></item>
        /// <item><term><c>$['Name','Address']</c></term> <description>Identifies both the <c>Garden.Name</c> and <c>Garden.Address</c> fields.
        /// This may be similar to separately configuring matching settings for <c>"Name"</c> followed by <c>"Address"</c>.</description></item>
        /// </list>
        ///
        /// <para>For more information about and examples of JSON Path syntax, see https://goessner.net/articles/JsonPath/ .</para>
        /// </remarks>
        public class SnapshotField
        {
            #region Internal fields
            private readonly SnapshotSettings Settings;
            private readonly string JSONPath;
            #endregion

            #region Constructors
            /// <summary>
            /// Constructor taking a reference to the <see cref="SnapshotSettings"/> the field is related to and JSON Path identifying the field.
            /// </summary>
            /// <remarks>
            /// Obtain a <see cref="SnapshotField"/> object by calling <see cref="SnapshotSettings.Field"/>.
            /// </remarks>
            internal SnapshotField(SnapshotSettings settings, string jsonPath)
            {
                Settings = settings;
                JSONPath = jsonPath;
            }
            #endregion

            #region Methods
            /// <summary>
            /// Configures element(s) in a compound object that are identified by a <see cref="SnapshotField"/> to be included in snapshot matching.
            /// </summary>
            ///
            /// <remarks>
            /// <para>When a compound object (that is, an object that is <em>not</em> a primitive type, string, or similar) is matched against
            /// a snapshotted value, it is sometimes helpful to select one or more specific elements of the object to be matched. Fields identifing such elements
            /// can be configured to be matched by calling this method.</para>
            ///
            /// <para>If this method is not called for <em>any</em> field in a <see cref="SnapshotSettings"/> then the entire actual object is compared to the snapshot value.</para>
            ///
            /// <para>Details of fields configured for inclusion by calling this method are ignored when matching simple primitive or string values again a snapshot.</para>
            /// </remarks>
            /// <seealso cref="Exclude"/>
            public SnapshotField Include()
            {
                Settings._includedPaths.Add(JSONPath);
                return this;
            }

            /// <summary>
            /// Configures element(s) in a compound object that are identified by a <see cref="SnapshotField"/> to be excluded from snapshot matching.
            /// </summary>
            ///
            /// <remarks>
            /// When a compound object (that is, an object that is <em>not</em> a primitive type, string, or similar) is matched against a snapshotted value,
            /// it is sometimes helpful to exclude particular elements of the object from the match. Fields identifying such elements
            /// can be configured to be excluded by calling this method.
            /// </remarks>
            /// <seealso cref="Include"/>
            public SnapshotField Exclude()
            {
                Settings._excludedPaths.Add(JSONPath);
                return this;
            }
            #endregion
        }
    }
}
