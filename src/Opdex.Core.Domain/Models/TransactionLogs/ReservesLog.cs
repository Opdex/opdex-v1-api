using System;
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
        
        public ReservesLog(long id, long transactionId, string address, int sortOrder, ulong reserveCrs, string reserveSrc)
            : base(nameof(ReservesLog), id, transactionId, address, sortOrder)
        {
            ReserveCrs = reserveCrs;
            ReserveSrc = reserveSrc;
        }
        
        public ulong ReserveCrs { get; }
        public string ReserveSrc { get; }
    }
}