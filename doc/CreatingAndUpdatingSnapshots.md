# Creating and updating snapshots

SnapTest is able to automatically create snapshots which are missing, or refresh existing snapshots based on the actual results. The following sections describe how this is managed.

> __TIP__: If a snapshot is refreshed or created when running a test, the snapshot comparison is deemed to pass.


## Automatically creating missing snapshots

Run tests with the `SNAPTEST_CREATE_MISSING_SNAPSHOTS` environment variable set to any non-blank value to automatically create any snapshots that are missing. Snapshots will be created with the actual values which are passed in to the snapshot comparison operations that are performed by the test run.

For example:

```shell
jonas@DTP001:~/src/snaptest-dotnet/examples$ SNAPTEST_CREATE_MISSING_SNAPSHOTS=yes dotnet test --filter Santa_lives_at_the_NorthPole
[...]
Created or refreshed snapshot file at /home/jonas/src/snaptest-dotnet/examples/SnapTest.NUnit.Examples/_snapshots/SantaTests.Santa_lives_at_the_NorthPole.txt
===> Tip: Review the content of created and refreshed snapshot files to ensure they reflect expected output.

Test Run Successful.
Total tests: 1
     Passed: 1
 Total time: 0.3542 Seconds

jonas@dtp001:~/src/snaptest-dotnet/examples$ cat SnapTest.NUnit.Examples/_snapshots/SantaTests.Santa_lives_at_the_NorthPole.txt
{
  "Latitude": 90.0,
  "Longitude": 0.0
}
```


## Automatically refresh all snapshots to reflect actual values

Run tests with the `SNAPTEST_REFRESH` environment variable set to any non-blank value to force all snapshots to be refreshed to reflect the actual value passed in to snapshot comparisons performed by the test run.

For example:

```shell
jonas@DTP001:~/src/snaptest-dotnet/examples$ SNAPTEST_REFRESH=yes dotnet test
[...]
Created or refreshed snapshot file at /home/jonas/src/snaptest-dotnet/examples/SnapTest.NUnit.Examples/_snapshots/IncludeExcludeTests.Sydney_time_zone_is_correct.txt
===> Tip: Review the content of created and refreshed snapshot files to ensure they reflect expected output.
Created or refreshed snapshot file at /home/jonas/src/snaptest-dotnet/examples/SnapTest.NUnit.Examples/_snapshots/SantaTests.Santa_has_no_time_zone.txt
[...]

Test Run Successful.
Total tests: 7
     Passed: 7
 Total time: 0.9665 Seconds
 ```

> __TIP:__ Be sure to have an appropriate backup, checked in copy, or way to recover snapshot files before forcing a refresh like this in case of any unfortunate accidents!


## Mismatched actual files

If a snapshot comparison fails, a "mismatched actual" file is created containing the actual value used in the comparison. This output may be helpful to investigate a failing test, or to manually update the snapshot if it is out of date.

If a snapshot comparison succeeds, any existing mismatched actual file for the snapshot is deleted. This generally means that mismatched actual files will only remain on the filesystem when the last run of a test failed, and/or when a test/snapshot name has changed during development.

Mismatched actual files typically have an extension of `.txt.actual`. The source control system should be configured to ignore these files. For example, add a line like the following to the top level `.gitignore` file:

```
**/_snapshots/*.txt.actual
```

### Mismatched actual files and snapshot groups

With [snapshot groups](SnapshotGroups.md) it is possible to store a group of snapshotted values in a single snapshot file. Each snapshotted value is identified by a unique key.

While the snapshotted values are stored in a single group file when using snapshot groups, a separate mismatched actual file is created for each snapshot comparison that fails. The mismatched actual file names include the unique key identifying the snapshot within the group.

For example, consider a situation where 2 snapshot matches are performed with the following settings:
- Both snapshots use a `SnapshotName` of `MyShapshot`
- The snapshots use `SnapshotGroup` values of `A` and `B` respectively

The snapshot file `MySnapshot.txt` will contain a JSON representation of the expected values:

```json
{
    "A": { "expectedA": "some value" },
    "B": { "expectedB": "some other value" }
}
```

If the comparisons performed by both snapshots fail, the following mismatched actual files will be created:

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

If the mismatched actual outputs are correct, possible steps to appropriately update the snapshot file include:

1. Edit the `MySnapshot.txt` file and make updates to reflect the correct details based on the details in the `.txt.actual` files; OR

1. Edit the `MySnapshot.txt` file and _delete_ the keys to be updated, then re-run the tests with the `SNAPTEST_CREATE_MISSING_SNAPSHOTS` environment variable set.
