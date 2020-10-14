using System;
using System.IO;

namespace SnapTest.Tests
{
    /// <summary>
    /// Helper class that builds a SnapshotSettings that readers and writes snapshot data in a temporary file.
    /// </summary>
    /// <remarks>
    /// TempFileSnapshotSettings objects should be disposed once they are no longer needed to ensure
    /// that temporary files they create get deleted from the filesystem.
    /// </remarks>
    public class TempFileSnapshotSettings : SnapshotSettings, IDisposable
    {
        public readonly string SnapshotFileName = Path.GetTempFileName();

        public TempFileSnapshotSettings()
        {
            if (File.Exists(SnapshotFileName))
                File.Delete(SnapshotFileName);

            SnapshotName = Path.GetFileNameWithoutExtension(SnapshotFileName);
            SnapshotDirectory = Path.GetDirectoryName(SnapshotFileName);
            SnapshotExtension = Path.GetExtension(SnapshotFileName);

            // Set properties to fixed defined values for stable testing
            SnapshotGroup = null;
            IndentJson = true;
            SelectPath = null;
            ExcludedPaths.Clear();
            CreateMissingSnapshots = false;
            ForceSnapshotRefresh = false;

            EnsureSnapshotFilesAreDeleted();
        }

        protected void EnsureSnapshotFilesAreDeleted()
        {
            var f = MismatchedActualFilePath;
            if (File.Exists(f))
                File.Delete(f);

            f = SnapshotFilePath;
            if (File.Exists(f))
                File.Delete(f);
        }

        public void Dispose() => EnsureSnapshotFilesAreDeleted();
    }
}
