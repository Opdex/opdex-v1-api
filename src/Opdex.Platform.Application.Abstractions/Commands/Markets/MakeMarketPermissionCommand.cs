using MediatR;
using Opdex.Platform.Domain.Models.Markets;

namespace Opdex.Platform.Application.Abstractions.Commands.Markets
{
    public class MakeMarketPermissionCommand : IRequest<long>
    {
        public MakeMarketPermissionCommand(MarketPermission marketPermission)
        {
            MarketPermission = marketPermission;
        }

        public MarketPermission MarketPermission { get; }
    }
}