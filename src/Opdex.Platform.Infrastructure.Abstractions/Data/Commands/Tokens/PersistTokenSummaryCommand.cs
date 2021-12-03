using MediatR;
using Opdex.Platform.Domain.Models.Tokens;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Tokens;

public class PersistTokenSummaryCommand : IRequest<ulong>
{
    public PersistTokenSummaryCommand(TokenSummary tokenSummary)
    {
        TokenSummary = tokenSummary ?? throw new ArgumentNullException(nameof(tokenSummary), "Token summary must be provided");
    }

    public TokenSummary TokenSummary { get; }
}