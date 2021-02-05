using ProjectX.Common;
using System.Collections.Generic;

namespace ProjectX.Identity.Domain
{
    public sealed class AddressObject : ValueObject
    {
        public string Country { get; private set; }
        public string City { get; private set; }
        public string Address { get; private set; }

        /// <summary>
        /// For ORM
        /// </summary>
        private AddressObject() {}

        public AddressObject(string country, string city, string address)
        {
            Country = country;
            City = city;
            Address = address;
        }

        protected override IEnumerable<object> GetAtomicValues() =>
            new[]
            {
                Country,
                City,
                Address
            };
    }
}
