using System;
using MediatR;
using Opdex.Platform.Domain.Models.Addresses;

namespace Opdex.Platform.Application.Abstractions.Commands.Addresses
{
    public class MakeAddressStakingCommand : IRequest<ulong>
    {
        public MakeAddressStakingCommand(AddressStaking addressStaking)
        {
            AddressStaking = addressStaking ?? throw new ArgumentNullException(nameof(addressStaking));
        }

        public AddressStaking AddressStaking { get; }
    }
}
