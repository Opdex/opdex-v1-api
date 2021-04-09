using System;
using Opdex.Core.Common.Extensions;

namespace Opdex.Core.Domain.Models.TransactionLogs
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
        
        public string Staker { get; }
        public string Amount { get; }
        public string Weight { get; }
    }
}