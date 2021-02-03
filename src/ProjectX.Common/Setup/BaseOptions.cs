using ProjectX.Common.SeedWork;

namespace ProjectX.Common.Setup
{
    public class BaseOptions : IOptions
    {
        public string ApiName { get; set; }

        public string IdentityUrl { get; set; }
    }
}
