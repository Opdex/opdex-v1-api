using System;
using Newtonsoft.Json;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Domain.Models.TransactionLogs
{
    public class ApprovalLog : TransactionLog
    {
        public ApprovalLog(dynamic log, string address, int sortOrder) 
            : base(TransactionLogType.ApprovalLog, address, sortOrder)
        {
            string owner = log?.owner;
            string spender = log?.spender;
            string amount = log?.amount;
            string oldAmount = log?.oldAmount;
            
            if (!owner.HasValue())
            {
                throw new ArgumentNullException(nameof(owner));
            }
            
            if (!spender.HasValue())
            {
                throw new ArgumentNullException(nameof(spender));
            }
            
            if (!amount.IsNumeric())
            {
                throw new ArgumentNullException(nameof(amount));
            }
            
            if (!oldAmount.IsNumeric())
            {
                throw new ArgumentNullException(nameof(oldAmount));
            }

            Owner = owner;
            Spender = spender;
            Amount = amount;
            OldAmount = oldAmount;
        }
        
        public ApprovalLog(long id, long transactionId, string address, int sortOrder, string details)
            : base(TransactionLogType.ApprovalLog, id, transactionId, address, sortOrder)
        {
            var logDetails = DeserializeLogDetails(details);
            Owner = logDetails.Owner;
            Spender = logDetails.Spender;
            Amount = logDetails.Amount;
            OldAmount = logDetails.OldAmount;
        }
        
        public string Owner { get; }
        public string Spender { get; }
        public string Amount { get; }
        public string OldAmount { get; }
        
        private struct LogDetails
        {
            public string Owner { get; set; }
            public string Spender { get; set; }
            public string Amount { get; set; }
            public string OldAmount { get; set; }
        }

        private LogDetails DeserializeLogDetails(string details)
        {
            return JsonConvert.DeserializeObject<LogDetails>(details);
        }

        public override string SerializeLogDetails()
        {
            return JsonConvert.SerializeObject(new LogDetails
            {
                Owner = Owner,
                Spender = Spender,
                Amount = Amount,
                OldAmount = OldAmount
            });
        }
    }
}