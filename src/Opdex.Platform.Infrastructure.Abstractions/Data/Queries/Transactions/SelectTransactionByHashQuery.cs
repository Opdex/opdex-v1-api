using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Transactions;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Transactions;

public class SelectTransactionByHashQuery : FindQuery<Transaction>
{
    public SelectTransactionByHashQuery(Sha256 hash, bool findOrThrow = true)
        : base(findOrThrow)
    {
        Hash = hash;
    }

    public Sha256 Hash { get; }
}