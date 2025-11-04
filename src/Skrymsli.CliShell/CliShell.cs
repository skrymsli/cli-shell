namespace Skrymsli.CliShell
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Skrymsli.CliShell.Internal;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class CliShell
    {
        public static IConfigureShell Create<T>(string[] args)
        {
            var interfaceType = typeof(IRunnableTask);
            var assembly = typeof(T).Assembly;

            var taskTypes = assembly.GetTypes()
                .Where(type => type.IsClass && !type.IsAbstract &&
                           interfaceType.IsAssignableFrom(type) &&
                           !type.IsGenericTypeDefinition) // Exclude open generic types unless needed
                .ToList();
            return new ConfigureShell(args, taskTypes);
        }
    }

    internal sealed class ConfigureShell(string[] args, List<Type> taskTypes) : RunCli(args), IConfigureShell, IRegisterServices
    {
        public IRegisterServices WithConfiguration(Action<IConfigurationBuilder> configure)
        {
            configure(Builder.Configuration);
            return this;
        }

        public IRunCli WithServices(Action<IServiceCollection, IConfiguration> configure)
        {
            foreach (var type in taskTypes)
            {
                Builder.Services.AddScoped(typeof(IRunnableTask), type);
            }

            configure(Builder.Services, Builder.Configuration);
            return this;
        }
    }

    internal class RunCli : IRunCli
    {
        protected ICommandLineArguments CommandLineArgumentsProvider { get; set; }

        public RunCli(string[] args)
        {
            CommandLineArgumentsProvider = new ArgumentsProvider(args);
            Builder = Host.CreateApplicationBuilder(CommandLineArgumentsProvider.Arguments);
            Builder.Services.AddSingleton<ICommandLineArguments>(CommandLineArgumentsProvider);
        }

        protected HostApplicationBuilder Builder { get; private set; }

        public async Task<int> Run()
        {
            Builder.Services.AddScoped<CliRunner>();

            var host = Builder.Build();
            var runner = host.Services.GetRequiredService<CliRunner>();
            return await runner.Run();
        }
    }

    public interface IRunCli
    {
        Task<int> Run();
    }

    public interface IConfigureShell : IRunCli
    {
        IRegisterServices WithConfiguration(Action<IConfigurationBuilder> configure);
    }

    public interface IRegisterServices : IRunCli
    {
        IRunCli WithServices(Action<IServiceCollection, IConfiguration> configure);
    }
}
