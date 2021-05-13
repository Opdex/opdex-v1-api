using MediatR;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vault
{
    public class SelectVaultQuery : IRequest<Domain.Models.ODX.Vault>
    {
    }
}