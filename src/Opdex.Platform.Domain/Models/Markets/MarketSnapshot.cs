using System;
using Opdex.Platform.Common;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;

namespace Opdex.Platform.Domain.Models.Markets
{
    public class MarketSnapshot
    {
        public MarketSnapshot(long marketId, long transactionCount, decimal liquidity, decimal volume, string weight,
            decimal weightUsd, decimal providerRewards, decimal stakerRewards, SnapshotType snapshotType, DateTime startDate, DateTime endDate)
        {
            if (marketId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(marketId), "Market id must be greater than 0.");
            }

            if (transactionCount < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(transactionCount), "Transaction count must be greater than 0.");
            }

            MarketId = marketId;
            TransactionCount = transactionCount;
            Liquidity = liquidity;
            Volume = volume;
            Weight = weight;
            WeightUsd = weightUsd;
            ProviderRewards = providerRewards;
            StakerRewards = stakerRewards;
            SnapshotType = snapshotType;
            StartDate = startDate;
            EndDate = endDate;
        }

        public MarketSnapshot(long id, long marketId, long transactionCount, decimal liquidity, decimal volume, string weight,
            decimal weightUsd, decimal providerRewards, decimal stakerRewards, SnapshotType snapshotType, DateTime startDate, DateTime endDate)
        {
            Id = id;
            MarketId = marketId;
            TransactionCount = transactionCount;
            Liquidity = liquidity;
            Volume = volume;
            Weight = weight;
            WeightUsd = weightUsd;
            ProviderRewards = providerRewards;
            StakerRewards = stakerRewards;
            SnapshotType = snapshotType;
            StartDate = startDate;
            EndDate = endDate;
        }

        public long Id { get; }
        public long MarketId { get; }
        public long TransactionCount { get; }
        public decimal Liquidity { get; private set; }
        public decimal Volume { get; private set; }
        public string Weight { get; private set; }
        public decimal WeightUsd { get; private set; }
        public decimal ProviderRewards { get; private set; }
        public decimal StakerRewards { get; private set; }
        public SnapshotType SnapshotType { get; }
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }

        public void ProcessSwapLog(SwapLog log, TokenSnapshot crsSnapshot, Token crs)
        {
            var volumeCrs = (log.AmountCrsIn + log.AmountCrsOut).ToString();

            var crsVolumeDecimal = volumeCrs.ToRoundedDecimal(2, crs.Decimals);
            Volume += Math.Round(crsVolumeDecimal * crsSnapshot.Price, 2);

            var rewards = Math.Round(Volume * .003m / 6, 2);

            StakerRewards = rewards; // 1/6
            ProviderRewards = Math.Round(rewards * 5, 2); // 5/6
        }

        // public void ProcessReservesLog(ReservesLog log, TokenSnapshot crsSnapshot, Token crs)
        // {
        //     var reserveCrsRounded = log.ReserveCrs.ToString().ToRoundedDecimal(2, crs.Decimals);
        //     
        //     // * 2, for reserve Crs USD amount and reserve Src, they are equal
        //     ReserveUsd = Math.Round(reserveCrsRounded * crsSnapshot.Price * 2, 2);
        // }

        // public void ProcessMintLog()
        // {
        //     
        // }
        //
        // public void ProcessBurnLog()
        // {
        //     
        // }

        // public void ProcessStakingLog<T>(T log, TokenSnapshot odxSnapshot, Token odx) 
        //     where T : TransactionLog
        // {
        //     var weight = log switch
        //     {
        //         StartStakingLog startStakingLog => startStakingLog.TotalStaked ?? "0",
        //         StopStakingLog stopStakingLog => stopStakingLog.TotalStaked ?? "0",
        //         _ => "0"
        //     };
        //     
        //     const int precision = 2;
        //     var odxDecimal = weight.ToRoundedDecimal(precision, odx.Decimals);
        //     var odxWeightUsd = Math.Round(odxDecimal * odxSnapshot.Price, precision);
        //
        //     Weight = Weight.Add(weight);
        //     WeightUsd += odxWeightUsd;
        // }
    }
}