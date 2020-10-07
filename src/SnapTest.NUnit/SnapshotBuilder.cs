﻿using NUnit.Framework;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

using SnapTest.Middleware;

namespace SnapTest.NUnit
{
    /// <summary>
    /// </summary>
    public class SnapshotBuilder: SnapshotBuilderBase
    {
        public SnapshotBuilder()
        {
            WithFileStorageOptions(_ => _.SnapshotDirectory = GetSnapshotDirectoryFromStackTrace());
        }

        public string SnapshotDirectoryTail = "_snapshots";

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

        #region Helper methods
        private string GetSnapshotDirectoryFromStackTrace()
            => Path.Combine(
                new StackTrace(true).GetFrames()
                    .Where(_ => IsNUnitTestMethod(_.GetMethod()) || IsNUnitTestMethod(FindAsynchMethodBase(_.GetMethod())))
                    .Select(_ => Path.GetDirectoryName(_.GetFileName()))
                    .FirstOrDefault()
                    ?? string.Empty,
                SnapshotDirectoryTail ?? string.Empty
            );

        private bool IsNUnitTestMethod(MethodBase method)
        {
            return
                method != null && (
                    method.GetCustomAttributes<TestAttribute>().Any()
                    || method.GetCustomAttributes<TestCaseAttribute>().Any()
                    || method.GetCustomAttributes<TestCaseSourceAttribute>().Any()
                    || method.GetCustomAttributes<TheoryAttribute>().Any()
                );
        }

        private static MethodBase FindAsynchMethodBase(MemberInfo method)
        {
            Type methodDeclaringType = method?.DeclaringType;
            Type classDeclaringType = methodDeclaringType?.DeclaringType;

            if (classDeclaringType == null)
                return null;

            return (
                from methodInfo in classDeclaringType.GetMethods()
                let stateMachineAttribute = methodInfo.GetCustomAttribute<System.Runtime.CompilerServices.AsyncStateMachineAttribute>()
                where stateMachineAttribute != null && stateMachineAttribute.StateMachineType == methodDeclaringType
                select methodInfo
            ).SingleOrDefault();
        }
        #endregion
    }
}
