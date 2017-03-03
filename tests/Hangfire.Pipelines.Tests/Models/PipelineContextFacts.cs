using System;

using Hangfire.Pipelines.Models;
using Hangfire.Pipelines.Storage;

using NSubstitute;

using Ploeh.AutoFixture;

using Xunit;

namespace Hangfire.Pipelines.Tests.Models
{
    public class PipelineContextFacts
    {
        private static readonly Fixture Autofixture = new Fixture();

        private readonly IPipelineStorage _storage = Substitute.For<IPipelineStorage>();
        private readonly Guid _pipelineId = Autofixture.Create<Guid>();

        [Fact]
        public void On_Load_load_from_storage()
        {
            IPipelineContext<string> context = new PipelineContext<string>(_storage, _pipelineId);

            // Act
            context.Load();

            // Assert
            _storage.Received().Get<string>(_pipelineId, "PipelineEntity");
        }
    }
}