namespace SnapTest
{
    public interface IMessageWriter
    {
        /// <summary>
        /// Emit an informational message related to snapshot processing. Messages give insight into snapshot files that are created and updated.
        /// </summary>
        /// <remarks>
        /// An IMessageWriter can be specified for snapshot processing in the <see cref="SnapshotSettings.MessageWriter"/> property
        /// of the SnapshotSettings object passed to <see cref="Snapshot.CompareTo"/>.
        /// No message writer is configured by default, and as such any messages are simply discarded.
        /// </remarks>
        /// <param name="message">The text of the message to be written.</param>
        void Write(string message);
    }
}
