using Microsoft.AspNetCore.StaticFiles;
using System.IO;

namespace ProjectX.FileStorage.Application.SeedWork
{
    /// <summary>
    /// Collection of helpers for working with files.
    /// </summary>
    public static class FileUtill
    {
        private static readonly FileExtensionContentTypeProvider _contentTypeProvider = new FileExtensionContentTypeProvider();

        public static string DefaultContentType = "application/octet-stream";

        /// <returns>
        /// Returns mime type of the file name, or <see cref="FileUtill.DefaultContentType"/> if mime type was not found.
        /// </returns>
        public static string TryGetContentType(string fileName)
        {
            if (string.IsNullOrEmpty(fileName) || !_contentTypeProvider.TryGetContentType(fileName, out string contentType))
            {
                return DefaultContentType;
            }

            return contentType;
        }

        /// <returns>
        /// Returns extension of the file name, or <see cref="string.Empty"/> if extension not found.
        /// </returns>
        public static string TryGetExtension(string fileName)
        {
            var extension = Path.GetExtension(fileName);

            return string.IsNullOrEmpty(extension)
                 ? string.Empty
                 : extension;
        }

        /// <returns>
        /// Returns random file name.
        /// </returns>
        public static string GenerateFileName(string fileExtension)
        {
            if (string.IsNullOrEmpty(fileExtension))
            {
                return Path.GetRandomFileName();
            }

            return $"{Path.GetRandomFileName()}{fileExtension}";
        }

        /// <returns>
        /// Returns true if the filename contains file extension, otherwise false.
        /// </returns>
        public static bool HasExtension(string fileName) => Path.HasExtension(fileName);
    }
}
