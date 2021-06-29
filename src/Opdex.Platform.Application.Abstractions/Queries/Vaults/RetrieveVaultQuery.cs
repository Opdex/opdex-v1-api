using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.ODX;

namespace Opdex.Platform.Application.Abstractions.Queries.Vaults
{
    public class RetrieveVaultQuery : FindQuery<Vault>
    {
        public RetrieveVaultQuery(bool findOrThrow) : base(findOrThrow)
        {
        }
    }
}
