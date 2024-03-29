using Newtonsoft.Json;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using System;

namespace Opdex.Platform.Domain.Models.TransactionLogs.MiningGovernances;

public class RewardMiningPoolLog : TransactionLog
{
    public RewardMiningPoolLog(dynamic log, Address address, int sortOrder)
        : base(TransactionLogType.RewardMiningPoolLog, address, sortOrder)
    {
        Address stakingPool = (string)log?.stakingPool;
        Address miningPool = (string)log?.miningPool;
        UInt256 amount = UInt256.Parse((string)log?.amount);

        if (stakingPool == Address.Empty)
        {
            throw new ArgumentNullException(nameof(stakingPool), "Staking pool address must be set.");
        }

        if (miningPool == Address.Empty)
        {
            throw new ArgumentNullException(nameof(miningPool), "Mining pool address must be set.");
        }

        if (amount == UInt256.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than 0.");
        }

        StakingPool = stakingPool;
        MiningPool = miningPool;
        Amount = amount;
    }

    public RewardMiningPoolLog(ulong id, ulong transactionId, Address address, int sortOrder, string details)
        : base(TransactionLogType.RewardMiningPoolLog, id, transactionId, address, sortOrder)
    {
        var logDetails = DeserializeLogDetails(details);
        StakingPool = logDetails.StakingPool;
        MiningPool = logDetails.MiningPool;
        Amount = logDetails.Amount;
    }

    public Address StakingPool { get; }
    public Address MiningPool { get; }
    public UInt256 Amount { get; }

    private struct LogDetails
    {
        public Address StakingPool { get; set; }
        public Address MiningPool { get; set; }
        public UInt256 Amount { get; set; }
    }

    private static LogDetails DeserializeLogDetails(string details)
    {
        return JsonConvert.DeserializeObject<LogDetails>(details);
    }

    public override string SerializeLogDetails()
    {
        return JsonConvert.SerializeObject(new LogDetails
        {
            StakingPool = StakingPool,
            MiningPool = MiningPool,
            Amount = Amount
        });
    }
}