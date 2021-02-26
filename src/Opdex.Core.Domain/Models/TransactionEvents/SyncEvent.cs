using System;
using Opdex.Core.Common.Extensions;

namespace Opdex.Core.Domain.Models.TransactionEvents
{
    public class SyncEvent : TransactionEvent
    {
        public SyncEvent(dynamic log, string address, int sortOrder) 
            : base(nameof(SyncEvent), address, sortOrder)
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