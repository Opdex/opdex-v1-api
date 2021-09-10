using System;
using Newtonsoft.Json;
using Opdex.Platform.Common;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;

namespace Opdex.Platform.Domain.Models.TransactionLogs.Vaults
{
    public class RevokeVaultCertificateLog : TransactionLog
    {
        public RevokeVaultCertificateLog(dynamic log, Address address, int sortOrder)
            : base(TransactionLogType.RevokeVaultCertificateLog, address, sortOrder)
        {
            Address owner = log?.owner;
            UInt256 oldAmount = UInt256.Parse(log?.oldAmount);
            UInt256 newAmount = UInt256.Parse(log?.newAmount);
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
            OldAmount = oldAmount;
            NewAmount = newAmount;
            VestedBlock = vestedBlock;
        }

        public RevokeVaultCertificateLog(long id, long transactionId, Address address, int sortOrder, string details)
            : base(TransactionLogType.RevokeVaultCertificateLog, id, transactionId, address, sortOrder)
        {
            var logDetails = DeserializeLogDetails(details);
            Owner = logDetails.Owner;
            OldAmount = logDetails.OldAmount;
            NewAmount = logDetails.NewAmount;
            VestedBlock = logDetails.VestedBlock;
        }

        public Address Owner { get; }
        public UInt256 OldAmount { get; }
        public UInt256 NewAmount { get; }
        public ulong VestedBlock { get; }

        private struct LogDetails
        {
            public Address Owner { get; set; }
            public UInt256 OldAmount { get; set; }
            public UInt256 NewAmount { get; set; }
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
                OldAmount = OldAmount,
                NewAmount = NewAmount,
                VestedBlock = VestedBlock
            });
        }
    }
}
