using System;
using Newtonsoft.Json;
using Opdex.Core.Common.Extensions;

namespace Opdex.Core.Domain.Models.TransactionLogs
{
    public class ExitStakingPoolLog : TransactionLog
    {
        public ExitStakingPoolLog(dynamic log, string address, int sortOrder)
            : base(nameof(ExitStakingPoolLog), address, sortOrder)
        {
            string staker = log?.staker;
            string amount = log?.amount;

            if (!staker.HasValue())
            {
                throw new ArgumentNullException(nameof(staker));
            }
            
            if (!amount.HasValue())
            {
                throw new ArgumentNullException(nameof(amount));
            }

            Staker = staker;
            Amount = amount;
        }
        
        public ExitStakingPoolLog(long id, long transactionId, string address, int sortOrder, string details)
            : base(nameof(ExitStakingPoolLog), id, transactionId, address, sortOrder)
        {
            var logDetails = DeserializeLogDetails(details);
            Staker = logDetails.Staker;
            Amount = logDetails.Amount;
        }
        
        public string Staker { get; }
        public string Amount { get; }
        
        private struct LogDetails
        {
            public string Staker { get; set; }
            public string Amount { get; set; }
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
                Amount = Amount
            });
        }
    }
}