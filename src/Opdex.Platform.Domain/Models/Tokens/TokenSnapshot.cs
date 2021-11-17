using System;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.OHLC;
using System.Collections.Generic;
using System.Linq;

namespace Opdex.Platform.Domain.Models.Tokens
{
    public class TokenSnapshot
    {
        public TokenSnapshot(ulong tokenId, ulong marketId, SnapshotType snapshotType, DateTime dateTime)
        {
            if (tokenId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(marketId), "Token id must be greater than 0.");
            }

            if (!snapshotType.IsValid())
            {
                throw new ArgumentOutOfRangeException(nameof(snapshotType), "Snapshot type must be valid.");
            }

            TokenId = tokenId;
            MarketId = marketId;
            SnapshotType = snapshotType;
            Price = new OhlcDecimalSnapshot();
            StartDate = dateTime.ToStartOf(snapshotType);
            EndDate = dateTime.ToEndOf(snapshotType);
        }

        public TokenSnapshot(ulong id, ulong tokenId, ulong marketId, OhlcDecimalSnapshot price, SnapshotType snapshotType, DateTime startDate,
                             DateTime endDate, DateTime modifiedDate)
        {
            Id = id;
            TokenId = tokenId;
            MarketId = marketId;
            Price = price;
            SnapshotType = snapshotType;
            StartDate = startDate;
            EndDate = endDate;
            ModifiedDate = modifiedDate;
        }

        public ulong Id { get; private set; }
        public ulong TokenId { get; }
        public ulong MarketId { get; }
        public OhlcDecimalSnapshot Price { get; private set; }
        public SnapshotType SnapshotType { get; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public DateTime ModifiedDate { get; private set; }

        /// <summary>
        /// Determine if the snapshot is stale compared to a provided datetime.
        /// </summary>
        /// <param name="endDate">The current end date to check against</param>
        /// <returns>true if the record is stale</returns>
        public bool IsStale(DateTime endDate) => EndDate < endDate;

        /// <summary>
        /// Rewinds a daily snapshot using all existing hourly snapshots from the same day.
        /// </summary>
        /// <param name="snapshots">List of all hourly snapshots for the day.</param>
        public void RewindDailySnapshot(IList<TokenSnapshot> snapshots)
        {
            if (!snapshots.Any()) return;

            // This snapshot must be a daily type
            if (SnapshotType != SnapshotType.Daily)
            {
                throw new Exception("Only daily snapshots can be rewound.");
            }

            // Validate the snapshots being used for the rewind
            var allValidSnapshots = snapshots.All(s =>
            {
                var matchingToken = s.TokenId == TokenId;
                var matchingMarket = s.MarketId == MarketId;
                var isHourlyType = s.SnapshotType == SnapshotType.Hourly;
                var sameDay = s.StartDate.Date == StartDate.Date && s.EndDate.Date == EndDate.Date;

                return isHourlyType && sameDay && matchingToken && matchingMarket;
            });

            if (!allValidSnapshots)
            {
                throw new Exception("Daily snapshots can only rewind using hourly snapshots");
            }

            // Verify order is correct
            snapshots = snapshots.OrderBy(snapshot => snapshot.EndDate).ToList();

            Price = new OhlcDecimalSnapshot(snapshots.Select(snapshot => snapshot.Price).ToList());
        }

        /// <summary>
        /// Update the token price of an existing token snapshot.
        /// </summary>
        /// <param name="price">The USD price of the token.</param>
        public void UpdatePrice(decimal price)
        {
            UpdatePriceExecute(price, false);
        }

        /// <summary>
        /// Update the token price of na existing snapshot using reserve ratios and CRS USD price.
        /// </summary>
        /// <param name="reserveCrs">The total CRS reserves of the associated liquidity pool.</param>
        /// <param name="reserveSrc">The total SRC reserves of the associated liquidity pool.</param>
        /// <param name="crsUsd">The USD price of a single CRS token.</param>
        /// <param name="srcSats">The total number of sats per SRC token this snapshot represents.</param>
        public void UpdatePrice(ulong reserveCrs, UInt256 reserveSrc, decimal crsUsd, ulong srcSats)
        {
            var crsPerSrc = reserveCrs.Token0PerToken1(reserveSrc, srcSats);
            var price = MathExtensions.TotalFiat(crsPerSrc, crsUsd, TokenConstants.Cirrus.Sats);

            UpdatePriceExecute(price, false);
        }

        /// <summary>
        /// Resets a stale snapshot Id to 0 and calculates the next price using
        /// CRS per SRC token ratio and the USD price of a single CRS token.
        /// </summary>
        /// <param name="crsPerSrc">CRS tokens per SRC token ratio from this token's liquidity pool.</param>
        /// <param name="crsUsd">The USD cost of a single full CRS token at the time of this snapshot.</param>
        /// <param name="blockTime">The block time that represents this new snapshot.</param>
        public void ResetStaleSnapshot(UInt256 crsPerSrc, decimal crsUsd, DateTime blockTime)
        {
            Id = 0;

            UpdatePriceExecute(MathExtensions.TotalFiat(crsPerSrc, crsUsd, TokenConstants.Cirrus.Sats), true);

            StartDate = blockTime.ToStartOf(SnapshotType);
            EndDate = blockTime.ToEndOf(SnapshotType);
        }

        /// <summary>
        /// Resets a snapshot Id to 0 and sets a new USD price and start/end time.
        /// </summary>
        /// <param name="price">The USD price of a single full token.</param>
        /// <param name="blockTime">The block time that represents this new snapshot.</param>
        public void ResetStaleSnapshot(decimal price, DateTime blockTime)
        {
            Id = 0;

            UpdatePriceExecute(price, true);

            StartDate = blockTime.ToStartOf(SnapshotType);
            EndDate = blockTime.ToEndOf(SnapshotType);
        }

        private void UpdatePriceExecute(decimal price, bool reset)
        {
            Price.Update(Math.Round(price, 8, MidpointRounding.AwayFromZero), reset);
            ModifiedDate = DateTime.UtcNow;
        }
    }
}
