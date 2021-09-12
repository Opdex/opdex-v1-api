using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.Governances;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Governances
{
    public class ProcessNominationLogCommand : ProcessTransactionLogCommand
    {
        public ProcessNominationLogCommand(TransactionLog log, Address sender, ulong blockHeight) : base(sender, blockHeight)
        {
            Log = log as NominationLog ?? throw new ArgumentNullException(nameof(log));
        }

        public NominationLog Log { get; }
    }
}
