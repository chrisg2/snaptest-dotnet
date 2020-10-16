# Snapshot settings

The behavior of snapshot comparisons is controlled through a `SnapshotSettings` object that may be provided when a comparison is performed.

## General settings

The following settings are defined in the `SnapTest.SnapshotSettings` class:

Setting|Description|Default
---|---|---
`SnapshotName`|The name of the snapshot. Used as the basename of the snapshot filename.|`null` (*)
`SnapshotGroup`|A key used to identify the particular snapshot to use out of a group of snapshots identified by `SnapshotName`. See [Snapshot Groups](SnapshotGroups.md) for more information.|`null` (*)
`IncludedPaths`|A list of JSON Paths identifying element(s) to be included from a compound object when it is compared to a snapshot. See [Filtering Values](Filtering.md) for more information.|None
`ExcludedPaths`|A list of JSON Paths identifying element(s) to be excluded from a compound object when it is compared to a snapshot. See [Filtering Values](Filtering.md) for more information.|None
`CreateMissingSnapshots`|Flag indicating whether missing snapshots should be created based on actual values provided when a snapshot is compared.|`true` if the `SNAPTEST_CREATE_MISSING_SNAPSHOTS` environment variable is not empty; `false` otherwise
`ForceSnapshotRefresh`|Flag indicating whether snapshot files should be forcibly refreshed to reflect actual values provided for snapshot comparisons.|`true` if the `SNAPTEST_REFRESH` environment variable is not empty; `false` otherwise
`IndentJson`|Flag used to control whether serialized JSON for an actual value saved to a snapshot file when either `ForceSnapshotRefresh` or `CreateMissingSnapshots` are `true` has new lines and indentation (`true`, which is the default), or has no indentation and appears on a single line (`false`).|`true`
`SnapshotDirectoryPath`|Path to the directory in which snapshot files are stored. If not set, the current working directory is used.|`null` (*)
`SnapshotExtension`|The extension to append as a suffix to snapshot filenames, including a ".".|`.txt`
`MismatchedActualExtension`|The extension to append as a suffix to mismatched actual snapshot filenames, including a ".".|`.txt.actual`
`SnapshotComparer`|An object impementing the `ISnapshotComparer` interface to be used for comparing an actual value to a snapshotted value.|`SnapshotComparer.Default`
`MessageWriter`|An object impementing the `IMessageWriter` interface to be used for emitting informational messages during snapshot processing.|`null` (*)

Defaults for some settings marked with (*) may be overridden in snapshot settings classes defined by SnapTest modules for different test frameworks.

## NUnit settings

The `SnapTest.NUnit.SnapshotSettings` class is derived from `SnapTest.SnapshotSettings` (and so inherits all settings from that base class), and adds the following additional settings for comparing snapshots in NUnit tests:

Setting|Description|Default
---|---|---
`SnapshotSubdirectory`|Subdirectory name under the directory containing the NUnit test source file to store snapshot files in.<br/><br/>This value is appended to the `SnapshotDirectoryPath` when `SnapshotSettingsBuilder.Build` determines the default snapshot directory path to use. If `SnapshotDirectoryPath` has otherwise been explicitly set then this setting is ignored.|`_snapshots`
`DefaultSnapshotGroupFromNUnitTestName`|Flag indicating whether to use the NUnit test name (taken from the NUnit Framework `TestContext.CurrentContext.Test.TestName` value) as the default `SnapshotGroup`.<br/><br/>If `SnapshotGroup` has otherwise been set to a non-null value then the value of this setting is ignored.|`false`

In addition, defaults for following settings from the `SnapTest.SnapshotSettings` are overridden by `SnapTest.NUnit.SnapshotSettings`:

Setting|Default
---|---
`SnapshotName`|Set based on the NUnit `TestContext.Current.Test.ClassName` and `Name` properties.<br/><br/><list><li>If `DefaultSnapshotGroupFromNUnitTestName` is `false` (the default), `SnapshotName` defaults to _ClassName_._TestName_, where _ClassName_ is the text after the last "." of `Test.ClassName`, and _TestName_ is `Test.Name`.</li><li>Otherwise the `SnapshotName` defaults to _ClassName_.</li></list>
`SnapshotGroup`|If `DefaultSnapshotGroupFromNUnitTestName` is `false` (the default), `SnapshotGroup` is not set by default.<br/><br/>Otherwise it defaults to the NUnit `TestContext.CurrentContext.Test.Name` property value.
`SnapshotDirectoryPath`|The directory that contains the source file for the test being executed, with `SnapshotSubdirectory` appended.
`SnapshotComparer`|An instance of an internal class implementating the `ISnapshotComparer` interface that generates an NUnit `ConstraintResult` recording the result of the comparison.
`MessageWriter`|An instance of an internal class that uses the NUnit `TestContext.Progress.WriteLine` method to emit informational messages.


### Building SnapTest.NUnit.SnapshotSettings instances

Instances of the `SnapTest.NUnit.SnapshotSettings` class should be created using the `SnapshotBuilder` class.

This is commonly done for you when you call `SnapshotDoes.Match()`, but you can also create a `SnapshotBuilder` yourself and pass it as a parameter to `SnapshotDoes.Match()`.

`SnapshotBuilder.WithSettings(settings => { ... })` can be called to register actions to be invoked when the builder builds a `SnapshotSettings` object. The action takes the settings object that is being built as its single argument. Actions are invoked in the order of the calls that were made to `WithSettings`.

For example:

```C#
var cities = CityModel.Cities.AllCities.OrderBy(_ => _.Name);

var builder = new SnapshotSettingsBuilder();
builder.WithSettings(_ => _.IncludedPaths.Add("$..['Name','Location']"));
builder.WithSettings(_ => _.DefaultSnapshotGroupFromNUnitTestName = true);

Assert.That(cities, SnapshotDoes.Match(builder));
```
