﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SnapTest.Tests
{
    public class SnapshotBasicFunctionalTest
    {
        #region Helper classes used to execute test cases and determine expected outputs
        private class OrderedContractResolver : DefaultContractResolver
        {
            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
                => base.CreateProperties(type, memberSerialization).OrderBy(p => p.PropertyName).ToList();
        }

        public class InputConditions: TempFileSnapshotSettings
        {
            #region Constructors
            /// <summary>
            ///
            /// </summary>
            /// <param name="actual">Actual value to be matched against the snapshot.</param>
            /// <param name="snapshotFileContents">Contents to be written to the snapshot file before performing the snapshot match operation.</param>
            public InputConditions(object actual, string snapshotFileContents = null)
            {
                Actual = actual;
                SnapshotFileContents = snapshotFileContents;
            }
            #endregion

            #region Properties
            /// <summary>
            /// Raw actual value to be matched against the snapshot.
            /// </summary>
            public object Actual;

            /// <summary>
            /// The contents to be written to the snapshot file as an input condition.
            /// Non-null values are generally (under normal conditions) expected to finish with Environment.NewLine.
            /// </summary>
            /// <value></value>
            public string SnapshotFileContents;

            #region Properties to get expected observable outcomes from calling Snapshot.MatchTo
            public bool ExpectedMatchToResult {
                get {
                    if (ForceSnapshotRefresh
                        || (CreateMissingSnapshots && SnapshotFileContents == null)
                        || (SnapshotFileContents == ActualSerialized + Environment.NewLine)
                    )
                        return true;

                    try {
                        var sn = JToken.Parse(ExpectedSnapshotFileContents);
                        if (SnapshotGroupKey != null)
                            sn = (sn as JObject).Property(SnapshotGroupKey)?.Value;

                        return JToken.DeepEquals(sn, ActualJson);
                    }
                    catch
                    {
                        return false;
                    }
                }
            }

            public JToken ActualJson {
                get {
                    if (Actual == null)
                        return JValue.CreateNull();

                    var ja = JToken.FromObject(Actual, JsonSerializer.CreateDefault(new JsonSerializerSettings() { ContractResolver = new OrderedContractResolver() }));

                    foreach (var e in ExcludedPaths.Select(_ => ja.SelectTokens(_)).SelectMany(_ => _).ToList().Where(_ => _.Parent != null)) {
                        // If token's parent is a JProperty then that is what needs to be removed.
                        // Otherwise remove the token itself.
                        (e.Parent is JProperty ? e.Parent : e).Remove();
                    }

                    if (IncludedPaths.Any()) {
                        var selected = IncludedPaths.Select(_ => ja.SelectTokens(_)).SelectMany(_ => _).ToArray();
                        switch (selected.Length) {
                            case 0: ja = null; break;
                            case 1: ja = selected.First(); break;
                            default: ja = new JArray(selected); break;
                        }
                    }

                    return ja;
                }
            }

            /// <summary>
            /// The serialized string corresponding to Actual, with no trailing new line and new lines represented by Environment.NewLine.
            /// </summary>
            /// <value></value>
            public string ActualSerialized => Serialize(ActualJson);

            private string Serialize(object value)
                => value is JValue jValue && (jValue.Type == JTokenType.String || jValue.Type == JTokenType.Guid)
                ? jValue.ToString()
                : JsonConvert.SerializeObject(value, IndentJson ? Formatting.Indented : Formatting.None);

            public string ExpectedSnapshotFileContents {
                get {
                    if (!ForceSnapshotRefresh && !CreateMissingSnapshots)
                        return SnapshotFileContents;

                    if (SnapshotGroupKey == null) {
                        if (!ForceSnapshotRefresh && SnapshotFileContents != null)
                            return SnapshotFileContents;

                        // otherwise: if (ForceSnapshotRefresh || SnapshotFileContents == null)
                        return ActualSerialized + Environment.NewLine;
                    }

                    JObject ja;
                    if (SnapshotFileContents == null) {
                        ja = new JObject();
                        ja.Add(SnapshotGroupKey, ActualJson);
                    } else {
                        ja = JObject.Parse(SnapshotFileContents);
                        var jp = ja.Property(SnapshotGroupKey);
                        if (ForceSnapshotRefresh && jp != null)
                            jp.Value.Replace(ActualJson);
                        else if (ForceSnapshotRefresh || (CreateMissingSnapshots && jp == null))
                            ja.Add(SnapshotGroupKey, ActualJson);
                        else
                            return SnapshotFileContents; // No change to snapshot file contents expected
                    }

                    return Serialize(ja) + Environment.NewLine;
                }
            }

            public string ExpectedMismatchFileContents {
                get {
                    if (ExpectedMatchToResult)
                        return null;

                    if (SnapshotGroupKey == null)
                        return ActualSerialized + Environment.NewLine;

                    var ja = new JObject();
                    ja.Add(SnapshotGroupKey, ActualJson);
                    return Serialize(ja) + Environment.NewLine;
                }
            }
            #endregion
            #endregion

            #region Methods
            public bool PerformMatchTo()
            {
                EnsureSnapshotFilesAreDeleted();

                File.WriteAllText(MismatchedActualFilePath, "initial mismatch file");
                if (SnapshotFileContents != null)
                    File.WriteAllText(SnapshotFilePath, SnapshotFileContents);

                return Snapshot.MatchTo(Actual, this);
            }

            public override string ToString()
            {
                var settings = new List<string>();

                settings.Add($"Actual={JsonConvert.SerializeObject(Actual, Formatting.None)}");
                settings.Add($"SnapshotFileContents={JsonConvert.SerializeObject(SnapshotFileContents, Formatting.None)}");

                // Output any non-default option values
                if (SnapshotGroupKey != null)
                    settings.Add($"SnapshotGroupKey=\"{SnapshotGroupKey}\"");

                if (IndentJson != true)
                    settings.Add($"IndentJson={IndentJson}");

                if (IncludedPaths != null)
                    settings.Add($"IncludedPaths=[\"{string.Join("\",\"", IncludedPaths)}\"]");

                if (ExcludedPaths.Any())
                    settings.Add($"ExcludedPaths=[\"{string.Join("\",\"", ExcludedPaths)}\"]");

                if (CreateMissingSnapshots != false)
                    settings.Add($"CreateMissingSnapshots={CreateMissingSnapshots}");

                if (ForceSnapshotRefresh != false)
                    settings.Add($"ForceSnapshotRefresh={ForceSnapshotRefresh}");

                return $"InputConditions({string.Join(", ", settings)})";
            }
            #endregion
        }

        #region Confirm various assumptions about selected InputConditions class behaviors to check for bugs in test framework
        // Other assumptions are verified directly in the SnapshotTestCases method

        [Test]
        public void InputConditions_ActualSerialized_string_is_not_quoted()
        {
            using var i = new InputConditions("string", null);
            Assume.That(i.ActualSerialized, Is.EqualTo("string"));
        }

        [Test]
        public void InputConditions_ActualSerialized_Guid_is_not_quoted()
        {
            using var i = new InputConditions(Guid.NewGuid(), null);
            Assume.That(i.ActualSerialized, Does.Not.Contain("\""));
        }

        [Test]
        public void InputConditions_ActualSerialized_contains_indents_with_IndentJson_default()
        {
            using var i = new InputConditions(Garden.Flagstaff);
            Assume.That(i.ActualSerialized, Does.Contain($"{{{Environment.NewLine}  "));
            Assume.That(i.ActualSerialized, Does.Contain(": "));
        }

        [Test]
        public void InputConditions_ActualSerialized_does_not_contain_indents_with_IndentJson_false()
        {
            using var i = new InputConditions(Garden.Flagstaff) { IndentJson = false };
            Assume.That(i.ActualSerialized, Does.Not.Contain($"{{{Environment.NewLine}  "));
            Assume.That(i.ActualSerialized, Does.Not.Contain(": "));
        }

        [Test]
        public void InputConditions_ActualSerialized_does_not_depend_on_property_order()
        {
            // ActualSerialized values should be the same, regardless of the order the properties are defined in.
            // This subsequently ensures that values serialized by SnapTest are similarly unaffected by order.
            using var i1 = new InputConditions(new {a = 1, b = 2});
            using var i2 = new InputConditions(new {b = 2, a = 1});
            Assume.That(i1.ActualSerialized, Is.EqualTo(i2.ActualSerialized));
        }

        [Test]
        public void InputConditions_IncludedPaths_with_array_elements_gives_expected_results()
        {
            using (var i = new InputConditions(Garden.Flagstaff) { IndentJson = false }) {
                i.Field("Trees[1,0]").Include();
                Assume.That(i.ActualSerialized, Is.EqualTo("[\"Eucalyptus\",\"Elm\"]"));
            }

            using (var i = new InputConditions(Garden.Flagstaff) { IndentJson = false }) {
                i.Field("Trees[0,1]").Include();
                Assume.That(i.ActualSerialized, Is.EqualTo("[\"Elm\",\"Eucalyptus\"]"));
            }

            using (var i = new InputConditions(Garden.Flagstaff) { IndentJson = false }) {
                i.Field("Trees[0,0]").Include();
                Assume.That(i.ActualSerialized, Is.EqualTo("[\"Elm\",\"Elm\"]"));
            }

            using (var i = new InputConditions(Garden.Flagstaff) { IndentJson = false }) {
                i.Field("$['Name','Address']").Include();
                Assume.That(i.ActualSerialized, Is.EqualTo("[\"Flagstaff\",{\"Postcode\":\"3000\",\"Street\":\"William\"}]"));
            }

            using (var i = new InputConditions(Garden.Flagstaff) { IndentJson = false }) {
                i.Field("Name").Include();
                i.Field("Address").Include();
                Assume.That(i.ActualSerialized, Is.EqualTo("[\"Flagstaff\",{\"Postcode\":\"3000\",\"Street\":\"William\"}]"));
            }
        }

        [Test]
        public void InputConditions_ExcludePaths_excludes_expected_fields()
        {
            using (var i = new InputConditions(Garden.Flagstaff) { IndentJson = false }) {
                Assume.That(i.ActualSerialized, Does.Contain("Name"));
                Assume.That(i.ActualSerialized, Does.Contain("Street"));
                i.Field("Name").Exclude(); // Exclude a value
                Assume.That(i.ActualSerialized, Does.Not.Contain("Name"));
            }

            using (var i = new InputConditions(Garden.Flagstaff) { IndentJson = false }) {
                i.Field("Address.Street").Exclude(); // Exclude a nested value
                Assume.That(i.ActualSerialized, Does.Contain("Address"));
                Assume.That(i.ActualSerialized, Does.Not.Contain("Street"));
            }

            using (var i = new InputConditions(Garden.Flagstaff) { IndentJson = false }) {
                i.Field("Address").Exclude(); // Exclude an object
                Assume.That(i.ActualSerialized, Does.Not.Contain("Address"));
            }

            using (var i = new InputConditions(Garden.Flagstaff) { IndentJson = false }) {
                i.Field("Trees").Exclude();
                Assume.That(i.ActualSerialized, Does.Not.Contain("Trees"));
            }

            using (var i = new InputConditions(Garden.Flagstaff) { IndentJson = false }) {
                i.Field("Trees[0,1,0]").Exclude(); // Exclude array elements
                Assume.That(i.ActualSerialized, Does.Contain("\"Trees\":[\"Morton Bay Fig\"]"));
            }
        }
        #endregion
        #endregion

        #region Helpers to get and clean up InputCondition objects
        // Keep track of all InputCondition objects created in SnapshotTestCases so they can be disposed in DisposeInputConditions
        private static readonly List<InputConditions> _inputConditions = new List<InputConditions>();

        private static InputConditions NewInputConditions(object actual, string snapshotFileContents = null)
        {
            var ic = new InputConditions(actual, snapshotFileContents);
            _inputConditions.Add(ic);
            return ic;
        }

        [OneTimeTearDown]
        public static void DisposeInputConditions()
            => _inputConditions.ForEach(_ => _.Dispose());
        #endregion

        #region Test case data for core functional testing
        public static IEnumerable<InputConditions> SnapshotTestCases()
        {
            // SnapshotSettings settings to be exercised (with default value from TempFileSnapshotSettings noted):
            // - CreateMissingSnapshots (false)
            // - ForceSnapshotRefresh (false)
            // - IndentJson (true)
            // - IncludedPaths (null)
            // - ExcludedPaths (empty)
            // - SnapshotGroupKey (null)

            // Construct a sample dictionary with property names containing special characters
            var sampleDict = new Dictionary<string, int>();
            var nastyPropertyName = "prop.[]\"'(); e,rty";
            sampleDict.Add($"{nastyPropertyName}1", 42);
            sampleDict.Add($"{nastyPropertyName}2", -1);

            var testValues = new object[] {
                null,
                "",
                "simple ¤¥£¢©۝ string",
                Environment.NewLine,
                42,
                new { },
                new { a = 42 },
                Guid.NewGuid(),
                new { a = Guid.NewGuid() },
                Garden.Flagstaff,
                sampleDict
            };

            InputConditions i;

            foreach (object actual in testValues) {
                // Test all combinations of ForceSnapshotRefresh, CreateMissingSnaphots and existing snapshot file configuration
                foreach (var forceSnapshotRefresh in new bool[]{true, false}) {
                    foreach (var creatingMissingSnapshots in new bool[]{true, false}) {
                        foreach (var snFile in new string[]{null, "{ \"bogusSnapshot\": { } }"}) {
                            i = NewInputConditions(actual);
                            i.ForceSnapshotRefresh = forceSnapshotRefresh;
                            i.CreateMissingSnapshots = creatingMissingSnapshots;
                            i.SnapshotFileContents = snFile;
                            Assume.That(i.ExpectedMatchToResult, Is.EqualTo(forceSnapshotRefresh || (creatingMissingSnapshots && snFile == null)));
                            yield return i;
                        }

                        i = NewInputConditions(actual);
                        i.ForceSnapshotRefresh = forceSnapshotRefresh;
                        i.CreateMissingSnapshots = creatingMissingSnapshots;
                        i.SnapshotFileContents = i.ActualSerialized + Environment.NewLine;
                        Assume.That(i.ExpectedMatchToResult, Is.True);
                        yield return i;
                    }
                }

                // Test IndentJson = true & false
                foreach (var indent in new bool[]{true, false}) {
                    i = NewInputConditions(actual);
                    i.ForceSnapshotRefresh = true;
                    i.IndentJson = indent;
                    yield return i;
                }
            }


            // Check that ordering of properties in Json is not important for matching
            i = NewInputConditions(new {a = 1, b = 2});
            i.SnapshotFileContents = "{\"b\":2,\"a\":1}"+Environment.NewLine;
            Assume.That(i.ExpectedMatchToResult, Is.True); // Verify that ordering is not important in logic used by InputConditions
            yield return i;

            i = NewInputConditions(new {a = 1, b = 2});
            i.SnapshotFileContents = "{\"a\":1,\"b\":2}"+Environment.NewLine;
            Assume.That(i.ExpectedMatchToResult, Is.True); // Verify that ordering is not important in logic used by InputConditions
            yield return i;


            #region Test cases involving IncludedPaths and ExcludedPaths
            foreach (var actual in new object[]{null, "astring", 42, Guid.NewGuid()}) {
                // Various IncludedPathss on primitive value
                foreach (var p in new string[]{"$", "PropertyDoesNotExist"}) {
                    i = NewInputConditions(actual);
                    i.Field(p).Include();
                    yield return i;
                }

                // ExcludePaths on a primitive value
                i = NewInputConditions(actual);
                i.Field("PropertyDoesNotExist").Exclude();
                yield return i;
            }


            // Test various IncludedPaths and ExcludePaths values
            foreach (var jsonPath in new string[]{
                "$", "Name", "Address", "Address.Postcode",
                "Trees", "Trees[1]", "Trees[1,0]", "Trees[0,1]", "Trees[0,0]", "Trees[0,1,0]",
                "$['Name','Address']"
            }) {
                i = NewInputConditions(Garden.Flagstaff);
                i.Field(jsonPath).Include();
                yield return i;

                if (jsonPath != "$") { // Can't exclude root
                    i = NewInputConditions(Garden.Flagstaff);
                    i.Field(jsonPath).Exclude();
                    yield return i;
                }
            }

            i = NewInputConditions(Garden.Flagstaff);
            i.Field("Name").Include(); // Multiple IncludePath calls
            i.Field("Address").Include();
            yield return i;

            // Test IncludedPaths and ExcludePaths with a property name containing special characters
            i = NewInputConditions(sampleDict);
            i.Field($"$['{nastyPropertyName.Replace("'", "\\'")}1']").Include();
            Assume.That(i.ActualSerialized, Is.EqualTo("42"));
            yield return i;

            i = NewInputConditions(sampleDict);
            i.IndentJson = false;
            i.Field($"$['{nastyPropertyName.Replace("'", "\\'")}2']").Exclude(); // Exclude property with special characters in it
            Assume.That(i.ActualSerialized, Is.EqualTo($"{{\"{nastyPropertyName.Replace("\\", "\\\\").Replace("\"", "\\\"")}1\":42}}"));
            yield return i;


            // Test IncludedPaths and ExcludedPaths set together
            i = NewInputConditions(Garden.Flagstaff);
            i.IndentJson = false;
            i.Field("Name").Include();
            i.Field("Address.Street").Exclude(); // Exclude something unrelated to what is being selected
            Assume.That(i.ActualSerialized, Is.EqualTo("Flagstaff"));
            yield return i;

            i = NewInputConditions(Garden.Flagstaff);
            i.IndentJson = false;
            i.Field("Address").Include();
            i.Field("Address.Postcode").Exclude(); // Exclude a value within the object being selected
            Assume.That(i.ActualSerialized, Is.EqualTo("{\"Street\":\"William\"}"));
            yield return i;

            i = NewInputConditions(Garden.Flagstaff);
            i.IndentJson = false;
            i.Field("Trees").Include();
            i.Field("Trees[0]").Exclude(); // Exclude array element (repeated)
            i.Field("Trees[0]").Exclude();
            Assume.That(i.ActualSerialized, Is.EqualTo("[\"Eucalyptus\",\"Morton Bay Fig\"]"));
            yield return i;

            i = NewInputConditions(Garden.Flagstaff);
            i.IndentJson = false;
            i.Field("Trees").Include();
            i.Field("Trees[0]").Exclude(); // Exclude array elements
            i.Field("Trees[1]").Exclude();
            Assume.That(i.ActualSerialized, Is.EqualTo("[\"Morton Bay Fig\"]"));
            yield return i;

            i = NewInputConditions(Garden.Flagstaff);
            i.IndentJson = false;
            i.Field("Trees").Include();
            i.Field("Trees[1]").Exclude(); // Exclude array elements (reverse order to previous test)
            i.Field("Trees[0]").Exclude();
            Assume.That(i.ActualSerialized, Is.EqualTo("[\"Morton Bay Fig\"]"));
            yield return i;

            i = NewInputConditions(Garden.Flagstaff);
            i.IndentJson = false;
            i.Field("Trees").Include();
            i.Field("Trees[0,1]").Exclude(); // Exclude array elements
            Assume.That(i.ActualSerialized, Is.EqualTo("[\"Morton Bay Fig\"]"));
            yield return i;
            #endregion

            #region Grouping tests
            // Check that simple grouping works
            i = NewInputConditions("value");
            i.SnapshotGroupKey = "group";
            i.ForceSnapshotRefresh = true;
            i.IndentJson = false;
            Assume.That(i.ExpectedMatchToResult, Is.True);
            Assume.That(i.ExpectedSnapshotFileContents, Is.EqualTo("{\"group\":\"value\"}" + Environment.NewLine));
            Assume.That(i.ExpectedMismatchFileContents, Is.Null);
            yield return i;

            i = NewInputConditions("value");
            i.SnapshotGroupKey = "group";
            i.CreateMissingSnapshots = true;
            i.IndentJson = false;
            Assume.That(i.ExpectedMatchToResult, Is.True);
            Assume.That(i.ExpectedSnapshotFileContents, Is.EqualTo("{\"group\":\"value\"}" + Environment.NewLine));
            Assume.That(i.ExpectedMismatchFileContents, Is.Null);
            yield return i;

            i = NewInputConditions("value");
            i.SnapshotGroupKey = "group";
            i.ForceSnapshotRefresh = true;
            i.IndentJson = false;
            i.SnapshotFileContents = "{\"another\":42}";
            Assume.That(i.ExpectedMatchToResult, Is.True);
            Assume.That(i.ExpectedSnapshotFileContents, Is.EqualTo("{\"another\":42,\"group\":\"value\"}" + Environment.NewLine));
            Assume.That(i.ExpectedMismatchFileContents, Is.Null);
            yield return i;

            i = NewInputConditions("value");
            i.SnapshotGroupKey = "group";
            i.CreateMissingSnapshots = true;
            i.IndentJson = false;
            i.SnapshotFileContents = "{\"another\":42}";
            Assume.That(i.ExpectedMatchToResult, Is.True);
            Assume.That(i.ExpectedSnapshotFileContents, Is.EqualTo("{\"another\":42,\"group\":\"value\"}" + Environment.NewLine));
            Assume.That(i.ExpectedMismatchFileContents, Is.Null);
            yield return i;

            i = NewInputConditions("value");
            i.SnapshotGroupKey = "group";
            i.IndentJson = false;
            i.SnapshotFileContents = "{\"another\":42}" + Environment.NewLine;
            Assume.That(i.ExpectedMatchToResult, Is.False);
            Assume.That(i.ExpectedSnapshotFileContents, Is.EqualTo("{\"another\":42}" + Environment.NewLine));
            Assume.That(i.ExpectedMismatchFileContents, Is.EqualTo("{\"group\":\"value\"}" + Environment.NewLine));
            yield return i;

            i = NewInputConditions("value");
            i.SnapshotGroupKey = "group";
            i.IndentJson = false;
            i.SnapshotFileContents = "{\"another\":42,\"group\":\"value\"}" + Environment.NewLine;
            Assume.That(i.ExpectedMatchToResult, Is.True);
            Assume.That(i.ExpectedSnapshotFileContents, Is.EqualTo("{\"another\":42,\"group\":\"value\"}" + Environment.NewLine));
            Assume.That(i.ExpectedMismatchFileContents, Is.Null);
            yield return i;
            #endregion
        }
        #endregion

        [TestCaseSource(nameof(SnapshotTestCases))]
        public static void Snapshot_MatchTo_gives_expected_result(InputConditions inputConditions)
        {
            Assert.That(inputConditions.PerformMatchTo(), Is.EqualTo(inputConditions.ExpectedMatchToResult));
        }

        [TestCaseSource(nameof(SnapshotTestCases))]
        public static void Snapshot_MatchTo_creates_expected_snapshot_file(InputConditions inputConditions)
        {
            inputConditions.PerformMatchTo();

            var c = inputConditions.ExpectedSnapshotFileContents;
            if (c == null) {
                Assert.That(inputConditions.SnapshotFilePath, Does.Not.Exist);
            } else {
                Assert.That(inputConditions.SnapshotFilePath, Does.Exist);
                Assert.That(File.ReadAllText(inputConditions.SnapshotFilePath), Is.EqualTo(c));
            }
        }

        [TestCaseSource(nameof(SnapshotTestCases))]
        public static void Snapshot_MatchTo_creates_expected_mismatch_file(InputConditions inputConditions)
        {
            inputConditions.PerformMatchTo();

            var c = inputConditions.ExpectedMismatchFileContents;
            if (c == null) {
                Assert.That(inputConditions.MismatchedActualFilePath, Does.Not.Exist);
            } else {
                Assert.That(inputConditions.MismatchedActualFilePath, Does.Exist);
                Assert.That(File.ReadAllText(inputConditions.MismatchedActualFilePath), Is.EqualTo(c));
            }
        }

        [Test]
        public static void Snapshot_MatchTo_throws_ArgmentNullException_with_null_options()
            => Assert.Throws<ArgumentNullException>(() => Snapshot.MatchTo("actual", null));

        [Test]
        public static void Snapshot_Compare_attempting_to_exclude_root_throws()
        {
            using var i = new InputConditions(Garden.Flagstaff);
            i.Field("$").Exclude();
            Assert.Throws<SnapTestParseException>(() => i.PerformMatchTo());
        }

        [TestCase("")]
        [TestCase("plain unquoted string")]
        [TestCase("{ bad json }")]
        public static void Snapshot_Compare_with_non_json_snapshot_file_throws(string contents)
        {
            using var i = new InputConditions(null) { SnapshotFileContents = contents };
            Assert.Throws<SnapTestParseException>(() => i.PerformMatchTo());
        }

        [TestCase("")]
        [TestCase(" ")]
        [TestCase("\n")]
        public static void SnapshotSettings_SnapshotGroupKey_must_contain_non_whitespace_character(string snapshotGroupKey)
            => Assert.Throws<ArgumentOutOfRangeException>(() => new SnapshotSettings() { SnapshotGroupKey = snapshotGroupKey });
    }
}
