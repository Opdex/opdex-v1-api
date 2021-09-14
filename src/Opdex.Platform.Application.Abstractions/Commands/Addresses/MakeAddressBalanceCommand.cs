using System;
using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Addresses;

namespace Opdex.Platform.Application.Abstractions.Commands.Addresses
{
    public class MakeAddressBalanceCommand : IRequest<long>
    {
        public MakeAddressBalanceCommand(AddressBalance addressBalance, Address token, ulong blockHeight)
        {
            AddressBalance = addressBalance ?? throw new ArgumentNullException(nameof(addressBalance));
            Token = token != Address.Empty ? token : throw new ArgumentNullException(nameof(token));
            BlockHeight = blockHeight > 0 ? blockHeight : throw new ArgumentOutOfRangeException(nameof(blockHeight));
        }

        public AddressBalance AddressBalance { get; }
        public Address Token { get; }
        public ulong BlockHeight { get; }
    }
}
