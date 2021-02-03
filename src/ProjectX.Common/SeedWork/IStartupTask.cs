using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Common
{
    public interface IStartupTask
    {
        Task ExecuteAsync(CancellationToken cancellationToken = default);
    }
}
