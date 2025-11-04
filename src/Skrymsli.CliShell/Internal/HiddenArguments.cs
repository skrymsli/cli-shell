namespace Skrymsli.CliShell.Internal
{
    internal sealed class ArgumentsProvider(string[] args) : ICommandLineArguments
    {
        public string[] Arguments => args;
    }

    public interface ICommandLineArguments
    {
        public string[] Arguments { get; }
    }
}
