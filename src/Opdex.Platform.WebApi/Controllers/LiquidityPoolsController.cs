using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Opdex.Platform.Application.Abstractions.EntryCommands.LiquidityPools.Quotes;
using Opdex.Platform.Application.Abstractions.EntryQueries.LiquidityPools;
using Opdex.Platform.Application.Abstractions.EntryQueries.LiquidityPools.Snapshots;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.WebApi.Models;
using Opdex.Platform.WebApi.Models.Requests.WalletTransactions;
using Opdex.Platform.WebApi.Models.Responses.Pools;
using Opdex.Platform.WebApi.Models.Responses.Transactions;
using System.Linq;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Requests.Quotes;
using System.Net;

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
        /// <param name="cancellationToken">Cancellation token.</param>
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
                                                                                                [FromQuery] IEnumerable<Address> pools,
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

        /// <summary>Create Liquidity Pool Quote</summary>
        /// <remarks>Quote a create liquidity pool transaction.</remarks>
        /// <param name="request">A create liquidity pool request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A create liquidity pool transaction quote.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(TransactionQuoteResponseModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<TransactionQuoteResponseModel>> CreateLiquidityPool(CreateLiquidityPoolRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new CreateCreateLiquidityPoolTransactionQuoteCommand(_context.Market, _context.Wallet, request.Token), cancellationToken);

            var quote = _mapper.Map<TransactionQuoteResponseModel>(response);

            return Ok(quote);
        }

        /// <summary>Get Liquidity Pool</summary>
        /// <remarks>Returns the liquidity pool that matches the provided address.</remarks>
        /// <param name="address">Contract address to get pools of</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The requested pools</returns>
        [HttpGet("{address}")]
        [ProducesResponseType(typeof(LiquidityPoolResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<LiquidityPoolResponseModel>> LiquidityPool([FromRoute] Address address, CancellationToken cancellationToken)
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
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns><see cref="LiquidityPoolSnapshotHistoryResponseModel"/> with a list of historical data points.</returns>
        [HttpGet("{address}/history")]
        [ProducesResponseType(typeof(LiquidityPoolSnapshotHistoryResponseModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<LiquidityPoolSnapshotHistoryResponseModel>> LiquidityPoolHistory([FromRoute] Address address,
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

        /// <summary>Add Liquidity Quote</summary>
        /// <remarks>Quote an add liquidity transaction.</remarks>
        /// <param name="address">The liquidity pool address.</param>
        /// <param name="request">An add liquidity request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Quote an add liquidity transaction.</returns>
        [HttpPost("{address}/add")]
        [ProducesResponseType(typeof(TransactionQuoteResponseModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<TransactionQuoteResponseModel>> AddLiquidityQuote([FromRoute] Address address, AddLiquidityRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new CreateAddLiquidityTransactionQuoteCommand(address, _context.Wallet, request.AmountCrs, request.AmountSrc,
                                                                                              request.AmountCrsMin, request.AmountSrcMin, request.Recipient,
                                                                                              request.Deadline), cancellationToken);

            var quote = _mapper.Map<TransactionQuoteResponseModel>(response);

            return Ok(quote);
        }

        /// <summary>Add Liquidity Amount In Quote</summary>
        /// <remarks>Providing an amount of tokenA in a liquidity pool, returns an equal amount of tokenB for liquidity provisioning.</remarks>
        /// <param name="address">The liquidity pool address.</param>
        /// <param name="request">Request model detailing how many of which tokens are desired to be deposited.</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The quoted number of tokens to be deposited.</returns>
        [HttpPost("{address}/add/amount-in")]
        [ProducesResponseType(typeof(AddLiquidityAmountInQuoteResponseModel), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<AddLiquidityAmountInQuoteResponseModel>> AddLiquidityAmountInQuote([FromRoute] Address address,
                                                                                                          [FromBody] AddLiquidityQuoteRequestModel request,
                                                                                                          CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetLiquidityPoolAddLiquidityAmountInQuoteQuery(request.AmountIn, request.TokenIn,
                                                                                                 address, _context.Market), cancellationToken);

            var response = new AddLiquidityAmountInQuoteResponseModel { AmountIn = result };

            return Ok(response);
        }

        /// <summary>Remove Liquidity Quote</summary>
        /// <remarks>Quote a remove liquidity transaction.</remarks>
        /// <param name="address">The liquidity pool address.</param>
        /// <param name="request">A remove liquidity request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Quote a remove liquidity transaction.</returns>
        [HttpPost("{address}/remove")]
        [ProducesResponseType(typeof(TransactionQuoteResponseModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<TransactionQuoteResponseModel>> RemoveLiquidityQuote([FromRoute] Address address, RemoveLiquidityRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new CreateRemoveLiquidityTransactionQuoteCommand(address, _context.Wallet, request.Liquidity, request.AmountCrsMin,
                                                                                                 request.AmountSrcMin, request.Recipient, request.Deadline), cancellationToken);

            var quote = _mapper.Map<TransactionQuoteResponseModel>(response);

            return Ok(quote);
        }

        /// <summary>Sync Quote</summary>
        /// <remarks>Quote a sync transaction.</remarks>
        /// <param name="address">The liquidity pool address.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Quote a sync transaction.</returns>
        [HttpPost("{address}/sync")]
        [ProducesResponseType(typeof(TransactionQuoteResponseModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<TransactionQuoteResponseModel>> SyncQuote([FromRoute] Address address, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new CreateSyncTransactionQuoteCommand(address, _context.Wallet), cancellationToken);

            var quote = _mapper.Map<TransactionQuoteResponseModel>(response);

            return Ok(quote);
        }

        /// <summary>Skim Quote</summary>
        /// <remarks>Quote a skim transaction.</remarks>
        /// <param name="address">The liquidity pool address.</param>
        /// <param name="request">A skim request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Quote a skim transaction.</returns>
        [HttpPost("{address}/skim")]
        [ProducesResponseType(typeof(TransactionQuoteResponseModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<TransactionQuoteResponseModel>> SkimQuote([FromRoute] Address address, SkimRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new CreateSkimTransactionQuoteCommand(address, _context.Wallet, request.Recipient), cancellationToken);

            var quote = _mapper.Map<TransactionQuoteResponseModel>(response);

            return Ok(quote);
        }

        /// <summary>Start Staking Quote</summary>
        /// <remarks>Quote a start staking transaction.</remarks>
        /// <param name="address">The liquidity pool address.</param>
        /// <param name="request">A start staking request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Quote a start staking transaction.</returns>
        [HttpPost("{address}/staking/start")]
        [ProducesResponseType(typeof(TransactionQuoteResponseModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<TransactionQuoteResponseModel>> StartStakingQuote([FromRoute] Address address, StartStakingRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new CreateStartStakingTransactionQuoteCommand(address, _context.Wallet, request.Amount), cancellationToken);

            var quote = _mapper.Map<TransactionQuoteResponseModel>(response);

            return Ok(quote);
        }

        /// <summary>Stop Staking Quote</summary>
        /// <remarks>Quote a stop staking transaction.</remarks>
        /// <param name="address">The liquidity pool address.</param>
        /// <param name="request">A stop staking request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Quote a stop staking transaction.</returns>
        [HttpPost("{address}/staking/stop")]
        [ProducesResponseType(typeof(TransactionQuoteResponseModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<TransactionQuoteResponseModel>> StopStakingQuote([FromRoute] Address address, StopStakingRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new CreateStopStakingTransactionQuoteCommand(address, _context.Wallet, request.Amount, request.Liquidate), cancellationToken);

            var quote = _mapper.Map<TransactionQuoteResponseModel>(response);

            return Ok(quote);
        }

        /// <summary>Collect Staking Rewards Quote</summary>
        /// <remarks>Quote a collect staking rewards transaction.</remarks>
        /// <param name="address">The liquidity pool address.</param>
        /// <param name="request">A collect staking rewards request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Quote a collect staking rewards transaction.</returns>
        [HttpPost("{address}/staking/collect")]
        [ProducesResponseType(typeof(TransactionQuoteResponseModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<TransactionQuoteResponseModel>> CollectStakingRewardsQuote([FromRoute] Address address, CollectStakingRewardsRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new CreateCollectStakingRewardsTransactionQuoteCommand(address, _context.Wallet, request.Liquidate), cancellationToken);

            var quote = _mapper.Map<TransactionQuoteResponseModel>(response);

            return Ok(quote);
        }
    }
}
