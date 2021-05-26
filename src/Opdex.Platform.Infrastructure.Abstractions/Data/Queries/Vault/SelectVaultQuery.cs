using MediatR;
using Opdex.Platform.Common.Queries;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vault
{
    public class SelectVaultQuery : FindQuery<Domain.Models.ODX.Vault>
    {
        public SelectVaultQuery(bool findOrThrow) : base(findOrThrow)
        {
        }
    }
}