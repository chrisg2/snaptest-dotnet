# snaptest-dotnet

![.NET Core](https://github.com/chrisg2/snaptest-dotnet/workflows/.NET%20Core/badge.svg)

This is snaptest-dotnet, a tool to enable effective snapshot testing with .NET Core and .NET Framework.

This library has been inspired by:
- [Jest snapshot testing](https://jestjs.io/docs/en/snapshot-testing)
- [Snapshooter](https://github.com/SwissLife-OSS/snapshooter)
- [Snapper](https://theramis.github.io/Snapper/)

Treat this as experimental status. This means (amongst other things) that the interfaces and classes exposed from the SnapTest namespace are subject to change. If you are interested in using this library and the potential for changes is problematic for you then get in touch and we can discuss.

# Points to document

## Default snapshot file naming

### NUnit

By default snapshot files are placed in `<Test source file directory path>/_snapshots/<Test class name>.<Test name>.txt`.

The components used to construct the full snapshot file path can be individually specified as follows:

```C#
var builder = new SnapshotBuilder()
    .WithFileStorageOptions(_ => _.SnapshotDirectory = @"C:\MyPath")
    .WithFileStorageOptions(_ => _.Extension = ".snapshot")
;

Assert.That("actual output", Does.MatchSnapshot("filename", builder));
```

With the above settings the full path of snapshot file used by `Does.MatchSnapshot` will be `C:\MyPath\filename.snapshot`.

To override the directory name `_snapshots` that is appended by default to the source file directory path (that is, when the `SnapshotDirectory` file storage option has not be explicitly set), set the `SnapshotBuilder.SnapshotDirectoryTail` property:
```C#
var builder = new SnapshotBuilder() { SnapshotDirectoryTail = ".snapshots" };
```

Any of the following special characters in the filename are replaced with `_` to avoid using filenames which are not possible to have on filesystems with both Windows and UNIX-like operating systems: `/|:*?\"<>`

TIP: It is possible that the same default snapshot filename selected for multiple tests may be the same. To avoid this, consider explicitly setting the test name. For example:

```C#
Assert.That(actualValue, Does.MatchSnapshot(nameof(MyTestClass) + ".Overridden_test_name_that_is_unique"));
```

# Things to be done

1. Consider renaming Snapshot.CompareTo to Snapshot.ExecutePipeline or similar

1. Add unit tests for SnapshotBuilderBase

1. Add filtering middleware

1. Put strings into resx files

1. Look at other projects to check for other interesting capabilities/ideas

1. Add SnapTest interfaces for Xunit and MSTest

1. Add XML comments for all public interfaces

1. Documentation
