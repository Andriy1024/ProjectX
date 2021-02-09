using ProjectX.Common;

namespace ProjectX.Identity.Application
{
    public class FindUserQuery : IQuery<UserDto>
    {
        public FindUserQuery(long id)
        {
            Id = id;
        }

        public long Id { get; }
    }
}
