# Filtering values

> _TODO: Documentation to be written_

## Filter certain elements of the actual result to be included or excluded using JSON Paths

Use the `IncludedPaths` and `ExcludedPaths` settings to specify JSON Paths identifying elements of the actual result to be included or excluded from the snapshot.

```C#
[Test]
public void Sydney_time_zone_is_correct()
{
    var sydney = CityModel.Cities.AllCities.Where(c => c.Name == "Sydney").FirstOrDefault();

    var builder = new SnapshotSettingsBuilder().WithSettings(_ => {
        // Include only the TimeZone field in the snapshot
        _.IncludedPaths.Add("TimeZone");
        // Exclude the current time from the snapshot as it changes from moment to moment
        _.ExcludedPaths.Add("TimeZone.CurrentTime");
    });

    Assert.That(sydney, SnapshotDoes.Match(builder));
}
```
