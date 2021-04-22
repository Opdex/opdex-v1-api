using System.Collections.Generic;
using MediatR;
using Opdex.Platform.Application.Abstractions.Models;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Tokens
{
    public class GetAllTokensQuery : IRequest<IEnumerable<TokenDto>>
    {
    }
}