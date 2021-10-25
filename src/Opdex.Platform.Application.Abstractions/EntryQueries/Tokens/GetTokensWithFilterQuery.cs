using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Tokens;
using System;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Tokens
{
    public class GetTokensWithFilterQuery : IRequest<TokensDto>
    {
        public GetTokensWithFilterQuery(TokensCursor cursor)
        {
            Cursor = cursor ?? throw new ArgumentNullException(nameof(cursor), "Tokens cursor must be set.");
        }

        public TokensCursor Cursor { get; }
    }
}
