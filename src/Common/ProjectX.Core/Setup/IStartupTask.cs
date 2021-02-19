using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Core
{
    public interface IStartupTask
    {
        Task ExecuteAsync(CancellationToken cancellationToken = default);
    }
}
