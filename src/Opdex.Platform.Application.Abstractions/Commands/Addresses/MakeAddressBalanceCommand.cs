using System;
using MediatR;
using Opdex.Platform.Domain.Models.Addresses;

namespace Opdex.Platform.Application.Abstractions.Commands.Addresses
{
    public class MakeAddressBalanceCommand : IRequest<long>
    {
        public MakeAddressBalanceCommand(AddressBalance addressBalance)
        {
            AddressBalance = addressBalance ?? throw new ArgumentNullException(nameof(addressBalance));
        }

        public AddressBalance AddressBalance { get; }
    }
}
