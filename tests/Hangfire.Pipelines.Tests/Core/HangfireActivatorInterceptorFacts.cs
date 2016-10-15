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
    public class HangfireActivatorInterceptorFacts
    {
        private static readonly Fixture Autofixture = new Fixture();

        [Fact]
        public void After_job_has_materialized_set_pipeline_context()
        {
            var storage = Substitute.For<IPipelineStorage>();
            var filter = new HangfireActivatorInterceptor(storage);
            var step = Autofixture.Build<MockStep>().Without(x => x.PipelineContext).Create();
            var pipelineId = Autofixture.Create<Guid>();

            // Act
            filter.SetUpContext(typeof(MockStep), step, () => pipelineId);

            // Assert
            step.PipelineContext.Should().NotBeNull();
        }
    }

    public class MockStep : IPipelineTask<string>
    {
        public IPipelineContext<string> PipelineContext { get; set; }
    }
}