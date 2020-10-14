using NUnit.Framework.Constraints;
using System;

// See the following page for information about writing custom NUnit constraints: https://docs.nunit.org/articles/nunit/extending-nunit/Custom-Constraints.html

namespace SnapTest.NUnit
{
    /// <summary>
    /// Helper class with properties and methods that supply a number of snapshotting-related constraints used in NUnit constraint-based assertions.
    /// </summary>
    ///
    /// <example>
    /// Common simple use of this class with all default settings looks like the following:
    /// <code>
    /// Assert.That(actualValue, SnapshotDoes.Match());
    /// </code>
    /// </example>
    public class SnapshotDoes
    {
        /// <summary>
        /// Returns a <see cref="SnapshotConstraint"/> that will use settings from <paramref name="settingsBuilder"/> (if supplied),
        /// or a default <see cref="SnapshotSettingsBuilder"/> (if not supplied).
        /// </summary>
        /// <param name="settingsBuilder">A builder to create a <see cref="SnapshotSettings"/> object when needed
        /// to perform a snapshot comparison. This value (if supplied) is used to initialize the returned <see cref="SnapshotConstraint.SettingsBuilder"/>.</param>
        public static SnapshotConstraint Match(SnapshotSettingsBuilder settingsBuilder = null)
            => new SnapshotConstraint(settingsBuilder);

        /// <summary>
        /// Returns a <see cref="SnapshotConstraint"/> that will use the specified snapshot name, along with settings from
        /// <paramref name="settingsBuilder"/> (if supplied) or a default <see cref="SnapshotSettingsBuilder"/> (if not supplied).
        /// </summary>
        /// <param name="snapshotName">The name of the snapshot, used to initialize the returned <see cref="SnapshotConstraint.SnapshotName"/>.</param>
        /// <param name="settingsBuilder">A builder to create a <see cref="SnapshotSettings"/> object when needed
        /// to perform a snapshot comparison. This value (if supplied) is used to initialize the returned <see cref="SnapshotConstraint.SettingsBuilder"/>.</param>
        public static SnapshotConstraint Match(string snapshotName, SnapshotSettingsBuilder settingsBuilder = null)
            => new SnapshotConstraint(snapshotName, settingsBuilder);

        /// <summary>
        /// Returns a <see cref="SnapshotConstraint"/> that will use the specified specified action to initialize
        /// <see cref="SnapshotSettings"/> objects when they are created as needed.
        /// </summary>
        /// <param name="settingsInitializer">An action to be called to set values in <see cref="SnapshotSettings"/> objects created
        /// by the returned <see cref="SnapshotConstraint"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="settingsInitializer"/> is null.
        /// </exception>
        public static SnapshotConstraint Match(Action<SnapshotSettings> settingsInitializer)
            => new SnapshotConstraint(settingsInitializer);
    }

    public static class SnapshotConstraintExtensions
    {
        /// <summary>
        /// Returns a <see cref="SnapshotConstraint"/> that will use settings from <paramref name="settingsBuilder"/> (if supplied),
        /// or a default <see cref="SnapshotSettingsBuilder"/> (if not supplied).
        /// The constraint is appended to a provided <paramref name="expression"/>.
        /// </summary>
        /// <param name="expression">The NUnit <see cref="ConstraintExpression"/> to append the newly created <see cref="SnapshotConstraint"/> to.</param>
        /// <param name="settingsBuilder">A builder to create a <see cref="SnapshotSettings"/> object when needed
        /// to perform a snapshot comparison. This value (if supplied) is used to initialize the returned <see cref="SnapshotConstraint.SettingsBuilder"/>.</param>
        public static SnapshotConstraint MatchSnapshot(this ConstraintExpression expression, SnapshotSettingsBuilder settingsBuilder = null)
            => expression.MatchSnapshot(() => new SnapshotConstraint(settingsBuilder));

        /// <summary>
        /// Returns a <see cref="SnapshotConstraint"/> that will use the specified snapshot name, along with settings from
        /// <paramref name="settingsBuilder"/> (if supplied) or a default <see cref="SnapshotSettingsBuilder"/> (if not supplied).
        /// The constraint is appended to a provided <paramref name="expression"/>.
        /// </summary>
        /// <param name="expression">The NUnit <see cref="ConstraintExpression"/> to append the newly created <see cref="SnapshotConstraint"/> to.</param>
        /// <param name="snapshotName">The name of the snapshot, used to initialize the returned <see cref="SnapshotConstraint.SnapshotName"/>.</param>
        /// <param name="settingsBuilder">A builder to create a <see cref="SnapshotSettings"/> object when needed
        /// to perform a snapshot comparison. This value (if supplied) is used to initialize the returned <see cref="SnapshotConstraint.SettingsBuilder"/>.</param>
        public static SnapshotConstraint MatchSnapshot(this ConstraintExpression expression, string snapshotName, SnapshotSettingsBuilder settingsBuilder = null)
            => expression.MatchSnapshot(() => new SnapshotConstraint(snapshotName, settingsBuilder));

        /// <summary>
        /// Returns a <see cref="SnapshotConstraint"/> that will use the specified specified action to initialize
        /// <see cref="SnapshotSettings"/> objects when they are created as needed.
        /// The constraint is appended to a provided <paramref name="expression"/>.
        /// </summary>
        /// <param name="expression">The NUnit <see cref="ConstraintExpression"/> to append the newly created <see cref="SnapshotConstraint"/> to.</param>
        /// <param name="settingsInitializer">An action to be called to set values in <see cref="SnapshotSettings"/> objects created
        /// by the returned <see cref="SnapshotConstraint"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="settingsInitializer"/> is null.
        /// </exception>
        public static SnapshotConstraint MatchSnapshot(this ConstraintExpression expression, Action<SnapshotSettings> settingsInitializer)
            => expression.MatchSnapshot(() => new SnapshotConstraint(settingsInitializer));

        /// <summary>
        /// Helper method to create a <see cref="SnapshotConstraint"/> and append it to a <see cref="ConstraintExpression"/>.
        /// </summary>
        private static SnapshotConstraint MatchSnapshot(this ConstraintExpression expression, Func<SnapshotConstraint> constraintBuilder)
        {
            var constraint = constraintBuilder();
            expression.Append(constraint);
            return constraint;
        }
    }
}
