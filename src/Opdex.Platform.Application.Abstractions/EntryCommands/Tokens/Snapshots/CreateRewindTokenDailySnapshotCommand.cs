using MediatR;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Tokens.Snapshots
{
    public class CreateRewindTokenDailySnapshotCommand : IRequest<bool>
    {
        public CreateRewindTokenDailySnapshotCommand(ulong tokenId, ulong marketId, decimal crsUsdStartOfDay, DateTime startOfDay,
                                                     DateTime endOfDay, ulong blockHeight)
        {
            if (tokenId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(tokenId), "Token id must be greater than 0.");
            }

            if (marketId < 1)
            {
                // We do not rewind CRS snapshots with marketId of 0, until we have global average snapshots for
                // SRC token using marketId = 0, we'll want this check.
                throw new ArgumentOutOfRangeException(nameof(marketId), "Market id must be greater than zero.");
            }

            if (crsUsdStartOfDay <= 0m)
            {
                throw new ArgumentOutOfRangeException(nameof(crsUsdStartOfDay), "CRS USD at the start of the day must be greater than 0.");
            }

            if (startOfDay.Equals(default) || endOfDay.Equals(default) || startOfDay >= endOfDay || startOfDay.Date != endOfDay.Date)
            {
                throw new Exception("Start date time must be prior to end date time and the dates must match.");
            }

            if (blockHeight == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
            }

            TokenId = tokenId;
            MarketId = marketId;
            CrsUsdStartOfDay = crsUsdStartOfDay;
            StartDate = startOfDay;
            EndDate = endOfDay;
            BlockHeight = blockHeight;
        }

        public ulong TokenId { get; }
        public ulong MarketId { get; }
        public decimal CrsUsdStartOfDay { get; }
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }
        public ulong BlockHeight { get; }
    }
}
