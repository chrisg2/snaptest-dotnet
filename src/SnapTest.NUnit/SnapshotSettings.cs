namespace SnapTest.NUnit
{
    public class SnapshotSettings: SnapTest.SnapshotSettings
    {
        public string SnapshotDirectoryTail { get; set; } = "_snapshots";
        public bool DefaultSnapshotGroupFromNUnitTestName { get; set; } = false;
    }
}
