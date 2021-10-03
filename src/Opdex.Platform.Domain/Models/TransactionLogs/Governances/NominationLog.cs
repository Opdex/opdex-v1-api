using Newtonsoft.Json;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using System;

namespace Opdex.Platform.Domain.Models.TransactionLogs.Governances
{
    public class NominationLog : TransactionLog
    {
        public NominationLog(dynamic log, Address address, int sortOrder)
            : base(TransactionLogType.NominationLog, address, sortOrder)
        {
            Address stakingPool = (string)log?.stakingPool;
            Address miningPool = (string)log?.miningPool;
            UInt256 weight = UInt256.Parse((string)log?.weight);

            if (stakingPool == Address.Empty)
            {
                throw new ArgumentNullException(nameof(stakingPool), "Staking pool address must be set.");
            }

            if (miningPool == Address.Empty)
            {
                throw new ArgumentNullException(nameof(miningPool), "Mining pool address must be set.");
            }

            StakingPool = stakingPool;
            MiningPool = miningPool;
            Weight = weight;
        }

        public NominationLog(ulong id, ulong transactionId, Address address, int sortOrder, string details)
            : base(TransactionLogType.NominationLog, id, transactionId, address, sortOrder)
        {
            var logDetails = DeserializeLogDetails(details);
            StakingPool = logDetails.StakingPool;
            MiningPool = logDetails.MiningPool;
            Weight = logDetails.Weight;
        }

        public Address StakingPool { get; }
        public Address MiningPool { get; }
        public UInt256 Weight { get; }

        private struct LogDetails
        {
            public Address StakingPool { get; set; }
            public Address MiningPool { get; set; }
            public UInt256 Weight { get; set; }
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
                Weight = Weight
            });
        }
    }
}
