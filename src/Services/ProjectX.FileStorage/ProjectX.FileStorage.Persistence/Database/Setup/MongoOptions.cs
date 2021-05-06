using ProjectX.Core.SeedWork;

namespace ProjectX.FileStorage.Persistence.Database.Setup
{
    public class MongoOptions : IOptions
    {
        public string DatabaseName { get; set; }

        public string[] Collections { get; set; }
    }
}
