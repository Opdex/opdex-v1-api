using System;
using Opdex.Core.Common.Extensions;

namespace Opdex.Core.Domain.Models.TransactionLogs
{
    public class DistributionLog : TransactionLog
    {
        public DistributionLog(dynamic log, string address, int sortOrder)
            : base(nameof(EnterStakingPoolLog), address, sortOrder)
        {
            string ownerAddress = log?.ownerAddress;
            string miningAddress = log?.miningAddress;
            string ownerAmount = log?.ownerAmount;
            string miningAmount = log?.miningAmount;
            uint yearIndex = log?.yearIndex;

            if (!ownerAddress.HasValue())
            {
                throw new ArgumentNullException(nameof(ownerAddress));
            }
            
            if (!miningAddress.HasValue())
            {
                throw new ArgumentNullException(nameof(miningAddress));
            }
            
            if (!ownerAmount.HasValue())
            {
                throw new ArgumentNullException(nameof(ownerAmount));
            }
            
            if (!miningAmount.HasValue())
            {
                throw new ArgumentNullException(nameof(miningAmount));
            }

            OwnerAddress = ownerAddress;
            MiningAddress = miningAddress;
            OwnerAmount = ownerAmount;
            MiningAmount = miningAmount;
            YearIndex = yearIndex;
        }
        
        public string OwnerAddress { get; }
        public string MiningAddress { get; }
        public string OwnerAmount { get; }
        public string MiningAmount { get; }
        public uint YearIndex { get; }
    }
}