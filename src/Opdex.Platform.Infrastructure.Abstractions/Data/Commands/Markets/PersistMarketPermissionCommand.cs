using MediatR;
using Opdex.Platform.Domain.Models.Markets;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Markets
{
    public class PersistMarketPermissionCommand : IRequest<long>
    {
        public PersistMarketPermissionCommand(MarketPermission marketPermission)
        {
            MarketPermission = marketPermission;
        }

        public MarketPermission MarketPermission { get; }
    }
}