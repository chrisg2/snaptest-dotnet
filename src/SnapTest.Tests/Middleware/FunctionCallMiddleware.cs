using NUnit.Framework;

using SnapTest.Middleware;

namespace SnapTest.Tests
{
    public class FunctionCallMiddlewareTest
    {
        [Test]
        public void FunctionaCallMiddleware_functions_called_correctly()
        {
            int counter = 0;
            int call1Counter = -1;
            int call2Counter = -1;
            int call3Counter = -1;

            var pipe = new SnapshotMiddlewarePipeline()
                .Use(new FunctionCallMiddleware(_ => { call1Counter = counter++; return true; }))
                .Use(new FunctionCallMiddleware(_ => { call2Counter = counter++; return false; }))
                .Use(new FunctionCallMiddleware(_ => { call3Counter = counter++; return true; }))
            ;

            // Final result is false as one of the functions returned false
            Assert.That(pipe.MiddlewarePipeline.Process(null), Is.False);

            // All function call middlewares should have been called
            Assert.That(call1Counter, Is.EqualTo(0), "1st function not called in expected order");
            Assert.That(call2Counter, Is.EqualTo(1), "2nd function not called in expected order");
            Assert.That(call3Counter, Is.EqualTo(2), "3rd function not called in expected order");
        }
    }
}
