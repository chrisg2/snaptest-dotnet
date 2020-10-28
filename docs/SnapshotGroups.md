# Snapshot groups

Snapshot groups are a mechanism to store a group of multiple snapshotted values in a single snapshot file. Each snapshotted value is identified by a unique key, known as the "snapshot group key". This technique can be helpful to reduce the number of separate snapshot files that get created when working with a number of related snapshots, and store related snapshot content in a single file.

When using a snapshot group, the snapshot file contains a JSON object with properties keyed by the snapshot group keys. For example:

```json
{
    "snapshot group key 1": { ... snapshotted value ...},
    "snapshot group key 2": { ... snapshotted value ...}
}
```

Do at least one of the following things to use a snapshot group:

1. Set the `SnapshotGroupKey` setting to a non-empty value.

1. Set the `DefaultSnapshotGroupKeyFromTestName` setting to `true`. This will result in the following defaults being applied when a `SnapshotSettings` instance is built (if these settings are not otherwise explicitly specified):
    * `SnapshotGroupKey` is set to the test name
    * `SnapshotName` is set to the test class (fixture) name.

1. Apply the `UseSnapshotGroup` attribute to the test class or method. This will have the effect of causing the `DefaultSnapshotGroupKeyFromTestName` setting to be true for any snapshot matches performed by the test class/method.

See [Creating and updating snapshots](CreatingAndUpdatingSnapshots.md) for some notes on the implications of using snapshot groups when working with mismatched actual files.


## `UseSnapshotGroup` attribute

Applying the `UseSnapshotGroup` to a test class (fixture) or method is a convenient way to arrange for all snapshot matches performed by the class/method to use a snapshot group.

The `UseSnapshotGroup` attribute has a `SnapshotName` property which can be set to specify the name to be used for snapshot matches performed by the class/method.

Here is an example illustrating how this looks:

```C#
using NUnit.Framework;
using SnapTest.NUnit;
using System.Linq;

namespace SnapTest.NUnit.Examples
{
    [UseSnapshotGroup(SnapshotName = "LocalitiesSnapshotGroup")]
    public class SnapshotGroupTests
    {
        [Test]
        public void Santas_home_location()
        {
            var santasCoords
                = Model.Localities.All
                    .Where(_ => _.Landmarks.Contains("Santa's Workshop"))
                    .FirstOrDefault()
                    .Coordinates;

            Assert.That(santasCoords, SnapshotDoes.Match());
        }

        [Test]
        public void Sydney_location()
        {
            var sydneyCoords
                = Model.Localities.All
                    .Where(_ => _.Name == "Sydney")
                    .FirstOrDefault()
                    .Coordinates;

            Assert.That(sydneyCoords, SnapshotDoes.Match());
        }
    }
}
```

The associated `_snapshots/LocalitiesSnapshotGroup.txt` file contains:

```json
{
  "Santas_home_location": {
    "Latitude": 90.0,
    "Longitude": 0.0
  },
  "Sydney_location": {
    "Latitude": -33.8688,
    "Longitude": 151.2093
  }
}
```


## A note on JSON indentation and snapshot groups

When matching snapshots using snapshot groups, ensure a consistent value is used for the `IndentJson` setting applied to all snapshots within a group. If different values for this setting are used by different snapshot match operations then the formatting of the snapshot file may change when a snapshot operation refreshes the snapshot file. It is suggested to always leave the `IndentJson` set to its default value of `true` when working with snapshot groups.
