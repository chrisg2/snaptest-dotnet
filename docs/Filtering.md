# Filtering values

> _TODO: Documentation to be written_

## Filter certain elements of the actual result to be included or excluded using JSON Paths

Call `SnapshotSettings.Field(...).Include()` and `SnapshotSettings.Field(...).Exclude()` to specify JSON Paths identifying elements of the actual result to be included or excluded from the snapshot.

For example, in an NUnit test:
```C#
[Test]
public void Can_include_and_exclude_fields()
{
    var sydney = Model.Localities.All.Where(c => c.Name == "Sydney").FirstOrDefault();

    var builder = SnapshotSettings.GetBuilder().WithSettings(_ => {
        // Include only the TimeZone field in the snapshot
        _.Field("TimeZone").Include();
        // Exclude the current time from the snapshot as it changes from moment to moment
        _.Field("TimeZone.CurrentTime").Exclude();
    });

    Assert.That(sydney, SnapshotDoes.Match(builder));
}
```

Or in an xUnit.net test:
```C#
[Fact]
public void Can_include_and_exclude_fields()
{
    var sydney = Model.Localities.All.Where(c => c.Name == "Sydney").FirstOrDefault();

    var builder = SnapshotSettings.GetBuilder().WithSettings(_ => {
        // Include only the TimeZone field in the snapshot
        _.Field("TimeZone").Include();
        // Exclude the current time from the snapshot as it changes from moment to moment
        _.Field("TimeZone.CurrentTime").Exclude();
    });

    SnapshotAssert.Matches(sydney, builder);
}
```
