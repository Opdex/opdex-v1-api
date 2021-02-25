using System;
using Opdex.Core.Common.Extensions;

namespace Opdex.Core.Domain.Models.TransactionReceipt.LogEvents
{
    public class SyncEvent : LogEventBase
    {
        public SyncEvent(dynamic log) : base(nameof(SyncEvent))
        {
            ulong reserveCrs = log?.ReserveCrs;
            string reserveSrc = log?.ReserveSrc;
            
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
        
        public ulong ReserveCrs { get; }
        public string ReserveSrc { get; }
    }
}