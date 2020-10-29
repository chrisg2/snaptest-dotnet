# Creating and updating snapshots

SnapTest is able to automatically save actual values passed to match against snapshots to create snapshots which are missing, or refresh existing snapshots. The following sections describe how this is managed.

> __TIP__: When a snapshot is refreshed or created while perform a snapshot match, the match is deemed to succeed.


## Automatically creating missing snapshots

Any snapshot that does not already exist will be created by a test run while the `SNAPTEST_CREATE_MISSING_SNAPSHOTS` environment variable is set to any non-blank value. Each snapshot will be created with the actual value which is passed in to the snapshot match operation performed during the test run.

For example, if the snapshot for the `Can_use_simple_Assert_constraint` test does not already exist:

```shell
jonas@DTP001:~/src/snaptest-dotnet/examples$ SNAPTEST_CREATE_MISSING_SNAPSHOTS=yes dotnet test --filter Can_use_simple_Assert_constraint
[...]
Created or refreshed snapshot file at /home/jonas/src/snaptest-dotnet/examples/SnapTest.NUnit.Examples/_snapshots/Tests.Can_use_simple_Assert_constraint.txt
===> Tip: Review the content of created and refreshed snapshot files to ensure they reflect expected output.

Test Run Successful.
Total tests: 1
     Passed: 1
 Total time: 0.3542 Seconds

jonas@dtp001:~/src/snaptest-dotnet/examples$ cat SnapTest.NUnit.Examples/_snapshots/Tests.Can_use_simple_Assert_constraint.txt
{
  "Latitude": 90.0,
  "Longitude": 0.0
}
```

An alternate way to create a missing snapshot is to (temporarily) change the test code to set the `CreateMissingSnapshots` setting to `true`, run the test, and then revert the test code.

For example, given the following NUnit test:

```C#
[Test]
public void Can_use_simple_Assert_constraint()
{
    var actual = ...;
    Assert.That(actual, SnapshotDoes.Match());
}
```

This could temporarily be changed as follows to create a missing snapshot:

```C#
[Test]
public void Can_use_simple_Assert_constraint()
{
    var actual = ...;
    Assert.That(actual, SnapshotDoes.Match()
        .WithSettings(_ => _.CreateMissingSnapshots = true));
}
```

An equivalent xUnit.net test may look like:

```C#
[Fact]
public void Can_use_simple_snapshot_match()
{
    var actual = ...;
    SnapshotAssert.Matches(actual, _ => _.CreateMissingSnapshots = true);
}
```


## Automatically refresh all snapshots to reflect actual values

Run tests with the `SNAPTEST_REFRESH` environment variable set to any non-blank value to force all snapshots to be refreshed to reflect the actual value passed in to snapshot matches performed by the test run.

For example:

```shell
jonas@DTP001:~/src/snaptest-dotnet/examples$ SNAPTEST_REFRESH=yes dotnet test
[...]
Created or refreshed snapshot file at /home/jonas/src/snaptest-dotnet/examples/SnapTest.NUnit.Examples/_snapshots/Tests.Can_include_and_exclude_fields.txt
===> Tip: Review the content of created and refreshed snapshot files to ensure they reflect expected output.
Created or refreshed snapshot file at /home/jonas/src/snaptest-dotnet/examples/SnapTest.NUnit.Examples/_snapshots/Tests.Can_use_constraint_expression.txt
[...]

Test Run Successful.
Total tests: 7
     Passed: 7
 Total time: 0.9665 Seconds
 ```

> __TIP:__ Be sure to have an appropriate backup, checked in copy, or way to recover snapshot files before forcing a refresh like this in case of any unfortunate accidents!

An alternate way to force a snapshot to be refreshed is to (temporarily) change the test code to set the `ForceSnapshotRefresh` setting value to `true`, run the test, and then revert the test code.

For example, given the following NUnit test:

```C#
[Test]
public void Can_use_simple_Assert_constraint()
{
    var actual = ...;
    Assert.That(actual, SnapshotDoes.Match());
}
```

This could temporarily be changed as follows to force a refresh of the snapshot:

```C#
[Test]
public void Can_use_simple_Assert_constraint()
{
    var actual = ...;
    Assert.That(actual, SnapshotDoes.Match()
        .WithSettings(_ => _.ForceSnapshotRefresh = true));
}
```

Or with xUnit.net:

```C#
[Fact]
public void Can_use_simple_snapshot_match()
{
    var actual = ...;
    SnapshotAssert.Matches(actual, _ => _.ForceSnapshotRefresh = true);
}
```


## Mismatched actual files

If a snapshot match fails, a "mismatched actual" file is created containing the actual value used in the match operation. This file may be helpful to investigate a failing test, or to manually update the snapshot file if it is out of date.

If the latest actual output now reflects the expected output, the mismatched actual file can simply be moved to overwrite the current snapshot (unless snapshot groups are being used, in which case updates are a little more complicated - see [Mismatched actual files and snapshot groups](#mismatched-actual-files-and-snapshot-groups) below). This may be a useful alternate approach to update a snapshot rather than using the `SNAPTEST_REFRESH` environment variable (or `ForceSnapshotRefresh` setting) described above.

If a snapshot match succeeds, any existing mismatched actual file for the snapshot is deleted. This generally means that mismatched actual files will only remain on the filesystem under one of the following conditions:
1. The last run of the associated test failed, or
1. The snapshot name used by a test has changed during development.

Mismatched actual files have (by default) an extension of `.txt.actual`. It is good practice to configure the source control system to ignore these files. For example, add a line like the following to the top level `.gitignore` file:

```
**/_snapshots/*.txt.actual
```


### Mismatched actual files and snapshot groups

[Snapshot groups](SnapshotGroups.md) are a mechanism to store a group of snapshotted values in a single snapshot file. Each snapshotted value is identified by a unique key, known as the "snapshot group key".

While the snapshotted values are stored in a single group file when using snapshot groups, a separate mismatched actual file is created for each snapshot match operation that fails. The name of the mismatched actual file includes the snapshot group key identifying the snapshot within the group.

For example, consider a situation where 2 snapshot matches are performed with the following settings:
- Both snapshots use a `SnapshotName` of `MyShapshot`
- The snapshots use `SnapshotGroupKey` values of `A` and `B` respectively

The snapshot file `MySnapshot.txt` will contain a JSON representation of the expected values for both groups:

```json
{
    "A": { "expectedA": "some value" },
    "B": { "expectedB": "some other value" }
}
```

If the matches performed against both snapshots fail, the following mismatched actual files will be created showing the actual values provided to the match operations:

1. `MySnapshot.A.txt.actual`:
    ```json
    {
        "A": { "expectedA": "different value" }
    }
    ```

1. `MySnapshot.B.txt.actual`:
    ```json
    {
        "B": { "expectedB": "different other value" }
    }
    ```

If the contents of the mismatched actual files are correct, examples of possible approaches to update the snapshot file include:

1. Edit the `MySnapshot.txt` file and make updates to reflect the correct details based on the details in the `.txt.actual` files; OR

1. Edit the `MySnapshot.txt` file and _delete_ the keys to be updated, then re-run the tests with the `SNAPTEST_CREATE_MISSING_SNAPSHOTS` environment variable set.

1. Re-run the the specific tests with the `SNAPSHOT_REFRESH` environment variable set.

1. Edit the code of the specific tests to (temporarily) set the `SnapshotSettings.ForceSnapshotRefresh` setting to true, run the test, then revert the test code change.
