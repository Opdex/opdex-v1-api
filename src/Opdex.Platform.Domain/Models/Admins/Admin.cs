using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Domain.Models.Admins
{
    public class Admin
    {
        public Admin(long id, Address address)
        {
            if (id < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "Id must be greater than 0.");
            }

            if (address == Address.Empty)
            {
                throw new ArgumentException("Address must be provided.", nameof(address));
            }

            Id = id;
            Address = address;
        }

        public long Id { get; }

        public Address Address { get; }
    }
}
