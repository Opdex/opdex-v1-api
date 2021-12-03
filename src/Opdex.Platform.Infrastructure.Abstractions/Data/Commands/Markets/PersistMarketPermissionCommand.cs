using MediatR;
using Opdex.Platform.Domain.Models.Markets;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Markets;

public class PersistMarketPermissionCommand : IRequest<ulong>
{
    public PersistMarketPermissionCommand(MarketPermission marketPermission)
    {
        MarketPermission = marketPermission;
    }

    public MarketPermission MarketPermission { get; }
}