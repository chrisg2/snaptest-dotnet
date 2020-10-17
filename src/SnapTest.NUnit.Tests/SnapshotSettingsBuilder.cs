using NUnit.Framework;
using System.Collections.Generic;
using System.IO;

namespace SnapTest.NUnit.Tests
{
    public class SnapshotSettingsBuilderTest
    {
        [Test]
        public void Does_MatchSnapshot_can_be_used_with_builder()
        {
            var builder = new SnapshotSettingsBuilder().WithSettings(_ => _.SnapshotExtension = ".txt");
            Assert.That("actual output", SnapshotDoes.Match(builder));
        }

        [Test]
        public void Does_Not_MatchSnapshot_can_be_used_with_builder()
        {
            var builder = new SnapshotSettingsBuilder().WithSettings(_ => _.SnapshotExtension = ".txt");
            Assert.That("different actual output", Does.Not.MatchSnapshot(builder));
        }

        #region Tests verifying behavior of SnapshotSettingsBuilder.Build().SnapshotDirectoryPath
        [Test]
        public void Default_SnapshotDirectoryPath_in_Test_ends_with_source_directory()
            => Assert.That(new SnapshotSettingsBuilder().Build().SnapshotDirectoryPath, Does.EndWith(Path.Combine("SnapTest.NUnit.Tests", "_snapshots")));

        [TestCase]
        public void Default_SnapshotDirectoryPath_in_TestCase_ends_with_source_directory()
            => Assert.That(new SnapshotSettingsBuilder().Build().SnapshotDirectoryPath, Does.EndWith(Path.Combine("SnapTest.NUnit.Tests", "_snapshots")));

        [TestCaseSource(nameof(SimpleTestCaseSource))]
        public void Default_SnapshotDirectoryPath_in_TestCaseSource_ends_with_source_directory(string _)
            => Assert.That(new SnapshotSettingsBuilder().Build().SnapshotDirectoryPath, Does.EndWith(Path.Combine("SnapTest.NUnit.Tests", "_snapshots")));

        public static IEnumerable<object> SimpleTestCaseSource() { yield return "a value"; }

        [Theory]
        public void Default_SnapshotDirectoryPath_in_Theory_ends_with_source_directory()
            => Assert.That(new SnapshotSettingsBuilder().Build().SnapshotDirectoryPath, Does.EndWith(Path.Combine("SnapTest.NUnit.Tests", "_snapshots")));

        [Test]
        public void SnapshotSubdirectory_can_be_set()
        {
            var builder = new SnapshotSettingsBuilder();
            builder.WithSettings(_ => _.SnapshotSubdirectory = "tail");
            Assert.That(builder.Build().SnapshotDirectoryPath, Does.EndWith(Path.Combine("SnapTest.NUnit.Tests", "tail")));
        }

        [Test]
        public void SnapshotSubdirectory_can_be_blank()
        {
            var builder = new SnapshotSettingsBuilder();
            builder.WithSettings(_ => _.SnapshotSubdirectory = string.Empty);
            Assert.That(builder.Build().SnapshotDirectoryPath, Does.EndWith("SnapTest.NUnit.Tests"));
        }

        [Test]
        public void SnapshotSubdirectory_can_be_null()
        {
            var builder = new SnapshotSettingsBuilder();
            builder.WithSettings(_ => _.SnapshotSubdirectory = null);
            Assert.That(builder.Build().SnapshotDirectoryPath, Does.EndWith("SnapTest.NUnit.Tests"));
        }

        [Test]
        public void Built_SnapshotSettings_SnapshotName_matches_class_dot_method()
            => Assert.That(new SnapshotSettingsBuilder().Build().SnapshotName, Is.EqualTo(nameof(SnapshotSettingsBuilderTest) + "." + nameof(Built_SnapshotSettings_SnapshotName_matches_class_dot_method)));

        [TestCaseSource(nameof(SimpleTestCaseSource))]
        public void Built_SnapshotSettings_SnapshotName_matches_class_dot_method_paramvalue_with_TestCaseSource(string param)
            => Assert.That(
                new SnapshotSettingsBuilder().Build().SnapshotName,
                Is.EqualTo(
                    $"{nameof(SnapshotSettingsBuilderTest)}.{nameof(Built_SnapshotSettings_SnapshotName_matches_class_dot_method_paramvalue_with_TestCaseSource)}(\"{param}\")"));

        [Test]
        public void SnapshotName_takes_explicitly_set_value()
        {
            var builder = new SnapshotSettingsBuilder().WithSettings(_ => _.SnapshotName = "explicit");
            Assert.That(builder.Build().SnapshotName, Is.EqualTo("explicit"));
        }


        [Test]
        public void SnapshotDirectoryPath_takes_explicitly_set_value()
        {
            var builder = new SnapshotSettingsBuilder().WithSettings(_ => _.SnapshotDirectoryPath = "explicit");
            Assert.That(builder.Build().SnapshotDirectoryPath, Is.EqualTo("explicit"));
        }

        [Test]
        public void SnapshotGroupKey_with_DefaultSnapshotGroupKeyFromNUnitTestName_set_defaults_to_NUnit_TestName()
        {
            var builder = new SnapshotSettingsBuilder().WithSettings(_ => _.DefaultSnapshotGroupKeyFromNUnitTestName = true);
            Assert.That(builder.Build().SnapshotGroupKey, Is.EqualTo(TestContext.CurrentContext.Test.Name));
        }

        [Test]
        public void SnapshotGroupKey_with_DefaultSnapshotGroupKeyFromNUnitTestName_set_takes_explicitly_set_value()
        {
            var builder = new SnapshotSettingsBuilder().WithSettings(_ => {
                _.DefaultSnapshotGroupKeyFromNUnitTestName = true;
                _.SnapshotGroupKey = "explicit";
            });

            Assert.That(builder.Build().SnapshotGroupKey, Is.EqualTo("explicit"));
        }

        [Test]
        public void SnapshotName_with_DefaultSnapshotGroupKeyFromNUnitTestName_set_is_fixture_class_name()
        {
            var builder = new SnapshotSettingsBuilder().WithSettings(_ => _.DefaultSnapshotGroupKeyFromNUnitTestName = true);
            Assert.That(builder.Build().SnapshotName, Is.EqualTo(nameof(SnapshotSettingsBuilderTest)));
        }
        #endregion
    }
}
