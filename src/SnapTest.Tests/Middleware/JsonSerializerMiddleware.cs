using NUnit.Framework;

using SnapTest.Middleware;

namespace SnapTest.Tests
{
    public class JsonSerializerMiddlwareTest
    {
        [Test]
        public void Expected_remains_unset()
        {
            var context = new SnapshotContext() { Actual = "actual value" };
            new JsonSerializerMiddlware().Process(context);
            Assert.That(context.ExpectedValueKnown, Is.False);
        }

        [TestCaseSource(nameof(JsonTestCases))]
        public void Actual_and_Expected_values_serialize(object inputValue, string actualOutputValue, string expectedOutputValue, bool? shouldSerializeStrings)
        {
            var context = new SnapshotContext() { Actual = inputValue, Expected = inputValue };
            var js = new JsonSerializerMiddlware();

            if (shouldSerializeStrings.HasValue)
                js.ShouldSerializeStrings = shouldSerializeStrings.Value;

            js.Process(context);

            Assert.That(context.Actual, Is.EqualTo(actualOutputValue));
            Assert.That(context.Expected, Is.EqualTo(expectedOutputValue ?? actualOutputValue));
        }

        public static System.Collections.Generic.IEnumerable<object[]> JsonTestCases()
        {
            yield return new object[]{ null, "null", null, null };
            yield return new object[]{ 42, "42", null, null };
            yield return new object[]{ "a value", "a value", null, null };
            yield return new object[]{ "b value", "b value", null, false };
            yield return new object[]{ "c value", "\"c value\"", "c value", true };

            yield return new object[]{
                new { item1 = "the value", item2 = 42 },
                "{\n  \"item1\": \"the value\",\n  \"item2\": 42\n}".Replace("\n", System.Environment.NewLine),
                null,
                null
            };
        }
   }
}
