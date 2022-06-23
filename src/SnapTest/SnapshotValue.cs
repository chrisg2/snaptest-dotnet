using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SnapTest
{
    /// <summary>
    /// This class represents a snapshot value, which can be null value, simple (primitive) value, object containing properties and other values, or an array of values.
    /// </summary>
    /// <remarks>
    /// <para>Much of the interface exposed by this class is marked as "internal" as the details of SnapshotValue objects are
    /// generally intended to be opaque for code outside the SnapTest assembly.</para>
    ///
    /// <para>The implementation of this class is based on <see cref="Newtonsoft.Json"/> capabilities and represents
    /// underlying values as <see cref="JToken"/> values, but the purpose of this class is to hide that detail from all users of the class.</para>
    /// </remarks>
    public class SnapshotValue
    {
        #region Fields
        private readonly static JsonSerializer jsonSerializer;

        /// <summary>
        /// A SnapshotValue is a wrapper around a <see cref="JToken"/> object. This field stores the underlying object.
        /// </summary>
        private readonly JToken jToken;
        #endregion

        #region Constructors
        static SnapshotValue()
        {
            // For serializing objects to a JToken representing the value underlying a SnapshotValue we use a serializer that:
            // 1. Orders properties alphabetically (ensured by OrderedContractResolver); and
            // 2. Translates Guid objects cleanly to strings that will compare with strings read from a snapshot file (ensured by GuidConverter)

            jsonSerializer = JsonSerializer.CreateDefault(new JsonSerializerSettings() { ContractResolver = new OrderedContractResolver() });
            jsonSerializer.Converters.Add(new GuidConverter());
        }

        private SnapshotValue(JToken jToken)
        {
            if (jToken == null)
                throw new ArgumentNullException(nameof(jToken));

            this.jToken = jToken;
        }
        #endregion

        #region Internal properties
        /// <summary>
        /// Identifies whether the <see cref="SnapshotValue"/> is an object, that is, a container able to hold properties.
        /// </summary>
        internal bool IsObject => jToken is JObject;

        /// <summary>
        ///  Identifies whether the <see cref="SnapshotValue"/> is a simple string value.
        /// </summary>
        internal bool IsString => jToken is JValue jValue && jValue.Type == JTokenType.String;
        #endregion

        #region Methods
        #region Public methods
        /// <summary>
        /// Serialize a <see cref="SnapshotValue"/> to JSON form.
        /// </summary>
        /// <param name="indented">Flag indicating whether the serialized JSON string should be indented.</param>
        /// <returns>The snapshot value as a JSON string.</returns>
        public string Serialize(bool indented) => JsonConvert.SerializeObject(jToken, indented ? Formatting.Indented : Formatting.None);

        public override string ToString() => jToken.ToString();
        #endregion

        #region Methods to create new SnapshotValues
        /// <summary>
        /// Creates a <see cref="SnapshotValue"/> representing a null value.
        /// </summary>
        /// <returns>A <see cref="SnapshotValue"/> representing a null value</returns>
        internal static SnapshotValue CreateNull() => new SnapshotValue(JValue.CreateNull());
        internal static SnapshotValue CreateObject() => new SnapshotValue(new JObject());
        internal static SnapshotValue FromObject(object o) => new SnapshotValue(JToken.FromObject(o, jsonSerializer));
        internal static SnapshotValue ValueFromObject(object o) => new SnapshotValue(JValue.FromObject(o));
        internal static SnapshotValue Parse(string json) => new SnapshotValue(JToken.Parse(json));
        internal static SnapshotValue ArrayToken(IEnumerable<SnapshotValue> content) => new SnapshotValue(new JArray(content.Select(_ => _.jToken)));
        #endregion

        #region Internal methods to perform various operations on snapshot values
        internal static bool DeepEquals(SnapshotValue t1, SnapshotValue t2) => JToken.DeepEquals(t1.jToken, t2.jToken);

        internal void Add(string propertyName, SnapshotValue value)
        {
            if (!(jToken is JObject o))
                throw new InvalidOperationException("Add can only be called for SnapshotValues that represent an object");

            o.Add(propertyName, value.jToken);
        }

        internal SnapshotValue PropertyValue(string name)
        {
            var v = (jToken as JObject)?.Property(name)?.Value;
            return v == null ? null : new SnapshotValue(v);
        }

        internal void Replace(SnapshotValue value) => jToken.Replace(value.jToken);

        internal void RemovePaths(IEnumerable<string> paths)
            => paths
                .Select(p => {
                    var tokens = jToken.SelectTokens(p);
                    if (tokens.Where(_ => _.Root == _).Any())
                        throw new SnapTestParseException($"Excluded JSON Path '{p}' matched root token of the result. The entire result cannot be excluded from a snapshot match.");

                    return tokens;
                })
                .SelectMany(_ => _)
                .ToList() // Force expansion of JPath references to actual tokens so results won't be dynamically determined while removals take place in the subsequent loop
                .ForEach(
                    // If token's parent is a JProperty then that is what needs to be removed. Otherwise remove the token itself.
                    _ => { if (_.Parent != null) (_.Parent is JProperty ? _.Parent : _).Remove(); }
                );

        internal IEnumerable<SnapshotValue> SelectTokens(string selectPath) => jToken.SelectTokens(selectPath ?? "$").Select(_ => new SnapshotValue(_));
        #endregion
        #endregion

        #region Helper classes
        /// <summary>
        /// Newtonsoft.Json converter that converts Guid values to strings.
        /// Using this converter when calling JToken.FromObject ensures that JSON tokens created from Guid object
        /// can be successfully compared to JSON tokens that are deserialized from a string representation of the Guid.
        /// </summary>
        private class GuidConverter: JsonConverter<Guid>
        {
            public override void WriteJson(JsonWriter writer, Guid value, JsonSerializer serializer)
                => writer.WriteValue(value.ToString());

            public override Guid ReadJson(JsonReader reader, Type objectType, Guid existingValue, bool hasExistingValue, JsonSerializer serializer)
                => throw new NotImplementedException();
        }

        private class OrderedContractResolver : DefaultContractResolver
        {
            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
                => base.CreateProperties(type, memberSerialization).OrderBy(p => p.PropertyName).ToList();
        }
        #endregion
    }
}
