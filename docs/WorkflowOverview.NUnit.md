# Workflow for writing and running snapshot-based tests - NUnit

Snapshot-based testing is a technique where expected output from running tests is stored in snapshot files. When tests are run, the actual output produced is matched against the expected output that has been previously saved.

Here is an overview of a typical workflow for developing and running snapshot-based tests using SnapTest. This example assumes you are using [NUnit](https://nunit.org/) - see [here](WorkflowOverview.Xunit.md) for a similar workflow based on xUnit.net tests.

<!-- This content is duplicated in the root README.md. Try to keep the two renditions in sync! -->

1. __Add SnapTest to your test project__

    Add the `SnapTest.NUnit` Nuget package to your NUnit-based test project:

    ```shell
    dotnet add package SnapTest.NUnit
    ```

1. __Add test assertions to match actual output against snapshotted output__

    `DoesMatch.Snapshot()` can be used as an assertion expression in your test.

    Example:
    ```C#
    // SantaTests.cs

    using NUnit.Framework;
    using SnapTest.NUnit;
    using System.Linq;

    public class SantaTests
    {
        [Test]
        public void Santa_lives_at_the_NorthPole()
        {
            var santasHomeLocation = Model.Localities.All.Where(_ => _.Landmarks.Contains("Santa's Workshop")).Select(_ => _.Coordinates).FirstOrDefault();
            Assert.That(santasHomeLocation, SnapshotDoes.Match());
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

    ... the NUnit test output will indicate the change:
    ```
    Created mismatched actual file at /home/jonas/src/Santa.Tests/_snapshots/SantaTests.Santa_lives_at_the_NorthPole=.txt
    ===> Tip: Review the content of mismatched actual files and use them to update snapshot files as appropriate.
    X Santa_lives_at_the_NorthPole [93ms]
    Error Message:
        Expected string length 33 but was 35. Strings differ at index 29.
    Expected: "{"Latitude":90.0,"Longitude":0.0}"
    But was:  "{"Latitude":90.0,"Longitude":135.0}"
    ----------------------------------------^

    Stack Trace:
        at SantaTests.Santa_lives_at_the_NorthPole() in /home/jonas/src/Santa.Tests/SantaTests.cs:line 13
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
