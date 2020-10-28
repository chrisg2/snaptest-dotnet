using NUnit.Framework;
using System.Collections.Generic;
using System.IO;

namespace SnapTest.NUnit.Tests
{
    public class SnapshotSettingsBuilderTest
    {
        #region Tests verifying behavior of SnapshotSettingsBuilder.Build().SnapshotDirectoryPath
        [Test]
        public void Default_SnapshotDirectoryPath_in_Test_ends_with_source_directory()
            => Assert.That(SnapshotSettings.GetBuilder().Build().SnapshotDirectoryPath, Does.EndWith(Path.Combine("SnapTest.NUnit.Tests", "_snapshots")));

        [TestCase]
        public void Default_SnapshotDirectoryPath_in_TestCase_ends_with_source_directory()
            => Assert.That(SnapshotSettings.GetBuilder().Build().SnapshotDirectoryPath, Does.EndWith(Path.Combine("SnapTest.NUnit.Tests", "_snapshots")));

        [TestCaseSource(nameof(SimpleTestCaseSource))]
        public void Default_SnapshotDirectoryPath_in_TestCaseSource_ends_with_source_directory(string _)
            => Assert.That(SnapshotSettings.GetBuilder().Build().SnapshotDirectoryPath, Does.EndWith(Path.Combine("SnapTest.NUnit.Tests", "_snapshots")));

        public static IEnumerable<object> SimpleTestCaseSource() { yield return "a value"; }

        [Theory]
        public void Default_SnapshotDirectoryPath_in_Theory_ends_with_source_directory()
            => Assert.That(SnapshotSettings.GetBuilder().Build().SnapshotDirectoryPath, Does.EndWith(Path.Combine("SnapTest.NUnit.Tests", "_snapshots")));

        [TestCase(null)]
        [TestCase("")]
        [TestCase("tail")]
        public void SnapshotSubdirectory_can_be_set(string snapshotSubdirectory)
        {
            var builder = SnapshotSettings.GetBuilder();
            builder.WithSettings(_ => _.SnapshotSubdirectory = snapshotSubdirectory);
            Assert.That(builder.Build().SnapshotDirectoryPath, Does.EndWith(Path.Combine("SnapTest.NUnit.Tests", snapshotSubdirectory ?? string.Empty)));
        }

        [Test]
        public void Built_SnapshotSettings_SnapshotName_matches_class_dot_Test_method()
            => Assert.That(SnapshotSettings.GetBuilder().Build().SnapshotName, Is.EqualTo($"{nameof(SnapshotSettingsBuilderTest)}.{nameof(Built_SnapshotSettings_SnapshotName_matches_class_dot_Test_method)}"));

        [TestCaseSource(nameof(SimpleTestCaseSource))]
        public void Built_SnapshotSettings_SnapshotName_matches_class_dot_TestCaseSource_method_with_paramvalue(string param)
            // This test varies from the equivalent xUnit.net test: with xUnit.net the SnapshotName does *not* include
            // the parameter value, while with NUnit the parameter value *is* included
            => Assert.That(
                SnapshotSettings.GetBuilder().Build().SnapshotName,
                Is.EqualTo(
                    $"{nameof(SnapshotSettingsBuilderTest)}.{nameof(Built_SnapshotSettings_SnapshotName_matches_class_dot_TestCaseSource_method_with_paramvalue)}(\"{param}\")"));

        [Test]
        public void SnapshotGroupKey_with_DefaultSnapshotGroupKeyFromTestName_set_defaults_to_NUnit_TestName()
        {
            var builder = SnapshotSettings.GetBuilder().WithSettings(_ => _.DefaultSnapshotGroupKeyFromTestName = true);
            Assert.That(builder.Build().SnapshotGroupKey, Is.EqualTo(TestContext.CurrentContext.Test.Name));
        }

        [Test(ExpectedResult = "explicit")]
        public string SnapshotGroupKey_with_DefaultSnapshotGroupKeyFromTestName_set_takes_explicitly_set_value()
            => SnapshotSettings.GetBuilder().WithSettings(_ => {
                _.DefaultSnapshotGroupKeyFromTestName = true;
                _.SnapshotGroupKey = "explicit";
            })
                .Build()
                .SnapshotGroupKey;

        [Test(ExpectedResult = nameof(SnapshotSettingsBuilderTest))]
        public string SnapshotName_with_DefaultSnapshotGroupKeyFromTestName_set_is_fixture_class_name()
            => SnapshotSettings.GetBuilder().WithSettings(_ => _.DefaultSnapshotGroupKeyFromTestName = true)
                .Build()
                .SnapshotName;
        #endregion
    }
}
