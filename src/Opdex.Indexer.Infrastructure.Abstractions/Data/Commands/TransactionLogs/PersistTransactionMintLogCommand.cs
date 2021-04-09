using System;
using MediatR;
using Opdex.Core.Domain.Models.TransactionLogs;

namespace Opdex.Indexer.Infrastructure.Abstractions.Data.Commands.TransactionLogs
{
    public class PersistTransactionMintLogCommand : IRequest<long>
    {
        public PersistTransactionMintLogCommand(MintLog mintLog)
        {
            MintLog = mintLog ?? throw new ArgumentNullException(nameof(mintLog));
        }
        
        public MintLog MintLog { get; }
    }
}