using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Cortside.AspNetCore
{
    public static class HostExtensions
    {
        /// <summary>
        /// Runs all of the startup tasks and then starts the host
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public static async Task RunWithTasksAsync(this IHost host, CancellationToken cancellationToken = default)
        {
            // Execute all the tasks
            foreach (var startupTask in host.Services.GetServices<IStartupTask>())
            {
                await startupTask.ExecuteAsync(cancellationToken).ConfigureAwait(false);
            }

            // Start the tasks as normal
            await host.RunAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
