using System.Threading.Tasks;

using Hangfire.Pipelines.Models;

namespace Hangfire.Pipelines.Tests.Models
{
    public class TestTasks
    {
        public class One : IPipelineTask<string>
        {
            public IPipelineContext<string> PipelineContext { get; set; }

            public int RunInt(int value)
            {
                return value;
            }

            public Task<int> RunIntAsync()
            {
                return Task.FromResult(1);
            }
        }

        public class Two : IPipelineTask<int>
        {
            public IPipelineContext<int> PipelineContext { get; set; }

            public string RunString()
            {
                var something = PipelineContext.Get<string>("something");
                var lastStep = PipelineContext.Entity;
                return "";
            }

            public Task<string> RunStringAsync(string value)
            {
                return Task.FromResult(value);
            }
        }
    }
}