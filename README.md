# snaptest-dotnet

![.NET Core](https://github.com/chrisg2/snaptest-dotnet/workflows/.NET%20Core/badge.svg)

This is snaptest-dotnet, a tool to enable effective snapshot testing with .NET Core and .NET Framework.

This library has been inspired by:
- [Jest snapshot testing](https://jestjs.io/docs/en/snapshot-testing)
- [Snapshooter](https://github.com/SwissLife-OSS/snapshooter)
- [Snapper](https://theramis.github.io/Snapper/)

Treat this as experimental status. This means (amongst other things) that the interfaces and classes exposed from the SnapTest namespace are subject to change. If you are interested in using this library and the potential for changes is problematic for you then get in touch and we can discuss.

# Points to document

## Snapshot file contents

- Strings saved verbatim with an extra `Environment.NewLine` at the end

- Guids are stored as text in the pattern `XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX` where X is a hex digit (0,1,2,3,4,5,6,7,8,9,A,B,C,D,E,F), with an `Enviornment.NewLine` at the end

- Other objects are serialized in JSON format, with properties ordered by name


## Default snapshot file naming

### NUnit

By default snapshot files are placed in `<Test source file directory path>/_snapshots/<Test class name>.<Test name>.txt`.

The components used to construct the full snapshot file path can be individually specified as follows:

```C#
var builder = new SnapshotSettingsBuilder()
    .WithSettings(_ => {
        _.SnapshotDirectory = @"C:\MyPath";
        _.SnapshotExtension = ".snapshot";
        _.MismatchedActualExtension = ".snapshot.actual"
        _.TestName = "filename"
    });

Assert.That("actual output", SnapshotDoes.Match(builder));
```

With the above settings the full path of snapshot file used by `SnapshotDoes.Match` will be `C:\MyPath\filename.snapshot`. If a snapshot comparison fails and a mismatch file is created then it will be created at `C:\MyPath.filename.snapshot.actual`.

To override the directory name `_snapshots` that is appended by default to the source file directory path (that is, when the `SnapshotDirectory` setting has not be explicitly set), set the `SnapshotSettings.SnapshotDirectoryTail` property:
```C#
var builder = new SnapshotSettingsBuilder().WithSettings(_ => _.SnapshotDirectoryTail = ".snapshots");
```

Any of the following special characters in the filename are replaced with `_` to avoid using filenames which are not possible to have on filesystems with both Windows and UNIX-like operating systems: `/|:*?\"<>`

TIP: It is possible that the same default snapshot filename selected for multiple tests may be the same. This may be desired (for example, when multiple tests are intended to share the same snapshot). However in a situation where it is not desired, consider explicitly setting the test name to ensure the snapshot file name for each test is unique. For example:

```C#
Assert.That(actualValue, SnapshotDoes.Match(nameof(MyTestClass) + ".Overridden_test_name_that_is_unique"));
```


# Things to be done

1. Consider renaming Snapshot.CompareTo to Snapshot.ExecutePipeline or similar

1. Add filtering middleware

1. Put strings into resx files

1. Look at other projects to check for other interesting capabilities/ideas

1. Add SnapTest interfaces for Xunit and MSTest

1. Add XML comments for all public interfaces

1. Documentation
