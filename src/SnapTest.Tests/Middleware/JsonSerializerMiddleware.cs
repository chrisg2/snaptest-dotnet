using NUnit.Framework;
using System.Collections.Generic;

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
                js.Options.SerializeStrings = shouldSerializeStrings.Value;

            js.Process(context);

            Assert.That(context.Actual, Is.EqualTo(actualOutputValue));
            Assert.That(context.Expected, Is.EqualTo(expectedOutputValue ?? actualOutputValue));
        }

        public static IEnumerable<object[]> JsonTestCases()
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

		[Test]
        public void Cannot_exclude_entire_result()
        {
            var js = new JsonSerializerMiddlware();
			js.Options.ExcludedPaths.Add("$");
            Assert.Throws<SnapTestException>(() => js.Process(new SnapshotContext() { Actual = new Garden() { Name = "Rosehill" } }));
        }

        [TestCaseSource(nameof(FilterTestCases))]
        public void Json_filtering_is_effective(object inputValue, string selectPath, IEnumerable<string> excludePaths, string expectedSerializedInputValue)
        {
            var js = new JsonSerializerMiddlware();
            js.Options.WriteIndented = false;
            js.Options.SelectPath = selectPath;

            if (excludePaths != null)
                js.Options.ExcludedPaths.AddRange(excludePaths);

			var context = new SnapshotContext() { Actual = inputValue };
            js.Process(context);

            Assert.That(context.Actual, Is.EqualTo(expectedSerializedInputValue));
        }

        public static IEnumerable<object[]> FilterTestCases()
        {
            yield return new object[]{ null, null, null, "null" };
            yield return new object[]{ "string value", null, null, "string value" };
            yield return new object[]{ 42, null, null, "42" };
            yield return new object[]{ true, null, null, "true" };

			// Test cases that select what to include, with no exclude filters applied
            yield return new object[]{ Flagstaff, null, null, "{\"Name\":\"Flagstaff\",\"Rating\":4,\"Address\":{\"Street\":\"William\",\"Postcode\":\"3000\"},\"Trees\":[\"Elm\",\"Eucalyptus\",\"Morton Bay Fig\"]}" };
            yield return new object[]{ Flagstaff, "$", null, "{\"Name\":\"Flagstaff\",\"Rating\":4,\"Address\":{\"Street\":\"William\",\"Postcode\":\"3000\"},\"Trees\":[\"Elm\",\"Eucalyptus\",\"Morton Bay Fig\"]}" };
            yield return new object[]{ Flagstaff, "Name", null, "\"Flagstaff\"" };
            yield return new object[]{ Flagstaff, "Address", null, "{\"Street\":\"William\",\"Postcode\":\"3000\"}" };
            yield return new object[]{ Flagstaff, "Trees", null, "[\"Elm\",\"Eucalyptus\",\"Morton Bay Fig\"]" };
            yield return new object[]{ Flagstaff, "Trees[1]", null, "\"Eucalyptus\"" };
            yield return new object[]{ Flagstaff, "Trees[1,0]", null, "[\"Eucalyptus\",\"Elm\"]" };

			// Test cases with exclude filters applied but no include select filter
            yield return new object[]{
				Flagstaff, null,
				new string[]{ "Name" }, // Exclude a value
				"{\"Rating\":4,\"Address\":{\"Street\":\"William\",\"Postcode\":\"3000\"},\"Trees\":[\"Elm\",\"Eucalyptus\",\"Morton Bay Fig\"]}"
			};

            yield return new object[]{
				Flagstaff, null,
				new string[]{ "Address.Street" }, // Exclude a nested value
				"{\"Name\":\"Flagstaff\",\"Rating\":4,\"Address\":{\"Postcode\":\"3000\"},\"Trees\":[\"Elm\",\"Eucalyptus\",\"Morton Bay Fig\"]}"
			};

            yield return new object[]{
				Flagstaff, null,
				new string[]{ "Address" }, // Exclude an object
				"{\"Name\":\"Flagstaff\",\"Rating\":4,\"Trees\":[\"Elm\",\"Eucalyptus\",\"Morton Bay Fig\"]}"
			};

            yield return new object[]{
				Flagstaff, null,
				new string[]{ "Trees" }, // Exclude an array
				"{\"Name\":\"Flagstaff\",\"Rating\":4,\"Address\":{\"Street\":\"William\",\"Postcode\":\"3000\"}}"
			};

            yield return new object[]{
				Flagstaff, null,
				new string[]{ "Trees[0,1]" }, // Exclude array elements
				"{\"Name\":\"Flagstaff\",\"Rating\":4,\"Address\":{\"Street\":\"William\",\"Postcode\":\"3000\"},\"Trees\":[\"Morton Bay Fig\"]}"
			};

			// Test cases with both select and exclude filters applied
            yield return new object[]{
				Flagstaff,
				"Name",
				new string[]{ "Address.Street" }, // Exclude something unrelated to what is being selected
				"\"Flagstaff\""
			};

            yield return new object[]{
				Flagstaff,
				"Address",
				new string[]{ "Address.Postcode" }, // Exclude a value
				"{\"Street\":\"William\"}"
			};

            yield return new object[]{
				Flagstaff,
				"Trees",
				new string[]{ "Trees[0]", "Trees[0]" }, // Exclude array elements
				"[\"Eucalyptus\",\"Morton Bay Fig\"]"
			};

            yield return new object[]{
				Flagstaff,
				"Trees",
				new string[]{ "Trees[0]", "Trees[1]" }, // Exclude array elements
				"[\"Morton Bay Fig\"]"
			};

            yield return new object[]{
				Flagstaff,
				"Trees",
				new string[]{ "Trees[0,1]" }, // Exclude array elements
				"[\"Morton Bay Fig\"]"
			};

            yield return new object[]{
				Flagstaff,
				"Trees",
				new string[]{ "Trees[1]", "Trees[0]" }, // Exclude array elements
				"[\"Morton Bay Fig\"]"
			};
        }

        private class Address
        {
            public string Street;
            public string Postcode;
        }

        private class Garden
        {
            public string Name;
            public int Rating;
            public Address Address;
            public IEnumerable<string> Trees;
        }

		private readonly static Garden Flagstaff = new Garden() {
			Name = "Flagstaff",
			Rating = 4,
			Address = new Address() { Street = "William", Postcode = "3000" },
			Trees = new List<string>(new string[] { "Elm", "Eucalyptus", "Morton Bay Fig" })
		};
   }
}
