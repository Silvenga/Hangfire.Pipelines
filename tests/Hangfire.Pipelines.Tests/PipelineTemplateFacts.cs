using System;
using System.Linq.Expressions;

using Hangfire.Pipelines.Core;
using Hangfire.Pipelines.Models;

using NSubstitute;

using Xunit;

namespace Hangfire.Pipelines.Tests
{
    public class PipelineTemplateFacts
    {
        [Fact]
        public void Can_add_step()
        {
            ////var factory = Substitute.For<IExpressionFactory>();
            //var template = new PipelineDefinition<TypeFixture>(factory, null);

            //Expression<Action<StepFixture>> expression = x => x.Instance();

            //// Act
            //template.AddStep(expression);

            // Assert
        }
    }

    public class TypeFixture
    {
    }

    public class StepFixture : IPipelineTask<TypeFixture>
    {
        public void Instance()
        {
        }

        public PipelineContext<TypeFixture> PipelineContext { get; set; }
    }
}