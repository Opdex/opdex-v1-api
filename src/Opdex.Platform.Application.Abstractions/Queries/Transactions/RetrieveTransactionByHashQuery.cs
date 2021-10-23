using System;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Transactions;

namespace Opdex.Platform.Application.Abstractions.Queries.Transactions
{
    public class RetrieveTransactionByHashQuery : FindQuery<Transaction>
    {
        public RetrieveTransactionByHashQuery(Sha256 hash, bool findOrThrow = true) : base(findOrThrow)
        {
            Hash = hash;
        }

        public Sha256 Hash { get; }
    }
}
