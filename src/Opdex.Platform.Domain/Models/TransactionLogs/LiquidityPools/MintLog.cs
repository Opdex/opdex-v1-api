using System;
using Newtonsoft.Json;
using Opdex.Platform.Common;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;

namespace Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools
{
    public class MintLog : TransactionLog
    {
        public MintLog(dynamic log, Address address, int sortOrder)
            : base(TransactionLogType.MintLog, address, sortOrder)
        {
            Address sender = log?.sender;
            Address to = log?.to;
            ulong amountCrs = log?.amountCrs;
            UInt256 amountSrc = UInt256.Parse(log?.amountSrc);
            UInt256 amountLpt = UInt256.Parse(log?.amountLpt);
            UInt256 totalSupply = UInt256.Parse(log?.totalSupply);

            if (sender == Address.Empty)
            {
                throw new ArgumentNullException(nameof(sender), "Sender address must be set.");
            }

            if (to == Address.Empty)
            {
                throw new ArgumentNullException(nameof(to), "To address must be set.");
            }

            if (amountCrs < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(amountCrs), "CRS amount must be greater than 0.");
            }

            Sender = sender;
            To = to;
            AmountCrs = amountCrs;
            AmountSrc = amountSrc;
            AmountLpt = amountLpt;
            TotalSupply = totalSupply;
        }

        public MintLog(long id, long transactionId, Address address, int sortOrder, string details)
            : base(TransactionLogType.MintLog, id, transactionId, address, sortOrder)
        {
            var logDetails = DeserializeLogDetails(details);
            Sender = logDetails.Sender;
            To = logDetails.To;
            AmountCrs = logDetails.AmountCrs;
            AmountSrc = logDetails.AmountSrc;
            AmountLpt = logDetails.AmountLpt;
            TotalSupply = logDetails.TotalSupply;
        }

        public Address Sender { get; }
        public Address To { get; }
        public ulong AmountCrs { get; }
        public UInt256 AmountSrc { get; }
        public UInt256 AmountLpt { get; }
        public UInt256 TotalSupply { get; }

        private struct LogDetails
        {
            public Address Sender { get; set; }
            public Address To { get; set; }
            public ulong AmountCrs { get; set; }
            public UInt256 AmountSrc { get; set; }
            public UInt256 AmountLpt { get; set; }
            public UInt256 TotalSupply { get; set; }
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
                AmountCrs = AmountCrs,
                AmountSrc = AmountSrc,
                AmountLpt = AmountLpt,
                TotalSupply = TotalSupply
            });
        }
    }
}
