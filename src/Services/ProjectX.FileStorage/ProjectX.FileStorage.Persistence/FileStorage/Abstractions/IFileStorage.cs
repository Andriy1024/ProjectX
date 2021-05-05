using ProjectX.FileStorage.Persistence.FileStorage.Abstractions;
using ProjectX.FileStorage.Persistence.FileStorage.Models;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.FileStorage.Application.Interfaces
{
    public interface IFileStorage
    {
        Task<IStorageEntry> UploadAsync(UploadOptions options, CancellationToken cancellationToken = default);
        Task<Stream> DownloadAsync(DownloadOptions options, CancellationToken cancellationToken = default);
        Task DeleteAsync(DeleteOptions options, CancellationToken cancellationToken = default);
    }
}
