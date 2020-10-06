using System;

namespace SnapTest
{
    public class SnapTestException: Exception
    {
        public SnapTestException() { }
        public SnapTestException(string message) : base(message) { }
        public SnapTestException(string message, Exception innerException) : base(message, innerException) { }
    }
}
