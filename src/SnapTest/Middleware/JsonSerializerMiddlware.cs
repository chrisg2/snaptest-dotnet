using System.Text.Json;

namespace SnapTest.Middleware
{
    /// <remarks>
    /// The following Json serialization options are configured to use by default:
    /// * IgnoreNullValues = true
    /// * WriteIndented = true
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
        public bool ShouldSerializeStrings = false;
        public JsonSerializerOptions Options { get; } = new JsonSerializerOptions { IgnoreNullValues = true, WriteIndented = true };

        public override bool Process(SnapshotContext context)
        {
            if (ShouldSerializeStrings || !(context.Actual is string))
                context.Actual = JsonSerializer.Serialize(context.Actual, Options);

            if (!ProcessNext(context))
                return false;

            if (context.ExpectedValueKnown && !(context.Expected is string))
                context.Expected = JsonSerializer.Serialize(context.Expected, Options);

            return true;
        }
    }
}
