# Default snapshot file naming

## NUnit

By default snapshot files are placed in `<Test source file directory path>/_snapshots/<Test class name>.<Test name>.txt`.

The components used to construct the full snapshot file path can be individually specified as follows:

```C#
var builder = new SnapshotSettingsBuilder()
    .WithSettings(_ => {
        _.SnapshotDirectoryPath = @"C:\MyPath";
        _.SnapshotExtension = ".snapshot";
        _.MismatchedActualExtension = ".snapshot.actual"
        _.SnapshotName = "filename"
    });

Assert.That("actual output", SnapshotDoes.Match(builder));
```

With the above settings the full path of snapshot file used by `SnapshotDoes.Match` will be `C:\MyPath\filename.snapshot`. If a snapshot comparison fails and a mismatch file is created then it will be created at `C:\MyPath.filename.snapshot.actual`.

To override the directory name `_snapshots` that is appended by default to the source file directory path (that is, when the `SnapshotDirectoryPath` setting has not be explicitly set), set the `SnapshotSettings.SnapshotSubdirectory` property:
```C#
var builder = new SnapshotSettingsBuilder().WithSettings(_ => _.SnapshotSubdirectory = ".snapshots");
```

Any of the following special characters in the filename are replaced with `_` to avoid using filenames which are not possible to have on filesystems with both Windows and UNIX-like operating systems: `/|:*?\"<>`

> __TIP:__ It is possible that the same default snapshot filename selected for multiple tests may be the same. This may be desired (for example, when multiple tests are intended to share the same snapshot). However in a situation where it is not desired, consider explicitly setting the test name to ensure the snapshot file name for each test is unique. For example:
>
> ```C#
> Assert.That(actualValue, SnapshotDoes.Match(nameof(MyTestClass) + ".Overridden_test_name_that_is_unique"));
> ```
