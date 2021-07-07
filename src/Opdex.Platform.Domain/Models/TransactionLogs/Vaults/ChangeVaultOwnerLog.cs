using Newtonsoft.Json;
using Opdex.Platform.Common.Extensions;
using System;

namespace Opdex.Platform.Domain.Models.TransactionLogs.Vaults
{
    public class ChangeVaultOwnerLog : TransactionLog
    {
        public ChangeVaultOwnerLog(dynamic log, string address, int sortOrder)
            : base(TransactionLogType.ChangeVaultOwnerLog, address, sortOrder)
        {
            string from = log?.from;
            string to = log?.to;

            if (!from.HasValue())
            {
                throw new ArgumentNullException(nameof(from), "From address must be set.");
            }

            if (!to.HasValue())
            {
                throw new ArgumentNullException(nameof(to), "To address must be set.");
            }

            From = from;
            To = to;
        }

        public ChangeVaultOwnerLog(long id, long transactionId, string address, int sortOrder, string details)
            : base(TransactionLogType.ChangeVaultOwnerLog, id, transactionId, address, sortOrder)
        {
            var logDetails = DeserializeLogDetails(details);
            From = logDetails.From;
            To = logDetails.To;
        }

        public string From { get; }
        public string To { get; }

        private struct LogDetails
        {
            public string From { get; set; }
            public string To { get; set; }
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