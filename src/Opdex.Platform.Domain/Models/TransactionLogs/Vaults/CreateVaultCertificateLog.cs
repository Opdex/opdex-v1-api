using System;
using Newtonsoft.Json;
using Opdex.Platform.Common;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;

namespace Opdex.Platform.Domain.Models.TransactionLogs.Vaults
{
    public class CreateVaultCertificateLog : TransactionLog
    {
        public CreateVaultCertificateLog(dynamic log, Address address, int sortOrder)
            : base(TransactionLogType.CreateVaultCertificateLog, address, sortOrder)
        {
            Address owner = log?.owner;
            UInt256 amount = UInt256.Parse(log?.amount);
            ulong vestedBlock = log?.vestedBlock;

            if (owner == Address.Empty)
            {
                throw new ArgumentNullException(nameof(owner), "Owner must be set.");
            }

            if (vestedBlock < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(vestedBlock), "Vested block must be greater than 0.");
            }

            Owner = owner;
            Amount = amount;
            VestedBlock = vestedBlock;
        }

        public CreateVaultCertificateLog(long id, long transactionId, Address address, int sortOrder, string details)
            : base(TransactionLogType.CreateVaultCertificateLog, id, transactionId, address, sortOrder)
        {
            var logDetails = DeserializeLogDetails(details);
            Owner = logDetails.Owner;
            Amount = logDetails.Amount;
            VestedBlock = logDetails.VestedBlock;
        }

        public Address Owner { get; }
        public UInt256 Amount { get; }
        public ulong VestedBlock { get; }

        private struct LogDetails
        {
            public Address Owner { get; set; }
            public UInt256 Amount { get; set; }
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
