using System;
using Newtonsoft.Json;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Domain.Models.TransactionLogs
{
    public class CreateVaultCertificateLog : TransactionLog
    {
        public CreateVaultCertificateLog(dynamic log, string address, int sortOrder) 
            : base(TransactionLogType.CreateVaultCertificateLog, address, sortOrder)
        {
            string owner = log?.owner;
            string amount = log?.amount;
            ulong vestedBlock = log?.vestedBlock;

            if (!owner.HasValue())
            {
                throw new ArgumentNullException(nameof(owner));
            }

            if (!amount.IsNumeric())
            {
                throw new ArgumentNullException(nameof(amount));
            }
            
            if (vestedBlock < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(vestedBlock));
            }

            Owner = owner;
            Amount = amount;
            VestedBlock = vestedBlock;
        }
        
        public CreateVaultCertificateLog(long id, long transactionId, string address, int sortOrder, string details)
            : base(TransactionLogType.CreateVaultCertificateLog, id, transactionId, address, sortOrder)
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