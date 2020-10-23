using System.Collections.Generic;
using System.IO;
using Xunit;

namespace SnapTest.Xunit.Tests
{
    public class SnapshotSettingsBuilderTest
    {
        #region Tests verifying behavior of SnapshotSettingsBuilder.Build().SnapshotDirectoryPath
        [Fact]
        public void Default_SnapshotDirectoryPath_in_Fact_ends_with_source_directory()
            => Assert.EndsWith(Path.Combine("SnapTest.Xunit.Tests", "_snapshots"), SnapshotSettings.GetBuilder().Build().SnapshotDirectoryPath);

        [Theory]
		[InlineData(1)]
		[InlineData(2)]
        public void Default_SnapshotDirectoryPath_in_Theory_ends_with_source_directory(int arg)
		{
            // Avoid warning xUnit1026: Theory method 'Snapshot_works_in_Theory_test' on test class 'TestAttributes' does not use parameter 'arg'
            arg = arg * -1;

            Assert.EndsWith(Path.Combine("SnapTest.Xunit.Tests", "_snapshots"), SnapshotSettings.GetBuilder().Build().SnapshotDirectoryPath);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("tail")]
        public void SnapshotSubdirectory_can_be_set(string snapshotSubdirectory)
        {
            var builder = SnapshotSettings.GetBuilder();
            builder.WithSettings(_ => _.SnapshotSubdirectory = snapshotSubdirectory);
            Assert.EndsWith(Path.Combine("SnapTest.Xunit.Tests", snapshotSubdirectory ?? string.Empty), builder.Build().SnapshotDirectoryPath);
        }

        [Fact]
        public void Built_SnapshotSettings_SnapshotName_matches_class_dot_Fact_method()
            => Assert.Equal($"{nameof(SnapshotSettingsBuilderTest)}.{nameof(Built_SnapshotSettings_SnapshotName_matches_class_dot_Fact_method)}", SnapshotSettings.GetBuilder().Build().SnapshotName);

        [Theory]
        [MemberData(nameof(SimpleTestCaseSource))]
        public void Built_SnapshotSettings_SnapshotName_matches_class_dot_Theory_method(string param)
        {
            param = param + string.Empty; // Modify param to avoid warning xUnit1026: Theory method 'Built_SnapshotSettings_SnapshotName_matches_class_dot_Theory_method' on test class 'SnapshotSettingsBuilderTest' does not use parameter 'param'.

            // This test varies from the equivalent NUnit test: with xUnit.net the SnapshotName does *not* include
            // the parameter value, while with NUnit the parameter value *is* included
            Assert.Equal(
                $"{nameof(SnapshotSettingsBuilderTest)}.{nameof(Built_SnapshotSettings_SnapshotName_matches_class_dot_Theory_method)}",
                SnapshotSettings.GetBuilder().Build().SnapshotName);
        }

        public static IEnumerable<object[]> SimpleTestCaseSource() { yield return new object[]{ "a value" }; }

        [Fact]
        public void SnapshotGroupKey_with_DefaultSnapshotGroupKeyFromTestName_set_defaults_to_Fact_method_name()
        {
            var builder = SnapshotSettings.GetBuilder().WithSettings(_ => _.DefaultSnapshotGroupKeyFromTestName = true);
            Assert.Equal(nameof(SnapshotGroupKey_with_DefaultSnapshotGroupKeyFromTestName_set_defaults_to_Fact_method_name), builder.Build().SnapshotGroupKey);
        }

        [Fact]
        public void SnapshotGroupKey_with_DefaultSnapshotGroupKeyFromTestName_set_takes_explicitly_set_value()
        {
            var builder = SnapshotSettings.GetBuilder().WithSettings(_ => {
                _.DefaultSnapshotGroupKeyFromTestName = true;
                _.SnapshotGroupKey = "explicit";
            });

            Assert.Equal("explicit", builder.Build().SnapshotGroupKey);
        }

        [Fact]
        public void SnapshotName_with_DefaultSnapshotGroupKeyFromTestName_set_is_test_class_name()
        {
            var builder = SnapshotSettings.GetBuilder().WithSettings(_ => _.DefaultSnapshotGroupKeyFromTestName = true);
            Assert.Equal(nameof(SnapshotSettingsBuilderTest), builder.Build().SnapshotName);
        }
        #endregion
    }
}
