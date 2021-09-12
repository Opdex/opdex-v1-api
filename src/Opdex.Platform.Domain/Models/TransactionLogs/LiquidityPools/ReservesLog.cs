using System;
using Newtonsoft.Json;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;

namespace Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools
{
    public class ReservesLog : TransactionLog
    {
        public ReservesLog(dynamic log, Address address, int sortOrder)
            : base(TransactionLogType.ReservesLog, address, sortOrder)
        {
            ulong reserveCrs = log?.reserveCrs;
            UInt256 reserveSrc = UInt256.Parse((string)log?.reserveSrc);

            if (reserveCrs == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(reserveCrs), "Reserve CRS must be greater than 0.");
            }

            if (reserveSrc == UInt256.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(reserveSrc), "Reserve SRC must be greater than 0.");
            }

            ReserveCrs = reserveCrs;
            ReserveSrc = reserveSrc;
        }

        public ReservesLog(long id, long transactionId, Address address, int sortOrder, string details)
            : base(TransactionLogType.ReservesLog, id, transactionId, address, sortOrder)
        {
            var logDetails = DeserializeLogDetails(details);
            ReserveCrs = logDetails.ReserveCrs;
            ReserveSrc = logDetails.ReserveSrc;
        }

        public ulong ReserveCrs { get; }
        public UInt256 ReserveSrc { get; }

        private struct LogDetails
        {
            public ulong ReserveCrs { get; set; }
            public UInt256 ReserveSrc { get; set; }
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
    }
}
