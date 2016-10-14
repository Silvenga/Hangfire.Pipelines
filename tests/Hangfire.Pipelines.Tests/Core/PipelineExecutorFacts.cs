﻿using System;
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

            var pipeline = new PipelineExecutor<object>(steps, storage);

            // Act
            pipeline.Process(null, executor);

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

            var pipeline = new PipelineExecutor<object>(steps, storage);

            // Act
            pipeline.Process(null, executor);

            // Assert
            step2.Received().StartContinuation(executor, Arg.Any<string>(), Arg.Any<Guid>());
            step3.Received().StartContinuation(executor, Arg.Any<string>(), Arg.Any<Guid>());
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
            step2.StartContinuation(executor, id1, Arg.Any<Guid>()).Returns(id2);

            var pipeline = new PipelineExecutor<object>(steps, storage);

            // Act
            pipeline.Process(null, executor);

            // Assert
            step2.Received().StartContinuation(executor, id1, Arg.Any<Guid>());
            step3.Received().StartContinuation(executor, id2, Arg.Any<Guid>());
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

            var pipeline = new PipelineExecutor<object>(steps, storage);

            // Act
            var pipelineId = pipeline.Process(null, executor);

            // Assert
            step1.Received().StartNew(executor, Arg.Is(pipelineId));
            step2.Received().StartContinuation(executor, Arg.Any<string>(), Arg.Is(pipelineId));
            step3.Received().StartContinuation(executor, Arg.Any<string>(), Arg.Is(pipelineId));
        }

        [Fact]
        public void Before_steps_are_processed_entity_should_be_stored()
        {
            var step = Substitute.For<IExpressionContainer>();
            var steps = new List<IExpressionContainer> {step};
            var storage = Substitute.For<IPipelineStorage>();

            var pipeline = new PipelineExecutor<object>(steps, storage);
            var obj = AutoFixture.Create<object>();

            // Act
            var pipelineId = pipeline.Process(obj, null);

            // Assert
            storage.Received().Set(pipelineId, "PipelineEntity", obj);
        }
    }
}