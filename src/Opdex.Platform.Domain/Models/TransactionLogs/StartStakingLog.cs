using System;
using Newtonsoft.Json;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Domain.Models.TransactionLogs
{
    public class StartStakingLog : TransactionLog
    {
        public StartStakingLog(dynamic log, string address, int sortOrder)
            : base(TransactionLogType.StartStakingLog, address, sortOrder)
        {
            string staker = log?.staker;
            string amount = log?.amount;
            string totalStaked = log?.totalStaked;

            if (!staker.HasValue())
            {
                throw new ArgumentNullException(nameof(staker));
            }
            
            if (!amount.HasValue())
            {
                throw new ArgumentNullException(nameof(amount));
            }
            
            if (!totalStaked.HasValue())
            {
                throw new ArgumentNullException(nameof(totalStaked));
            }

            Staker = staker;
            Amount = amount;
            TotalStaked = totalStaked;
        }
        
        public StartStakingLog(long id, long transactionId, string address, int sortOrder, string details)
            : base(TransactionLogType.StartStakingLog, id, transactionId, address, sortOrder)
        {
            var logDetails = DeserializeLogDetails(details);
            Staker = logDetails.Staker;
            Amount = logDetails.Amount;
            TotalStaked = logDetails.TotalStaked;
        }
        
        public string Staker { get; }
        public string Amount { get; }
        public string TotalStaked { get; }
        
        private struct LogDetails
        {
            public string Staker { get; set; }
            public string Amount { get; set; }
            public string TotalStaked { get; set; }
        }

        private static LogDetails DeserializeLogDetails(string details)
        {
            return JsonConvert.DeserializeObject<LogDetails>(details);
        }

        public override string SerializeLogDetails()
        {
            return JsonConvert.SerializeObject(new LogDetails
            {
                Staker = Staker,
                Amount = Amount,
                TotalStaked = TotalStaked
            });
        }
    }
}