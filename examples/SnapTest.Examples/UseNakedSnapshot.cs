using SnapTest;
using System;
using System.IO;
using System.Linq;

namespace SnapTest.Examples
{
    public class NakedSnapshotExample
    {
        /// <summary>
        /// Illustrates naked use of SnapTest.Snapshot.MatchTo without a test framework:
        /// <list type="number">
        /// <item>Create and initialize a SnapshotSettings</item>
        /// <item>Call Snapshot.MatchTo()</item>
        /// </list>
        /// </summary>
        public static bool UseNakedSnapshot()
        {
            var snapshotName = nameof(UseNakedSnapshot);
            var snapshotFile = FabricateSnapshotFile(snapshotName);

            try {
                var santasHomeLocation
                    = Model.Localities.All
                        .Where(_ => _.Landmarks.Contains("Santa's Workshop"))
                        .Select(_ => _.Coordinates)
                        .FirstOrDefault();

                var settings = new SnapshotSettings() {
                    SnapshotName = snapshotName,
                    SnapshotDirectoryPath = Path.GetDirectoryName(snapshotFile),
                    MessageWriter = new MessageWriter(),
                    IndentJson = false
                };

                var result = Snapshot.MatchTo(santasHomeLocation, settings);

                if (result)
                    Console.WriteLine("Santa's home location is correct");
                else
                    Console.WriteLine("Santa's home location is not looking good");

                return result;
            }
            finally {
                if (!string.IsNullOrEmpty(snapshotFile))
                    File.Delete(snapshotFile);
            }
        }

        private static string FabricateSnapshotFile(string snapshotName)
        {
            var expectedSnapshot = "{\"Latitude\":90.0,\"Longitude\":0.0}";

            var snapshotFile = Path.Combine(Path.GetTempPath(), $"{snapshotName}.txt");
            Console.WriteLine($"Using snapshotFile {snapshotFile}");
            File.WriteAllText(snapshotFile, expectedSnapshot);

            return snapshotFile;
        }

        private class MessageWriter: IMessageWriter
        {
            public void Write(string message) => Console.WriteLine(message);
        }
    }
}
