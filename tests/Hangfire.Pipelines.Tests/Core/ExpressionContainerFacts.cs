using System;

using Hangfire.Pipelines.Core;
using Hangfire.Pipelines.Executors;

using NSubstitute;

using Ploeh.AutoFixture;

using Xunit;

namespace Hangfire.Pipelines.Tests.Core
{
    public class ExpressionContainerFacts
    {
        private static readonly Fixture Autofixture = new Fixture();

        [Fact]
        public void StartNew_should_execute_startNew_function()
        {
            var expression = Autofixture.Create<string>();
            var recorder = Substitute.For<IRecorderFixture>();
            var executor = Substitute.For<IStepExecutor>();
            var pipelineId = Autofixture.Create<Guid>();

            IExpressionContainer container = new ExpressionContainer(expression, recorder.StartNew, recorder.StartContinuation);

            // Act
            container.StartNew(executor, pipelineId);

            // Assert
            recorder.Received().StartNew(executor, pipelineId);
        }

        [Fact]
        public void StartContinuation_should_execute_startContinuation_function()
        {
            var expression = Autofixture.Create<string>();
            var recorder = Substitute.For<IRecorderFixture>();
            var executor = Substitute.For<IStepExecutor>();
            var pipelineId = Autofixture.Create<Guid>();
            var parrentId = Autofixture.Create<string>();

            IExpressionContainer container = new ExpressionContainer(expression, recorder.StartNew, recorder.StartContinuation);

            // Act
            container.StartContinuation(executor, pipelineId, parrentId);

            // Assert
            recorder.Received().StartContinuation(executor, pipelineId, parrentId);
        }
    }

    public interface IRecorderFixture
    {
        string StartNew(IStepExecutor executor, Guid pipelineId);

        string StartContinuation(IStepExecutor executor, Guid pipelineId, string parrentId);
    }
}