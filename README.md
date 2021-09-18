# snaptest-dotnet

![Build and Test](https://github.com/chrisg2/snaptest-dotnet/workflows/Build%20and%20Test/badge.svg) ![Test Examples](https://github.com/chrisg2/snaptest-dotnet/workflows/Test%20Examples/badge.svg)

This is snaptest-dotnet, a tool to enable effective snapshot testing with .NET Core and .NET Framework. Packages for working with either [NUnit](https://nunit.org) or [xUnit.net](https://xunit.net) test frameworks are available.

This library has taken inspiration from:
- [Jest snapshot testing](https://jestjs.io/docs/en/snapshot-testing)
- [Snapshooter](https://github.com/SwissLife-OSS/snapshooter)
- [Snapper](https://theramis.github.io/Snapper/)

Treat this as experimental status. This means (amongst other things) that the interfaces and classes exposed from the SnapTest namespace are subject to change. If you are interested in using this library and the potential for changes is problematic for you then get in touch and we can discuss.

## What is snapshot testing?

Snapshot testing can help to simplify recording of expected data produced by tests, especially when the expected data is somewhat complex. Consider the following NUnit test:

```C#
[Test]
public void Each_field_can_be_asserted()
{
    var santasHomeLocation
        = Model.Localities.All
            .Where(_ => _.Landmarks.Contains("Santa's Workshop"))
            .FirstOrDefault();

    Assert.That(santasHomeLocation.Coordinates.Latitude, Is.EqualTo(90.0));
    Assert.That(santasHomeLocation.Coordinates.Longitude, Is.EqualTo(0.0));
    Assert.That(santasHomeLocation.Landmarks.Length, Is.EqualTo(1));
    Assert.That(santasHomeLocation.Name, Is.EqualTo("North Pole"));
    Assert.That(santasHomeLocation.TimeZone, Is.Null);
}
```

A similar test using snaptest-dotnet looks like:

```C#
[Test]
public void Can_use_simple_Assert_constraint()
{
    var santasHomeLocation
        = Model.Localities.All
            .Where(_ => _.Landmarks.Contains("Santa's Workshop"))
            .FirstOrDefault();

    Assert.That(santasHomeLocation, SnapshotDoes.Match());
}
```

For an xUnit.net test the `Assert` statement looks like:

```C#
    SnapshotAssert.Matches(santasHomeLocation);

    // Or fluent style: santasHomeLocation.ShouldMatchSnapshot();
```

This compares the actual results against snapshotted expected results stored in JSON format in the file `_snapshots/Tests.Can_use_simple_Assert_constraint.txt`:

```json
{
  "Coordinates": {
    "Latitude": 90.0,
    "Longitude": 0.0
  },
  "Landmarks": [
    "Santa's Workshop"
  ],
  "Name": "North Pole",
  "TimeZone": null
}
```


## Getting Started

The following example illustrates how to get started using snaptest-dotnet with NUnit tests. See [here](docs/WorkflowOverview.Xunit.md) for a similar example for xUnit.net tests.


<!-- This content is duplicated in docs/WorkflowOverview.md. Try to keep the two renditions in sync! -->

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

    It is also good practice to configure your version control tool to ignore `*=.txt.` files in the `_snapshots` subdirectory (these files will be created when a snapshot match fails).

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


## Going Deeper

- Learn more about SnapTest in the [documentation](docs/README.md).

- Browse [examples of tests using SnapTest](examples).


## Questions and feedback sought

### How should configuration to force snapshots to be updated or missing snapshots created be managed?

Currently the `SNAPTEST_REFRESH` and `SNAPTEST_CREATE_MISSING_SNAPSHOTS` environment variables are set when running tests to automatically create or update snapshot files based on actual values. Alternatively, code can explicitly set the `SnapshotSettings` `ForceSnapshotRefresh` and `CreateMissingSnapshots` properties.

However these options may not suit all kinds of workflows and development practices. What other kinds of approaches for controlling this might be helpful to suit different styles of development practices & workflows?


## License

This project is licensed under the terms of the MIT License. See [License](LICENSE.md) for details.
