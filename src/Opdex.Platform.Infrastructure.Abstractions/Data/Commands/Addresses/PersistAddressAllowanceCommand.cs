using System;
using MediatR;
using Opdex.Platform.Domain.Models.Addresses;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Addresses
{
    public class PersistAddressAllowanceCommand : IRequest<long>
    {
        public PersistAddressAllowanceCommand(AddressAllowance addressAllowance)
        {
            AddressAllowance = addressAllowance ?? throw new ArgumentNullException(nameof(addressAllowance));
        }
        
        public AddressAllowance AddressAllowance { get; }
    }
}