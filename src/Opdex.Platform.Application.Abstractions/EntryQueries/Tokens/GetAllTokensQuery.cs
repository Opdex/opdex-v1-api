using System.Collections.Generic;
using MediatR;
using Opdex.Core.Application.Abstractions.Models;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Tokens
{
    public class GetAllTokensQuery : IRequest<IEnumerable<TokenDto>>
    {
    }
}