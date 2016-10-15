﻿using System;
using System.Linq;

using Hangfire.Pipelines.Models;
using Hangfire.Pipelines.Storage;

using JetBrains.Annotations;

namespace Hangfire.Pipelines.Core
{
    public class PipelineInterceptor
    {
        private readonly IPipelineStorage _storage;

        public PipelineInterceptor(IPipelineStorage storage)
        {
            _storage = storage;
        }

        public void SetUpContext(Type jobType, object task, Func<Guid> getPipelineId)
        {
            var pipelineTaskType = GetPipelineTaskType(jobType);
            if (pipelineTaskType == null)
            {
                return;
            }

            var id = getPipelineId();

            var pipelineContext = CreateContext(pipelineTaskType.GenericTypeArguments, _storage, id);
            SetContext(jobType, task, pipelineContext);
            pipelineContext.Load();
        }

        public void TearDownContext(Type jobType, object task)
        {
            var pipelineTaskType = GetPipelineTaskType(jobType);
            if (pipelineTaskType == null)
            {
                return;
            }

            var pipelineContext = GetContext(jobType, task);
            pipelineContext.Save();
        }

        [CanBeNull]
        private Type GetPipelineTaskType(Type jobType)
        {
            return jobType.GetInterfaces().SingleOrDefault(x =>
                               x.IsGenericType &&
                               x.GetGenericTypeDefinition() == typeof(IPipelineTask<>));
        }

        private void SetContext(Type jobType, object task, IPipelineContext pipelineContext)
        {
            var method = jobType.GetProperty(nameof(IPipelineTask<object>.PipelineContext));
            method.SetValue(task, pipelineContext);
        }

        private IPipelineContext GetContext(Type jobType, object task)
        {
            var method = jobType.GetProperty(nameof(IPipelineTask<object>.PipelineContext));
            return (IPipelineContext) method.GetValue(task);
        }

        private IPipelineContext CreateContext(Type[] typeArgs, [NotNull] IPipelineStorage storage, Guid pipelineId)
        {
            var type = typeof(PipelineContext<>);
            var genericType = type.MakeGenericType(typeArgs);
            var instance = Activator.CreateInstance(genericType, storage, pipelineId);
            return (IPipelineContext) instance;
        }
    }
}