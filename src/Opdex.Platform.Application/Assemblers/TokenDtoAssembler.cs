using AutoMapper;
using MediatR;
using Opdex.Platform.Application.Abstractions.Models.TokenDtos;
using Opdex.Platform.Application.Abstractions.Queries.Pools;
using Opdex.Platform.Application.Abstractions.Queries.Pools.Snapshots;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Tokens.Snapshots;
using Opdex.Platform.Common;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Tokens;
using System;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Assemblers
{
    public class TokenDtoAssembler : IModelAssembler<Token, TokenDto>
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        private const SnapshotType SnapshotType = Common.SnapshotType.Hourly;

        public TokenDtoAssembler(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        // Todo: A stale liquidity pool token snapshot is dependent on an updated liquidity pool reserves USD.
        // reserves USD is dependant on CRS and SRC token price in the pool.
        // Current implementation does not refresh a stale pool snapshot when trying to refresh a stale lp token snapshot
        public async Task<TokenDto> Assemble(Token token)
        {
            var now = DateTime.UtcNow;
            var tokenDto = _mapper.Map<TokenDto>(token);

            if (!token.MarketId.HasValue)
            {
                return tokenDto;
            }

            var marketId = token.MarketId.Value;
            var tokenSnapshot = await _mediator.Send(new RetrieveTokenSnapshotWithFilterQuery(token.Id, marketId, now, SnapshotType));

            var tokenIsCrs = token.Address == TokenConstants.Cirrus.Address;

            // Update stale snapshot if the token being assembled is not CRS
            if (tokenSnapshot.EndDate < now && !tokenIsCrs)
            {
                // Get crs and snapshot
                var crs = await _mediator.Send(new RetrieveTokenByAddressQuery(TokenConstants.Cirrus.Address));
                var crsSnapshot = await _mediator.Send(new RetrieveTokenSnapshotWithFilterQuery(crs.Id, 0, now, SnapshotType));

                if (token.IsLpt)
                {
                    // get pool and snapshot
                    var pool = await _mediator.Send(new RetrieveLiquidityPoolByLpTokenIdQuery(token.Id));
                    var poolSnapshot = await _mediator.Send(new RetrieveLiquidityPoolSnapshotWithFilterQuery(pool.Id, now, SnapshotType));
                    var lptUsd = token.TotalSupply.FiatPerToken(poolSnapshot.Reserves.Usd, TokenConstants.LiquidityPoolToken.Sats);

                    tokenSnapshot.ResetStaleSnapshot(lptUsd, now);
                }
                else
                {
                    // get pool and snapshot
                    var pool = await _mediator.Send(new RetrieveLiquidityPoolBySrcTokenIdAndMarketIdQuery(token.Id, marketId));
                    var poolSnapshot = await _mediator.Send(new RetrieveLiquidityPoolSnapshotWithFilterQuery(pool.Id, now, SnapshotType));

                    // Calc number of CRS per SRC token in pool
                    var crsPerSrc = poolSnapshot.Reserves.Crs.Token0PerToken1(poolSnapshot.Reserves.Src, token.Sats);

                    // Reset stale snapshot with crsPerSrc and crs USD cost
                    tokenSnapshot.ResetStaleSnapshot(crsPerSrc, crsSnapshot.Price.Close, now);
                }
            }

            tokenDto.Summary = _mapper.Map<TokenSnapshotDto>(tokenSnapshot);

            return tokenDto;
        }
    }
}
