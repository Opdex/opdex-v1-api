using System;
using Newtonsoft.Json;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Domain.Models.TransactionLogs.Tokens
{
    public class DistributionLog : TransactionLog
    {
        public DistributionLog(dynamic log, string address, int sortOrder)
            : base(TransactionLogType.DistributionLog, address, sortOrder)
        {
            string vaultAmount = log?.vaultAmount;
            string miningAmount = log?.miningAmount;
            uint periodIndex = log?.periodIndex;
            string totalSupply = log?.totalSupply;
            ulong nextDistributionBlock = log?.nextDistributionBlock;

            if (!vaultAmount.IsNumeric())
            {
                throw new ArgumentOutOfRangeException(nameof(vaultAmount), "Vault amount must only contain numeric digits.");
            }

            if (!miningAmount.IsNumeric())
            {
                throw new ArgumentOutOfRangeException(nameof(miningAmount), "Mining amount must only contain numeric digits.");
            }

            if (!totalSupply.IsNumeric())
            {
                throw new ArgumentOutOfRangeException(nameof(totalSupply), "Total supply must only contain numeric digits.");
            }

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

        public DistributionLog(long id, long transactionId, string address, int sortOrder, string details)
            : base(TransactionLogType.DistributionLog, id, transactionId, address, sortOrder)
        {
            var logDetails = DeserializeLogDetails(details);
            VaultAmount = logDetails.VaultAmount;
            MiningAmount = logDetails.MiningAmount;
            PeriodIndex = logDetails.PeriodIndex;
            TotalSupply = logDetails.TotalSupply;
            NextDistributionBlock = logDetails.NextDistributionBlock;
        }

        public string VaultAmount { get; }
        public string MiningAmount { get; }
        public uint PeriodIndex { get; }
        public string TotalSupply { get; }
        public ulong NextDistributionBlock { get; }

        private struct LogDetails
        {
            public string VaultAmount { get; set; }
            public string MiningAmount { get; set; }
            public uint PeriodIndex { get; set; }
            public string TotalSupply { get; set; }
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
