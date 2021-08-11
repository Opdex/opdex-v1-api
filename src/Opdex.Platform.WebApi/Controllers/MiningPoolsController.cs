using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Quote;
using Opdex.Platform.Application.Abstractions.EntryQueries.Pools;
using Opdex.Platform.WebApi.Models;
using Opdex.Platform.WebApi.Models.Requests.WalletTransactions;
using Opdex.Platform.WebApi.Models.Responses.Pools;
using Opdex.Platform.WebApi.Models.Responses.Transactions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("mining-pools")]
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

        /// <summary>
        /// Retrieves mining pool details.
        /// </summary>
        /// <param name="address">Address of the mining pool.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Mining pool details.</returns>
        [HttpGet("{address}")]
        [ProducesResponseType(typeof(MiningPoolResponseModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<MiningPoolResponseModel>> GetMiningPool(string address, CancellationToken cancellationToken)
        {
            var dto = await _mediator.Send(new GetMiningPoolByAddressQuery(address), cancellationToken);
            var response = _mapper.Map<MiningPoolResponseModel>(dto);
            return Ok(response);
        }

        /// <summary>
        /// Quote a start mining transaction.
        /// </summary>
        /// <param name="address">The address of the mining pool.</param>
        /// <param name="request">A <see cref="StartMiningRequest"/> of how many tokens to mine with.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns><see cref="TransactionQuoteResponseModel"/> with the quoted result and the properties used to obtain the quote.</returns>
        [HttpPost("{address}/start")]
        [ProducesResponseType(typeof(TransactionQuoteResponseModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<TransactionQuoteResponseModel>> StartMining(string address, StartMiningQuote request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new CreateStartMiningTransactionQuoteCommand(_context.Wallet, request.Amount,
                                                                                             request.MiningPool), cancellationToken);

            var quote = _mapper.Map<TransactionQuoteResponseModel>(response);

            return Ok(quote);
        }
    }
}
