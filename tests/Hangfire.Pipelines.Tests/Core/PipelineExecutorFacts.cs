using System;
using System.Collections.Generic;

using Hangfire.Pipelines.Core;
using Hangfire.Pipelines.Executors;
using Hangfire.Pipelines.Storage;

using NSubstitute;

using Ploeh.AutoFixture;

using Xunit;

namespace Hangfire.Pipelines.Tests.Core
{
    public class PipelineExecutorFacts
    {
        private static readonly Fixture AutoFixture = new Fixture();

        [Fact]
        public void First_step_processed_should_be_invoked_via_StartNew()
        {
            var step = Substitute.For<IExpressionContainer>();
            var steps = new List<IExpressionContainer> {step};
            var storage = Substitute.For<IPipelineStorage>();
            var executor = Substitute.For<IStepExecutor>();

            var pipeline = new PipelineExecutor<object>(steps, storage, executor);

            // Act
            pipeline.Process(null);

            // Assert
            step.Received().StartNew(executor, Arg.Any<Guid>());
        }

        [Fact]
        public void Nth_plus_one_step_processed_should_be_invoked_via_StartContinuation()
        {
            var step1 = Substitute.For<IExpressionContainer>();
            var step2 = Substitute.For<IExpressionContainer>();
            var step3 = Substitute.For<IExpressionContainer>();
            var steps = new List<IExpressionContainer> {step1, step2, step3};

            var storage = Substitute.For<IPipelineStorage>();
            var executor = Substitute.For<IStepExecutor>();

            var pipeline = new PipelineExecutor<object>(steps, storage, executor);

            // Act
            pipeline.Process(null);

            // Assert
            step2.Received().StartContinuation(executor, Arg.Any<Guid>(), Arg.Any<string>());
            step3.Received().StartContinuation(executor, Arg.Any<Guid>(), Arg.Any<string>());
        }

        [Fact]
        public void Parrent_id_is_correctly_passed_to_next_step()
        {
            var step1 = Substitute.For<IExpressionContainer>();
            var step2 = Substitute.For<IExpressionContainer>();
            var step3 = Substitute.For<IExpressionContainer>();
            var steps = new List<IExpressionContainer> {step1, step2, step3};

            var storage = Substitute.For<IPipelineStorage>();
            var executor = Substitute.For<IStepExecutor>();

            var id1 = AutoFixture.Create<string>();
            var id2 = AutoFixture.Create<string>();

            step1.StartNew(executor, Arg.Any<Guid>()).Returns(id1);
            step2.StartContinuation(executor, Arg.Any<Guid>(), id1).Returns(id2);

            var pipeline = new PipelineExecutor<object>(steps, storage, executor);

            // Act
            pipeline.Process(null);

            // Assert
            step2.Received().StartContinuation(executor, Arg.Any<Guid>(), id1);
            step3.Received().StartContinuation(executor, Arg.Any<Guid>(), id2);
        }

        [Fact]
        public void Pipeline_id_is_correctly_passed_to_each_step()
        {
            var step1 = Substitute.For<IExpressionContainer>();
            var step2 = Substitute.For<IExpressionContainer>();
            var step3 = Substitute.For<IExpressionContainer>();
            var steps = new List<IExpressionContainer> {step1, step2, step3};

            var storage = Substitute.For<IPipelineStorage>();
            var executor = Substitute.For<IStepExecutor>();

            var pipeline = new PipelineExecutor<object>(steps, storage, executor);

            // Act
            var pipelineId = pipeline.Process(null);

            // Assert
            step1.Received().StartNew(executor, Arg.Is(pipelineId));
            step2.Received().StartContinuation(executor, Arg.Is(pipelineId), Arg.Any<string>());
            step3.Received().StartContinuation(executor, Arg.Is(pipelineId), Arg.Any<string>());
        }

        [Fact]
        public void Before_steps_are_processed_entity_should_be_stored()
        {
            var step = Substitute.For<IExpressionContainer>();
            var steps = new List<IExpressionContainer> {step};
            var storage = Substitute.For<IPipelineStorage>();

            var pipeline = new PipelineExecutor<object>(steps, storage, null);
            var obj = AutoFixture.Create<object>();

            // Act
            var pipelineId = pipeline.Process(obj);

            // Assert
            storage.Received().Set(pipelineId, "PipelineEntity", obj);
        }
    }
}