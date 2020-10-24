# Snapshot Testing with NUnit

The `SnapTest.NUnit` package provides the following key classes in the `SnapTest.NUnit` namespace for writing snapshot-based tests with [NUnit](https://nunit.org):

- `SnapshotConstraint`: NUnit `Constraint` that matches an actual value against an expected value that is stored in a snapshot file.
- `SnapshotSettings`: Settings that control how snapshot processing is performed.
- `SnapshotDoes`: Helper class with properties and methods that supply a number of snapshotting-related constraints used in NUnit constraint-based assertions.

Here are some examples of snapshot-based tests illustrating typical uses of these classes.


## Basic test matching an actual value against a snapshotted value

The most common pattern for matching an actual value again a snapshotted value in an NUnit test simply calls `Assert.That(actualValue, SnapshotDoes.Match())`. For example:

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


## NUnit constraint expressions can match snapshots using `MatchSnapshot()`

The `MatchSnapshot()` extension method can be used to perform a snapshot match in an NUnit constraint expression. For example:

```C#
[Test]
public void Can_use_constraint_expression()
{
    var santasTimeZone
        = Model.Localities.All
            .Where(_ => _.Landmarks.Contains("Santa's Workshop"))
            .Select(_ => _.TimeZone)
            .FirstOrDefault();

    Assert.That(santasTimeZone, Is.Null.And.MatchSnapshot());
}
```


## Overriding default settings

Details on how a snapshot is stored or matched are controlled by [snapshot settings](SnapshotSettings.md). Settings can be explicitly overidden using various coding styles. Here are some typical examples.


### Using a settings builder to configure settings

Construct a `SnapshotSettingsBuilder` to configure settings as desired. The builder can be passed as a parameter to 'SnapshotDoes.Match(builder)' to be used when performing a snapshot match.

The following example illustrates a builder being configured in a test fixture set up method so that the same settings can be used across multiple tests. `SnapshotSettings.GetBuilder()` could alternately be called in an individual test method if the builder did not need to be shared across tests.

```C#
private SnapshotSettingsBuilder<SnapshotSettings> commonBuilder;

[SetUp]
public void SetUp()
{
    // Store snapshots in a snapshot group file named .snapshots/SettingsOverrides.json
    commonBuilder = SnapshotSettings.GetBuilder().WithSettings(_ => {
        _.SnapshotName = "SettingsOverrides";
        _.DefaultSnapshotGroupKeyFromTestName = true;
        _.MismatchedActualExtension = ".actual.json";
        _.SnapshotExtension = ".json";
        _.SnapshotSubdirectory = ".snapshots";
    });
}

[Test]
public void SnapshotDoesMatch_can_accept_builder()
{
    var landmarks = Model.Localities.All.Select(_ => _.Landmarks).SelectMany(_ => _).OrderBy(_ => _);

    Assert.That(landmarks, SnapshotDoes.Match(commonBuilder));
}
```


### Using `SnapshotDoes.Match().WithSettings(...)` to configure settings

Settings can also be configured by calling `SnapshotConstraint.WithSettings(settings => { ... })`. `WithSettings` takes a single argument which is an action that will be called when a new `SnapshotSettings` object is created and needs to be initialized to perform the snapshot match.

For example:

```C#
[Test]
public void WithSettings_can_be_called_on_SnapshotConstraint()
{
    var localities = Model.Localities.All.OrderBy(_ => _.Name);

    Assert.That(localities, SnapshotDoes.Match(commonBuilder).WithSettings(_ =>
        _.IncludedPaths.Add("$..['Name','Coordinates']")
    ));
}
```


### Explicitly specifying a snapshot name

The snapshot name (which is used to determine the snapshot file name) defaults to the NUnit test name, but can be explicitly overridden when calling `SnapshotDoes.Match`:

```C#
[Test]
public void SnapshotDoesMatch_can_accept_name()
{
    var firstLocalityName = Model.Localities.All.OrderBy(_ => _.Name).FirstOrDefault()?.Name;

    Assert.That(firstLocalityName, SnapshotDoes.Match("SampleSnapshotName"));
}
```

This style of calling gives the same result as setting `TestName` in the settings builder (which tends to be more verbose for simple scenarios):

```C#
Assert.That(firstLocalityName, SnapshotDoes.Match().WithSettings(_ =>
    _.TestName = "SampleSnapshotName"));
```
