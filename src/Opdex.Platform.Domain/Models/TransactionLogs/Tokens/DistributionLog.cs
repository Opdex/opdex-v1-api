using System;
using Newtonsoft.Json;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;

namespace Opdex.Platform.Domain.Models.TransactionLogs.Tokens
{
    public class DistributionLog : TransactionLog
    {
        public DistributionLog(dynamic log, Address address, int sortOrder)
            : base(TransactionLogType.DistributionLog, address, sortOrder)
        {
            UInt256 vaultAmount = UInt256.Parse(log?.vaultAmount);
            UInt256 miningAmount = UInt256.Parse(log?.miningAmount);
            uint periodIndex = log?.periodIndex;
            UInt256 totalSupply = UInt256.Parse(log?.totalSupply);
            ulong nextDistributionBlock = log?.nextDistributionBlock;

            if (nextDistributionBlock == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(nextDistributionBlock), "Next distribution block must be greater than 0.");
            }

            VaultAmount = vaultAmount;
            MiningAmount = miningAmount;
            PeriodIndex = periodIndex;
            TotalSupply = totalSupply;
            NextDistributionBlock = nextDistributionBlock;
        }

        public DistributionLog(long id, long transactionId, Address address, int sortOrder, string details)
            : base(TransactionLogType.DistributionLog, id, transactionId, address, sortOrder)
        {
            var logDetails = DeserializeLogDetails(details);
            VaultAmount = logDetails.VaultAmount;
            MiningAmount = logDetails.MiningAmount;
            PeriodIndex = logDetails.PeriodIndex;
            TotalSupply = logDetails.TotalSupply;
            NextDistributionBlock = logDetails.NextDistributionBlock;
        }

        public UInt256 VaultAmount { get; }
        public UInt256 MiningAmount { get; }
        public uint PeriodIndex { get; }
        public UInt256 TotalSupply { get; }
        public ulong NextDistributionBlock { get; }

        private struct LogDetails
        {
            public UInt256 VaultAmount { get; set; }
            public UInt256 MiningAmount { get; set; }
            public uint PeriodIndex { get; set; }
            public UInt256 TotalSupply { get; set; }
            public ulong NextDistributionBlock { get; set; }
        }

        private static LogDetails DeserializeLogDetails(string details)
        {
            return JsonConvert.DeserializeObject<LogDetails>(details);
        }

        public override string SerializeLogDetails()
        {
            return JsonConvert.SerializeObject(new LogDetails
            {
                VaultAmount = VaultAmount,
                MiningAmount = MiningAmount,
                PeriodIndex = PeriodIndex,
                TotalSupply = TotalSupply,
                NextDistributionBlock = NextDistributionBlock
            });
        }
    }
}
