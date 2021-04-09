using System;
using Newtonsoft.Json;
using Opdex.Core.Common.Extensions;

namespace Opdex.Core.Domain.Models.TransactionLogs
{
    public class DistributionLog : TransactionLog
    {
        public DistributionLog(dynamic log, string address, int sortOrder)
            : base(nameof(DistributionLog), address, sortOrder)
        {
            string ownerAddress = log?.ownerAddress;
            string miningAddress = log?.miningAddress;
            string ownerAmount = log?.ownerAmount;
            string miningAmount = log?.miningAmount;
            uint periodIndex = log?.periodIndex;

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
            PeriodIndex = periodIndex;
        }
        
        public DistributionLog(long id, long transactionId, string address, int sortOrder, string details)
            : base(nameof(DistributionLog), id, transactionId, address, sortOrder)
        {
            var logDetails = DeserializeLogDetails(details);
            OwnerAddress = logDetails.OwnerAddress;
            MiningAddress = logDetails.MiningAddress;
            OwnerAmount = logDetails.OwnerAmount;
            MiningAmount = logDetails.MiningAmount;
            PeriodIndex = logDetails.PeriodIndex;
        }
        
        public string OwnerAddress { get; }
        public string MiningAddress { get; }
        public string OwnerAmount { get; }
        public string MiningAmount { get; }
        public uint PeriodIndex { get; }
        
        private sealed class LogDetails
        {
            public string OwnerAddress { get; set; }
            public string MiningAddress { get; set; }
            public string OwnerAmount { get; set; }
            public string MiningAmount { get; set; }
            public uint PeriodIndex { get; set; }
        }

        private static LogDetails DeserializeLogDetails(string details)
        {
            return JsonConvert.DeserializeObject<LogDetails>(details);
        }

        public override string SerializeLogDetails()
        {
            return JsonConvert.SerializeObject(new LogDetails
            {
                OwnerAddress = OwnerAddress,
                MiningAddress = MiningAddress,
                OwnerAmount = OwnerAmount,
                MiningAmount = MiningAmount,
                PeriodIndex = PeriodIndex
            });
        }
    }
}