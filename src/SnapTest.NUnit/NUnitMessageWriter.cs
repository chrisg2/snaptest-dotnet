using NUnit.Framework;

namespace SnapTest.NUnit
{
    internal class NUnitMessageWriter: IMessageWriter
    {
        public void Write(string message) => TestContext.Progress.WriteLine(message);
    }
}
