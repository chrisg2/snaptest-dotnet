using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.IO;
using System.Linq;
using System.Reflection;
//using System.Runtime.CompilerServices;
using Xunit;

namespace SnapTest.Xunit
{
    /// <summary>
    /// Settings used to control how snapshot processing is performed within the context of an xUnit.net test.
    /// </summary>
    /// <remarks>
    /// Instances of this class are typically obtained by first creating a <see cref="SnapshotSettingsBuilder&lt;SnapshotSettings&gt;"/>
    /// by calling <see cref="GetBuilder"/>, configuring the builder to set settings as appropriate, and then calling the builder's
    /// <see cref="SnapshotSettingsBuilder&lt;SnapshotSettings&gt;.Build"/> method.
    /// </remarks>
    public class SnapshotSettings: SnapshotTestFrameworkSettingsBase
    {
        #region Constructors
        /// <summary>
        /// This default constructor is private to avoid directly creating new objects.
        /// Use <see cref="Build"/> to create and initialize <see cref="SnapshotSettings"/> objects.
        /// </summary>
        private SnapshotSettings() { }
        #endregion

        #region Methods
        /// <summary>
        /// Creates a new <see cref="SnapshotSettingsBuilder&lt;SnapshotSettings&gt;"/> that can be used to create and initialize <see cref="SnapshotSettings"/> objects
        /// as needed when a snapshot comparison is performed.
        /// </summary>
        public static SnapshotSettingsBuilder<SnapshotSettings> GetBuilder()
            => new SnapshotSettingsBuilder<SnapshotSettings>(Build);

        /// <summary>
        /// Create a new <see cref="SnapshotSettings"/> object for snapshot operations with the xUnit.net testing framework.
        /// </summary>
        private static SnapshotSettings Build()
            => new SnapshotSettings() { MessageWriter = new XunitMessageWriter() };

        #region Private helper methods
        protected override string DeriveSnapshotNameFromTestContext()
        {
            var method = FindTestMethodInStackTrace().Item1;
            var className = method.ReflectedType.Name;

            return DefaultSnapshotGroupKeyFromTestName ? className : $"{className}.{method.Name}";
        }

        protected override string DeriveSnapshotGroupKeyFromTestContext()
            => FindTestMethodInStackTrace().Item1.Name;

        protected override bool IsTestMethod(MethodBase method)
            =>  method.GetCustomAttributes<FactAttribute>().Any()
                || method.GetCustomAttributes<TheoryAttribute>().Any()
            ;
        #endregion
        #endregion
    }
}
