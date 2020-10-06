namespace SnapTest
{
    public class SnapshotContext
    {
        public Snapshot Snapshot;
        public string TestName;
        public object Actual;
        public bool ExpectedValueKnown { get; private set; }
        object _expected;
        public object Expected {
            get {
                if (!ExpectedValueKnown)
                    throw new System.InvalidOperationException("Expected cannot be accessed until it has been explicitly set");

                return _expected;
            }

            set {
                _expected = value;
                ExpectedValueKnown = true;
            }
        }

        public SnapshotContext(string testName = null) { TestName = testName; }

        /// <summary>
        /// Outputs a message while doing a snapshot comparison. This is used to output messages that may be of interest to somebody
        /// monitoring snapshot activity, such as when snapshot files are created.
        /// </summary>
        /// <remarks>
        /// This method calls <ref cref="Message(string")/>, which does nothing by default. The intention is that that method
        /// will be overridden in derived classes to write messages to an appropriate destination - such as the output stream
        /// for the test library that is being used.
        /// </remarks>
        public void Message(string message, params object[] args) { Message(string.Format(message, args)); }

        /// <summary>
        /// Outputs a message while doing a snapshot comparison. This is used to output messages that may be of interest to somebody
        /// monitoring snapshot activity, such as when snapshot files are created.
        /// </summary>
        /// <remarks>
        /// The default implementation of this method does nothing. The intention is that the method will be overridden
        /// in derived classes to write messages to an appropriate destination - such as the output stream for the test
        /// library that is being used.
        /// </remarks>
        public virtual void Message(string message) { }
    }
}
