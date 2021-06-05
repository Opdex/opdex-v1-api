using System;
using Newtonsoft.Json;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Domain.Models.TransactionLogs.Vault
{
    public class RedeemVaultCertificateLog : TransactionLog
    {
        public RedeemVaultCertificateLog(dynamic log, string address, int sortOrder)
            : base(TransactionLogType.RedeemVaultCertificateLog, address, sortOrder)
        {
            string owner = log?.owner;
            string amount = log?.amount;
            ulong vestedBlock = log?.vestedBlock;

            if (!owner.HasValue())
            {
                throw new ArgumentNullException(nameof(owner), "Owner must be set.");
            }

            if (!amount.IsNumeric())
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount must only contain numeric digits.");
            }

            if (vestedBlock < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(vestedBlock), "Vested block must be greater than 0.");
            }

            Owner = owner;
            Amount = amount;
            VestedBlock = vestedBlock;
        }

        public RedeemVaultCertificateLog(long id, long transactionId, string address, int sortOrder, string details)
            : base(TransactionLogType.RedeemVaultCertificateLog, id, transactionId, address, sortOrder)
        {
            var logDetails = DeserializeLogDetails(details);
            Owner = logDetails.Owner;
            Amount = logDetails.Amount;
            VestedBlock = logDetails.VestedBlock;
        }

        public string Owner { get; }
        public string Amount { get; }
        public ulong VestedBlock { get; }

        private struct LogDetails
        {
            public string Owner { get; set; }
            public string Amount { get; set; }
            public ulong VestedBlock { get; set; }
        }

        private static LogDetails DeserializeLogDetails(string details)
        {
            return JsonConvert.DeserializeObject<LogDetails>(details);
        }

        public override string SerializeLogDetails()
        {
            return JsonConvert.SerializeObject(new LogDetails
            {
                Owner = Owner,
                Amount = Amount,
                VestedBlock = VestedBlock
            });
        }
    }
}