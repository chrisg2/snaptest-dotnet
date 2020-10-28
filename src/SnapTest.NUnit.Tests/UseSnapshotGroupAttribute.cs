using NUnit.Framework;

namespace SnapTest.NUnit.Tests
{
    [UseSnapshotGroup]
    public class UseSnapshotGroupOnClass
    {
        [Test]
        public void UseSnapshotGroupAttribute_on_class_is_effective()
            => Assert.That("actual-UseSnapshotGroupOnClass.UseSnapshotGroupAttribute_on_class_is_effective",
                SnapshotDoes.Match());
    }

    [UseSnapshotGroup(SnapshotName = "UseSnapshotGroupOnClassWithSnapshotName.ExplicitlySpecifiedSnapshotNameOnClass")]
    public class UseSnapshotGroupOnClassWithSnapshotName
    {
        [Test]
        public void UseSnapshotGroupAttribute_on_class_is_effective()
            => Assert.That("actual-UseSnapshotGroupOnClassWithSnapshotName.UseSnapshotGroupAttribute_on_class_is_effective",
                SnapshotDoes.Match());

        [Test]
        public void UseSnapshotGroupAttribute_on_class_with_overridden_SnapshotGroupKey_is_effective()
            => Assert.That("actual-UseSnapshotGroupOnClassWithSnapshotName.UseSnapshotGroupAttribute_on_class_with_overridden_SnapshotGroupKey_is_effective",
                SnapshotDoes.Match(_ => _.SnapshotGroupKey = "UseSnapshotGroupAttribute_on_class_with_overridden_SnapshotGroupKey_is_effective - overridden group key"));

        [Test]
        [UseSnapshotGroup(SnapshotName = "UseSnapshotGroupOnClassWithSnapshotName.OverriddenSnapshotNameOnMethod")]
        public void UseSnapshotGroupAttribute_on_class_with_overridden_SnapshotName_on_method_is_effective()
            => Assert.That("actual-UseSnapshotGroupOnClassWithSnapshotName.UseSnapshotGroupAttribute_on_class_with_overridden_SnapshotName_on_method_is_effective",
                SnapshotDoes.Match());
    }

    public class UseSnapshotGroupOnMethod
    {
        [Test]
        [UseSnapshotGroup]
        public void UseSnapshotGroupAttribute_on_method_is_effective()
            => Assert.That("actual-UseSnapshotGroupOnMethod.UseSnapshotGroupAttribute_on_method_is_effective",
                SnapshotDoes.Match());

        [Test]
        [UseSnapshotGroup]
        public void UseSnapshotGroupAttribute_with_SnapshotGroupKey_on_method_is_effective()
            => Assert.That("actual-UseSnapshotGroupOnMethod.UseSnapshotGroupAttribute_with_SnapshotGroupKey_on_method_is_effective",
                SnapshotDoes.Match(_ => _.SnapshotGroupKey = "UseSnapshotGroupAttribute_with_SnapshotGroupKey_on_method_is_effective - overridden group key"));

        [Test]
        [UseSnapshotGroup(SnapshotName = "UseSnapshotGroupOnMethod.ExplicitlySpecifiedSnapshotNameOnMethod")]
        public void UseSnapshotGroupAttribute_with_SnapshotName_on_method_is_effective()
            => Assert.That("actual-UseSnapshotGroupOnMethod.UseSnapshotGroupAttribute_with_SnapshotName_on_method_is_effective",
                SnapshotDoes.Match());

        [Test]
        [UseSnapshotGroup(SnapshotName = "UseSnapshotGroupOnMethod.ExplicitlySpecifiedSnapshotNameOnMethod")]
        public void UseSnapshotGroupAttribute_with_SnapshotGroupKey_and_SnapshotName_on_method_is_effective()
            => Assert.That("actual-UseSnapshotGroupOnMethod.UseSnapshotGroupAttribute_with_SnapshotGroupKey_and_SnapshotName_on_method_is_effective",
                SnapshotDoes.Match(_ => _.SnapshotGroupKey = "UseSnapshotGroupAttribute_with_SnapshotGroupKey_and_SnapshotName_on_method_is_effective - overridden group key"));
    }
}
