using System;
using MediatR;
using Opdex.Platform.Domain.Models.Addresses;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Addresses
{
    public class PersistAddressStakingCommand : IRequest<ulong>
    {
        public PersistAddressStakingCommand(AddressStaking addressStaking)
        {
            AddressStaking = addressStaking ?? throw new ArgumentNullException(nameof(addressStaking));
        }

        public AddressStaking AddressStaking { get; }
    }
}
