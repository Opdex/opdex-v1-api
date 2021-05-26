using MediatR;
using Opdex.Platform.Common.Queries;

namespace Opdex.Platform.Application.Abstractions.Queries.Vault
{
    public class RetrieveVaultQuery : FindQuery<Domain.Models.ODX.Vault>
    {
        public RetrieveVaultQuery(bool findOrThrow) : base(findOrThrow)
        {
            
        }
    }
}