using System.IO;

namespace ProjectX.FileStorage.Persistence.FileStorage.Models
{
    public class DownloadOptions
    {
        public string Location { get; }

        public DownloadOptions(string location)
        {
            Location = location;
        }

        public DownloadOptions(string location, string name)
        {
            Location = Path.Combine(location, name);
        }
    }
}
