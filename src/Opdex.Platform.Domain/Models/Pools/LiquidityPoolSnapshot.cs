using System;
using Opdex.Platform.Common;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;

namespace Opdex.Platform.Domain.Models.Pools
{
    public class LiquidityPoolSnapshot
    {
        public LiquidityPoolSnapshot(long liquidityPoolId, SnapshotType snapshotType, DateTime startDate, DateTime endDate)
        {
            if (liquidityPoolId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(liquidityPoolId));
            }

            LiquidityPoolId = liquidityPoolId;
            TransactionCount = 0;
            ReserveCrs = "0";
            ReserveSrc = "0";
            ReserveUsd = 0.00m;
            VolumeCrs = "0";
            VolumeSrc = "0";
            VolumeUsd = 0.00m;
            StakingWeight = "0";
            StakingUsd = 0.00m;
            ProviderRewards = 0.00m;
            StakerRewards = 0.00m;
            SnapshotType = snapshotType;
            StartDate = startDate;
            EndDate = endDate;
        }
        
        public LiquidityPoolSnapshot(long id, long liquidityPoolId, long transactionCount, string reserveCrs, string reserveSrc, decimal reserveUsd,
            string volumeCrs, string volumeSrc, decimal volumeUsd, string stakingWeight, decimal stakingUsd, decimal providerRewards, decimal stakerRewards,
            SnapshotType snapshotType, DateTime startDate, DateTime endDate)
        {
            Id = id;
            LiquidityPoolId = liquidityPoolId;
            TransactionCount = transactionCount;
            ReserveCrs = reserveCrs;
            ReserveSrc = reserveSrc;
            ReserveUsd = reserveUsd;
            VolumeCrs = volumeCrs;
            VolumeSrc = volumeSrc;
            VolumeUsd = volumeUsd;
            StakingWeight = stakingWeight;
            StakingUsd = stakingUsd;
            ProviderRewards = providerRewards;
            StakerRewards = stakerRewards;
            SnapshotType = snapshotType;
            StartDate = startDate;
            EndDate = endDate;
        }
        
        public long Id { get; }
        public long LiquidityPoolId { get; }
        public long TransactionCount { get; private set; }
        public string ReserveCrs { get; private set; }
        public string ReserveSrc { get; private set; }
        public decimal ReserveUsd { get; private set; }
        public string VolumeCrs { get; private set; }
        public string VolumeSrc { get; private set; }
        public decimal VolumeUsd { get; private set; }
        public string StakingWeight { get; private set; }
        public decimal StakingUsd { get; private set; }
        public decimal ProviderRewards { get; private set; }
        public decimal StakerRewards { get; private set; }
        public SnapshotType SnapshotType { get; }
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }

        public void ProcessSwapLog(SwapLog log, TokenSnapshot crsSnapshot)
        {
            var volumeCrs = log.AmountCrsIn + log.AmountCrsOut;
            VolumeCrs = VolumeCrs.Add(volumeCrs.ToString());
            
            var volumeSrc = log.AmountSrcIn.Add(log.AmountSrcOut);
            VolumeSrc = VolumeSrc.Add(volumeSrc);

            var crsVolumeDecimal = VolumeCrs.ToRoundedDecimal(8, TokenConstants.Cirrus.Decimals);
            VolumeUsd = Math.Round(crsVolumeDecimal * crsSnapshot.Price, 2, MidpointRounding.AwayFromZero);
            
            var rewards = Math.Round(VolumeUsd * .003m / 6, 2, MidpointRounding.AwayFromZero);
            
            StakerRewards = rewards; // 1/6
            ProviderRewards = Math.Round(rewards * 5, 2, MidpointRounding.AwayFromZero); // 5/6
        }
        
        public void ProcessReservesLog(ReservesLog log, TokenSnapshot crsSnapshot)
        {
            ReserveCrs = log.ReserveCrs.ToString();
            ReserveSrc = log.ReserveSrc;

            var reserveCrsRounded = ReserveCrs.ToRoundedDecimal(2, TokenConstants.Cirrus.Decimals);
            
            // * 2, for reserve Crs USD amount and reserve Src, they are equal
            ReserveUsd = Math.Round(reserveCrsRounded * crsSnapshot.Price * 2, 2, MidpointRounding.AwayFromZero);
        }
        
        public void ProcessStakingLog(StakeLog log, TokenSnapshot odxSnapshot)
        {
            const int precision = 2;
            var odxDecimal = log.TotalStaked.ToRoundedDecimal(precision, TokenConstants.Opdex.Decimals);
            var odxWeightUsd = Math.Round(odxDecimal * odxSnapshot.Price, precision, MidpointRounding.AwayFromZero);

            StakingWeight = log.TotalStaked;
            StakingUsd = odxWeightUsd;
        }

        public void IncrementTransactionCount()
        {
            TransactionCount += 1;
        }
    }
}