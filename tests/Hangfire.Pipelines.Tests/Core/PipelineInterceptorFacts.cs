using System;

using FluentAssertions;

using Hangfire.Pipelines.Core;
using Hangfire.Pipelines.Models;
using Hangfire.Pipelines.Storage;

using NSubstitute;

using Ploeh.AutoFixture;

using Xunit;

namespace Hangfire.Pipelines.Tests.Core
{
    public class PipelineInterceptorFacts
    {
        private static readonly Fixture Autofixture = new Fixture();

        private readonly IPipelineStorage _storage = Substitute.For<IPipelineStorage>();

        [Fact]
        public void On_SetUpContext_and_not_a_pipeline_task_do_nothing()
        {
            var step = Autofixture.Create<object>();
            var interceptor = new PipelineInterceptor();

            // Act
            interceptor.SetUpContext(typeof(object), step, () => default(Guid), _storage);

            // Assert
        }

        [Fact]
        public void On_SetUpContext_set_pipeline_context()
        {
            var step = Substitute.ForPartsOf<MockStep>();
            var interceptor = new PipelineInterceptor();

            // Act
            interceptor.SetUpContext(typeof(MockStep), step, () => default(Guid), _storage);

            // Assert
            step.PipelineContext.Should().NotBeNull();
        }

        [Fact]
        public void On_SetUpContext_set_pipeline_id()
        {
            var step = Substitute.ForPartsOf<MockStep>();
            var pipelineId = Autofixture.Create<Guid>();
            var interceptor = new PipelineInterceptor();

            // Act
            interceptor.SetUpContext(typeof(MockStep), step, () => pipelineId, _storage);

            // Assert
            step.PipelineContext.PipelineId.Should().Be(pipelineId);
        }

        [Fact]
        public void On_SetUpContext_set_pipeline_storage()
        {
            var step = Substitute.ForPartsOf<MockStep>();
            var interceptor = new PipelineInterceptor();

            // Act
            interceptor.SetUpContext(typeof(MockStep), step, () => default(Guid), _storage);

            // Assert
            step.PipelineContext.Storage.Should().Be(_storage);
        }

        [Fact]
        public void On_SetUpContext_load_entity_from_storage()
        {
            var step = Substitute.For<MockStep>();
            var pipelineId = Autofixture.Create<Guid>();

            var interceptor = new PipelineInterceptor();

            // Act
            interceptor.SetUpContext(typeof(MockStep), step, () => pipelineId, _storage);

            // Assert
            _storage.Received().Get<string>(pipelineId, "PipelineEntity");
        }

        [Fact]
        public void On_TearDownContext_save_entity()
        {
            var result = Autofixture.Create<object>();
            var pipelineId = Autofixture.Create<Guid>();

            var interceptor = new PipelineInterceptor();

            // Act
            interceptor.TearDownContext(result, _storage, pipelineId);

            // Assert
            _storage.Received().Set(pipelineId, "PipelineEntity", result);
        }
    }

    public class MockStep : IPipelineTask<string>
    {
        public virtual IPipelineContext<string> PipelineContext { get; set; }
    }
}