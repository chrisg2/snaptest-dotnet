using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SnapTest
{
    public partial class Snapshot
    {
        #region Json manipulation methods
        private static JToken ActualValueAsJson(object actual, SnapshotSettings settings)
        {
            if (actual == null)
                return JValue.CreateNull();

            // Use a serializer that:
            // 1. Orders properties alphabetically (ensured by OrderedContractResolver); and
            // 2. Translates Guid objects cleanly to strings that will compare with strings read from a snapshot file (ensured by GuidConverter)

            var serializer = JsonSerializer.CreateDefault(new JsonSerializerSettings() { ContractResolver = new OrderedContractResolver() });
            serializer.Converters.Add(new GuidConverter());

            return FilterValue(JToken.FromObject(actual, serializer), settings);
        }

        private static string Serialize(object value, SnapshotSettings settings)
            => value is JValue jValue && jValue.Type == JTokenType.String
            ? jValue.ToString()
            : JsonConvert.SerializeObject(value, settings.IndentJson ? Formatting.Indented : Formatting.None);

        private static JToken FilterValue(JToken value, SnapshotSettings settings)
        {
            // Remove parts of the value which are explicitly excluded in settings.ExcludedPaths
            RemoveExcludedPaths(value, settings);

            // Select parts of the value which are identified by settings.SelectPath
            var selected = ApplySelectPath(value, settings).ToArray();

            switch (selected.Length) {
                case 0: return null;
                case 1: return selected.First();        // SelectPatch matched exactly 1 token - explode it and return it directly
                default: return new JArray(selected);   // SelectPatch matched >1 token - return them as an array
            }
        }

        private static void RemoveExcludedPaths(JToken root, SnapshotSettings settings)
        {
            var toRemove = settings.ExcludedPaths
                .Select(p => {
                    var tokens = root.SelectTokens(p);
                    if (tokens.Where(_ => _.Root == _).Any())
                        throw new SnapTestParseException($"JsonPath '{p}' in ExcludedPaths matched root token of the result. You cannot exclude the entire result.");

                    return tokens;
                })
                .SelectMany(_ => _)
                .ToList() // Force expansion of JPath references to actual tokens here so results won't be dynamically determined while removals take place in the subsequent loop
                .Where(_ => _.Parent != null);

            foreach (var e in toRemove) {
                // If token's parent is a JProperty then that is what needs to be removed. Otherwise remove the token itself.
                (e.Parent is JProperty ? e.Parent : e).Remove();
            }
        }

        private static IEnumerable<JToken> ApplySelectPath(JToken root, SnapshotSettings settings)
            => settings.SelectPath == null ? new JToken[]{ root } : root.SelectTokens(settings.SelectPath);
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
