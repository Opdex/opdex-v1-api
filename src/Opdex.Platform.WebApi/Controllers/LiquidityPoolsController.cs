using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Opdex.Platform.Application.Abstractions.EntryQueries.LiquidityPools;
using Opdex.Platform.Application.Abstractions.EntryQueries.LiquidityPools.Snapshots;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.WebApi.Models;
using Opdex.Platform.WebApi.Models.Responses.Pools;
using System.Linq;

namespace Opdex.Platform.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("liquidity-pools")]
    public class LiquidityPoolsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IApplicationContext _context;

        public LiquidityPoolsController(IMediator mediator, IMapper mapper, IApplicationContext applicationContext)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
        }

        /// <summary>Get Liquidity Pools</summary>
        /// <remarks>
        /// Retrieve a list of pools that match the filters provided.
        /// </remarks>
        /// <param name="staking">Filter liquidity pools by staking status. Default null is ignored.</param>
        /// <param name="mining">Filter liquidity pools by mining status. Default null is ignored.</param>
        /// <param name="nominated">Filter liquidity pools by nomination status. Default null is ignored.</param>
        /// <param name="skip">For pagination, the amount of records to skip. Default is 0.</param>
        /// <param name="take">For pagination, the amount of records to take. Default is 10.</param>
        /// <param name="sortBy">
        /// "Liquidity", "Volume", "StakingWeight", "StakingUsd", "ProviderRewards", "MarketRewards" filters.
        /// Single filter use at a time. Default is none.
        /// </param>
        /// <param name="orderBy">"ASC" or "DESC". Default is descending, is ignored if sortBy is null.</param>
        /// <param name="pools">A list of liquidity pool addresses to filter for specific pools.</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>List of <see cref="LiquidityPoolResponseModel"/>'s that match the filter criteria.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<LiquidityPoolResponseModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<LiquidityPoolResponseModel>>> LiquidityPools([FromQuery] bool? staking,
                                                                                                [FromQuery] bool? mining,
                                                                                                [FromQuery] bool? nominated,
                                                                                                [FromQuery] uint? skip,
                                                                                                [FromQuery] uint? take,
                                                                                                [FromQuery] string sortBy,
                                                                                                [FromQuery] string orderBy,
                                                                                                [FromQuery] IEnumerable<string> pools,
                                                                                                CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetLiquidityPoolsWithFilterQuery(_context.Market,
                                                                                   staking,
                                                                                   mining,
                                                                                   nominated,
                                                                                   skip ?? 0,
                                                                                   take ?? 10,
                                                                                   sortBy,
                                                                                   orderBy,
                                                                                   pools), cancellationToken);

            var response = _mapper.Map<IEnumerable<LiquidityPoolResponseModel>>(result);

            return Ok(response);
        }

        /// <summary>Get Liquidity Pool</summary>
        /// <remarks>Returns the liquidity pool that matches the provided address.</remarks>
        /// <param name="address">Contract address to get pools of</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>The requested pools</returns>
        [HttpGet("{address}")]
        [ProducesResponseType(typeof(LiquidityPoolResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<LiquidityPoolResponseModel>> LiquidityPool(string address, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetLiquidityPoolByAddressQuery(address), cancellationToken);

            var response = _mapper.Map<LiquidityPoolResponseModel>(result);

            return Ok(response);
        }

        /// <summary>Get Liquidity Pool History</summary>
        /// <remarks>
        /// Retrieve historical data points for a liquidity pool such as reserves, volume, staking and associated token costs.
        /// </remarks>
        /// <param name="address">The address of the liquidity pool.</param>
        /// <param name="candleSpan">"Hourly" or "Daily" determining the time span of each data point. Default is daily.</param>
        /// <param name="timespan">"1D", "1W", "1M", "1Y" determining how much history to fetch. Default is 1 week.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns><see cref="LiquidityPoolSnapshotHistoryResponseModel"/> with a list of historical data points.</returns>
        [HttpGet("{address}/history")]
        [ProducesResponseType(typeof(LiquidityPoolSnapshotHistoryResponseModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<LiquidityPoolSnapshotHistoryResponseModel>> LiquidityPoolHistory(string address,
                                                                                                        [FromQuery] string candleSpan,
                                                                                                        [FromQuery] string timespan,
                                                                                                        CancellationToken cancellationToken)
        {
            var liquidityPool = await _mediator.Send(new RetrieveLiquidityPoolByAddressQuery(address), cancellationToken);

            var srcToken = await _mediator.Send(new RetrieveTokenByIdQuery(liquidityPool.SrcTokenId), cancellationToken);

            var poolSnapshotDtos = await _mediator.Send(new GetLiquidityPoolSnapshotsWithFilterQuery(address,
                                                                                                     candleSpan,
                                                                                                     timespan), cancellationToken);

            var response = new LiquidityPoolSnapshotHistoryResponseModel
            {
                Address = liquidityPool.Address,
                SnapshotHistory = poolSnapshotDtos.Select(snapshot =>
                {
                    snapshot.SrcTokenDecimals = srcToken.Decimals;
                    return _mapper.Map<LiquidityPoolSnapshotResponseModel>(snapshot);
                })
            };

            return Ok(response);
        }
    }
}
