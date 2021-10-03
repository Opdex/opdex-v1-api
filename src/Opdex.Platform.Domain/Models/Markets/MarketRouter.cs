using System;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Blocks;

namespace Opdex.Platform.Domain.Models.Markets
{
    public class MarketRouter : BlockAudit
    {
        public MarketRouter(Address address, ulong marketId, bool isActive, ulong createdBlock) : base(createdBlock)
        {
            if (address == Address.Empty)
            {
                throw new ArgumentNullException(nameof(address), "Router address must be set.");
            }

            if (marketId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(marketId), "MarketId must be greater than 0.");
            }

            Address = address;
            MarketId = marketId;
            IsActive = isActive;
        }

        public MarketRouter(ulong id, Address address, ulong marketId, bool isActive, ulong createdBlock, ulong modifiedBlock) : base(createdBlock, modifiedBlock)
        {
            Id = id;
            Address = address;
            MarketId = marketId;
            IsActive = isActive;
        }

        public ulong Id { get; }
        public Address Address { get; }
        public ulong MarketId { get; }
        public bool IsActive { get; }
    }
}
