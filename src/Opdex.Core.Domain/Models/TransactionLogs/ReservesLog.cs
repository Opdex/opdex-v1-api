using System;
using Newtonsoft.Json;
using Opdex.Core.Common.Extensions;

namespace Opdex.Core.Domain.Models.TransactionLogs
{
    public class ReservesLog : TransactionLog
    {
        public ReservesLog(dynamic log, string address, int sortOrder) 
            : base(nameof(ReservesLog), address, sortOrder)
        {
            ulong reserveCrs = log?.reserveCrs;
            string reserveSrc = log?.reserveSrc;
            
            if (reserveCrs < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(reserveCrs));
            }
            
            if (!reserveSrc.HasValue())
            {
                throw new ArgumentNullException(nameof(reserveSrc));
            }

            ReserveCrs = reserveCrs;
            ReserveSrc = reserveSrc;
        }
        
        public ReservesLog(long id, long transactionId, string address, int sortOrder, string details)
            : base(nameof(ReservesLog), id, transactionId, address, sortOrder)
        {
            var logDetails = DeserializeLogDetails(details);
            ReserveCrs = logDetails.ReserveCrs;
            ReserveSrc = logDetails.ReserveSrc;
        }
        
        public ulong ReserveCrs { get; }
        public string ReserveSrc { get; }
        
        private sealed class LogDetails
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
    }
}