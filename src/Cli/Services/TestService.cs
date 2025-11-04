namespace CliShell.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal sealed class TestService : ITestService
    {
        public string GetMessage() => "Hello from the test service.";
    }

    internal interface ITestService
    {
        string GetMessage();
    }
}
