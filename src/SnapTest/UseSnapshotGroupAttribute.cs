using System;

namespace SnapTest
{
    /// <summary>
    /// The <c>UseSnapshotGroup</c> attribute can be used to mark a class or test method so that snapshot
    /// matches performed within the class/method operate with the default value of the
    /// <see cref="SnapshotTestFrameworkSettingsBase.DefaultSnapshotGroupKeyFromTestName"/> setting as <c>true</c>.
    /// The <see cref="SnapshotName"/> property of the attribute can also be set to explicitly set the snapshot name.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class UseSnapshotGroupAttribute : Attribute
    {
        /// <summary>
        /// The snapshot name to use for snapshot matches performed within the class/method that this
        /// attribute is applied to. This name is used if the <see cref="SnapshotSettings.SnapshotName"/> property
        /// is not otherwise explicitly specified.
        /// </summary>
        public string SnapshotName { get; set; }
    }
}
