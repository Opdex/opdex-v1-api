using System;
using Opdex.Core.Common.Extensions;

namespace Opdex.Core.Domain.Models.TransactionLogs
{
    public class ExitStakingPoolLog : TransactionLog
    {
        public ExitStakingPoolLog(dynamic log, string address, int sortOrder)
            : base(nameof(EnterStakingPoolLog), address, sortOrder)
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
        
        public string Staker { get; }
        public string Amount { get; }
    }
}