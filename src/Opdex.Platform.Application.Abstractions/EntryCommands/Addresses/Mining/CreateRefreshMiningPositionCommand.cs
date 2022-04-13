using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Addresses.Mining;

public class CreateRefreshMiningPositionCommand : IRequest<MiningPositionDto>
{
    public CreateRefreshMiningPositionCommand(Address miner, Address miningPool)
    {
        Miner = miner != Address.Empty ? miner : throw new ArgumentNullException(nameof(miner), "Miner address must be set.");
        MiningPool = miningPool != Address.Empty ? miningPool : throw new ArgumentNullException(nameof(miningPool), "Mining pool address must be set.");
    }

    public Address Miner { get; }
    public Address MiningPool { get; }
}
