using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Opdex.Platform.Application.Abstractions.EntryQueries.Pools;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.EntryQueries.Transactions;
using Microsoft.AspNetCore.Authorization;
using Opdex.Platform.Application.Abstractions.EntryQueries.Pools.Snapshots;
using Opdex.Platform.Application.Abstractions.Queries.Pools;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.WebApi.Models;
using Opdex.Platform.WebApi.Models.Responses.Pools;

namespace Opdex.Platform.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("pools")]
    public class PoolsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IApplicationContext _applicationContext;

        public PoolsController(IMediator mediator, IMapper mapper, IApplicationContext applicationContext)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _applicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
        }

        /// <summary>
        /// Get a list of all available pools
        /// </summary>
        /// <remarks>
        /// To be updated to include pagination and filtering
        /// </remarks>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>List of pools</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<LiquidityPoolResponseModel>>> GetAllPools(uint skip, uint take, CancellationToken cancellationToken)
        {
            var query = new GetAllPoolsByMarketAddressQuery(_applicationContext.Market);

            var result = await _mediator.Send(query, cancellationToken);

            var response = _mapper.Map<IEnumerable<LiquidityPoolResponseModel>>(result);

            return Ok(response);
        }

        /// <summary>
        /// Returns the pools that matches the provided address.
        /// </summary>
        /// <param name="address">Contract address to get pools of</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>The requested pools</returns>
        [HttpGet("{address}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<LiquidityPoolResponseModel>> GetPool(string address, CancellationToken cancellationToken)
        {
            var query = new GetLiquidityPoolByAddressQuery(address);

            var result = await _mediator.Send(query, cancellationToken);

            var response = _mapper.Map<LiquidityPoolResponseModel>(result);

            return Ok(response);
        }

        [HttpGet("{address}/history")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<LiquidityPoolSummaryResponseModel>> GetPoolHistory(string address, DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken)
        {
            var liquidityPool = await _mediator.Send(new RetrieveLiquidityPoolByAddressQuery(address));
            var srcToken = await _mediator.Send(new RetrieveTokenByIdQuery(liquidityPool.SrcTokenId));

            var poolSnapshotDtos = await _mediator.Send(new GetLiquidityPoolSnapshotsWithFilterQuery(address, startDate, endDate), cancellationToken);

            foreach (var snapshotDto in poolSnapshotDtos)
            {
                snapshotDto.SrcTokenDecimals = srcToken.Decimals;
            }

            var response = _mapper.Map<IEnumerable<LiquidityPoolSummaryResponseModel>>(poolSnapshotDtos);

            return Ok(response);
        }

        [HttpGet("{address}/transactions")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<TransactionDto>>> GetTransactionsForPool(string address, CancellationToken cancellationToken)
        {
            var query = new GetTransactionsByPoolWithFilterQuery(address, new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 });

            var response = await _mediator.Send(query, cancellationToken);

            return Ok(response);
        }
    }
}
