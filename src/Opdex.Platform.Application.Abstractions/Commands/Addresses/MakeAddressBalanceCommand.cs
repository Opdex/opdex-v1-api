using System;
using MediatR;
using Opdex.Platform.Domain.Models.Addresses;

namespace Opdex.Platform.Application.Abstractions.Commands.Addresses
{
    public class MakeAddressBalanceCommand : IRequest<long>
    {
        public MakeAddressBalanceCommand(AddressBalance addressAllowance)
        {
            AddressBalance = addressAllowance ?? throw new ArgumentNullException(nameof(addressAllowance));
        }
        
        public AddressBalance AddressBalance { get; }
    }
}