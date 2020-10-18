using NUnit.Framework;

namespace SnapTest.NUnit
{
    /// <summary>
    /// <see cref="IMessageWriter"/> implementation that emits messages using NUnit's <see cref="TestContext.Progress"/>.
    /// </summary>
    internal class NUnitMessageWriter: IMessageWriter
    {
        /// <inheritdoc/>
        public void Write(string message) => TestContext.Progress.WriteLine(message);
    }
}
