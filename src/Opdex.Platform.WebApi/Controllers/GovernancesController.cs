using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Opdex.Platform.Application.Abstractions.EntryCommands.Governances;
using Opdex.Platform.Application.Abstractions.EntryQueries.Governances;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models;
using Opdex.Platform.WebApi.Models.Requests.Governances;
using Opdex.Platform.WebApi.Models.Requests.WalletTransactions;
using Opdex.Platform.WebApi.Models.Responses.Governances;
using Opdex.Platform.WebApi.Models.Responses.Transactions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("governances")]
    public class GovernancesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IApplicationContext _context;

        public GovernancesController(IMediator mediator, IMapper mapper, IApplicationContext applicationContext)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
        }

        /// <summary>Get Governances</summary>
        /// <remarks>Retrieves a collection of mining governances.</remarks>
        /// <param name="filters">Filter parameters.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Mining governance results with paging.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(MiningGovernancesResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<MiningGovernancesResponseModel>> GetGovernances([FromQuery] GovernanceFilterParameters filters,
                                                                                       CancellationToken cancellationToken)
        {
            var certificates = await _mediator.Send(new GetMiningGovernancesWithFilterQuery(filters.BuildCursor()), cancellationToken);
            return Ok(_mapper.Map<MiningGovernancesResponseModel>(certificates));
        }

        /// <summary>Get Governance</summary>
        /// <remarks>Retrieves a governance smart contract's summary by its address.</remarks>
        /// <param name="address">The address of the governance smart contract.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns><see cref="MiningGovernanceResponseModel"/> summary</returns>
        /// <response code="404">Governance contract not found.</response>
        [HttpGet("{address}")]
        [ProducesResponseType(typeof(MiningGovernanceResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MiningGovernanceResponseModel>> Governance([FromRoute] Address address, CancellationToken cancellationToken)
        {
            var governanceDto = await _mediator.Send(new GetMiningGovernanceByAddressQuery(address), cancellationToken);

            var response = _mapper.Map<MiningGovernanceResponseModel>(governanceDto);

            return Ok(response);
        }

        /// <summary>Reward Mining Pools Quote</summary>
        /// <remarks>Quote a reward mining pools transaction where governance tokens are distributed to mining pools for liquidity mining.</remarks>
        /// <param name="address">The address of the governance contract.</param>
        /// <param name="request">The reward mining pool transaction quote request body.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Quote a stop staking transaction.</returns>
        /// <response code="404">Governance contract not found.</response>
        [HttpPost("{address}/reward-mining-pools")]
        [ProducesResponseType(typeof(TransactionQuoteResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RewardMiningPools([FromRoute] Address address,
                                                           [FromBody] RewardMiningPoolsRequest request,
                                                           CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new CreateRewardMiningPoolsTransactionQuoteCommand(address,
                                                                                                   _context.Wallet,
                                                                                                   request.FullDistribution), cancellationToken);

            var quote = _mapper.Map<TransactionQuoteResponseModel>(response);

            return Ok(quote);
        }
    }
}
