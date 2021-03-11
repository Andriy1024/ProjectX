using ProjectX.Core.SeedWork;

namespace ProjectX.Core.Setup
{
    public class BaseOptions : IOptions
    {
        public string ApiName { get; set; }

        public string IdentityUrl { get; set; }

        public string ExternalIdentityUrl { get; set; }   
    }
}
