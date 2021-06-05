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

            if (!vaultAmount.IsNumeric())
            {
                throw new ArgumentOutOfRangeException(nameof(vaultAmount), "Vault amount must only contain numeric digits.");
            }

            if (!miningAmount.IsNumeric())
            {
                throw new ArgumentOutOfRangeException(nameof(miningAmount), "Mining amount must only contain numeric digits.");
            }

            VaultAmount = vaultAmount;
            MiningAmount = miningAmount;
            PeriodIndex = periodIndex;
        }

        public DistributionLog(long id, long transactionId, string address, int sortOrder, string details)
            : base(TransactionLogType.DistributionLog, id, transactionId, address, sortOrder)
        {
            var logDetails = DeserializeLogDetails(details);
            VaultAmount = logDetails.VaultAmount;
            MiningAmount = logDetails.MiningAmount;
            PeriodIndex = logDetails.PeriodIndex;
        }

        public string VaultAmount { get; }
        public string MiningAmount { get; }
        public uint PeriodIndex { get; }

        private struct LogDetails
        {
            public string VaultAmount { get; set; }
            public string MiningAmount { get; set; }
            public uint PeriodIndex { get; set; }
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
                PeriodIndex = PeriodIndex
            });
        }
    }
}