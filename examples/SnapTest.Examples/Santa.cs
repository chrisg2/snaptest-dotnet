using SnapTest;
using System;
using System.IO;
using System.Linq;

namespace SnapTest.Examples
{
    public class SantaTests
    {
        /// <summary>
        /// Illustrates naked use of SnapTest.Snapshot.CompareTo without a test framework:<br/>
        /// 1. Create and initialize a SnapshotSettings<br/>
        /// 2. Call Snapshot.CompareTo()
        /// </summary>
        public static bool Santa_lives_at_the_NorthPole()
        {
            var snapshotFile = FabricateSnapshotFile(nameof(Santa_lives_at_the_NorthPole));

            try {
                var santasHomeLocation
                    = CityModel.Cities.AllCities
                        .Where(_ => _.Landmarks.Contains("Santa's Workshop"))
                        .Select(_ => _.Location)
                        .FirstOrDefault();

                var settings = new SnapshotSettings() {
                    SnapshotName = nameof(Santa_lives_at_the_NorthPole),
                    SnapshotDirectoryPath = Path.GetDirectoryName(snapshotFile),
                    MessageWriter = new MessageWriter(),
                    IndentJson = false
                };

                var result = Snapshot.CompareTo(santasHomeLocation, settings);

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
