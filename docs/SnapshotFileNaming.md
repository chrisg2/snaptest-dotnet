# Snapshot file naming

The paths of snapshot files are determined according to the following properties specified in a `SnapTest.SnapshotSettings` object:
- Snapshot file paths are derived from combining `SnapshotDirectoryPath`, `SnapshotName` and `SnapshotExtension`.
- Mismatched actual file paths are derived from combining `SnapshotDirectoryPath`, `SnapshotName`, `SnapshotGroupKey` and `MismatchedActualExtension`.

The default extensions used are:
Setting|Default
---|---
`SnapshotExtension`|`.txt`
`MismatchedActualExtension`|`.txt.actual`

Defaults for the other path-related settings are generally determined by the SnapTest package for the particular unit testing framework in use.


## Snapshot file naming with NUnit and xUnit.net tests

### Default snapshot file location and naming

Snapshot files are placed (by default) in `<Test source file directory>/_snapshots/<Test class name>.<Test name>.txt`.

### Overriding default details

The components used to construct the snapshot file path can be individually overridden by calling the `WithSettings` method on a `SnapshotSettingsBuilder` (or `SnapshotConstraint` for NUnit tests). For example:

```C#
var builder = SnapshotSettings.GetBuilder().WithSettings(_ => {
    _.SnapshotDirectoryPath = @"C:\MyPath";
    _.SnapshotName = "MySnapshot";
    _.SnapshotExtension = ".snapshot";
    _.MismatchedActualExtension = ".snapshot.actual";
});

// NUnit assertion:
Assert.That("actual output", SnapshotDoes.Match(builder));

// xUnit.net assertion:
SnapshotAssert.Matches("actual output", builder);
```

These settings will use the snapshot file `C:\MyPath\MySnapshot.snapshot`. If a snapshot match fails and a mismatched actual file is created then it will be created at `C:\MyPath\MySnapshot.snapshot.actual`.

To determine the snapshot directory based on the test source file directory but with another subdirectory name instead of `_snapshots`, set the `SnapshotSubdirectory` setting (and do _not_ explicitly set `SnapshotDirectoryPath`):

```C#
var builder = SnapshotSettings.GetBuilder().WithSettings(
    _ => _.SnapshotSubdirectory = ".snapshots"
);
```

> __TIP:__ It is possible that the snapshot filename selected for multiple tests may be the same. This may be desired (for example, when multiple tests are intended to share the same snapshot). However in a situation where it is not desired, consider overriding the default test name to ensure the snapshot file name for each test is unique.

For example (for NUnit):
>
> ```C#
> Assert.That(actualValue, SnapshotDoes.Match("Snapshot_name_that_is_unique"));
> ```

Or for xUnit.net:
>
> ```C#
> SnapshotAssert.Matches(actualValue, "Snapshot__name_that_is_unique");
> ```


### Special characters

Any of the following special characters that would otherwise appear in a snapshot filename are replaced with `_` to avoid filenames which are not possible to have on filesystems used with either Windows or UNIX-like operating systems: `/|:*?\"<>`


### Automatically determining test source file directories

Unless `SnapshotDirectoryPath` is explicitly set in a `SnapshotSettings` object, the snapshot directory is automatically determined based on the directory that contains the source file for the test being executed.

This directory is identified from information in the call stack at the time a `SnaphotSettings` object is built. This will generally work without a problem, but there are a few situations where this directory cannot be automatically determined. For example:
- If the call stack leading to a `SnapshotSettings` object being built does not contain a method that has an attribute applied that identifies it as an NUnit or xUnit.net test method.
- If the test assembly is compiled without debugging information.
- In certain situations where async methods are used.

If this occurs a `SnapTestException` is thrown with a message that explains the situation. The `SnapshotDirectoryPath` can be explicitly set in the test code to help avoid such a failure.
