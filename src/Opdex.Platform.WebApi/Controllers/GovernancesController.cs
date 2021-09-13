using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Opdex.Platform.Application.Abstractions.EntryCommands.Governances;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet;
using Opdex.Platform.Application.Abstractions.EntryQueries.Governances;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Governances;
using Opdex.Platform.WebApi.Models;
using Opdex.Platform.WebApi.Models.Requests.WalletTransactions;
using Opdex.Platform.WebApi.Models.Responses;
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
        private readonly NetworkType _network;

        public GovernancesController(IMediator mediator, IMapper mapper, IApplicationContext applicationContext, OpdexConfiguration opdexConfiguration)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
            _network = opdexConfiguration?.Network ?? throw new ArgumentNullException(nameof(opdexConfiguration));
        }

        /// <summary>Get Governances</summary>
        /// <remarks>Retrieves a collection of mining governances.</remarks>
        /// <param name="minedToken">The address of a mined token.</param>
        /// <param name="limit">Number of certificates to take must be greater than 0 and less than 51.</param>
        /// <param name="direction">The order direction of the results, either "ASC" or "DESC".</param>
        /// <param name="cursor">The cursor when paging.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Mining governances</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<MiningGovernancesResponseModel>> GetGovernances([FromQuery] Address minedToken,
                                                                                       [FromQuery] SortDirectionType direction,
                                                                                       [FromQuery] uint limit,
                                                                                       [FromQuery] string cursor,
                                                                                       CancellationToken cancellationToken)
        {
            MiningGovernancesCursor pagingCursor;

            if (cursor.HasValue())
            {
                if (!Base64Extensions.TryBase64Decode(cursor, out var decodedCursor) || !MiningGovernancesCursor.TryParse(decodedCursor, out var parsedCursor))
                {
                    return new ValidationErrorProblemDetailsResult(nameof(cursor), "Cursor not formed correctly.");
                }
                pagingCursor = parsedCursor;
            }
            else
            {
                pagingCursor = new MiningGovernancesCursor(minedToken, direction, limit, PagingDirection.Forward, default);
            }

            var certificates = await _mediator.Send(new GetMiningGovernancesWithFilterQuery(pagingCursor), cancellationToken);
            return Ok(_mapper.Map<MiningGovernancesResponseModel>(certificates));
        }

        /// <summary>Get Governance</summary>
        /// <remarks>Retrieves a governance smart contract's summary by its address.</remarks>
        /// <param name="address">The address of the governance smart contract.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns><see cref="MiningGovernanceResponseModel"/> summary</returns>
        [HttpGet("{address}")]
        [ProducesResponseType(typeof(MiningGovernanceResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
        [HttpPost("{address}/reward-mining-pools")]
        [ProducesResponseType(typeof(ActionResult<TransactionQuoteResponseModel>), StatusCodes.Status200OK)]
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
