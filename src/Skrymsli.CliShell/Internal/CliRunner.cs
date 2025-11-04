using CommandLine;
using CommandLine.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Skrymsli.CliShell.Internal
{
    public sealed class CliRunner(IServiceScopeFactory scopeFactory, ICommandLineArguments args)
    {
        public Task<int> Run()
        {
            int retval = -1;
            try
            {
                using var scope = scopeFactory.CreateScope();
                var verbs = scope.ServiceProvider.GetRequiredService<IEnumerable<IRunnableTask>>();

                var concreteTypes = verbs.Select(v => v.GetType()).ToArray();
                var parser = new Parser();
                var result = parser.ParseArguments(args.Arguments, concreteTypes);


                result.WithParsed(async options =>
                {
                    var runnable = options as IRunnableTask;
                    if (runnable != null)
                    {
                        retval = await runnable.Execute(scope, CancellationToken.None);
                    }
                });
                result.WithNotParsed(errs =>
                {
                    // Handle errors
                    var helpText = HelpText.AutoBuild(result, h =>
                    {
                        // You can customize the help text here, e.g., add header/footer
                        return HelpText.DefaultParsingErrorsHandler(result, h);
                    },
                    errs => errs); // Use default error handler for formatting errors in help

        Console.WriteLine(helpText);
                    retval = -1;
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unhandled exception: {ex.Message}");
                retval = -1;
            }

            return Task.FromResult(retval);
        }
    }
}
