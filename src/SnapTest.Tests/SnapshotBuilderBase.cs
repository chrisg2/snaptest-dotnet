using NUnit.Framework;
using System;
using System.IO;

using SnapTest.Middleware;

namespace SnapTest.Tests
{
    // Simple non-abstract mock class desrived from SnapshotBuilderBase for testing
    public class SnapshotBuilder : SnapshotBuilderBase
    {
        protected override bool IsTestMethod(System.Reflection.MethodBase method) => false;
    }

    public class SnapshotBuilderBaseTest
    {
        #region Test With*Options methods can be called and have expected effect
        [Test]
        public void WithFileStorageOptions_is_effective()
        {
            var b = new SnapshotBuilder();

            var initialOptions = b.BuildFileStorageOptions();

            // Set a couple of properties
            b.WithFileStorageOptions(_ => _.SnapshotDirectory = "dir");
            b.WithFileStorageOptions(_ => _.SnapshotExtension = ".snapshot");

            var finalOptions = b.BuildFileStorageOptions();

            // Assume initial option state
            Assume.That(initialOptions.SnapshotDirectory, Is.Not.EqualTo("dir"));
            Assume.That(initialOptions.SnapshotExtension, Is.Not.EqualTo(".snapshot"));

            // Assert final option state
            Assert.That(finalOptions.SnapshotDirectory, Is.EqualTo("dir"));
            Assert.That(finalOptions.SnapshotExtension, Is.EqualTo(".snapshot"));
        }

        [Test]
        public void WithJsonOptions_is_effective()
        {
            var b = new SnapshotBuilder();

            var initialOptions = b.BuildJsonOptions();

            // Set a couple of properties to non-default values
            b.WithJsonOptions(_ => _.IgnoreNullValues = true);
            b.WithJsonOptions(_ => _.WriteIndented = true);

            var finalOptions = b.BuildJsonOptions();

            // Assume initial option state
            Assume.That(initialOptions.IgnoreNullValues, Is.False);
            Assume.That(initialOptions.WriteIndented, Is.False);

            // Assert final option state
            Assert.That(finalOptions.IgnoreNullValues, Is.True);
            Assert.That(finalOptions.WriteIndented, Is.True);
        }
        #endregion

        #region Test SnapshotBuilderBase.Use*Middleware methods can be called
        [Test]
        public void Can_UseFileStorageReadingMiddleware()
        {
            var builder = new SnapshotBuilder();

            builder.WithFileStorageOptions(_ => _.ForceSnapshotRefresh = true);
            builder.UseFileStorageReadingMiddleware();
            builder.Use(_ => { Assert.That(_.Expected, Is.EqualTo("value")); return false; });

            Assert.That(builder.MiddlewarePipeline.Process(new SnapshotContext() { Actual = "value" }), Is.False);
        }

        [Test]
        public void Can_UseFileStorageReadingMiddleware_on_pipeline()
        {
            var builder = new SnapshotBuilder();
            var pipeline = new SnapshotMiddlewarePipeline();

            builder.WithFileStorageOptions(_ => _.ForceSnapshotRefresh = true);
            builder.UseFileStorageReadingMiddleware(pipeline);
            pipeline.Use(_ => { Assert.That(_.Expected, Is.EqualTo("value")); return false; });

            Assert.That(pipeline.MiddlewarePipeline.Process(new SnapshotContext() { Actual = "value" }), Is.False);
        }

        [Test]
        public void Can_UseJsonSerializerMiddlware()
        {
            var builder = new SnapshotBuilder();

            builder.UseJsonSerializerMiddlware();
            builder.Use(_ => { Assert.That(_.Actual, Is.EqualTo("null")); return false; });

            Assert.That(builder.MiddlewarePipeline.Process(new SnapshotContext() { Actual = null }), Is.False);
        }

        [Test]
        public void Can_UseJsonSerializerMiddlware_on_pipeline()
        {
            var builder = new SnapshotBuilder();
            var pipeline = new SnapshotMiddlewarePipeline();

            builder.UseJsonSerializerMiddlware(pipeline);
            pipeline.Use(_ => { Assert.That(_.Actual, Is.EqualTo("null")); return false; });

            Assert.That(pipeline.MiddlewarePipeline.Process(new SnapshotContext() { Actual = null }), Is.False);
        }

        [Test]
        public void Can_UseFileStorageWritingMiddleware()
        {
            using var builder = new TempFileSnapshotBuilder();

            builder.WithFileStorageOptions(_ => _.ForceSnapshotRefresh = true);
            builder.UseFileStorageWritingMiddleware();

            builder.BuildAndCompareTo("actual");
            Assert.That(builder.SnapshotFileName, Does.Exist);
            Assert.That(File.ReadAllText(builder.SnapshotFileName), Is.EqualTo("actual" + Environment.NewLine));
        }

        [Test]
        public void Can_UseFileStorageWritingMiddleware_on_pipeline()
        {
            using var builder = new TempFileSnapshotBuilder();
            var pipeline = new SnapshotMiddlewarePipeline();

            builder.WithFileStorageOptions(_ => _.ForceSnapshotRefresh = true);
            builder.UseFileStorageWritingMiddleware(pipeline);

            pipeline.MiddlewarePipeline.Process(new SnapshotContext() { TestName = builder.TestName, Actual = "actual" });
            Assert.That(builder.SnapshotFileName, Does.Exist);
            Assert.That(File.ReadAllText(builder.SnapshotFileName), Is.EqualTo("actual" + Environment.NewLine));
        }
        #endregion

        #region Test various forms of SnapshotBuilderBase.Use can be called
        [Test]
        public void Can_Use_ISnapshotMiddleware()
        {
            bool called = false;

            ISnapshotMiddleware mw = new FunctionCallMiddleware(_ => { called = true; return false; });

            Assert.That(new SnapshotBuilder().Use(mw).MiddlewarePipeline.Process(null), Is.False);
            Assert.That(called, Is.True);
        }

        public void Can_Use_T_without_initializer()
        {
            var context = new SnapshotContext() { Actual = new { item = 42 } };
            new SnapshotBuilder().Use<JsonSerializerMiddlware>().MiddlewarePipeline.Process(context);
            Assert.That(context.Actual, Is.EqualTo("{\n  \"item\": 42\n}".Replace("\n", System.Environment.NewLine)));
        }

        public void Can_Use_T_with_initializer()
        {
            var context = new SnapshotContext() { Actual = new { item = 42 } };
            new SnapshotBuilder().Use<JsonSerializerMiddlware>(_ => _.Options.WriteIndented = false).MiddlewarePipeline.Process(context);
            Assert.That(context.Actual, Is.EqualTo("{\"item\":42}"));
        }

        [Test]
        public void Can_Use_Func()
        {
            bool called = false;
            Assert.That(new SnapshotBuilder().Use(_ => { called = true; return false; }).MiddlewarePipeline.Process(null), Is.False);
            Assert.That(called, Is.True);
        }

        [Test]
        public void Can_Use_Action()
        {
            bool called = false;
            Assert.That(new SnapshotBuilder().Use(_ => { called = true; }).MiddlewarePipeline.Process(null), Is.True);
            Assert.That(called, Is.True);
        }
        #endregion
    }
}
