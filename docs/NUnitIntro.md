# Snapshot Testing with NUnit

The `SnapTest.NUnit` package provides the following key classes in the `SnapTest.NUnit` namespace for writing snapshot-based tests with [NUnit](https://nunit.org):

- `SnapshotConstraint`: NUnit `Constraint` that compares an actual value against an expected value that is stored in a snapshot file.
- `SnapshotSettings` which can be constructed by the an instance of `SnapshotSettingsBuilder`: Settings that control how snapshot processing is performed.
- `SnapshotDoes`: Helper class with properties and methods that supply a number of snapshotting-related constraints used in NUnit constraint-based assertions.

Here are some examples of snapshot-based tests illustrating typical uses of these classes.


## Basic test comparing an actual value to a snapshotted value

The most common pattern for comparing an actual value to a snapshotted value in an NUnit test simply calls `Assert.That(actualValue, SnapshotDoes.Match())`. For example:

```C#
[Test]
public void Santa_lives_at_the_NorthPole()
{
    var santasHomeLocation
        = CityModel.Cities.AllCities
            .Where(_ => _.Landmarks.Contains("Santa's Workshop"))
            .Select(_ => _.Location)
            .FirstOrDefault();

    Assert.That(santasHomeLocation, SnapshotDoes.Match());
}
```


## NUnit constraint expressions can match snapshots using `MatchSnapshot()`

The `MatchSnapshot()` extension method can be used to perform a snapshot comparison in an NUnit constraint expression. For example:

```C#
[Test]
public void Santa_has_no_time_zone()
{
    var santasTimeZone
        = CityModel.Cities.AllCities
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

The following example illustrates a builder being configured in a test fixture set up method so that the same settings can be used across multiple tests. The `SnapshotSettingsBuilder` could alternately be created in an individual test method if it did not need to be shared across tests.

```C#
private SnapshotSettingsBuilder commonBuilder;

[SetUp]
public void SetUp()
{
    // Store snapshots in a snapshot group file named .snapshots/SettingsOverrides.json
    commonBuilder = new SnapshotSettingsBuilder().WithSettings(_ => {
        _.SnapshotName = "SettingsOverrides";
        _.DefaultSnapshotGroupKeyFromNUnitTestName = true;
        _.MismatchedActualExtension = ".actual.json";
        _.SnapshotExtension = ".json";
        _.SnapshotSubdirectory = ".snapshots";
    });
}

[Test]
public void No_landmarks_have_been_added_or_lost()
{
    var landmarks = CityModel.Cities.AllCities.Select(_ => _.Landmarks).SelectMany(_ => _).OrderBy(_ => _);

    Assert.That(landmarks, SnapshotDoes.Match(commonBuilder));
}
```


### Using `SnapshotDoes.Match().WithSettings(...)` to configure settings

Settings can also be configured by calling `SnapshotConstraint.WithSettings(settings => { ... })`. `WithSettings` takes a single argument which is an action that will be called when a new `SnapshotSettings` object is created and needs to be initialized to perform the snapshot match.

For example:

```C#
[Test]
public void Cities_have_not_moved()
{
    var cities = CityModel.Cities.AllCities.OrderBy(_ => _.Name);

    Assert.That(cities, SnapshotDoes.Match(commonBuilder).WithSettings(_ =>
        _.IncludedPaths.Add("$..['Name','Location']")
    ));
}
```


### Explicitly specifying a snapshot name

The snapshot name (which is used to determine the snapshot file name) defaults to the NUnit test name, but can be explicitly overridden when calling `SnapshotDoes.Match`:

```C#
[Test]
public void First_city_name_has_not_moved()
{
    var firstCityName = CityModel.Cities.AllCities.OrderBy(_ => _.Name).FirstOrDefault()?.Name;

    Assert.That(firstCityName, SnapshotDoes.Match("First city name"));
}
```

This style of calling gives the same result as setting `TestName` in the settings builder (which tends to be more verbose for simple scenarios):

```C#
Assert.That(firstCityName, SnapshotDoes.Match().WithSettings(_ =>
    _.TestName = "First city name"));
```
