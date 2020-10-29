# Snapshot Testing with xUnit.net

The `SnapTest.Xunit` package provides the following key classes in the `SnapTest.Xunit` namespace for writing snapshot-based tests with [xUnit.net](https://xunit.net):

- `SnapshotAssert`: Static "assertion" class containing methods to perform snapshot matches of an actual value against an expected value that is stored in a snapshot file in the context of an xUnit.net.
- `SnapshotSettings`: Settings that control how snapshot processing is performed.

Here are some examples of snapshot-based tests illustrating typical uses of these classes.


## Basic test matching an actual value against a snapshotted value

The most common pattern for matching an actual value again a snapshotted value in an xUnit.net test simply calls `SnapshotAssert.Matches(actualValue)`. For example:

```C#
[Fact]
public void Can_use_simple_Matches_call()
{
    var santasHomeLocation
        = Model.Localities.All
            .Where(_ => _.Landmarks.Contains("Santa's Workshop"))
            .FirstOrDefault();

    SnapshotAssert.Matches(santasHomeLocation);
}
```

A fluent syntax using the `ShouldMatchSnapshot` extension method can also be used:

```C#
[Fact]
public void Can_use_fluent_ShouldMatchSnapshot_call()
{
    var santasHomeLocation
        = Model.Localities.All
            .Where(_ => _.Landmarks.Contains("Santa's Workshop"))
            .FirstOrDefault();

    santasHomeLocation.ShouldMatchSnapshot();
}
```


## Overriding default settings

Details on how a snapshot is stored or matched are controlled by [snapshot settings](SnapshotSettings.md). Settings can be explicitly overidden using various coding styles. Here are some typical examples.


### Using a settings builder to configure settings

Construct a `SnapshotSettingsBuilder` to configure settings as desired. The builder can be passed as a parameter to `SnapshotAssert.Matches(actualValue, builder)` or `ShouldMatchSnapshot(builder)` to be used when performing a snapshot match.

The following example illustrates a builder being configured in a method `GetSettingsBuilder` that can be called from multiple tests that need to share the same settings. `SnapshotSettings.GetBuilder()` could alternately be called in an individual test method if the builder did not need to be shared across tests.

```C#
private SnapshotSettingsBuilder<SnapshotSettings> GetSettingsBuilder()
{
    // Construct a SnapshotSettingsBuilder to provide common settings shared by all tests in this fixture.
    // The settings store snapshots in a snapshot group file named .snapshots/SettingsOverrides.json

    return SnapshotSettings.GetBuilder().WithSettings(_ => {
        _.SnapshotName = "SettingsOverrides";
        _.DefaultSnapshotGroupKeyFromTestName = true;
        _.MismatchedActualExtension = ".actual.json";
        _.SnapshotExtension = ".json";
        _.SnapshotSubdirectory = ".snapshots";
    });
}

[Fact]
public void Matches_can_accept_settings()
{
    var landmarks = Model.Localities.All.Select(_ => _.Landmarks).SelectMany(_ => _).OrderBy(_ => _);

    // Overide default settings by providing a SnapshotSettings when calling SnapshotAssert.Matches
    SnapshotAssert.Matches(landmarks, GetSettingsBuilder());

    // Or using fluent style:
    // landmarks.ShouldMatchSnapshot(GetSettingsBuilder());
}
```


### Explicitly specifying a snapshot name

The snapshot name (which is used to determine the name of the file the snapshot is stored in) defaults to the xUnit.net "{Test class name}.{Test name}", but can be explicitly overridden when calling `SnapshotAssert.Matches` or `ShouldMatchSnapshot`:

```C#
[Fact]
public void Matches_can_accept_name_and_settings()
{
    var firstLocalityName = Model.Localities.All.OrderBy(_ => _.Name).FirstOrDefault()?.Name;

    // The snapshot name defaults to the xUnit.net test method name, but can be explicitly overridden when calling SnapshotAssert.Matches
    SnapshotAssert.Matches(firstLocalityName, "SampleSnapshotName", GetSettingsBuilder());

    // Or using fluent style:
    // firstLocalityName.ShouldMatchSnapshot("SampleSnapshotName", GetSettingsBuilder());
}
```

This style of calling gives the same result as setting `TestName` in the settings builder (which tends to be more verbose for simple scenarios):

```C#
SnapshotAssert.Matches(firstLocalityName, GetSettingsBuilder().WithSettings(_ =>
    _.TestName = "SampleSnapshotName"));
```
