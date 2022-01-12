using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Transactions.TransactionLogs;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Transactions;

public class SelectTransactionForVaultProposalVoteRewindQueryHandler
    : IRequestHandler<SelectTransactionForVaultProposalVoteRewindQuery, Transaction>
{
    private const string Split = "Split";

    private static readonly string SqlQuery =
        @$"SELECT
                t.{nameof(TransactionEntity.Id)},
                t.`{nameof(TransactionEntity.To)}`,
                t.`{nameof(TransactionEntity.From)}`,
                t.{nameof(TransactionEntity.Hash)},
                t.{nameof(TransactionEntity.Block)},
                t.{nameof(TransactionEntity.GasUsed)},
                t.{nameof(TransactionEntity.NewContractAddress)},
                t.{nameof(TransactionEntity.Success)},
                t.{nameof(TransactionEntity.Error)},
                true as {Split},
                tl.{nameof(TransactionLogEntity.Id)},
                tl.{nameof(TransactionLogEntity.TransactionId)},
                tl.{nameof(TransactionLogEntity.LogTypeId)},
                tl.{nameof(TransactionLogEntity.SortOrder)},
                tl.{nameof(TransactionLogEntity.Contract)},
                tl.{nameof(TransactionLogEntity.Details)}
            FROM transaction t
            LEFT JOIN transaction_log tl
                ON t.{nameof(TransactionEntity.Id)} = tl.{nameof(TransactionLogEntity.TransactionId)}
            WHERE t.`{nameof(TransactionEntity.From)}` = @{nameof(SqlParams.Voter)} AND
                  tl.{nameof(TransactionLogEntity.Contract)} = @{nameof(SqlParams.Vault)} AND
                  tl.{nameof(TransactionLogEntity.LogTypeId)} IN @{nameof(SqlParams.LogTypes)} AND
                  JSON_EXTRACT(tl.{nameof(TransactionLogEntity.Details)}, '$.proposalId') = @{nameof(SqlParams.ProposalId)}
            ORDER BY t.{nameof(TransactionEntity.Block)} DESC
            LIMIT 5;".RemoveExcessWhitespace();

    private readonly IDbContext _context;
    private readonly IMapper _mapper;

    public SelectTransactionForVaultProposalVoteRewindQueryHandler(IDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<Transaction> Handle(SelectTransactionForVaultProposalVoteRewindQuery request, CancellationToken cancellationToken)
    {
        var query = DatabaseQuery.Create(SqlQuery, new SqlParams(request.Vault, request.Voter, request.ProposalPublicId), cancellationToken);

        var transactions = new Dictionary<ulong, Transaction>();

        await _context.ExecuteQueryAsync<TransactionEntity, TransactionLogEntity, Transaction>(query, (transaction, log) =>
        {
            var exists = transactions.TryGetValue(transaction.Id, out Transaction tx);
            if (!exists) tx = _mapper.Map<Transaction>(transaction);

            if (log != null)
            {
                tx.SetLog(_mapper.Map<TransactionLog>(log));
            }

            transactions[tx.Id] = tx;

            return tx;
        }, splitOn: Split);

        return transactions.Values.OrderByDescending(tx => tx.BlockHeight).FirstOrDefault();
    }

    private sealed class SqlParams
    {
        internal SqlParams(Address vault, Address voter, ulong proposalId)
        {
            Vault = vault;
            Voter = voter;
            ProposalId = proposalId;
        }

        public Address Vault { get; }
        public Address Voter { get; }
        public ulong ProposalId { get; }

        // Cannot be static or Dapper will fail: see this bug - https://github.com/DapperLib/Dapper/issues/621
        public List<uint> LogTypes => new()
        {
            (uint)TransactionLogType.VaultProposalVoteLog,
            (uint)TransactionLogType.VaultProposalWithdrawVoteLog
        };
    }
}
