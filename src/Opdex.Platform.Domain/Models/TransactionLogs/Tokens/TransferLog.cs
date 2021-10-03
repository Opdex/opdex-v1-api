using System;
using Newtonsoft.Json;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;

namespace Opdex.Platform.Domain.Models.TransactionLogs.Tokens
{
    public class TransferLog : TransactionLog
    {
        public TransferLog(dynamic log, Address address, int sortOrder)
            : base(TransactionLogType.TransferLog, address, sortOrder)
        {
            Address from = (string)log?.from;
            Address to = (string)log?.to;
            UInt256 amount = UInt256.Parse((string)log?.amount);

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
            Amount = amount;
        }

        public TransferLog(ulong id, ulong transactionId, Address address, int sortOrder, string details)
            : base(TransactionLogType.TransferLog, id, transactionId, address, sortOrder)
        {
            var logDetails = DeserializeLogDetails(details);
            From = logDetails.From;
            To = logDetails.To;
            Amount = logDetails.Amount;
        }

        public Address From { get; }
        public Address To { get; }
        public UInt256 Amount { get; }

        private struct LogDetails
        {
            public Address From { get; set; }
            public Address To { get; set; }
            public UInt256 Amount { get; set; }
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
                To = To,
                Amount = Amount
            });
        }
    }
}
