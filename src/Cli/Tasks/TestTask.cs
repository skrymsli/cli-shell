namespace CliShell.Tasks
{
    using CliShell.Services;
    using CommandLine;
    using Microsoft.Extensions.DependencyInjection;
    using Skrymsli.CliShell.Internal;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    [Verb("test", HelpText = "Demonstrates the cli pattern.")]
    internal sealed class TestTask : IRunnableTask
    {
        [Option('e', HelpText = "Provide extra information to the command")]
        public string ExtraInformation { get; set; } = string.Empty;

        public Task<int> Execute(IServiceScope serviceScope, CancellationToken cancellationToken)
        {
            var service = serviceScope.ServiceProvider.GetRequiredService<ITestService>();
            Console.WriteLine(service.GetMessage() + " " + ExtraInformation);
            return Task.FromResult(0);
        }
    }
}
