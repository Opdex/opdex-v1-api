using System;
using Newtonsoft.Json;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;

namespace Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools
{
    public class SwapLog : TransactionLog
    {
        public SwapLog(dynamic log, Address address, int sortOrder)
            : base(TransactionLogType.SwapLog, address, sortOrder)
        {
            Address sender =(string)log?.sender;
            Address to = (string)log?.to;
            ulong amountCrsIn = log?.amountCrsIn;
            ulong amountCrsOut = log?.amountCrsOut;
            UInt256 amountSrcIn = UInt256.Parse((string)log?.amountSrcIn);
            UInt256 amountSrcOut = UInt256.Parse((string)log?.amountSrcOut);

            if (sender == Address.Empty)
            {
                throw new ArgumentNullException(nameof(sender), "Sender address must be set.");
            }

            if (to == Address.Empty)
            {
                throw new ArgumentNullException(nameof(to), "To address must be set.");
            }

            if (amountCrsIn == 0 && amountCrsOut == 0)
            {
                throw new ArgumentException($"{nameof(amountCrsIn)} or {nameof(amountCrsOut)} must be greater than 0.");
            }

            if (amountSrcIn == UInt256.Zero && amountSrcOut == UInt256.Zero)
            {
                throw new ArgumentException($"{nameof(amountSrcIn)} or {nameof(amountSrcOut)} must be greater than 0.");
            }

            Sender = sender;
            To = to;
            AmountCrsIn = amountCrsIn;
            AmountCrsOut = amountCrsOut;
            AmountSrcIn = amountSrcIn;
            AmountSrcOut = amountSrcOut;
        }

        public SwapLog(ulong id, ulong transactionId, Address address, int sortOrder, string details)
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

        public Address Sender { get; }
        public Address To { get; }
        public ulong AmountCrsIn { get; }
        public UInt256 AmountSrcIn { get; }
        public ulong AmountCrsOut { get; }
        public UInt256 AmountSrcOut { get; }

        private struct LogDetails
        {
            public Address Sender { get; set; }
            public Address To { get; set; }
            public ulong AmountCrsIn { get; set; }
            public UInt256 AmountSrcIn { get; set; }
            public ulong AmountCrsOut { get; set; }
            public UInt256 AmountSrcOut { get; set; }
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
