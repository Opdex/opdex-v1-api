using System;
using MediatR;
using Opdex.Platform.Domain.Models.Addresses;

namespace Opdex.Platform.Application.Abstractions.Commands.Addresses;

public class MakeAddressMiningCommand : IRequest<ulong>
{
    public MakeAddressMiningCommand(AddressMining addressMining)
    {
        AddressMining = addressMining ?? throw new ArgumentNullException(nameof(addressMining));
    }

    public AddressMining AddressMining { get; }
}