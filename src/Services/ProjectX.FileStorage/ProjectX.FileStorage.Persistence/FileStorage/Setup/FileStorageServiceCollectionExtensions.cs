using Microsoft.Extensions.DependencyInjection;
using ProjectX.FileStorage.Application.Interfaces;
using ProjectX.FileStorage.Persistence.FileStorage.Local;

namespace ProjectX.FileStorage.Persistence.FileStorage.Setup
{
    public static class FileStorageServiceCollectionExtensions
    {
        public static IServiceCollection AddFileStorage(this IServiceCollection services)
            => services.AddSingleton<IFileStorage, LocalFileStorage>();
    }
}
