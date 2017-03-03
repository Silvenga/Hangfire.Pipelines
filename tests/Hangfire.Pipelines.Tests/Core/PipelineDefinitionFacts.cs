using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using FluentAssertions;

using Hangfire.Pipelines.Core;
using Hangfire.Pipelines.Executors;
using Hangfire.Pipelines.Models;
using Hangfire.Pipelines.Storage;

using NSubstitute;

using Ploeh.AutoFixture;

using Xunit;

namespace Hangfire.Pipelines.Tests.Core
{
    public class PipelineDefinitionFacts
    {
        private static readonly Fixture Autofixture = new Fixture();

        [Fact]
        public void Ctor_sets_properties()
        {
            var storage = Substitute.For<CreatePipelineStorage>();
            var executor = Substitute.For<CreateStepExecutor>();

            // Act
            var definition = new PipelineDefinition<object>(storage, executor);

            // Assert
            definition.StorageDelegate.Should().Be(storage);
            definition.ExecutorDelegate.Should().Be(executor);
        }

        [Fact]
        public void AddStep_creates_expression_container_with_step_name()
        {
            var storage = Substitute.For<IPipelineStorage>();
            var executor = Substitute.For<IStepExecutor>();
            var steps = new List<IExpressionContainer>();
            var pipelineName = Autofixture.Create<string>();
            var stepName = Autofixture.Create<string>();

            var definition = new PipelineDefinition<object>(storage, executor, steps);

            // Act
            var expression = Autofixture.Create<Expression<Func<PipelineTask, object>>>();
            definition.SetName(pipelineName).AddStep(expression, stepName);

            // Assert
            var subject = steps.Should().ContainSingle().Subject;

            var pipelineId = Autofixture.Create<Guid>();

            subject.StartNew(executor, pipelineId);
            executor.Received().RunNew(expression, pipelineId, pipelineName, stepName);

            var parrentId = Autofixture.Create<string>();
            subject.StartContinuation(executor, pipelineId, parrentId);
            executor.Received().RunContinuation(expression, pipelineId, parrentId, pipelineName, stepName);
        }

        [Fact]
        public void AddStep_creates_expression_container_with_default_step_name()
        {
            var storage = Substitute.For<IPipelineStorage>();
            var executor = Substitute.For<IStepExecutor>();
            var steps = new List<IExpressionContainer>();
            var pipelineName = Autofixture.Create<string>();

            var definition = new PipelineDefinition<object>(storage, executor, steps);

            // Act
            var expression = Autofixture.Create<Expression<Func<PipelineTask, object>>>();
            definition.SetName(pipelineName).AddStep(expression);

            // Assert
            var subject = steps.Should().ContainSingle().Subject;

            var pipelineId = Autofixture.Create<Guid>();

            const string defaultName = "Step 0";

            subject.StartNew(executor, pipelineId);
            executor.Received().RunNew(expression, pipelineId, pipelineName, defaultName);

            var parrentId = Autofixture.Create<string>();
            subject.StartContinuation(executor, pipelineId, parrentId);
            executor.Received().RunContinuation(expression, pipelineId, parrentId, pipelineName, defaultName);
        }

        [Fact]
        public void CreateExecutor_should_create_a_PipelineExecutor()
        {
            var pipelineId = Autofixture.Create<Guid>();

            var storage = Substitute.For<IPipelineStorage>();
            var executor = Substitute.For<IStepExecutor>();
            var steps = new List<IExpressionContainer>
            {
                Substitute.For<IExpressionContainer>(),
                Substitute.For<IExpressionContainer>()
            };

            var definition = new PipelineDefinition<object>(storage, executor, steps);

            // Act
            var result = definition.CreateExecutor();

            // Assert
            definition.StorageDelegate.Invoke(pipelineId).Should().Be(storage);
            definition.ExecutorDelegate.Invoke(pipelineId).Should().Be(executor);
            result.Steps.Should().BeEquivalentTo(steps);
        }
    }

    public class PipelineTask : IPipelineTask<object>
    {
        public IPipelineContext<object> PipelineContext { get; set; }
    }
}