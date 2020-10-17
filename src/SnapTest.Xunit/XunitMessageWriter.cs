using System;

namespace SnapTest.Xunit
{
    /// <summary>
    /// <see cref="IMessageWriter"/> implementation that emits messages using `Console.WriteLine`.
    /// </summary>
    internal class XunitMessageWriter: IMessageWriter
    {
        public void Write(string message) => Console.WriteLine(message);
    }
}
