using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace SnapTest.Middleware
{
    /// <remarks>
    /// The following Json serialization options are configured to use by default:
    /// * WriteIndented = true
    /// * SerializeStrings = false
    ///
    /// Default serialization options can be overwritten by changing them in the Options property.
    /// One way to do this is when calling Snapshot.Use. For example:
    ///
    /// var snapshot = new Snapshot();
    /// ...
    /// snapshot.Use<JsonSerializerMiddlware>(_ => _.Options.WriteIndented = false);
    /// </remarks>
    public class JsonSerializerMiddlware : SnapshotMiddlewareBase
    {
        public JsonSerializerOptions Options { get; } = new JsonSerializerOptions();

        public override bool Process(SnapshotContext context)
        {
            if (Options.SerializeStrings || !(context.Actual is string))
                context.Actual = FilterAndSerializeValue(context.Actual);

            if (!ProcessNext(context))
                return false;

            if (context.ExpectedValueKnown && !(context.Expected is string))
                context.Expected = FilterAndSerializeValue(context.Expected);

            return true;
        }

        private string FilterAndSerializeValue(object value)
        {
            if (value != null) {
                JToken root = JToken.FromObject(value);
                ApplyExcludedPaths(root);
                value = ApplySelectPath(root);
            }

            return JsonConvert.SerializeObject(value, Options.WriteIndented ? Formatting.Indented : Formatting.None);
        }

        private void ApplyExcludedPaths(JToken root)
        {
            List<JToken> toRemove = new List<JToken>();
            foreach (string p in Options.ExcludedPaths) {
                var tokens = root.SelectTokens(p);
                if (tokens.Where(_ => _.Root == _).Any())
                    throw new SnapTestException($"JsonPath '{p}' in ExcludedPaths matched root token of the result. You cannot exclude the entire result.");

                toRemove.AddRange(tokens);
            }

            foreach (var e in toRemove.Where(_ => _.Parent != null)) {
                // If token's parent is a JProperty then that is what needs to be removed.
                // Otherwise remove the token itself.
                (e.Parent is JProperty ? e.Parent : e).Remove();
            }
        }

        private object ApplySelectPath(JToken root)
        {
            if (Options.SelectPath == null)
                return root;

            var tokens = root.SelectTokens(Options.SelectPath).ToArray();

            // Result is:
            // 1. null if SelectPath matches nothing;
            // 2. The token itself if there is a single match; or
            // 3. An array if there are multiple matches

            if (tokens.Length == 0)
                return null;

            if (tokens.Length == 1)
                return tokens[0];

            return tokens;
        }
    }
}
