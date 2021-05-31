using System;
using Newtonsoft.Json;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools
{
    public class StakeLog : TransactionLog
    {
        public StakeLog(dynamic log, string address, int sortOrder)
            : base(TransactionLogType.StakeLog, address, sortOrder)
        {
            string staker = log?.staker;
            string amount = log?.amount;
            string totalStaked = log?.totalStaked;
            byte eventType = log?.eventType;

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
            EventType = eventType;
        }
        
        public StakeLog(long id, long transactionId, string address, int sortOrder, string details)
            : base(TransactionLogType.StakeLog, id, transactionId, address, sortOrder)
        {
            var logDetails = DeserializeLogDetails(details);
            Staker = logDetails.Staker;
            Amount = logDetails.Amount;
            TotalStaked = logDetails.TotalStaked;
            EventType = logDetails.EventType;
        }
        
        public string Staker { get; }
        public string Amount { get; }
        public string TotalStaked { get; }
        public byte EventType { get; }
        
        private struct LogDetails
        {
            public string Staker { get; set; }
            public string Amount { get; set; }
            public string TotalStaked { get; set; }
            public byte EventType { get; set; }
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
                TotalStaked = TotalStaked,
                EventType = EventType
            });
        }
    }
}