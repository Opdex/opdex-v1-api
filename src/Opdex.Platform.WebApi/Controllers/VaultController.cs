using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Opdex.Platform.Application.Abstractions.EntryCommands.Vault;
using Opdex.Platform.Common;
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
        private readonly IMediator _mediator;
        private readonly BlockExplorerConfiguration _blockExplorerConfig;

        public VaultController(IMediator mediator, IOptions<BlockExplorerConfiguration> blockExplorerConfig)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _blockExplorerConfig = blockExplorerConfig.Value;
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
            var transactionHash = await _mediator.Send(new ProcessSetVaultOwnerCommand(request.WalletName,
                                                                                       request.WalletAddress,
                                                                                       request.WalletPassword,
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
            var transactionHash = await _mediator.Send(new ProcessCreateVaultCertificateCommand(request.WalletName,
                                                                                                request.WalletAddress,
                                                                                                request.WalletPassword,
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
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        [HttpPost("{address}/certificates/{holder}/redeem")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<string>> RedeemCertificate(string address,
                                                                  string holder,
                                                                  RedeemVaultCertificateRequest request,
                                                                  CancellationToken cancellationToken)
        {
            var transactionHash = await _mediator.Send(new ProcessRedeemVaultCertificateCommand(request.WalletName,
                                                                                                request.WalletAddress,
                                                                                                request.WalletPassword,
                                                                                                address,
                                                                                                holder), cancellationToken);
            return Created(string.Format(_blockExplorerConfig.TransactionEndpoint, transactionHash), transactionHash);
        }

        /// <summary>
        /// Revokes all certificates issued to a specific holder
        /// </summary>
        /// <param name="address">Vault contract address</param>
        /// <param name="holder">Certificate holder address</param>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        [HttpPost("{address}/certificates/{holder}/revoke")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<string>> RevokeCertificate(string address,
                                                                  string holder,
                                                                  RevokeVaultCertificateRequest request,
                                                                  CancellationToken cancellationToken)
        {
            var transactionHash = await _mediator.Send(new ProcessRevokeVaultCertificateCommand(request.WalletName,
                                                                                                request.WalletAddress,
                                                                                                request.WalletPassword,
                                                                                                address,
                                                                                                holder), cancellationToken);
            return Created(string.Format(_blockExplorerConfig.TransactionEndpoint, transactionHash), transactionHash);
        }
    }
}
