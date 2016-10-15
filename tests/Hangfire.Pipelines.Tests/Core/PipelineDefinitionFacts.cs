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
            var storage = Substitute.For<IPipelineStorage>();
            var executor = Substitute.For<IStepExecutor>();
            var steps = new List<IExpressionContainer>();

            // Act
            var definition = new PipelineDefinition<object>(storage, executor, steps);

            // Assert
            definition.Storage.Should().Be(storage);
            definition.Executor.Should().Be(executor);
            definition.Steps.Should().BeSameAs(steps);
        }

        [Fact]
        public void AddStep_creates_expression_container()
        {
            var storage = Substitute.For<IPipelineStorage>();
            var executor = Substitute.For<IStepExecutor>();
            var steps = new List<IExpressionContainer>();

            var definition = new PipelineDefinition<object>(storage, executor, steps);

            // Act
            var expression = Autofixture.Create<Expression<Action<PipelineTask>>>();
            definition.AddStep(expression);

            // Assert
            var subject = steps.Should().ContainSingle().Subject;

            var pipelineId = Autofixture.Create<Guid>();
            subject.StartNew(executor, pipelineId);
            executor.Received().RunNew(expression, pipelineId);

            var parrentId = Autofixture.Create<string>();
            subject.StartContinuation(executor, pipelineId, parrentId);
            executor.Received().RunContinuation(expression, pipelineId, parrentId);
        }

        [Fact]
        public void CreateExecutor_should_create_a_PipelineExecutor()
        {
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
            result.Storage.Should().Be(storage);
            result.Executor.Should().Be(executor);
            result.Steps.Should().BeEquivalentTo(steps);
        }
    }

    public class PipelineTask : IPipelineTask<object>
    {
        public IPipelineContext<object> PipelineContext { get; set; }
    }
}