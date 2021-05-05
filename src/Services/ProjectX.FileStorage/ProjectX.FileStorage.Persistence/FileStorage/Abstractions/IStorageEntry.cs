namespace ProjectX.FileStorage.Persistence.FileStorage.Abstractions
{
    /// <summary>
    /// Represents file information
    /// </summary>
    public interface IStorageEntry
    {
        /// <summary>
        /// File name (fileName.png etc...)
        /// </summary>
        string Name { get; }

        /// <summary>
        /// File location without name (/avatars/users_23/, /videos/ etc...)
        /// </summary>
        string Location { get; }

        /// <summary>
        /// File extension (.png, .mp4, .js etc...)
        /// </summary>
        string Extension { get; }

        /// <summary>
        /// File MimeType (image/jpeg, video/mp4, text/plain etc...)
        /// </summary>
        string MimeType { get; }

        /// <summary>
        /// File length in bytes
        /// </summary>
        long Size { get; }
    }
}
