using System.Collections.Generic;
using MediatR;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.Models.TokenDtos;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Tokens
{
    public class GetAllTokensQuery : IRequest<IEnumerable<TokenDto>>
    {
    }
}