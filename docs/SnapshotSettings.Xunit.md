# xUnit.net settings

The `SnapTest.Xunit.SnapshotSettings` class is derived from `SnapTest.SnapshotTestFrameworkSettingsBase`, and provides additional capabilities for defaulting settings values as appropriate for performing snapshot match operations from within the context of running xUnit.net tests.

Defaults for following settings from the base class are overridden by `SnapTest.Xunit.SnapshotSettings` when a settings object is created using a builder obtained from `SnapTest.Xunit.SnapshotSettings.GetBuilder()`:

Setting|Default
---|---
`SnapshotName`|Set based on the xUnit.net test method that is executing at the time the snapshot settings are initialized.<br/><br/><list><li>If `DefaultSnapshotGroupKeyFromTestName` is `false` (the default), `SnapshotName` defaults to _ClassName_._TestName_, where _ClassName_ is the name of the class containing the test method, and _TestName_ is name of the test method itself.</li><li>Otherwise the `SnapshotName` defaults to _ClassName_.</li></list>
`SnapshotGroupKey`|If `DefaultSnapshotGroupKeyFromTestName` is `false` (the default), `SnapshotGroupKey` is not set by default.<br/><br/>Otherwise it defaults to the name of the xUnit.net test method that is executing at the time the snapshot settings are initialized.
`SnapshotComparer`|An instance of an internal `SnapTest.Xunit` class implementating the `ISnapshotEqualityComparer` interface that reports an appropriate xUnit.net test result according to the result of the comparison.
`MessageWriter`|An instance of an internal `SnapTest.Xunit` class that emits informational messages using `Console.WriteLine`.


## Building SnapTest.Xunit.SnapshotSettings instances

Instances of the `SnapTest.Xunit.SnapshotSettings` class are created using builders obtained by calling `SnapshotSettings.GetBuilder()`.

This is commonly done implicitly when `SnapshotAssert.Matches()` or `ShouldMatchSnapshot()` is called for simple snapshot matches. For more complex matches where non-default settings are needed, possible to call the static `SnapshotSettings.GetBuilder()` method directly to create a `SnapshotBuilder<SnapshotSettings>`, configure settings through the builder, and then pass it as a parameter to `SnapshotAssert.Matches()` or `ShouldMatchSnapshot()`.

`SnapshotBuilder<SnapshotSettings>.WithSettings(settings => { ... })` can be called to register actions to be invoked when the builder builds a `SnapshotSettings` object. The action takes the settings object that is being built as its single argument. Actions are invoked in the order of the calls that were made to `WithSettings`.

For example:

```C#
var localities = Model.Localities.All.OrderBy(_ => _.Name);

var builder = SnapshotSettings.GetBuilder();

builder.WithSettings(_ => {
    _.Field("$..['Name','Coordinates']").Include();
    _.DefaultSnapshotGroupKeyFromTestName = true;
});

SnapshotAssert.Matches(localities, builder);
```
