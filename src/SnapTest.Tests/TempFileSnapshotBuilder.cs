using System;
using System.IO;

namespace SnapTest.Tests
{
    /// <summary>
    /// Helper class that builds a Snapshot that readers and writes snapshot data in a temporary file.
    /// </summary>
    /// <remarks>
    /// Care should be taken to dispose TempFileSnapshotBuilder objects once they are no longer needed to ensure
    /// that temporary files they create get deleted from the filesystem.
    /// </remarks>
    public class TempFileSnapshotBuilder : SnapshotBuilderBase, IDisposable
    {
        public readonly string SnapshotFileName = Path.GetTempFileName();

        public string TempSnapshotDirectory => Path.GetDirectoryName(SnapshotFileName);
        public string TestName => Path.GetFileNameWithoutExtension(SnapshotFileName);
        public string Extension => Path.GetExtension(SnapshotFileName);

        public TempFileSnapshotBuilder()
        {
            EnsureSnapshotFileIsDeleted();

            WithFileStorageOptions(_ => {
                _.SnapshotDirectory = TempSnapshotDirectory;
                _.Extension = Extension;
                _.CreateMissingSnapshots = false;
                _.ForceSnapshotRefresh = false;
            });
        }

        public bool BuildAndCompareTo(object actual, SnapshotContext context = null)
        {
            if (context == null)
                context = new SnapshotContext();

            context.TestName = TestName;
            return Build().CompareTo(actual, context);
        }

        public void EnsureSnapshotFileIsDeleted()
        {
            if (File.Exists(SnapshotFileName))
                File.Delete(SnapshotFileName);
        }

        protected override bool IsTestMethod(System.Reflection.MethodBase method) => false;

        void IDisposable.Dispose() => EnsureSnapshotFileIsDeleted();
    }
}
