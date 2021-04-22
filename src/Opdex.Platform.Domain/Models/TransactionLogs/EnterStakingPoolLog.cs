using System;
using Newtonsoft.Json;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Domain.Models.TransactionLogs
{
    public class EnterStakingPoolLog : TransactionLog
    {
        public EnterStakingPoolLog(dynamic log, string address, int sortOrder)
            : base(nameof(EnterStakingPoolLog), address, sortOrder)
        {
            string staker = log?.staker;
            string amount = log?.amount;
            string weight = log?.weight;

            if (!staker.HasValue())
            {
                throw new ArgumentNullException(nameof(staker));
            }
            
            if (!amount.HasValue())
            {
                throw new ArgumentNullException(nameof(amount));
            }
            
            if (!weight.HasValue())
            {
                throw new ArgumentNullException(nameof(weight));
            }

            Staker = staker;
            Amount = amount;
            Weight = weight;
        }
        
        public EnterStakingPoolLog(long id, long transactionId, string address, int sortOrder, string details)
            : base(nameof(EnterStakingPoolLog), id, transactionId, address, sortOrder)
        {
            var logDetails = DeserializeLogDetails(details);
            Staker = logDetails.Staker;
            Amount = logDetails.Amount;
            Weight = logDetails.Weight;
        }
        
        public string Staker { get; }
        public string Amount { get; }
        public string Weight { get; }
        
        private struct LogDetails
        {
            public string Staker { get; set; }
            public string Amount { get; set; }
            public string Weight { get; set; }
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
                Weight = Weight
            });
        }
    }
}