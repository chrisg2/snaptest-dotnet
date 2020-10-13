﻿using NUnit.Framework.Constraints;
using System;

// See the following page for information about writing custom NUnit constraints: https://docs.nunit.org/articles/nunit/extending-nunit/Custom-Constraints.html

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
        public static SnapshotConstraint MatchSnapshot(SnapshotSettingsBuilder settingsBuilder = null)
            => new SnapshotConstraint(settingsBuilder);

        public static SnapshotConstraint MatchSnapshot(string testName)
            => new SnapshotConstraint(testName);

        public static SnapshotConstraint MatchSnapshot(Action<SnapshotSettings> settingsInitializer)
            => new SnapshotConstraint(settingsInitializer);
    }

    public static class SnapshotConstraintExtensions
    {
        public static SnapshotConstraint MatchSnapshot(this ConstraintExpression expression, SnapshotSettingsBuilder settingsBuilder = null)
            => expression.MatchSnapshot(() => new SnapshotConstraint(settingsBuilder));

        public static SnapshotConstraint MatchSnapshot(this ConstraintExpression expression, string testName)
            => expression.MatchSnapshot(() => new SnapshotConstraint(testName));

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
