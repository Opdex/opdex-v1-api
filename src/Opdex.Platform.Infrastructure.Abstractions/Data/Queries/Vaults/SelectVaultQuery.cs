using MediatR;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.ODX;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults
{
    public class SelectVaultQuery : FindQuery<Vault>
    {
        public SelectVaultQuery(bool findOrThrow) : base(findOrThrow)
        {
        }
    }
}
