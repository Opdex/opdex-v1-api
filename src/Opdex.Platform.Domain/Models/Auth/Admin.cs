using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Domain.Models.Auth;

public class Admin
{
    public Admin(ulong id, Address address)
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

    public ulong Id { get; }

    public Address Address { get; }
}
