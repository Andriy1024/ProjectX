using ProjectX.Core;
using System.Collections.Generic;

namespace ProjectX.Identity.Domain
{
    public sealed class Address : ValueObject
    {
        public string Country { get; private set; }
        public string City { get; private set; }
        public string Street { get; private set; }

        /// <summary>
        /// For ORM
        /// </summary>
        private Address() {}

        public Address(string country, string city, string street)
        {
            Country = country;
            City = city;
            Street = street;
        }

        protected override IEnumerable<object> GetAtomicValues() =>
            new[]
            {
                Country,
                City,
                Street
            };
    }
}
