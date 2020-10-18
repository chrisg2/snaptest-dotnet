using System;
using Xunit;

namespace SnapTest.Xunit.Tests
{
    public class SnapTestXunitTestBase
    {
        protected static readonly object Value1 = new { item1 = 42, item2 = new string[]{ "elem1", "elem2" }};
        protected static readonly string Value1SnapshotName = "Value1Snapshot";

        protected static readonly object Value2 = "simple string value {\n}";
        protected static readonly string Value2SnapshotName = "Value2Snapshot";

        protected SnapshotSettingsBuilder<SnapshotSettings> GetBuilderUsingSnapshotGroup()
            => SnapshotSettings.GetBuilder().WithSettings(_ => {
                _.DefaultSnapshotGroupKeyFromTestName = true;
            });
    }
}

