using System;
using System.Collections.Generic;
using System.Text.Json;

using SnapTest.Middleware;

namespace SnapTest
{
    /// <summary>
    /// </summary>
    public class SnapshotBuilder: SnapshotMiddlewarePipeline
    {
        #region Static and instance fields
        private List<Action<FileStorageOptions>> _fileStorageOptionBuilders = new List<Action<FileStorageOptions>>();
        private List<Action<JsonSerializerOptions>> _jsonSerializerOptionBuilders = new List<Action<JsonSerializerOptions>>();
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

        public SnapshotBuilder WithFileStorageOptions(Action<FileStorageOptions> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            _fileStorageOptionBuilders.Add(action);
            return this;
        }

        public SnapshotBuilder WithJsonOptions(Action<JsonSerializerOptions> action)
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
        #endregion

        #region Overrides/New'ed methods
        public new SnapshotBuilder Use(ISnapshotMiddleware middleware)
        {
            base.Use(middleware);
            return this;
        }

        public new SnapshotBuilder Use<T>(Action<T> initializer = null) where T : ISnapshotMiddleware, new()
        {
            base.Use<T>(initializer);
            return this;
        }

        public new SnapshotBuilder Use(Func<SnapshotContext, bool> process)
        {
            base.Use(process);
            return this;
        }

        public new SnapshotBuilder Use(Action<SnapshotContext> process)
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
        #endregion
   }
}
