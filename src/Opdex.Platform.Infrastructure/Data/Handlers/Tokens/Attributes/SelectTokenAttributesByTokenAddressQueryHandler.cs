using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens.Attributes;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Tokens.Attributes;

public class SelectTokenAttributesByTokenAddressQueryHandler : IRequestHandler<SelectTokenAttributesByTokenAddressQuery, IEnumerable<TokenAttribute>>
{
    private static readonly string SqlQuery =
        @$"SELECT
                ta.{nameof(TokenAttributeEntity.Id)},
                ta.{nameof(TokenAttributeEntity.TokenId)},
                ta.{nameof(TokenAttributeEntity.AttributeTypeId)}
            FROM token_attribute ta JOIN token t
                ON ta.{nameof(TokenAttributeEntity.TokenId)} = t.{nameof(TokenEntity.Id)}
            WHERE t.{nameof(TokenEntity.Address)} = @{nameof(SqlParams.TokenAddress)};".RemoveExcessWhitespace();

    private readonly IDbContext _context;
    private readonly IMapper _mapper;

    public SelectTokenAttributesByTokenAddressQueryHandler(IDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<IEnumerable<TokenAttribute>> Handle(SelectTokenAttributesByTokenAddressQuery request, CancellationToken cancellationToken)
    {
        var query = DatabaseQuery.Create(SqlQuery, new SqlParams(request.Token), cancellationToken);

        var result = await _context.ExecuteQueryAsync<TokenAttributeEntity>(query);

        return _mapper.Map<IEnumerable<TokenAttribute>>(result);
    }

    private sealed record SqlParams(Address TokenAddress);
}
