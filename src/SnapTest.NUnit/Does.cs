using NUnit.Framework;
using NUnit.Framework.Constraints;
using System;

// See the following page for guidance on writing custom NUnit constraints: https://docs.nunit.org/articles/nunit/extending-nunit/Custom-Constraints.html

namespace SnapTest.NUnit
{
    /// <summary>
    /// Helper class with properties and methods that supply a number of snapshotting-related constrains used in NUnit Asserts.
    /// </summary>
    public class Does: global::NUnit.Framework.Does
    {
        /// <summary>
        /// Returns a constraint that tests an object value matches a snapshot.
        /// </summary>
        public static SnapshotConstraint MatchSnapshot(string testName = null, SnapshotBuilder snapshotBuilder = null)
            => new SnapshotConstraint(testName, snapshotBuilder);

        public static SnapshotConstraint MatchSnapshot(SnapshotBuilder snapshotBuilder)
            => new SnapshotConstraint(null, snapshotBuilder);
    }

    public static class SnapshotConstraintExtensions
    {
        public static SnapshotConstraint MatchSnapshot(this ConstraintExpression expression, string testName = null, SnapshotBuilder snapshotBuilder = null)
        {
            var constraint = new SnapshotConstraint(testName, snapshotBuilder);
            expression.Append(constraint);
            return constraint;
        }

        public static SnapshotConstraint MatchSnapshot(this ConstraintExpression expression, SnapshotBuilder snapshotBuilder)
            => MatchSnapshot(null, snapshotBuilder);
    }
}
