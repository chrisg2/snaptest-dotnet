using System;

namespace SnapTest
{
    /// <summary>
    /// Base class for exceptions thrown by the SnapTest library.
    /// </summary>
    public class SnapTestException: Exception
    {
        public SnapTestException() { }
        public SnapTestException(string message) : base(message) { }
        public SnapTestException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// Exception thrown by the SnapTest library when an error related to parsing and interpreting snapshot values or other strings occurs.
    /// </summary>
    public class SnapTestParseException: SnapTestException
    {
        public SnapTestParseException() { }
        public SnapTestParseException(string message) : base(message) { }
        public SnapTestParseException(string message, Exception innerException) : base(message, innerException) { }
    }
}
