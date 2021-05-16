using System;
using MediatR;
using Opdex.Platform.Domain.Models.Addresses;

namespace Opdex.Platform.Application.Abstractions.Commands.Addresses
{
    public class MakeAddressAllowanceCommand : IRequest<long>
    {
        public MakeAddressAllowanceCommand(AddressAllowance addressAllowance)
        {
            AddressAllowance = addressAllowance ?? throw new ArgumentNullException(nameof(addressAllowance));
        }
        
        public AddressAllowance AddressAllowance { get; }
    }
}