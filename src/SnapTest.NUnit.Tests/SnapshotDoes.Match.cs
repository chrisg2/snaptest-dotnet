using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace SnapTest.NUnit.Tests
{
    public class Does_MatchSnapshot
    {
        [Test]
        public void SnapshotDoes_Match_can_accept_test_name()
            => Assert.That(SnapshotDoes.Match("snapshot name").SnapshotName, Is.EqualTo("snapshot name"));

        [Test]
        public void SnapshotDoes_Match_ApplyTo_result_properties_show_success_for_matching_content()
        {
            var result = SnapshotDoes.Match().ApplyTo("actual value");

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Status, Is.EqualTo(ConstraintStatus.Success));
        }

        [Test]
        public void SnapshotDoes_Match_ApplyTo_result_properties_show_failure_for_mismatching_content()
        {
            var result = SnapshotDoes.Match().ApplyTo("this actual value is different from snapshotted value");

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Status, Is.EqualTo(ConstraintStatus.Failure));
        }

        [Test]
        public void Does_Not_MatchSnapshot_ApplyTo_result_properties_show_success_for_mismatching_content()
        {
            IResolveConstraint expression = Does.Not.MatchSnapshot();
            var result = expression.Resolve().ApplyTo("this actual value is different from snapshotted value");

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Status, Is.EqualTo(ConstraintStatus.Success));
        }

        [TestCaseSource(nameof(SnapshotTestCases))]
        public void SnapshotDoes_Match_on_correct_value_passes(string testCaseName, object actualValue)
            => Assert.That(actualValue, SnapshotDoes.Match(nameof(SnapshotDoes_Match_on_correct_value_passes) + "." + testCaseName));

        [TestCaseSource(nameof(SnapshotTestCases))]
        public void Does_Not_MatchSnapshot_on_incorrect_value_passes(string testCaseName, object actualValue)
            => Assert.That(actualValue, Does.Not.MatchSnapshot("Snapshot_that_does_not_match_any_actual_value"));

        public static System.Collections.Generic.IEnumerable<object[]> SnapshotTestCases()
        {
            yield return new object[]{ "null", null, };
            yield return new object[]{ "simple_string", "simple string" };
            yield return new object[]{ "non-ANSI_chars_¤¥£¢©۝", "Non-ANSI string ¤¥£¢©۝" };
            yield return new object[]{ "special_chars_!@#$%^&*()_+-=[]\\{}|;':\",./<>?", "Special characters !@#$%^&*()_+-=[]\\{}|;':\",./<>?" };
            yield return new object[]{ "integer", 42 };
            yield return new object[]{ "anonymous_object", new { aItem = "string", bItem = new { b1Item = 5 } } };
            yield return new object[]{ "anonymous_object", new { bItem = new { b1Item = 5 }, aItem = "string" } }; // Different property order to previous line to ensure order does not matter
            yield return new object[]{ "guid", System.Guid.Parse("{81d130d2-802f-4cf1-d376-63edeb730e9f}") };
        }

        #region Exercise different interfaces for configuring snapshot settings
        [Test]
        public void SnapshotSubdirectory_can_be_set_through_SettingsBuilder()
            => Assert.That("a value", SnapshotDoes.Match(SnapshotSettings.GetBuilder().WithSettings(_ => _.SnapshotSubdirectory = "_alternateSnapshots")));

        [Test]
        public void SnapshotSubdirectory_can_be_set_through_WithSettings()
            => Assert.That("a value", SnapshotDoes.Match().WithSettings(_ => _.SnapshotSubdirectory = "_alternateSnapshots"));

        [Test]
        public void SnapshotSubdirectory_can_be_set_through_Match_action()
            => Assert.That("a value", SnapshotDoes.Match(_ => _.SnapshotSubdirectory = "_alternateSnapshots"));

        [Test]
        public void SnapshotSubdirectory_can_be_set_through_SettingsBuilder_on_ConstraintExpression()
            => Assert.That("a value", Is.False.Or.MatchSnapshot(SnapshotSettings.GetBuilder().WithSettings(_ => _.SnapshotSubdirectory = "_alternateSnapshots")));

        [Test]
        public void SnapshotSubdirectory_can_be_set_through_MatchSnapshot_on_ConstraintExpression()
            => Assert.That("a value", Is.False.Or.MatchSnapshot(_ => _.SnapshotSubdirectory = "_alternateSnapshots"));

        [Test]
        public void SnapshotExtension_can_be_set()
            => Assert.That("a value", SnapshotDoes.Match().WithSettings(_ => _.SnapshotExtension = ".alternate"));

        [Test]
        public void Does_Not_MatchSnapshot_can_be_used_with_builder()
        {
            var builder = SnapshotSettings.GetBuilder().WithSettings(_ => _.SnapshotExtension = ".txt");
            Assert.That("different actual output", Does.Not.MatchSnapshot(builder));
        }
        #endregion
    }
}
