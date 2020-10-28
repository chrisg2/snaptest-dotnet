using Xunit;

namespace SnapTest.Xunit.Tests
{
    [UseSnapshotGroup]
    public class UseSnapshotGroupOnClass
    {
        [Fact]
        public void UseSnapshotGroupAttribute_on_class_is_effective()
            => SnapshotAssert.Matches("actual-UseSnapshotGroupOnClass.UseSnapshotGroupAttribute_on_class_is_effective");
    }

    [UseSnapshotGroup(SnapshotName = "UseSnapshotGroupOnClassWithSnapshotName.ExplicitlySpecifiedSnapshotNameOnClass")]
    public class UseSnapshotGroupOnClassWithSnapshotName
    {
        [Fact]
        public void UseSnapshotGroupAttribute_on_class_is_effective()
            => SnapshotAssert.Matches("actual-UseSnapshotGroupOnClassWithSnapshotName.UseSnapshotGroupAttribute_on_class_is_effective");

        [Fact]
        public void UseSnapshotGroupAttribute_on_class_with_overridden_SnapshotGroupKey_is_effective()
            => SnapshotAssert.Matches("actual-UseSnapshotGroupOnClassWithSnapshotName.UseSnapshotGroupAttribute_on_class_with_overridden_SnapshotGroupKey_is_effective",
                _ => _.SnapshotGroupKey = "UseSnapshotGroupAttribute_on_class_with_overridden_SnapshotGroupKey_is_effective - overridden group key");

        [Fact]
        [UseSnapshotGroup(SnapshotName = "UseSnapshotGroupOnClassWithSnapshotName.OverriddenSnapshotNameOnMethod")]
        public void UseSnapshotGroupAttribute_on_class_with_overridden_SnapshotName_on_method_is_effective()
            => SnapshotAssert.Matches("actual-UseSnapshotGroupOnClassWithSnapshotName.UseSnapshotGroupAttribute_on_class_with_overridden_SnapshotName_on_method_is_effective");
    }

    public class UseSnapshotGroupOnMethod
    {
        [Fact]
        [UseSnapshotGroup]
        public void UseSnapshotGroupAttribute_on_method_is_effective()
            => SnapshotAssert.Matches("actual-UseSnapshotGroupOnMethod.UseSnapshotGroupAttribute_on_method_is_effective");

        [Fact]
        [UseSnapshotGroup]
        public void UseSnapshotGroupAttribute_with_SnapshotGroupKey_on_method_is_effective()
            => SnapshotAssert.Matches("actual-UseSnapshotGroupOnMethod.UseSnapshotGroupAttribute_with_SnapshotGroupKey_on_method_is_effective",
                _ => _.SnapshotGroupKey = "UseSnapshotGroupAttribute_with_SnapshotGroupKey_on_method_is_effective - overridden group key");

        [Fact]
        [UseSnapshotGroup(SnapshotName = "UseSnapshotGroupOnMethod.ExplicitlySpecifiedSnapshotNameOnMethod")]
        public void UseSnapshotGroupAttribute_with_SnapshotName_on_method_is_effective()
            => SnapshotAssert.Matches("actual-UseSnapshotGroupOnMethod.UseSnapshotGroupAttribute_with_SnapshotName_on_method_is_effective");

        [Fact]
        [UseSnapshotGroup(SnapshotName = "UseSnapshotGroupOnMethod.ExplicitlySpecifiedSnapshotNameOnMethod")]
        public void UseSnapshotGroupAttribute_with_SnapshotGroupKey_and_SnapshotName_on_method_is_effective()
            => SnapshotAssert.Matches("actual-UseSnapshotGroupOnMethod.UseSnapshotGroupAttribute_with_SnapshotGroupKey_and_SnapshotName_on_method_is_effective",
                _ => _.SnapshotGroupKey = "UseSnapshotGroupAttribute_with_SnapshotGroupKey_and_SnapshotName_on_method_is_effective - overridden group key");
    }
}
