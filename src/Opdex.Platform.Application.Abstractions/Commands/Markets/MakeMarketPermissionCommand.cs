using MediatR;
using Opdex.Platform.Domain.Models.Markets;

namespace Opdex.Platform.Application.Abstractions.Commands.Markets;

/// <summary>
/// Create a new make market permission command to persist an instance of a market.
/// </summary>
public class MakeMarketPermissionCommand : IRequest<ulong>
{
    /// <summary>
    /// Constructor to create a make market permission command.
    /// </summary>
    /// <param name="marketPermission">The market permission domain model to upsert to the database.</param>
    public MakeMarketPermissionCommand(MarketPermission marketPermission)
    {
        MarketPermission = marketPermission;
    }

    public MarketPermission MarketPermission { get; }
}