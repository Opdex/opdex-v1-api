using System;
using Newtonsoft.Json;
using Opdex.Platform.Common;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools
{
    public class ReservesLog : TransactionLog
    {
        public ReservesLog(dynamic log, string address, int sortOrder)
            : base(TransactionLogType.ReservesLog, address, sortOrder)
        {
            ulong reserveCrs = log?.reserveCrs;
            string reserveSrc = log?.reserveSrc;

            if (reserveCrs < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(reserveCrs), "Reserve CRS must be greater than 0.");
            }

            if (!reserveSrc.IsNumeric())
            {
                throw new ArgumentOutOfRangeException(nameof(reserveSrc), "Reserve SRC must only contain numeric digits.");
            }

            ReserveCrs = reserveCrs;
            ReserveSrc = reserveSrc;
        }

        public ReservesLog(long id, long transactionId, string address, int sortOrder, string details)
            : base(TransactionLogType.ReservesLog, id, transactionId, address, sortOrder)
        {
            var logDetails = DeserializeLogDetails(details);
            ReserveCrs = logDetails.ReserveCrs;
            ReserveSrc = logDetails.ReserveSrc;
        }

        public ulong ReserveCrs { get; }
        public string ReserveSrc { get; }

        private struct LogDetails
        {
            public ulong ReserveCrs { get; set; }
            public string ReserveSrc { get; set; }
        }

        private static LogDetails DeserializeLogDetails(string details)
        {
            return JsonConvert.DeserializeObject<LogDetails>(details);
        }

        public override string SerializeLogDetails()
        {
            return JsonConvert.SerializeObject(new LogDetails
            {
                ReserveCrs = ReserveCrs,
                ReserveSrc = ReserveSrc
            });
        }

        public string CrsPerSrc(ulong srcSats)
        {
            return (ReserveCrs.ToBigInteger() * srcSats / ReserveSrc.ToBigInteger()).ToString();
        }

        public string SrcPerCrs()
        {
            return (ReserveSrc.ToBigInteger() * TokenConstants.Cirrus.Sats / ReserveCrs.ToBigInteger()).ToString();
        }
    }
}