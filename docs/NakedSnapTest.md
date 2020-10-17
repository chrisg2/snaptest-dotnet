# Naked SnapTest'ing with no testing framework

While it is typical to use SnapTest with a testing framework such as NUnit, SnapTest can quite happily be used in a naked form without any such framework.

The primary entry point to compare an actual value with a snapshotted value is the `bool SnapTest.Snapshot.CompareTo(object actual, SnapshotSettings settings)` static method.

Key steps to use this method are:

1. Create and set properties on a `SnapshotSettings` object. The following properties should be minimally set:
    - `SnapshotName`
    - `SnapshotDirectory`

    Other properties can be set too, but their default values will typically work just fine for common scenarios.

1. Ensure the snapshotted value is stored in the file `{SnapshotDirectory}/{SnapshotName}{SnapshotExtension}` as specified in the settings. (The default `SnapshotExtension` is `.txt`.)

1. Call `Snapshot.Compare(actual, settings)` to compare the `actual` value to the snapshotted value identified according to the details in `settings`.

Here is an example (from https://github.com/chrisg2/snaptest-dotnet/tree/main/examples/SnapTest.Examples/Santa.cs):

```C#
public static bool Santa_lives_at_the_NorthPole()
{
    // Somewhat contrived, but for the sake of example we will
    // create a snapshot file to be compared.
    var snapshotFile = FabricateSnapshotFile(nameof(Santa_lives_at_the_NorthPole));

    try {
        // Obtain a value to be compared to the snapshot
        var santasHomeLocation
            = CityModel.Cities.AllCities
                .Where(_ => _.Landmarks.Contains("Santa's Workshop"))
                .Select(_ => _.Location)
                .FirstOrDefault();

        // Configure settings for the snapshot
        var settings = new SnapshotSettings() {
            SnapshotName = nameof(Santa_lives_at_the_NorthPole),
            SnapshotDirectoryPath = Path.GetDirectoryName(snapshotFile),
            MessageWriter = new MessageWriter(),
            IndentJson = false
        };

        // Compare the actual value to the snapshotted value
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

// Implementation of the IMessageWriter interface that sends SnapTest messages
// to Console standard output.
private class MessageWriter: IMessageWriter
{
    public void Write(string message) => Console.WriteLine(message);
}
```
