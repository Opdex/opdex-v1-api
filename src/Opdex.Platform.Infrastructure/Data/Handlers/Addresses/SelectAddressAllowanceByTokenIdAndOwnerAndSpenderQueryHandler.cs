using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Addresses
{
    public class SelectAddressAllowanceByTokenIdAndOwnerAndSpenderQueryHandler
        : IRequestHandler<SelectAddressAllowanceByTokenIdAndOwnerAndSpenderQuery, AddressAllowance>
    {
        private static readonly string SqlQuery =
            @$"SELECT
                {nameof(AddressAllowanceEntity.Id)},
                {nameof(AddressAllowanceEntity.TokenId)},
                {nameof(AddressAllowanceEntity.Owner)},
                {nameof(AddressAllowanceEntity.Spender)},
                {nameof(AddressAllowanceEntity.Allowance)},
                {nameof(AddressAllowanceEntity.CreatedBlock)},
                {nameof(AddressAllowanceEntity.ModifiedBlock)}
            FROM address_allowance
            WHERE {nameof(AddressAllowanceEntity.Owner)} = @{nameof(SqlParams.Owner)} AND
                {nameof(AddressAllowanceEntity.Spender)} = @{nameof(SqlParams.Spender)} AND
                {nameof(AddressAllowanceEntity.TokenId)} = @{nameof(SqlParams.TokenId)}
            LIMIT 1;";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectAddressAllowanceByTokenIdAndOwnerAndSpenderQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<AddressAllowance> Handle(SelectAddressAllowanceByTokenIdAndOwnerAndSpenderQuery request, CancellationToken cancellationToken)
        {
            var queryParams = new SqlParams(request.TokenId, request.Owner, request.Spender);
            var query = DatabaseQuery.Create(SqlQuery, queryParams, cancellationToken);

            var result = await _context.ExecuteFindAsync<AddressAllowanceEntity>(query);

            if (request.FindOrThrow && result == null)
            {
                throw new NotFoundException($"{nameof(AddressAllowance)} not found.");
            }

            return result == null ? null : _mapper.Map<AddressAllowance>(result);
        }

        private sealed class SqlParams
        {
            internal SqlParams(long tokenId, string owner, string spender)
            {
                TokenId = tokenId;
                Owner = owner;
                Spender = spender;
            }

            public long TokenId { get; }
            public string Owner { get; }
            public string Spender { get; }
        }
    }
}
