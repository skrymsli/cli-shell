namespace CliShell
{
    using CliShell.Services;
    using CliShell.Tasks;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Serilog;
    using Skrymsli.CliShell.Internal;

    internal sealed class Program
    {
        static async Task<int> Main(string[] args)
        {
            return await Skrymsli.CliShell.CliShell.Create<Program>(args)
                .WithConfiguration(config =>
                {
                    config
                        .AddJsonFile("appsettings.json")
                        .AddUserSecrets<Program>();
                })
                
                .WithServices((services, config) =>
                {
                    // Override the argument provider to filter out some arguments that we
                    // don't want to parse
                    var argumentFilter = new HiddenArguments(args);
                    services.AddSingleton<ICommandLineArguments>(argumentFilter);

                    services.AddSerilog(config =>
                    {
                        argumentFilter.ConfigureLogs(config);
                        config.WriteTo.Console();
                    });

                    services.AddScoped<ITestService, TestService>();
                })
                .Run();
        }
    }

    internal sealed class HiddenArguments : ICommandLineArguments
    {
        public HiddenArguments(string[] args)
        {
            Original = args;
            Arguments = args.Where(a => a != "-v" && a != "--verbose").ToArray();
        }

        public string[] Arguments { get; private set; }

        private string[] Original { get; }

        public void ConfigureLogs(Serilog.LoggerConfiguration config)
        {
            if (Original.Contains("-v") || Original.Contains("--verbose"))
            {
                config.MinimumLevel.Debug();
                config.Enrich.FromLogContext();
            }
            else
            {
                config.MinimumLevel.Error();
            }
        }
    }
}
