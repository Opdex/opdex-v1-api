using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Opdex.Platform.Application.Abstractions.EntryCommands.Vault;
using Opdex.Platform.Common;
using Opdex.Platform.WebApi.Models;
using Opdex.Platform.WebApi.Models.Requests.Vault;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("vault")]
    public class VaultController : ControllerBase
    {
        private readonly IApplicationContext _context;
        private readonly IMediator _mediator;
        private readonly BlockExplorerConfiguration _blockExplorerConfig;

        public VaultController(IApplicationContext context, IMediator mediator, IOptions<BlockExplorerConfiguration> blockExplorerConfig)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _blockExplorerConfig = blockExplorerConfig?.Value ?? throw new ArgumentNullException(nameof(blockExplorerConfig));
        }

        /// <summary>
        /// Sets the owner of a vault
        /// </summary>
        /// <param name="address">Vault contract address</param>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("{address}/owner")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<string>> SetOwner(string address,
                                                         SetVaultOwnerRequest request,
                                                         CancellationToken cancellationToken)
        {
            var transactionHash = await _mediator.Send(new ProcessSetVaultOwnerCommand(_context.Wallet,
                                                                                       address,
                                                                                       request.Owner), cancellationToken);
            return Created(string.Format(_blockExplorerConfig.TransactionEndpoint, transactionHash), transactionHash);
        }

        /// <summary>
        /// Issues a new certificate from the vault
        /// </summary>
        /// <param name="address">Vault contract address</param>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        [HttpPost("{address}/certificates")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<string>> CreateCertificate(string address,
                                                                  CreateVaultCertificateRequest request,
                                                                  CancellationToken cancellationToken)
        {
            var transactionHash = await _mediator.Send(new ProcessCreateVaultCertificateCommand(_context.Wallet,
                                                                                                address,
                                                                                                request.Holder,
                                                                                                request.Amount), cancellationToken);
            return Created(string.Format(_blockExplorerConfig.TransactionEndpoint, transactionHash), transactionHash);
        }

        /// <summary>
        /// Redeems all certificates issued to a specific holder
        /// </summary>
        /// <param name="address">Vault contract address</param>
        /// <param name="holder">Certificate holder address</param>
        /// <param name="cancellationToken"></param>
        [HttpPost("{address}/certificates/{holder}/redeem")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<string>> RedeemCertificate(string address,
                                                                  string holder,
                                                                  CancellationToken cancellationToken)
        {
            var transactionHash = await _mediator.Send(new ProcessRedeemVaultCertificateCommand(_context.Wallet,
                                                                                                address,
                                                                                                holder), cancellationToken);
            return Created(string.Format(_blockExplorerConfig.TransactionEndpoint, transactionHash), transactionHash);
        }

        /// <summary>
        /// Revokes all certificates issued to a specific holder
        /// </summary>
        /// <param name="address">Vault contract address</param>
        /// <param name="holder">Certificate holder address</param>
        /// <param name="cancellationToken"></param>
        [HttpPost("{address}/certificates/{holder}/revoke")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<string>> RevokeCertificate(string address,
                                                                  string holder,
                                                                  CancellationToken cancellationToken)
        {
            var transactionHash = await _mediator.Send(new ProcessRevokeVaultCertificateCommand(_context.Wallet,
                                                                                                address,
                                                                                                holder), cancellationToken);
            return Created(string.Format(_blockExplorerConfig.TransactionEndpoint, transactionHash), transactionHash);
        }
    }
}
