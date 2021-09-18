# Snapshot settings

The behavior of a snapshot match operation is controlled through a `SnapshotSettings` object that may be provided when a match is performed.

## General settings

The following settings are defined in the `SnapTest.SnapshotSettings` class:

Setting|Description|Default
---|---|---
`SnapshotName`|The name of the snapshot. Used as the basename of the snapshot filename.|`null` (*)
`SnapshotGroupKey`|A key used to identify the particular snapshot to use out of a group of snapshots identified by `SnapshotName`. See [Snapshot Groups](SnapshotGroups.md) for more information.|`null` (*)
`CreateMissingSnapshots`|Flag indicating whether missing snapshots should be created based on actual values provided when a snapshot is matched.|`true` if the `SNAPTEST_CREATE_MISSING_SNAPSHOTS` environment variable is not empty; otherwise `false`
`ForceSnapshotRefresh`|Flag indicating whether snapshot files should be forcibly refreshed to reflect actual values provided for snapshot matches.|`true` if the `SNAPTEST_REFRESH` environment variable is not empty; otherwise `false`
`IndentJson`|Flag used to control whether serialized JSON for an actual value saved to a snapshot file when either `ForceSnapshotRefresh` or `CreateMissingSnapshots` are `true` has new lines and indentation (`true`), or has no indentation and appears on a single line (`false`).|`true`
`SnapshotDirectoryPath`|Path to the directory in which snapshot files are stored. If not set, the current working directory is used.|`null` (*)
`SnapshotExtension`|The extension to append as a suffix to snapshot filenames, including a leading ".".|`.txt`
`MismatchedActualExtension`|The extension to append as a suffix to mismatched actual filenames. Any occurrence of the "*" character in this property is replaced with `SnapshotExtension`. When combined with the default value of `SnapshotExtension`, the default actual extension used for mismatched actual files is `=.txt`.|`=*`
`SnapshotComparer`|An object impementing the `ISnapshotEqualityComparer` interface to be used for comparing an actual value to a snapshotted value.|`SnapshotComparer.Default`
`MessageWriter`|An object impementing the `IMessageWriter` interface to be used for emitting informational messages during snapshot processing.|`null` (*)

Defaults for settings marked with (*) may be overridden in snapshot settings classes defined by SnapTest modules for different test frameworks.

Settings related to individual fields identified by JSON Paths can be configured by calling methods on the object returned from `SnapshotSettings.Field("{JSON Path...}")`:

Method|Description
---|---
`Include`|Include elements(s) from the actual compound object that are identified by the field's JSON path expression in the match against the snapshot. See [Filtering Values](Filtering.md) for more information. If this method is not explicitly called for any field then all elements of the object are included in the match.
`Exclude`|Exclude element(s) from the actual compound object that are identified by the field's JSON path expression from the matched against the snapshot. See [Filtering Values](Filtering.md) for more information.

The `SnapTest.SnapshotSettings` base class is intended to be sufficient for use with [naked SnapTest'ing](NakedSnapTest.md). The `SnapTest.SnapshotTestFrameworkSettingsBase` class derived from `SnapTest.SnapshotSettings`, is intended to be used as the base class for a snapshot settings classes defined for use with individual test frameworks, and defines some additional settings. These settings are used when snapshot match operations are performed from the context of tests run by various test frameworks such as NUnit and xUnit.net.

Setting|Description|Default
---|---|---
`SnapshotSubdirectory`|Subdirectory name to store snapshot files in under the directory containing the test source file.<br/><br/>The value of this setting is ignored if `SnapshotDirectoryPath` has otherwise been explicitly set.|`_snapshots`
`DefaultSnapshotGroupKeyFromTestName`|Flag indicating whether to use the test name as the default `SnapshotGroupKey`.<br/><br/>The value of this setting is ignored if `SnapshotGroupKey` has otherwise been set to a non-null value.|`false`

In addition, defaults for following settings from the `SnapTest.SnapshotSettings` are overridden by `SnapTest.SnapshotTestFrameworkSettingsBase`:

Setting|Default
---|---
`SnapshotDirectoryPath`|The directory that contains the source file for the test being executed, with `SnapshotSubdirectory` appended.


## Settings for test frameworks

See the following pages for additional details on how settings are applied with different test frameworks:

- [NUnit](SnapshotSettings.NUnit.md)
- [xUnit.net](SnapshotSettings.Xunit.md)
