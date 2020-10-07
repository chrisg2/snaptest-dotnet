using NUnit.Framework;
using System.Linq;
using System.Reflection;

using SnapTest.Middleware;

namespace SnapTest.NUnit
{
    /// <summary>
    /// </summary>
    public class SnapshotBuilder: SnapshotBuilderBase
    {
        public override ISnapshotMiddleware BuildDefaultMiddlewarePipeline()
        {
            var pipe = new SnapshotMiddlewarePipeline();
            UseFileStorageReadingMiddleware(pipe);
            UseJsonSerializerMiddlware(pipe);
            AddNUnitComparatorMiddleware(pipe);
            UseFileStorageWritingMiddleware(pipe);

            return pipe.MiddlewarePipeline;
        }

        public void AddNUnitComparatorMiddleware(SnapshotMiddlewarePipeline pipeline)
            => pipeline.Use<NUnitComparatorMiddleware>();

        protected override bool IsTestMethod(MethodBase method)
            =>  method.GetCustomAttributes<TestAttribute>().Any()
                || method.GetCustomAttributes<TestCaseAttribute>().Any()
                || method.GetCustomAttributes<TestCaseSourceAttribute>().Any()
                || method.GetCustomAttributes<TheoryAttribute>().Any()
            ;
    }
}
