using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Addresses
{
    public class SelectAddressAllowancesByOwnerWithFilterQueryHandler
        : IRequestHandler<SelectAddressAllowancesByOwnerWithFilterQuery, IEnumerable<AddressAllowance>>
    {
        private const string WhereFilter = "{WhereFilter}";

        private static readonly string SqlQuery =
            $@"SELECT
                {nameof(AddressAllowanceEntity.Id)},
                {nameof(AddressAllowanceEntity.TokenId)},
                {nameof(AddressAllowanceEntity.Owner)},
                {nameof(AddressAllowanceEntity.Spender)},
                {nameof(AddressAllowanceEntity.Allowance)},
                {nameof(AddressAllowanceEntity.CreatedBlock)},
                {nameof(AddressAllowanceEntity.ModifiedBlock)}
            FROM address_allowance {WhereFilter};";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectAddressAllowancesByOwnerWithFilterQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<AddressAllowance>> Handle(SelectAddressAllowancesByOwnerWithFilterQuery request, CancellationToken cancellationToken)
        {
            var queryParams = new SqlParams(request.Owner, request.Spender, request.TokenId);
            var query = DatabaseQuery.Create(QueryBuilder(request), queryParams, cancellationToken);

            var result = await _context.ExecuteQueryAsync<AddressAllowanceEntity>(query);

            return _mapper.Map<IEnumerable<AddressAllowance>>(result);
        }

        private static string QueryBuilder(SelectAddressAllowancesByOwnerWithFilterQuery request)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("WHERE {0} = @{1}", nameof(AddressAllowanceEntity.Owner), nameof(SqlParams.Owner));

            if (request.Spender.HasValue())
            {
                stringBuilder.AppendFormat(" AND {0} = @{1}", nameof(AddressAllowanceEntity.Spender), nameof(SqlParams.Spender));
            }

            if (request.TokenId != 0)
            {
                stringBuilder.AppendFormat(" AND {0} = @{1}", nameof(AddressAllowanceEntity.TokenId), nameof(SqlParams.TokenId));
            }

            return SqlQuery.Replace(WhereFilter, stringBuilder.ToString());
        }

        private sealed class SqlParams
        {
            internal SqlParams(string owner, string spender, long tokenId)
            {
                Owner = owner;
                Spender = spender;
                TokenId = tokenId;
            }

            public string Owner { get; }
            public string Spender { get; }
            public long TokenId { get; }
        }
    }
}
