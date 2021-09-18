# Workflow for writing and running snapshot-based tests - xUnit.net

Snapshot-based testing is a technique where expected output from running tests is stored in snapshot files. When tests are run, the actual output produced is matched against the expected output that has been previously saved.

Here is an overview of a typical workflow for developing and running snapshot-based tests using SnapTest. This example assumes you are using [xUnit.net](https://xunit.net/) - see [here](WorkflowOverview.NUnit.md) for a similar workflow based on NUnit tests.

1. __Add SnapTest to your test project__

    Add the `SnapTest.Xunit` Nuget package to your xUnit.net-based test project:

    ```shell
    dotnet add package SnapTest.Xunit
    ```

1. __Add test assertions to match actual output against snapshotted output__

    `SnapshotAssert.Matches()` (or the fluent `ShouldMatchSnapshot()`) can be used as an assertion expression in your test.

    Example:
    ```C#
    // SantaTests.cs

    using SnapTest.Xunit;
    using System.Linq;
    using Xunit;

    public class SantaTests
    {
        [Fact]
        public void Santa_lives_at_the_NorthPole()
        {
            var santasHomeLocation = Model.Localities.All.Where(_ => _.Landmarks.Contains("Santa's Workshop")).Select(_ => _.Coordinates).FirstOrDefault();

            SnapshotAssert.Matches(santasHomeLocation);

            // Or: santasHomeLocation.ShouldMatchSnapshot();
        }
    }
    ```

1. __Create the initial snapshot__

    Run tests with the `SNAPTEST_CREATE_MISSING_SNAPSHOTS` environment variable set to cause initial snapshot files to be created. Snapshot files are created in the `_snapshots` subdirectory under the directory containing the test source file.

    After creating a snapshot file, review its contents to verify they match what you expect.

    ```shell
    # Bash:
    SNAPTEST_CREATE_MISSING_SNAPSHOTS=yes dotnet test
    cat _snapshots/SantaTests.Santa_lives_at_the_NorthPole.txt
    ```

    ```PowerShell
    # PowerShell:
    $env:SNAPTEST_CREATE_MISSING_SNAPSHOTS = "yes"
    dotnet test
    cat _snapshots\SantaTests.Santa_lives_at_the_NorthPole.txt
    ```

    The snapshot file contains a JSON representation of the actual result provided to the assertion:
    ```json
    {
      "Latitude": 90.0,
      "Longitude": 0.0
    }
    ```

1. __Commit code and snapshot files to source control__

    Commit the test source file and associated snapshot file to your version control repository.

    It is also good practice to configure your version control tool to ignore `*=.txt` files in the `_snapshots` subdirectory (these files will be created when a snapshot match fails).

    ```shell
    echo '*=.txt' >>_snapshots/.gitignore
    git commit SantaTests.cs _snapshots/SantaTests.Santa_lives_at_the_NorthPole.txt _snapshots/.gitignore
    ```

1. __When a change occurs that results in different actual result...___

    ... the xUnit.net test output will indicate the change:
    ```
    $ dotnet test
    [...]
    Created mismatched actual file at /home/jonas/src/Santa.Tests/_snapshots/SantaTests.Santa_lives_at_the_NorthPole=.txt
    ===> Tip: Review the content of mismatched actual files and use them to update snapshot files as appropriate.
    [xUnit.net 00:00:00.71]     SantaTests.Santa_lives_at_the_NorthPole [FAIL]
    X SantaTests.Santa_lives_at_the_NorthPole [18ms]
    Error Message:
    Assert.Equal() Failure
                                            ↓ (pos 40)
    Expected: "{"Latitude":90.0,"Longitude":0.0}"
    Actual:   "{"Latitude":90.0,"Longitude":135.0}"
                                            ↑ (pos 40)
    Stack Trace:
        at SnapTest.Xunit.XunitSnapshotEqualityComparer.Equals(SnapshotValue actualValue, SnapshotValue snapshottedValue, SnapshotSettings settings)
    at SnapTest.Snapshot.MatchTo(Object actual, SnapshotSettings settings)
    at SnapTest.Xunit.SnapshotAssert.Matches(Object actual, String snapshotName, SnapshotSettingsBuilder`1 settingsBuilder)
    at SantaTests.Santa_lives_at_the_NorthPole() in /home/jonas/src/Santa.Tests/SantaTests.cs:line 12
    ```

    If the change is acceptable, update the snapshot file with the new actual value:
    ```shell
    # Bash:
    SNAPTEST_REFRESH=yes dotnet test --filter Santa_lives_at_the_NorthPole
    ```
    ```shell
    # PowerShell:
    $env:SNAPTEST_REFRESH = "yes"
    dotnet test --filter Santa_lives_at_the_NorthPole
    ```

    Or simply copy the `=.txt` snapshot file over the `.txt` file:
    ```shell
    cp _snapshots/SantaTests.Santa_lives_at_the_NorthPole=.txt _snapshots/SantaTests.Santa_lives_at_the_NorthPole.txt
    ```
