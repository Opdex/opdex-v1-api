using System;
using Newtonsoft.Json;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Domain.Models.TransactionLogs.Vault
{
    public class RevokeVaultCertificateLog : TransactionLog
    {
        public RevokeVaultCertificateLog(dynamic log, string address, int sortOrder) 
            : base(TransactionLogType.RevokeVaultCertificateLog, address, sortOrder)
        {
            string owner = log?.owner;
            string oldAmount = log?.oldAmount;
            string newAmount = log?.oldAmount;
            ulong vestedBlock = log?.vestedBlock;

            if (!owner.HasValue())
            {
                throw new ArgumentNullException(nameof(owner));
            }

            if (!oldAmount.IsNumeric())
            {
                throw new ArgumentNullException(nameof(oldAmount));
            }
            
            if (!newAmount.IsNumeric())
            {
                throw new ArgumentNullException(nameof(newAmount));
            }
            
            if (vestedBlock < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(vestedBlock));
            }

            Owner = owner;
            OldAmount = oldAmount;
            NewAmount = newAmount;
            VestedBlock = vestedBlock;
        }
        
        public RevokeVaultCertificateLog(long id, long transactionId, string address, int sortOrder, string details)
            : base(TransactionLogType.RevokeVaultCertificateLog, id, transactionId, address, sortOrder)
        {
            var logDetails = DeserializeLogDetails(details);
            Owner = logDetails.Owner;
            OldAmount = logDetails.OldAmount;
            NewAmount = logDetails.NewAmount;
            VestedBlock = logDetails.VestedBlock;
        }
        
        public string Owner { get; }
        public string OldAmount { get; }
        public string NewAmount { get; }
        public ulong VestedBlock { get; }
        
        private struct LogDetails
        {
            public string Owner { get; set; }
            public string OldAmount { get; set; }
            public string NewAmount { get; set; }
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