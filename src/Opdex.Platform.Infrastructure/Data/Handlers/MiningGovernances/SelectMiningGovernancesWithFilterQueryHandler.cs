using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.MiningGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.MiningGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.MiningGovernances;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.MiningGovernances
{
    public class SelectMiningGovernancesWithFilterQueryHandler : IRequestHandler<SelectMiningGovernancesWithFilterQuery, IEnumerable<MiningGovernance>>
    {
        private const string TableJoins = "{TableJoins}";
        private const string WhereFilter = "{WhereFilter}";
        private const string OrderBy = "{OrderBy}";
        private const string Limit = "{Limit}";

        private static readonly string SqlQuery =
            @$"SELECT
                g.{nameof(MiningGovernanceEntity.Id)},
                g.{nameof(MiningGovernanceEntity.Address)},
                g.{nameof(MiningGovernanceEntity.TokenId)},
                g.{nameof(MiningGovernanceEntity.NominationPeriodEnd)},
                g.{nameof(MiningGovernanceEntity.MiningDuration)},
                g.{nameof(MiningGovernanceEntity.MiningPoolsFunded)},
                g.{nameof(MiningGovernanceEntity.MiningPoolReward)},
                g.{nameof(MiningGovernanceEntity.NominationPeriodEnd)},
                g.{nameof(MiningGovernanceEntity.CreatedBlock)},
                g.{nameof(MiningGovernanceEntity.ModifiedBlock)}
            FROM mining_governance g
            {TableJoins}
            {WhereFilter}
            {OrderBy}
            {Limit}".RemoveExcessWhitespace();

        private const string InnerQuery = "{InnerQuery}";
        private const string SortDirection = "{SortDirection}";

        private static readonly string PagingBackwardQuery =
            @$"SELECT * FROM ({InnerQuery}) results ORDER BY results.{nameof(MiningGovernanceEntity.Id)} {SortDirection};";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectMiningGovernancesWithFilterQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<MiningGovernance>> Handle(SelectMiningGovernancesWithFilterQuery request, CancellationToken cancellationToken)
        {
            var miningGovernanceId = request.Cursor.Pointer;

            var queryParams = new SqlParams(miningGovernanceId, request.Cursor.MinedToken);

            var query = DatabaseQuery.Create(QueryBuilder(request), queryParams, cancellationToken);

            var results = await _context.ExecuteQueryAsync<MiningGovernanceEntity>(query);

            return _mapper.Map<IEnumerable<MiningGovernance>>(results);
        }

        private static string QueryBuilder(SelectMiningGovernancesWithFilterQuery request)
        {
            var whereFilter = string.Empty;
            var tableJoins = string.Empty;

            var filterOnMinedToken = request.Cursor.MinedToken != Address.Empty;

            if (filterOnMinedToken) tableJoins += $" JOIN token t ON t.{nameof(TokenEntity.Id)} = g.{nameof(MiningGovernanceEntity.TokenId)}";

            if (!request.Cursor.IsFirstRequest)
            {
                var sortOperator = string.Empty;

                // going forward in ascending order, use greater than
                if (request.Cursor.PagingDirection == PagingDirection.Forward && request.Cursor.SortDirection == SortDirectionType.ASC) sortOperator = ">";

                // going forward in descending order, use less than or equal to
                if (request.Cursor.PagingDirection == PagingDirection.Forward && request.Cursor.SortDirection == SortDirectionType.DESC) sortOperator = "<";

                // going backward in ascending order, use less than
                if (request.Cursor.PagingDirection == PagingDirection.Backward && request.Cursor.SortDirection == SortDirectionType.ASC) sortOperator = "<";

                // going backward in descending order, use greater than
                if (request.Cursor.PagingDirection == PagingDirection.Backward && request.Cursor.SortDirection == SortDirectionType.DESC) sortOperator = ">";

                whereFilter += $" WHERE g.{nameof(MiningGovernanceEntity.Id)} {sortOperator} @{nameof(SqlParams.MiningGovernanceId)}";
            }

            if (filterOnMinedToken)
            {
                whereFilter += whereFilter == "" ? " WHERE" : " AND";
                whereFilter += $" t.{nameof(TokenEntity.Address)} = @{nameof(SqlParams.MinedToken)}";
            }

            // Set the direction, moving backwards with previous requests, the sort order must be reversed first.
            string direction;

            if (request.Cursor.PagingDirection == PagingDirection.Backward)
            {
                direction = request.Cursor.SortDirection == SortDirectionType.DESC ? nameof(SortDirectionType.ASC) : nameof(SortDirectionType.DESC);
            }
            else
            {
                direction = Enum.GetName(typeof(SortDirectionType), request.Cursor.SortDirection);
            }

            var orderBy = $" ORDER BY g.{nameof(MiningGovernanceEntity.Id)} {direction}";

            var limit = $" LIMIT {request.Cursor.Limit + 1}";

            var query = SqlQuery.Replace(TableJoins, tableJoins)
                                .Replace(WhereFilter, whereFilter)
                                .Replace(OrderBy, orderBy)
                                .Replace(Limit, limit);

            if (request.Cursor.PagingDirection == PagingDirection.Forward) return $"{query};";
            // re-sort back into requested order
            else return PagingBackwardQuery.Replace(InnerQuery, query)
                                           .Replace(SortDirection, Enum.GetName(typeof(SortDirectionType), request.Cursor.SortDirection));
        }

        private sealed class SqlParams
        {
            internal SqlParams(ulong miningGovernanceId, Address minedToken)
            {
                MiningGovernanceId = miningGovernanceId;
                MinedToken = minedToken;
            }

            public ulong MiningGovernanceId { get; }
            public Address MinedToken { get; }
        }
    }
}
