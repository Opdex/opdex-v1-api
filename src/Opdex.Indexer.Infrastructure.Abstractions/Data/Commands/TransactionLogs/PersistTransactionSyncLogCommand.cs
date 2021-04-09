using System;
using MediatR;
using Opdex.Core.Domain.Models.TransactionLogs;

namespace Opdex.Indexer.Infrastructure.Abstractions.Data.Commands.TransactionLogs
{
    public class PersistTransactionReservesLogCommand : IRequest<long>
    {
        public PersistTransactionReservesLogCommand(ReservesLog syncLog)
        {
            ReservesLog = syncLog ?? throw new ArgumentNullException(nameof(syncLog));
        }
        
        public ReservesLog ReservesLog { get; }
    }
}