﻿using System;
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
    public class TempFileSnapshotBuilder : SnapshotBuilder, IDisposable
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

        public bool BuildAndCompareTo(object actual)
            => Build().CompareTo(actual, new SnapshotContext() { TestName = TestName });

        public void EnsureSnapshotFileIsDeleted()
        {
            if (File.Exists(SnapshotFileName))
                File.Delete(SnapshotFileName);
        }

        void IDisposable.Dispose()
            => EnsureSnapshotFileIsDeleted();
    }
}
