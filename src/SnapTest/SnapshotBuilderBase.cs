using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

using SnapTest.Middleware;

namespace SnapTest
{
    /// <summary>
    /// </summary>
    public abstract class SnapshotBuilderBase: SnapshotMiddlewarePipeline
    {
        #region Static and instance fields
        private List<Action<FileStorageOptions>> _fileStorageOptionBuilders = new List<Action<FileStorageOptions>>();
        private List<Action<JsonSerializerOptions>> _jsonSerializerOptionBuilders = new List<Action<JsonSerializerOptions>>();
        #endregion

        #region Properties
        public string SnapshotDirectoryTail = "_snapshots";
        #endregion

        #region Constructors
        public SnapshotBuilderBase()
            => WithFileStorageOptions(_ => _.SnapshotDirectory = GetSnapshotDirectoryFromStackTrace());
        #endregion

        #region Methods
        public FileStorageOptions BuildFileStorageOptions()
        {
            var options = new FileStorageOptions();
            BuildFileStorageOptions(options);
            return options;
        }

        public JsonSerializerOptions BuildJsonOptions()
        {
            var options = new JsonSerializerOptions();
            BuildJsonOptions(options);
            return options;
        }

        public SnapshotBuilderBase WithFileStorageOptions(Action<FileStorageOptions> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            _fileStorageOptionBuilders.Add(action);
            return this;
        }

        public SnapshotBuilderBase WithJsonOptions(Action<JsonSerializerOptions> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            _jsonSerializerOptionBuilders.Add(action);
            return this;
        }

        public Snapshot Build()
            => new Snapshot().Use(MiddlewarePipeline ?? BuildDefaultMiddlewarePipeline());

        public virtual ISnapshotMiddleware BuildDefaultMiddlewarePipeline()
        {
            var pipe = new SnapshotMiddlewarePipeline();

            UseFileStorageReadingMiddleware(pipe);
            UseJsonSerializerMiddlware(pipe);
            UseStringComparatorMiddleware(pipe);
            UseFileStorageWritingMiddleware(pipe);

            return pipe.MiddlewarePipeline;
        }

        public void UseFileStorageReadingMiddleware(SnapshotMiddlewarePipeline pipeline) => pipeline.Use<FileStorageReadingMiddleware>(_ => BuildFileStorageOptions(_.Options));
        public void UseFileStorageReadingMiddleware() => UseFileStorageReadingMiddleware(this);

        public void UseJsonSerializerMiddlware(SnapshotMiddlewarePipeline pipeline) => pipeline.Use<JsonSerializerMiddlware>(_ => BuildJsonOptions(_.Options));
        public void UseJsonSerializerMiddlware() => UseJsonSerializerMiddlware(this);

        public void UseStringComparatorMiddleware(SnapshotMiddlewarePipeline pipeline) => pipeline.Use<StringComparatorMiddleware>();
        public void UseStringComparatorMiddleware() => UseStringComparatorMiddleware(this);

        public void UseFileStorageWritingMiddleware(SnapshotMiddlewarePipeline pipeline) => pipeline.Use<FileStorageWritingMiddleware>(_ => BuildFileStorageOptions(_.Options));
        public void UseFileStorageWritingMiddleware() => UseFileStorageWritingMiddleware(this);

        protected string GetSnapshotDirectoryFromStackTrace()
            => Path.Combine(
                (
                    from frame in new StackTrace(1, true).GetFrames()
                    let method = frame.GetMethod()
                    where method != null
                    let syncMethod = (IsAsyncMethod(method) ? FindAsynchMethodBase(method) : method)
                    where IsTestMethod(syncMethod)
                    select Path.GetDirectoryName(frame.GetFileName())
                ).FirstOrDefault() ?? string.Empty,
                SnapshotDirectoryTail ?? string.Empty
            );

        protected abstract bool IsTestMethod(MethodBase method);
        #endregion

        #region Overrides/New'ed methods
        public new SnapshotBuilderBase Use(ISnapshotMiddleware middleware)
        {
            base.Use(middleware);
            return this;
        }

        public new SnapshotBuilderBase Use<T>(Action<T> initializer = null) where T : ISnapshotMiddleware, new()
        {
            base.Use<T>(initializer);
            return this;
        }

        public new SnapshotBuilderBase Use(Func<SnapshotContext, bool> process)
        {
            base.Use(process);
            return this;
        }

        public new SnapshotBuilderBase Use(Action<SnapshotContext> process)
        {
            base.Use(process);
            return this;
        }
        #endregion

        #region Internal helper methods
        protected void BuildFileStorageOptions(FileStorageOptions options)
            => _fileStorageOptionBuilders.ForEach(_ => _(options));

        protected void BuildJsonOptions(JsonSerializerOptions options)
            => _jsonSerializerOptionBuilders.ForEach(_ => _(options));

        private static bool IsAsyncMethod(MemberInfo method)
            => typeof(IAsyncStateMachine).IsAssignableFrom(method.DeclaringType);

        private static MethodBase FindAsynchMethodBase(MemberInfo method)
        {
            Type methodDeclaringType = method.DeclaringType;
            Type classDeclaringType = methodDeclaringType?.DeclaringType;

            if (classDeclaringType == null)
                return null;

            return (
                from methodInfo in classDeclaringType.GetMethods()
                let stateMachineAttribute = methodInfo.GetCustomAttribute<AsyncStateMachineAttribute>()
                where stateMachineAttribute != null && stateMachineAttribute.StateMachineType == methodDeclaringType
                select methodInfo
            ).SingleOrDefault();
        }
        #endregion
   }
}
