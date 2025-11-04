using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skrymsli.CliShell.Internal
{
    public interface IRunnableTask
    {
        Task<int> Execute(IServiceScope serviceScope, CancellationToken cancellationToken);
    }
}
