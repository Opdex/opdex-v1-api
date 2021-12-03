using System;
using MediatR;
using Opdex.Platform.Domain.Models.Addresses;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Addresses;

public class PersistAddressMiningCommand : IRequest<ulong>
{
    public PersistAddressMiningCommand(AddressMining addressMining)
    {
        AddressMining = addressMining ?? throw new ArgumentNullException(nameof(addressMining));
    }

    public AddressMining AddressMining { get; }
}