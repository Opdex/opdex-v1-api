using MediatR;
using Opdex.Platform.Domain.Models.TransactionLogs;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Transactions.TransactionLogs
{
    public class PersistTransactionLogCommand : IRequest<bool>
    {
        public PersistTransactionLogCommand(TransactionLog transactionLog)
        {
            TransactionLog = transactionLog ?? throw new ArgumentNullException(nameof(transactionLog), "Transaction log must be provided.");
        }

        public TransactionLog TransactionLog { get; }
    }
}
