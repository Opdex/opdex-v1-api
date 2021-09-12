using Newtonsoft.Json;
using Opdex.Platform.Common;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Domain.Models.TransactionLogs
{
    public abstract class OwnershipLog : TransactionLog
    {
        protected OwnershipLog(TransactionLogType logType, Address from, Address to, Address address, int sortOrder)
            : base(logType, address, sortOrder)
        {
            if (from == Address.Empty)
            {
                throw new ArgumentNullException(nameof(from), "From address must be set.");
            }

            if (to == Address.Empty)
            {
                throw new ArgumentNullException(nameof(to), "To address must be set.");
            }

            From = from;
            To = to;
        }

        protected OwnershipLog(TransactionLogType logType, long id, long transactionId, Address address, int sortOrder, string details)
            : base(logType, id, transactionId, address, sortOrder)
        {
            var logDetails = DeserializeLogDetails(details);
            From = logDetails.From;
            To = logDetails.To;
        }

        public Address From { get; }
        public Address To { get; }

        private struct LogDetails
        {
            public Address From { get; set; }
            public Address To { get; set; }
        }

        private static LogDetails DeserializeLogDetails(string details)
        {
            return JsonConvert.DeserializeObject<LogDetails>(details);
        }

        public override string SerializeLogDetails()
        {
            return JsonConvert.SerializeObject(new LogDetails
            {
                From = From,
                To = To
            });
        }
    }
}
