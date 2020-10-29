# NUnit settings

The `SnapTest.NUnit.SnapshotSettings` class is derived from `SnapTest.SnapshotTestFrameworkSettingsBase`, and provides additional capabilities for defaulting settings values as appropriate for performing snapshot match operations from within the context of running NUnit tests.

Defaults for following settings from the base class are overridden by `SnapTest.NUnit.SnapshotSettings` when a settings object is created using a builder obtained from `SnapTest.NUnit.SnapshotSettings.GetBuilder()`:

Setting|Default
---|---
`SnapshotName`|Set based on the NUnit `TestContext.Current.Test.ClassName` and `Name` properties.<br/><br/><list><li>If `DefaultSnapshotGroupKeyFromTestName` is `false` (the default), `SnapshotName` defaults to _ClassName_._TestName_, where _ClassName_ is the text after the last "." of `Test.ClassName`, and _TestName_ is `Test.Name`.</li><li>Otherwise the `SnapshotName` defaults to _ClassName_.</li></list>
`SnapshotGroupKey`|If `DefaultSnapshotGroupKeyFromTestName` is `false` (the default), `SnapshotGroupKey` is not set by default.<br/><br/>Otherwise it defaults to the NUnit `TestContext.CurrentContext.Test.Name` property value.
`SnapshotComparer`|An instance of an internal `SnapTest.NUnit` class implementating the `ISnapshotEqualityComparer` interface that generates an NUnit `ConstraintResult` recording the result of the comparison.
`MessageWriter`|An instance of an internal `SnapTest.NUnit` class that uses the NUnit `TestContext.Progress.WriteLine` method to emit informational messages.


## Building SnapTest.NUnit.SnapshotSettings instances

Instances of the `SnapTest.NUnit.SnapshotSettings` class are created using builders obtained by calling `SnapshotSettings.GetBuilder()`.

This is commonly done implicitly when `SnapshotDoes.Match()` is called for simple snapshot matches. For more complex matches where non-default settings are needed, possible to call the static `SnapshotSettings.GetBuilder()` method directly to create a `SnapshotBuilder<SnapshotSettings>`, configure settings through the builder, and then pass it as a parameter to `SnapshotDoes.Match()`.

`SnapshotBuilder<SnapshotSettings>.WithSettings(settings => { ... })` can be called to register actions to be invoked when the builder builds a `SnapshotSettings` object. The action takes the settings object that is being built as its single argument. Actions are invoked in the order of the calls that were made to `WithSettings`.

For example:

```C#
var localities = Model.Localities.All.OrderBy(_ => _.Name);

var builder = SnapshotSettings.GetBuilder();

builder.WithSettings(_ => {
    _.Field("$..['Name','Coordinates']").Include();
    _.DefaultSnapshotGroupKeyFromTestName = true;
});

Assert.That(localities, SnapshotDoes.Match(builder));
```
