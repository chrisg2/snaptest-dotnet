using NUnit.Framework;
using NUnit.Framework.Constraints;
using System.Collections.Generic;
using System.IO;

namespace SnapTest.NUnit.Tests
{
    public class Does_MatchSnapshot
    {
        [Test]
        public void Does_MatchSnapshot_is_not_null()
            => Assert.That(Does.MatchSnapshot(), Is.Not.Null);

        [Test]
        public void MatchSnapshot_can_accept_test_name()
            => Assert.That(Does.MatchSnapshot("snapshot name").TestName, Is.EqualTo("snapshot name"));

        [Test]
        public void MatchSnapshot_default_TestName_matches_class_dot_method()
            => Assert.That(Does.MatchSnapshot().TestName, Is.EqualTo(nameof(Does_MatchSnapshot) + "." + nameof(MatchSnapshot_default_TestName_matches_class_dot_method)));

        [TestCaseSource(nameof(SimpleTestCaseSource))]
        public void MatchSnapshot_default_TestName_matches_class_dot_method_paramvalue_with_TestCaseSource(string param)
            => Assert.That(Does.MatchSnapshot().TestName, Is.EqualTo(
                $"{nameof(Does_MatchSnapshot)}.{nameof(MatchSnapshot_default_TestName_matches_class_dot_method_paramvalue_with_TestCaseSource)}(\"{param}\")"
            ));

        public static IEnumerable<object> SimpleTestCaseSource() { yield return "a value"; }

        [Test]
        public void Does_MatchSnapshot_ApplyTo_result_properties_are_expected_for_matching_content()
        {
            var result = Does.MatchSnapshot().ApplyTo("actual value");

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Status, Is.EqualTo(ConstraintStatus.Success));
        }

        [Test]
        public void Does_MatchSnapshot_ApplyTo_result_properties_are_expected_for_mismatching_content()
        {
            var result = Does.MatchSnapshot().ApplyTo("this actual value is different from snapshotted value");

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Status, Is.EqualTo(ConstraintStatus.Failure));
        }

        [Test]
        public void Does_Not_MatchSnapshot_ApplyTo_result_properties_are_expected_for_mismatching_content()
        {
            IResolveConstraint expression = Does.Not.MatchSnapshot();
            var result = expression.Resolve().ApplyTo("this actual value is different from snapshotted value");

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Status, Is.EqualTo(ConstraintStatus.Success));
        }

        [TestCaseSource(nameof(SnapshotTestCases))]
        public void Assert_That_Does_MatchSnapshot_on_correct_value_passes(string testCaseName, object actualValue)
            => Assert.That(actualValue, Does.MatchSnapshot(nameof(Assert_That_Does_MatchSnapshot_on_correct_value_passes) + "." + testCaseName));

        [TestCaseSource(nameof(SnapshotTestCases))]
        public void Assert_That_Does_Not_MatchSnapshot_on_incorrect_value_passes(string testCaseName, object actualValue)
            => Assert.That(actualValue, Does.Not.MatchSnapshot(nameof(Assert_That_Does_Not_MatchSnapshot_on_incorrect_value_passes)));

        public static System.Collections.Generic.IEnumerable<object[]> SnapshotTestCases()
        {
            yield return new object[]{ "null", null, };
            yield return new object[]{ "simple_string", "simple string" };
            yield return new object[]{ "non-ANSI_string", "Non-ANSI string ¤¥£¢©۝" };
            yield return new object[]{ "integer", 42 };
            yield return new object[]{ "anonymous_object", new { aItem = "string", bItem = new { b1Item = 5 } } };
            yield return new object[]{ "guid", System.Guid.Parse("{81d130d2-802f-4cf1-d376-63edeb730e9f}") };
        }
    }
}
