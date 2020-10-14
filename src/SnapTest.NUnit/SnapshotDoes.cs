using NUnit.Framework.Constraints;
using System;

// See the following page for information about writing custom NUnit constraints: https://docs.nunit.org/articles/nunit/extending-nunit/Custom-Constraints.html

namespace SnapTest.NUnit
{
    /// <summary>
    /// Helper class with properties and methods that supply a number of snapshotting-related constrains used in NUnit Asserts.
    /// </summary>
    public class SnapshotDoes
    {
        /// <summary>
        /// Returns a constraint that tests an object value matches a snapshot.
        /// </summary>
        public static SnapshotConstraint Match(SnapshotSettingsBuilder settingsBuilder = null)
            => new SnapshotConstraint(settingsBuilder);

        public static SnapshotConstraint Match(string snapshotName, SnapshotSettingsBuilder settingsBuilder = null)
            => new SnapshotConstraint(snapshotName, settingsBuilder);

        public static SnapshotConstraint Match(Action<SnapshotSettings> settingsInitializer)
            => new SnapshotConstraint(settingsInitializer);
    }

    public static class SnapshotConstraintExtensions
    {
        public static SnapshotConstraint MatchSnapshot(this ConstraintExpression expression, SnapshotSettingsBuilder settingsBuilder = null)
            => expression.MatchSnapshot(() => new SnapshotConstraint(settingsBuilder));

        public static SnapshotConstraint MatchSnapshot(this ConstraintExpression expression, string snapshotName, SnapshotSettingsBuilder settingsBuilder = null)
            => expression.MatchSnapshot(() => new SnapshotConstraint(snapshotName, settingsBuilder));

        public static SnapshotConstraint MatchSnapshot(this ConstraintExpression expression, Action<SnapshotSettings> settingsInitializer)
            => expression.MatchSnapshot(() => new SnapshotConstraint(settingsInitializer));

        private static SnapshotConstraint MatchSnapshot(this ConstraintExpression expression, Func<SnapshotConstraint> constraintBuilder)
        {
            var constraint = constraintBuilder();
            expression.Append(constraint);
            return constraint;
        }
    }
}
