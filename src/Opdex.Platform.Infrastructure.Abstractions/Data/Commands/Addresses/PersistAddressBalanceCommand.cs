using System;
using MediatR;
using Opdex.Platform.Domain.Models.Addresses;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Addresses
{
    public class PersistAddressBalanceCommand : IRequest<ulong>
    {
        public PersistAddressBalanceCommand(AddressBalance addressAllowance)
        {
            AddressBalance = addressAllowance ?? throw new ArgumentNullException(nameof(addressAllowance));
        }

        public AddressBalance AddressBalance { get; }
    }
}
