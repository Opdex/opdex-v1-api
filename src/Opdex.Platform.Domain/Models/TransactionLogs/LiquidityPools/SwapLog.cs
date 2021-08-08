using System;
using Newtonsoft.Json;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools
{
    public class SwapLog : TransactionLog
    {
        public SwapLog(dynamic log, string address, int sortOrder)
            : base(TransactionLogType.SwapLog, address, sortOrder)
        {
            string sender = log?.sender;
            string to = log?.to;
            ulong amountCrsIn = log?.amountCrsIn;
            ulong amountCrsOut = log?.amountCrsOut;
            string amountSrcIn = log?.amountSrcIn;
            string amountSrcOut = log?.amountSrcOut;

            if (!sender.HasValue())
            {
                throw new ArgumentNullException(nameof(sender), "Sender address must be set.");
            }

            if (!to.HasValue())
            {
                throw new ArgumentNullException(nameof(to), "To address must be set.");
            }

            if (amountCrsIn < 1 && amountCrsOut < 1)
            {
                throw new ArgumentException($"{nameof(amountCrsIn)} or {nameof(amountCrsOut)} must be greater than 0.");
            }

            if (!amountSrcIn.IsNumeric())
            {
                throw new ArgumentOutOfRangeException(nameof(amountSrcIn), "SRC amount in must only contain numeric digits.");
            }

            if (!amountSrcOut.IsNumeric())
            {
                throw new ArgumentOutOfRangeException(nameof(amountSrcIn), "SRC amount out must only contain numeric digits.");
            }

            // TODO: validate SRC amounts

            Sender = sender;
            To = to;
            AmountCrsIn = amountCrsIn;
            AmountCrsOut = amountCrsOut;
            AmountSrcIn = amountSrcIn;
            AmountSrcOut = amountSrcOut;
        }

        public SwapLog(long id, long transactionId, string address, int sortOrder, string details)
            : base(TransactionLogType.SwapLog, id, transactionId, address, sortOrder)
        {
            var logDetails = DeserializeLogDetails(details);
            Sender = logDetails.Sender;
            To = logDetails.To;
            AmountCrsIn = logDetails.AmountCrsIn;
            AmountSrcIn = logDetails.AmountSrcIn;
            AmountCrsOut = logDetails.AmountCrsOut;
            AmountSrcOut = logDetails.AmountSrcOut;
        }

        public string Sender { get; }
        public string To { get; }
        public ulong AmountCrsIn { get; }
        public string AmountSrcIn { get; }
        public ulong AmountCrsOut { get; }
        public string AmountSrcOut { get; }

        private struct LogDetails
        {
            public string Sender { get; set; }
            public string To { get; set; }
            public ulong AmountCrsIn { get; set; }
            public string AmountSrcIn { get; set; }
            public ulong AmountCrsOut { get; set; }
            public string AmountSrcOut { get; set; }
        }

        private static LogDetails DeserializeLogDetails(string details)
        {
            return JsonConvert.DeserializeObject<LogDetails>(details);
        }

        public override string SerializeLogDetails()
        {
            return JsonConvert.SerializeObject(new LogDetails
            {
                Sender = Sender,
                To = To,
                AmountCrsIn = AmountCrsIn,
                AmountSrcIn = AmountSrcIn,
                AmountCrsOut = AmountCrsOut,
                AmountSrcOut = AmountSrcOut,
            });
        }
    }
}