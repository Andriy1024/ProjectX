using ProjectX.Core.SeedWork;

namespace ProjectX.Core.Setup
{
    public class ConnectionStrings : IOptions
    {
        public string DbConnection { get; set; }
    }
}
