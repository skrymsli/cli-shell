# CLI Shell

This project provides boilerplate for a command-line interface. 
Command line arguments are parsed by the CommandLineParser library. 
You add Tasks that correspond to the Verbs supported by your CLI program. 
The program is set up using standard .NET host building and dependency injection patterns.
This should be a Visual Studio template, but I'm not there yet.

## Getting Started

Here's the implementation of Main. You set up the configuration and register services and return the result of Run():

```
using Skrymsli.CliShell;

// This line will register all the Tasks in the assembly containing the type Program
CliShell.Create<Program>(args)
    .WithConfiguration(config =>
    {
        // Set up your IConfiguration sources here
        config
            .AddJsonFile("appsettings.json")
            .AddUserSecrets<Program>();
    })
    .WithServices((services, config) =>
    {
        // Register your services and dependencies here
        services.AddScoped<ITestService, TestService>();
    })
    // Run the program
    .Run();
```

This sets up a basic CLI application and automatically integrates with CommandLineParser. You can customize the configuration and services as usual.

## Defining Tasks

Here's a simple example of a Task that maps to a CLI verb. You implement IRunnableTask and decoate the class with the Verb attribute from CommandLineParser.
This class will be automatically registered and mapped to the "test" verb in the CLI.

```
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
```

The implementation of ITestService looks like this:

```
    internal sealed class TestService : ITestService
    {
        public string GetMessage() => "Hello from the test service.";
    }
```

This task can be run from the command line like this:

`dotnet run -- test -e ":)"`

And it will output:

```
Hello from the test service. :)
```

