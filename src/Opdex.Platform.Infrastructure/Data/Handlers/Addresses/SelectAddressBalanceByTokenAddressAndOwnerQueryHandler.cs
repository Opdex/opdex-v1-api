using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Addresses
{
    public class SelectAddressBalanceByTokenAddressAndOwnerQueryHandler
        : IRequestHandler<SelectAddressBalanceByTokenAddressAndOwnerQuery, AddressBalance>
    {
        private static readonly string SqlQuery =
            @$"SELECT
                ab.{nameof(AddressBalanceEntity.Id)},
                ab.{nameof(AddressBalanceEntity.TokenId)},
                ab.{nameof(AddressBalanceEntity.Owner)},
                ab.{nameof(AddressBalanceEntity.Balance)},
                ab.{nameof(AddressBalanceEntity.CreatedBlock)},
                ab.{nameof(AddressBalanceEntity.ModifiedBlock)}
            FROM address_balance ab
            JOIN token t ON t.{nameof(TokenEntity.Id)} = ab.{nameof(AddressAllowance.TokenId)}
            WHERE ab.{nameof(AddressBalanceEntity.Owner)} = @{nameof(SqlParams.Owner)} AND
                   t.{nameof(TokenEntity.Address)} = @{nameof(SqlParams.TokenAddress)}
            LIMIT 1;";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectAddressBalanceByTokenAddressAndOwnerQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<AddressBalance> Handle(SelectAddressBalanceByTokenAddressAndOwnerQuery request, CancellationToken cancellationToken)
        {
            var queryParams = new SqlParams(request.TokenAddress, request.Owner);
            var query = DatabaseQuery.Create(SqlQuery, queryParams, cancellationToken);

            var result = await _context.ExecuteFindAsync<AddressBalanceEntity>(query);

            if (request.FindOrThrow && result == null)
            {
                throw new NotFoundException($"{nameof(AddressBalance)} not found.");
            }

            return result == null ? null : _mapper.Map<AddressBalance>(result);
        }

        private sealed class SqlParams
        {
            internal SqlParams(string tokenAddress, string owner)
            {
                TokenAddress = tokenAddress;
                Owner = owner;
            }

            public string TokenAddress { get; }
            public string Owner { get; }
        }
    }
}
