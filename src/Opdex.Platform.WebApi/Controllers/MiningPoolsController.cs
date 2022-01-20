using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Opdex.Platform.Application.Abstractions.EntryCommands.MiningPools.Quotes;
using Opdex.Platform.Application.Abstractions.EntryQueries.MiningPools;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models;
using Opdex.Platform.WebApi.Models.Requests.MiningPools;
using Opdex.Platform.WebApi.Models.Responses.MiningPools;
using Opdex.Platform.WebApi.Models.Responses.Transactions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("v{version:apiVersion}/mining-pools")]
[ApiVersion("1")]
public class MiningPoolsController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    private readonly IApplicationContext _context;

    public MiningPoolsController(IMapper mapper, IMediator mediator, IApplicationContext context)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>Get Mining Pools</summary>
    /// <remarks>Retrieves paginated collection of mining pool details.</remarks>
    /// <param name="filters">Filter parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Details of mining pools with paging.</returns>
    [HttpGet]
    public async Task<ActionResult<MiningPoolsResponseModel>> GetMiningPools([FromQuery] MiningPoolFilterParameters filters,
                                                                             CancellationToken cancellationToken)
    {
        var vaults = await _mediator.Send(new GetMiningPoolsWithFilterQuery(filters.BuildCursor()), cancellationToken);
        return Ok(_mapper.Map<MiningPoolsResponseModel>(vaults));
    }

    /// <summary>Get Mining Pool</summary>
    /// <remarks>Retrieves mining pool details.</remarks>
    /// <param name="pool">Address of the mining pool.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Mining pool details.</returns>
    [HttpGet("{pool}")]
    public async Task<ActionResult<MiningPoolResponseModel>> GetMiningPool([FromRoute] Address pool, CancellationToken cancellationToken)
    {
        var dto = await _mediator.Send(new GetMiningPoolByAddressQuery(pool), cancellationToken);
        var response = _mapper.Map<MiningPoolResponseModel>(dto);
        return Ok(response);
    }

    /// <summary>Start Mining Quote</summary>
    /// <remarks>Quote a start mining transaction.</remarks>
    /// <param name="pool">The address of the mining pool.</param>
    /// <param name="request">A <see cref="MiningQuote"/> of how many tokens to mine with.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns><see cref="TransactionQuoteResponseModel"/> with the quoted result and the properties used to obtain the quote.</returns>
    [HttpPost("{pool}/start")]
    public async Task<ActionResult<TransactionQuoteResponseModel>> StartMining([FromRoute] Address pool, [FromBody] MiningQuote request,
                                                                               CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new CreateStartMiningTransactionQuoteCommand(pool, _context.Wallet, request.Amount), cancellationToken);

        var quote = _mapper.Map<TransactionQuoteResponseModel>(response);

        return Ok(quote);
    }

    /// <summary>Stop Mining Quote</summary>
    /// <remarks>Quote a stop mining transaction.</remarks>
    /// <param name="pool">The address of the mining pool.</param>
    /// <param name="request">A <see cref="MiningQuote"/> of how many tokens to stop mining with.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns><see cref="TransactionQuoteResponseModel"/> with the quoted result and the properties used to obtain the quote.</returns>
    [HttpPost("{pool}/stop")]
    public async Task<ActionResult<TransactionQuoteResponseModel>> StopMining([FromRoute] Address pool, [FromBody] MiningQuote request,
                                                                              CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new CreateStopMiningTransactionQuoteCommand(pool, _context.Wallet, request.Amount), cancellationToken);

        var quote = _mapper.Map<TransactionQuoteResponseModel>(response);

        return Ok(quote);
    }

    /// <summary>Collect Mining Rewards Quote</summary>
    /// <remarks>Quote a collect mining rewards transaction.</remarks>
    /// <param name="pool">The address of the mining pool.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns><see cref="TransactionQuoteResponseModel"/> with the quoted result and the properties used to obtain the quote.</returns>
    [HttpPost("{pool}/collect")]
    public async Task<ActionResult<TransactionQuoteResponseModel>> CollectMiningRewards([FromRoute] Address pool, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new CreateCollectMiningRewardsTransactionQuoteCommand(pool, _context.Wallet), cancellationToken);

        var quote = _mapper.Map<TransactionQuoteResponseModel>(response);

        return Ok(quote);
    }
}
