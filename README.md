# snaptest-dotnet

![.NET Core](https://github.com/chrisg2/snaptest-dotnet/workflows/.NET%20Core/badge.svg)

This is snaptest-dotnet, a tool to enable effective snapshot testing with .NET Core and .NET Framework.

This library has been inspired by:
- [Jest snapshot testing](https://jestjs.io/docs/en/snapshot-testing)
- [Snapshooter](https://github.com/SwissLife-OSS/snapshooter)
- [Snapper](https://theramis.github.io/Snapper/)

Treat this as experimental status. This means (amongst other things) that the interfaces and classes exposed from the SnapTest namespace are subject to change. If you are interested in using this library and the potential for changes is problematic for you then get in touch and we can discuss.


## Getting Started

1. __Add SnapTest to your test project__

    Add the `SnapTest` and `SnapTest.NUnit` Nuget packages to your NUnit-based test project:

    ```shell
    dotnet add package SnapTest
    dotnet add package SnapTest.NUnit
    ```

1. __Add test assertions to compare actual output against snapshotted output__

    `DoesMatch.Snapshot()` can be used as an assertion expression in your test.

    Example:
    ```C#
    // ChristmasTests.cs

    using NUnit.Framework;
    using SnapTest.NUnit;

    public class ChristmasTests
    {
        [Test]
        public void Santa_lives_at_the_NorthPole()
        {
            Assert.That(Santa.HomeCoordinates, DoesMatch.Snapshot());
        }
    }
    ```

1. __Create the initial snapshot__

    Run tests with the `SNAPTEST_CREATE_MISSING_SNAPSHOTS` environment variable set to cause initial snapshot files to be created. Snapshot files are created in the `_snapshots` subdirectory under the directory containing the test source file.

    After creating a snapshot file, review its contents to verify they match what you expect.

    ```shell
    # Bash:
    SNAPTEST_CREATE_MISSING_SNAPSHOTS=yes dotnet test
    cat _snapshots/ChristmasTests.Santa_lives_at_the_NorthPole.txt
    ```

    ```PowerShell
    # PowerShell:
    $env:SNAPTEST_CREATE_MISSING_SNAPSHOTS = "yes"
    dotnet test
    cat _snapshots\ChristmasTests.Santa_lives_at_the_NorthPole.txt
    ```

    The snapshot file contains a JSON representation of the actual result provided to the assertion:
    ```json
    {
      "Latitude": 90.0,
      "Longitude": 0.0
    }
    ```

1. __Commit code and snapshot files to source control__

    Commit the test source file and associated snapshot file to your version control repository. You may also configure your version control tool to ignore `*.txt.actual` files in the `_snapshots` subdirectory (these files will be created when a snapshot comparison fails).

    ```shell
    echo '*.txt.actual' >>_snapshots/.gitignore
    git commit ChristmasTests.cs _snapshots/ChristmasTests.Santa_lives_at_the_NorthPole.txt _snapshots/.gitignore
    ```

1. __When a change occurs that results in different actual result...___

    ... the NUnit test output will include output identifying the change:
    ```
    Created snapshot actual mismatched output file at /home/jonas/src/Christmas.Tests/_snapshots/ChristmasTests.Santa_lives_at_the_NorthPole.txt.actual
    ===> Tip: Review the content of mismatched output files to and use them to update snapshot files as appropriate.
    X Santa_lives_at_the_NorthPole [93ms]
    Error Message:
        Expected string length 33 but was 35. Strings differ at index 29.
    Expected: "{"Latitude":90.0,"Longitude":0.0}"
    But was:  "{"Latitude":90.0,"Longitude":135.0}"
    ----------------------------------------^

    Stack Trace:
        at ChristmasTests.Santa_lives_at_the_NorthPole() in /home/jonas/src/Christmas.Tests/ChristmasTests.cs:line 11
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

    Or simply copy the `.txt.actual` snapshot file over the `.txt` file:
    ```shell
    cp _snapshots/ChristmasTests.Santa_lives_at_the_NorthPole.txt.actual _snapshots/ChristmasTests.Santa_lives_at_the_NorthPole.txt
    ```


# Going Deeper

Learn more about SnapTest in the [documentation](doc).


# Questions and feedback sought

## How should configuration to force snapshots to be updated or missing snapshots created be managed?

Currently the `SNAPTEST_REFRESH` and `SNAPTEST_CREATE_MISSING_SNAPSHOTS` environment variables are set when running tests to automatically create or update snapshot files based on actual values. Alternatively, code can explicitly set the `SnapshotSettings` `ForceSnapshotRefresh` and `CreateMissingSnapshots` properties.

However these options may not suit all kinds of workflows and development practices. What other kinds of approaches for controlling this might be helpful to suit different styles of development practices & workflows?
