using System;
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
using Opdex.Platform.WebApi.Models;
using Opdex.Platform.WebApi.Models.Responses.Transactions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Requests;
using Opdex.Platform.WebApi.Models.Requests.LiquidityPools;
using Opdex.Platform.WebApi.Models.Responses.LiquidityPools;
using Opdex.Platform.WebApi.Models.Responses.LiquidityPools.Snapshots;

namespace Opdex.Platform.WebApi.Controllers;

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
    /// <remarks>Search and filter liquidity pools with pagination.</remarks>
    /// <param name="filters">Liquidity pool filter options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><see cref="LiquidityPoolsResponseModel"/> of matching results and paging details.</returns>
    /// <response code="200">Liquidity pool results returned.</response>
    /// <response code="400">The request is not valid.</response>
    /// <response code="401">Unauthorized.</response>
    [HttpGet]
    public async Task<ActionResult<LiquidityPoolsResponseModel>> LiquidityPools([FromQuery] LiquidityPoolFilterParameters filters,
                                                                                CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetLiquidityPoolsWithFilterQuery(filters.BuildCursor()), cancellationToken);

        var response = _mapper.Map<LiquidityPoolsResponseModel>(result);

        return Ok(response);
    }

    /// <summary>Create Liquidity Pool Quote</summary>
    /// <remarks>Quote a create liquidity pool transaction.</remarks>
    /// <param name="request">A create liquidity pool request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A create liquidity pool transaction quote.</returns>
    /// <response code="200">Create liquidity pool quote created.</response>
    /// <response code="400">The request is not valid.</response>
    /// <response code="401">Unauthorized.</response>
    [HttpPost]
    [ProducesResponseType(typeof(TransactionQuoteResponseModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TransactionQuoteResponseModel>> CreateLiquidityPool([FromBody] CreateLiquidityPoolQuoteRequest request,
                                                                                       CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new CreateCreateLiquidityPoolTransactionQuoteCommand(request.Market, _context.Wallet,
                                                                                                 request.Token), cancellationToken);

        var quote = _mapper.Map<TransactionQuoteResponseModel>(response);

        return Ok(quote);
    }

    /// <summary>Get Liquidity Pool</summary>
    /// <remarks>Returns the liquidity pool that matches the provided address.</remarks>
    /// <param name="pool">Contract address to get pools of</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The requested pool</returns>
    /// <response code="200">Liquidity pool details found.</response>
    /// <response code="400">The request is not valid.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Liquidity pool not found.</response>
    [HttpGet("{pool}")]
    public async Task<ActionResult<LiquidityPoolResponseModel>> LiquidityPool([FromRoute] Address pool, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetLiquidityPoolByAddressQuery(pool), cancellationToken);

        var response = _mapper.Map<LiquidityPoolResponseModel>(result);

        return Ok(response);
    }

    /// <summary>Get Liquidity Pool History</summary>
    /// <remarks>
    /// Retrieve historical data points for a liquidity pool such as reserves, volume, staking and associated token costs.
    /// </remarks>
    /// <param name="pool">The adress of the liquidity pool.</param>
    /// <param name="filters">Filter parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paged liquidity pool snapshot data.</returns>
    /// <response code="200">Liquidity pool snapshot results returned.</response>
    /// <response code="400">The request is not valid.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Liquidity pool not found.</response>
    [HttpGet("{pool}/history")]
    public async Task<ActionResult<LiquidityPoolSnapshotsResponseModel>> GetLiquidityPoolHistory([FromRoute] Address pool,
                                                                                                 [FromQuery] SnapshotFilterParameters filters,
                                                                                                 CancellationToken cancellationToken)
    {
        var poolSnapshotDtos = await _mediator.Send(new GetLiquidityPoolSnapshotsWithFilterQuery(pool, filters.BuildCursor()), cancellationToken);

        var response = _mapper.Map<LiquidityPoolSnapshotsResponseModel>(poolSnapshotDtos);

        return Ok(response);
    }

    /// <summary>Add Liquidity Quote</summary>
    /// <remarks>Quote an add liquidity transaction.</remarks>
    /// <param name="pool">The liquidity pool address.</param>
    /// <param name="request">An add liquidity request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Quote an add liquidity transaction.</returns>
    /// <response code="200">Add liquidity quote created.</response>
    /// <response code="400">The request is not valid.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Liquidity pool not found.</response>
    [HttpPost("{pool}/add")]
    public async Task<ActionResult<TransactionQuoteResponseModel>> AddLiquidityQuote([FromRoute] Address pool, [FromBody] AddLiquidityQuoteRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new CreateAddLiquidityTransactionQuoteCommand(pool, _context.Wallet, request.AmountCrs, request.AmountSrc,
                                                                                          request.AmountCrsMin, request.AmountSrcMin, request.Recipient,
                                                                                          request.Deadline), cancellationToken);

        var quote = _mapper.Map<TransactionQuoteResponseModel>(response);

        return Ok(quote);
    }

    /// <summary>Liquidity Amount In Quote</summary>
    /// <remarks>Providing an amount of token A in a liquidity pool, returns the required amount of token B for liquidity provisioning.</remarks>
    /// <param name="pool">Address of the liquidity pool.</param>
    /// <param name="request">Request model detailing how many of which tokens are desired to be deposited.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The quoted number of tokens to be deposited.</returns>
    /// <response code="200">Calculated required amount.</response>
    /// <response code="400">The request is not valid.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Liquidity pool not found.</response>
    [HttpPost("{pool}/add/amount-in")]
    public async Task<ActionResult<AddLiquidityAmountInQuoteResponseModel>> LiquidityAmountInQuote([FromRoute] Address pool,
                                                                                                   [FromBody] CalculateAddLiquidityAmountsRequestModel request,
                                                                                                   CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetLiquidityAmountInQuoteQuery(request.AmountIn,
                                                                             request.TokenIn,
                                                                             pool), cancellationToken);

        var response = new AddLiquidityAmountInQuoteResponseModel { AmountIn = result };

        return Ok(response);
    }

    /// <summary>Remove Liquidity Quote</summary>
    /// <remarks>Quote a remove liquidity transaction.</remarks>
    /// <param name="pool">The liquidity pool address.</param>
    /// <param name="request">A remove liquidity request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Quote a remove liquidity transaction.</returns>
    /// <response code="200">Remove liquidity quote created.</response>
    /// <response code="400">The request is not valid.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Liquidity pool not found.</response>
    [HttpPost("{pool}/remove")]
    public async Task<ActionResult<TransactionQuoteResponseModel>> RemoveLiquidityQuote([FromRoute] Address pool, [FromBody] RemoveLiquidityQuoteRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new CreateRemoveLiquidityTransactionQuoteCommand(pool, _context.Wallet, request.Liquidity, request.AmountCrsMin,
                                                                                             request.AmountSrcMin, request.Recipient, request.Deadline), cancellationToken);

        var quote = _mapper.Map<TransactionQuoteResponseModel>(response);

        return Ok(quote);
    }

    /// <summary>Sync Quote</summary>
    /// <remarks>Quote a sync transaction.</remarks>
    /// <param name="pool">The liquidity pool address.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Quote a sync transaction.</returns>
    /// <response code="200">Sync quote created.</response>
    /// <response code="400">The request is not valid.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Liquidity pool not found.</response>
    [HttpPost("{pool}/sync")]
    public async Task<ActionResult<TransactionQuoteResponseModel>> SyncQuote([FromRoute] Address pool, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new CreateSyncTransactionQuoteCommand(pool, _context.Wallet), cancellationToken);

        var quote = _mapper.Map<TransactionQuoteResponseModel>(response);

        return Ok(quote);
    }

    /// <summary>Skim Quote</summary>
    /// <remarks>Quote a skim transaction.</remarks>
    /// <param name="pool">The liquidity pool address.</param>
    /// <param name="request">A skim request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Quote a skim transaction.</returns>
    /// <response code="200">Skim quote created.</response>
    /// <response code="400">The request is not valid.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Liquidity pool not found.</response>
    [HttpPost("{pool}/skim")]
    public async Task<ActionResult<TransactionQuoteResponseModel>> SkimQuote([FromRoute] Address pool, [FromBody] SkimQuoteRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new CreateSkimTransactionQuoteCommand(pool, _context.Wallet, request.Recipient), cancellationToken);

        var quote = _mapper.Map<TransactionQuoteResponseModel>(response);

        return Ok(quote);
    }

    /// <summary>Start Staking Quote</summary>
    /// <remarks>Quote a start staking transaction.</remarks>
    /// <param name="pool">The liquidity pool address.</param>
    /// <param name="request">A start staking request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Quote a start staking transaction.</returns>
    /// <response code="200">Start staking quote created.</response>
    /// <response code="400">The request is not valid.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Liquidity pool not found.</response>
    [HttpPost("{pool}/staking/start")]
    public async Task<ActionResult<TransactionQuoteResponseModel>> StartStakingQuote([FromRoute] Address pool, [FromBody] StartStakingQuoteRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new CreateStartStakingTransactionQuoteCommand(pool, _context.Wallet, request.Amount), cancellationToken);

        var quote = _mapper.Map<TransactionQuoteResponseModel>(response);

        return Ok(quote);
    }

    /// <summary>Stop Staking Quote</summary>
    /// <remarks>Quote a stop staking transaction.</remarks>
    /// <param name="pool">The liquidity pool address.</param>
    /// <param name="request">A stop staking request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Quote a stop staking transaction.</returns>
    /// <response code="200">Stop staking quote created.</response>
    /// <response code="400">The request is not valid.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Liquidity pool not found.</response>
    [HttpPost("{pool}/staking/stop")]
    public async Task<ActionResult<TransactionQuoteResponseModel>> StopStakingQuote([FromRoute] Address pool, [FromBody] StopStakingQuoteRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new CreateStopStakingTransactionQuoteCommand(pool, _context.Wallet, request.Amount, request.Liquidate), cancellationToken);

        var quote = _mapper.Map<TransactionQuoteResponseModel>(response);

        return Ok(quote);
    }

    /// <summary>Collect Staking Rewards Quote</summary>
    /// <remarks>Quote a collect staking rewards transaction.</remarks>
    /// <param name="pool">The liquidity pool address.</param>
    /// <param name="request">A collect staking rewards request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Quote a collect staking rewards transaction.</returns>
    /// <response code="200">Collect staking rewards quote created.</response>
    /// <response code="400">The request is not valid.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Liquidity pool not found.</response>
    [HttpPost("{pool}/staking/collect")]
    public async Task<ActionResult<TransactionQuoteResponseModel>> CollectStakingRewardsQuote([FromRoute] Address pool, [FromBody] CollectStakingRewardsQuoteRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new CreateCollectStakingRewardsTransactionQuoteCommand(pool, _context.Wallet, request.Liquidate), cancellationToken);

        var quote = _mapper.Map<TransactionQuoteResponseModel>(response);

        return Ok(quote);
    }
}