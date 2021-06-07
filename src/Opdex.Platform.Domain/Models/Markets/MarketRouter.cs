using System;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Domain.Models.Markets
{
    public class MarketRouter : BlockAudit
    {
        public MarketRouter(string address, long marketId, bool isActive, ulong createdBlock) : base(createdBlock)
        {
            if (!address.HasValue())
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

        public MarketRouter(long id, string address, long marketId, bool isActive, ulong createdBlock, ulong modifiedBlock) : base(createdBlock, modifiedBlock)
        {
            Id = id;
            Address = address;
            MarketId = marketId;
            IsActive = isActive;
        }

        public long Id { get; }
        public string Address { get; }
        public long MarketId { get; }
        public bool IsActive { get; }
    }
}