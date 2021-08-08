using MediatR;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Common.Extensions;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Transactions
{
    public class GetTransactionByHashQuery : IRequest<TransactionDto>
    {
        public GetTransactionByHashQuery(string hash)
        {
            Hash = hash.HasValue() ? hash : throw new ArgumentNullException(nameof(hash), "Hash must be provided.");
        }

        public string Hash { get; }
    }
}
