using NUnit.Framework;

namespace SnapTest.Tests
{
    public class SnapshotSettingsTest
    {
        [Test(ExpectedResult = ".txt.actual")]
        public string MismatchedActualExtension_has_expected_default()
            => new SnapshotSettings().MismatchedActualExtension;

        [Test(ExpectedResult = "alternate.actual")]
        public string MismatchedActualExtension_follows_SnapshotExtension()
        {
            var s = new SnapshotSettings();
            s.SnapshotExtension = "alternate";
            return s.MismatchedActualExtension;
        }

        [Test(ExpectedResult = ".actualalternate")]
        public string MismatchedActualExtension_can_be_updated_with_star()
        {
            var s = new SnapshotSettings();
            s.MismatchedActualExtension = ".actual*";
            s.SnapshotExtension = "alternate";
            return s.MismatchedActualExtension;
        }

        [Test(ExpectedResult = "no-star-here")]
        public string MismatchedActualExtension_can_be_updated_without_star()
        {
            var s = new SnapshotSettings();
            s.MismatchedActualExtension = "no-star-here";
            s.SnapshotExtension = "alternate";
            return s.MismatchedActualExtension;
        }
    }
}
