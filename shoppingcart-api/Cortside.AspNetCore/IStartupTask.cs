using System.Threading;
using System.Threading.Tasks;

namespace Cortside.AspNetCore
{
    public interface IStartupTask
    {
        Task ExecuteAsync(CancellationToken cancellationToken = default);
    }
}
