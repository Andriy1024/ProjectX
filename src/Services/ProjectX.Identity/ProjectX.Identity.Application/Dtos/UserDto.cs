using System.Collections.Generic;

namespace ProjectX.Identity.Application
{
    public class UserDto
    {
        public long Id { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public bool EmailAlreadyConfirmed { get; set; }

        public AddressDto Address { get;set; }

        public IEnumerable<RoleDto> Roles { get; set; }
    }
}
