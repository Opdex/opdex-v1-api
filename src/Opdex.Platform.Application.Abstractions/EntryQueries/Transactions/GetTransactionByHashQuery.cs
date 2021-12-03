using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Transactions;

public class GetTransactionByHashQuery : IRequest<TransactionDto>
{
    public GetTransactionByHashQuery(Sha256 hash)
    {
        Hash = hash;
    }

    public Sha256 Hash { get; }
}