using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Opdex.Platform.Application.Abstractions.EntryCommands.Vaults;
using Opdex.Platform.Application.Abstractions.EntryQueries.Vaults.Pledges;
using Opdex.Platform.Application.Abstractions.EntryQueries.Vaults.Proposals;
using Opdex.Platform.Application.Abstractions.EntryQueries.Vaults.Votes;
using Opdex.Platform.Application.Abstractions.EntryQueries.Vaults;
using Opdex.Platform.Application.Abstractions.EntryQueries.Vaults.Certificates;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models;
using Opdex.Platform.WebApi.Models.Requests.Vaults;
using Opdex.Platform.WebApi.Models.Responses.Transactions;
using Opdex.Platform.WebApi.Models.Responses.Vaults;
using System.Threading.Tasks;
using System;
using System.Threading;

namespace Opdex.Platform.WebApi.Controllers;

[ApiController]
[Route("vaults")]
public class VaultsController : ControllerBase
{
    private readonly IApplicationContext _context;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public VaultsController(IApplicationContext context, IMapper mapper, IMediator mediator)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>Get Vaults</summary>
    /// <remarks>Retrieves vaults.</remarks>
    /// <param name="filters">Filter parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Vault paging results.</returns>
    [HttpGet]
    public async Task<ActionResult<VaultsResponseModel>> GetVaults([FromQuery] VaultFilterParameters filters, CancellationToken cancellationToken)
    {
        var vaults = await _mediator.Send(new GetVaultsWithFilterQuery(filters.BuildCursor()), cancellationToken);
        return Ok(_mapper.Map<VaultsResponseModel>(vaults));
    }

    /// <summary>Get Vault</summary>
    /// <remarks>Retrieves vault details.</remarks>
    /// <param name="vault">Address of the vault.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Vault details.</returns>
    [HttpGet("{vault}")]
    public async Task<ActionResult<VaultResponseModel>> GetVault([FromRoute] Address vault, CancellationToken cancellationToken)
    {
        var dto = await _mediator.Send(new GetVaultByAddressQuery(vault), cancellationToken);
        var response = _mapper.Map<VaultResponseModel>(dto);
        return Ok(response);
    }

    /// <summary>Get Vault Certificates</summary>
    /// <remarks>Retrieves vault certificates.</remarks>
    /// <param name="vault">Address of the vault.</param>
    /// <param name="filters">Filter parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Vault certificate paging results.</returns>
    [HttpGet("{vault}/certificates")]
    public async Task<ActionResult<VaultCertificatesResponseModel>> GetCertificates([FromRoute] Address vault, [FromQuery] VaultCertificateFilterParameters filters, CancellationToken cancellationToken)
    {
        var certificates = await _mediator.Send(new GetVaultCertificatesWithFilterQuery(vault, filters.BuildCursor()), cancellationToken);
        return Ok(_mapper.Map<VaultCertificatesResponseModel>(certificates));
    }

    /// <summary>Quote Redeem Vault Certificate</summary>
    /// <remarks>Quotes redeeming a vault certificate.</remarks>
    /// <param name="vault">Address of the vault.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Redeem vault certificate quote.</returns>
    [HttpPost("{vault}/certificates/redeem")]
    [Authorize]
    public async Task<ActionResult<TransactionQuoteResponseModel>> QuoteRedeemCertificate([FromRoute] Address vault, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new CreateRedeemVaultCertificateQuoteCommand(vault, _context.Wallet), cancellationToken);
        var quote = _mapper.Map<TransactionQuoteResponseModel>(response);
        return Ok(quote);
    }

    /// <summary>Get Vault Proposal Pledges</summary>
    /// <remarks>Retrieves vault proposal pledges.</remarks>
    /// <param name="vault">Address of the vault.</param>
    /// <param name="filters">Filter parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Vault proposal pledge paging results.</returns>
    [HttpGet("{vault}/pledges")]
    public async Task<ActionResult<VaultProposalPledgesResponseModel>> GetVaultProposalPledges([FromRoute] Address vault,
        [FromQuery] VaultProposalPledgeFilterParameters filters, CancellationToken cancellationToken)
    {
        var vaults = await _mediator.Send(new GetVaultProposalPledgesWithFilterQuery(vault, filters.BuildCursor()), cancellationToken);
        return Ok(_mapper.Map<VaultProposalPledgesResponseModel>(vaults));
    }

    /// <summary>Get Vault Proposals</summary>
    /// <remarks>Retrieves vault proposals.</remarks>
    /// <param name="vault">Address of the vault.</param>
    /// <param name="filters">Filter parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Vault proposal paging results.</returns>
    [HttpGet("{vault}/proposals")]
    public async Task<ActionResult<VaultProposalsResponseModel>> GetVaultProposals([FromRoute] Address vault, [FromQuery] VaultProposalFilterParameters filters,
                                                                                   CancellationToken cancellationToken)
    {
        var vaults = await _mediator.Send(new GetVaultProposalsWithFilterQuery(vault, filters.BuildCursor()), cancellationToken);
        return Ok(_mapper.Map<VaultProposalsResponseModel>(vaults));
    }

    /// <summary>Quote Propose Create Certificate</summary>
    /// <remarks>Quotes making a vault proposal to create a certificate.</remarks>
    /// <param name="vault">Address of the vault.</param>
    /// <param name="request">Vault proposal details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Vault proposal quote.</returns>
    [HttpPost("{vault}/proposals/create-certificate")]
    [Authorize]
    public async Task<ActionResult<TransactionQuoteResponseModel>> QuoteProposeCreateCertificate([FromRoute] Address vault,
                                                                                                 [FromBody] CreateCertificateVaultProposalQuoteRequest request,
                                                                                                 CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new CreateVaultProposalCreateCertificateQuoteCommand(vault, request.Owner, request.Amount, request.Description, _context.Wallet), cancellationToken);
        var quote = _mapper.Map<TransactionQuoteResponseModel>(response);
        return Ok(quote);
    }

    /// <summary>Quote Propose Revoke Certificate</summary>
    /// <remarks>Quotes making a vault proposal to revoke a certificate.</remarks>
    /// <param name="vault">Address of the vault.</param>
    /// <param name="request">Vault proposal details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Vault proposal quote.</returns>
    [HttpPost("{vault}/proposals/revoke-certificate")]
    [Authorize]
    public async Task<ActionResult<TransactionQuoteResponseModel>> QuoteProposeRevokeCertificate([FromRoute] Address vault,
                                                                                                 [FromBody] RevokeCertificateVaultProposalQuoteRequest request,
                                                                                                 CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new CreateVaultProposalRevokeCertificateQuoteCommand(vault, request.Owner, request.Description, _context.Wallet), cancellationToken);
        var quote = _mapper.Map<TransactionQuoteResponseModel>(response);
        return Ok(quote);
    }

    /// <summary>Quote Propose Change Minimum Pledge</summary>
    /// <remarks>Quotes making a vault proposal to change the minimum pledge amount.</remarks>
    /// <param name="vault">Address of the vault.</param>
    /// <param name="request">Vault proposal details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Vault proposal quote.</returns>
    [HttpPost("{vault}/proposals/minimum-pledge")]
    [Authorize]
    public async Task<ActionResult<TransactionQuoteResponseModel>> QuoteProposeMinimumPledge([FromRoute] Address vault,
                                                                                             [FromBody] MinimumPledgeVaultProposalQuoteRequest request,
                                                                                             CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new CreateVaultProposalMinimumPledgeQuoteCommand(vault, request.Amount, request.Description, _context.Wallet), cancellationToken);
        var quote = _mapper.Map<TransactionQuoteResponseModel>(response);
        return Ok(quote);
    }

    /// <summary>Quote Propose Change Minimum Vote</summary>
    /// <remarks>Quotes making a vault proposal to change the minimum vote amount.</remarks>
    /// <param name="vault">Address of the vault.</param>
    /// <param name="request">Vault proposal details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Vault proposal quote.</returns>
    [HttpPost("{vault}/proposals/minimum-vote")]
    [Authorize]
    public async Task<ActionResult<TransactionQuoteResponseModel>> QuoteProposeMinimumVote([FromRoute] Address vault, [FromBody] MinimumVoteVaultProposalQuoteRequest request,
                                                                                           CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new CreateVaultProposalMinimumVoteQuoteCommand(vault, request.Amount, request.Description, _context.Wallet), cancellationToken);
        var quote = _mapper.Map<TransactionQuoteResponseModel>(response);
        return Ok(quote);
    }

    /// <summary>Get Vault Proposal</summary>
    /// <remarks>Retrieves vault proposal details.</remarks>
    /// <param name="vault">Address of the vault.</param>
    /// <param name="proposalId">Id of the proposal in the vault.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Vault proposal details.</returns>
    [HttpGet("{vault}/proposals/{proposalId}")]
    public async Task<ActionResult<VaultProposalResponseModel>> GetProposal([FromRoute] Address vault, [FromRoute] ulong proposalId, CancellationToken cancellationToken)
    {
        var dto = await _mediator.Send(new GetVaultProposalByVaultAddressAndPublicIdQuery(vault, proposalId), cancellationToken);
        var response = _mapper.Map<VaultProposalResponseModel>(dto);
        return Ok(response);
    }

    /// <summary>Quote Complete Vault Proposal</summary>
    /// <remarks>Quotes completing a vault proposal.</remarks>
    /// <param name="vault">Address of the vault.</param>
    /// <param name="proposalId">Id of the proposal in the vault.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Vault proposal completion quote.</returns>
    [HttpPost("{vault}/proposals/{proposalId}/complete")]
    [Authorize]
    public async Task<ActionResult<TransactionQuoteResponseModel>> QuoteCompleteProposal([FromRoute] Address vault, [FromRoute] ulong proposalId, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new CreateCompleteVaultProposalQuoteCommand(vault, proposalId, _context.Wallet), cancellationToken);
        var quote = _mapper.Map<TransactionQuoteResponseModel>(response);
        return Ok(quote);
    }

    /// <summary>Quote Vault Proposal Pledge</summary>
    /// <remarks>Quotes a vault proposal pledge.</remarks>
    /// <param name="vault">Address of the vault.</param>
    /// <param name="proposalId">Id of the proposal in the vault.</param>
    /// <param name="request">Vault proposal pledge request details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Vault proposal pledge quote.</returns>
    [HttpPost("{vault}/proposals/{proposalId}/pledges")]
    [Authorize]
    public async Task<ActionResult<TransactionQuoteResponseModel>> QuotePledge([FromRoute] Address vault, [FromRoute] ulong proposalId,
                                                                               [FromBody] VaultProposalPledgeQuoteRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new CreateVaultProposalPledgeQuoteCommand(vault, proposalId, request.Amount, _context.Wallet), cancellationToken);
        var quote = _mapper.Map<TransactionQuoteResponseModel>(response);
        return Ok(quote);
    }

    /// <summary>Quote Withdraw Vault Proposal Pledge</summary>
    /// <remarks>Quotes withdrawing a vault proposal pledge.</remarks>
    /// <param name="vault">Address of the vault.</param>
    /// <param name="proposalId">Id of the proposal in the vault.</param>
    /// <param name="request">Vault proposal withdraw pledge request details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Vault proposal withdraw pledge quote.</returns>
    [HttpPost("{vault}/proposals/{proposalId}/pledges/withdraw")]
    [Authorize]
    public async Task<ActionResult<TransactionQuoteResponseModel>> QuoteWithdrawPledge([FromRoute] Address vault, [FromRoute] ulong proposalId,
                                                                                       [FromBody] VaultProposalWithdrawPledgeQuoteRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new CreateWithdrawVaultProposalPledgeQuoteCommand(vault, proposalId, request.Amount, _context.Wallet), cancellationToken);
        var quote = _mapper.Map<TransactionQuoteResponseModel>(response);
        return Ok(quote);
    }

    /// <summary>Get Vault Proposal Pledge</summary>
    /// <remarks>Retrieves vault proposal pledge details.</remarks>
    /// <param name="vault">Address of the vault.</param>
    /// <param name="proposalId">Id of the proposal in the vault.</param>
    /// <param name="pledger">Address of the pledger.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Vault proposal pledge details.</returns>
    [HttpGet("{vault}/proposals/{proposalId}/pledges/{pledger}")]
    public async Task<ActionResult<VaultProposalPledgeResponseModel>> GetPledge([FromRoute] Address vault, [FromRoute] ulong proposalId, [FromRoute] Address pledger,
                                                                                CancellationToken cancellationToken)
    {
        var dto = await _mediator.Send(new GetVaultProposalPledgeByVaultAddressPublicIdAndPledgerQuery(vault, proposalId, pledger), cancellationToken);
        var response = _mapper.Map<VaultProposalPledgeResponseModel>(dto);
        return Ok(response);
    }

    /// <summary>Quote Vault Proposal Vote</summary>
    /// <remarks>Quotes a vault proposal vote.</remarks>
    /// <param name="vault">Address of the vault.</param>
    /// <param name="proposalId">Id of the proposal in the vault.</param>
    /// <param name="request">Vault proposal vote request details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Vault proposal vote quote.</returns>
    [HttpPost("{vault}/proposals/{proposalId}/votes")]
    [Authorize]
    public async Task<ActionResult<TransactionQuoteResponseModel>> QuoteVote([FromRoute] Address vault, [FromRoute] ulong proposalId,
                                                                             [FromBody] VaultProposalVoteQuoteRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new CreateVaultProposalVoteQuoteCommand(vault, proposalId, request.Amount, request.InFavor, _context.Wallet), cancellationToken);
        var quote = _mapper.Map<TransactionQuoteResponseModel>(response);
        return Ok(quote);
    }

    /// <summary>Quote Withdraw Vault Proposal Vote</summary>
    /// <remarks>Quotes withdrawing a vault proposal vote.</remarks>
    /// <param name="vault">Address of the vault.</param>
    /// <param name="proposalId">Id of the proposal in the vault.</param>
    /// <param name="request">Vault proposal withdraw vote request details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Vault proposal withdraw vote quote.</returns>
    [HttpPost("{vault}/proposals/{proposalId}/votes/withdraw")]
    [Authorize]
    public async Task<ActionResult<TransactionQuoteResponseModel>> QuoteWithdrawVote([FromRoute] Address vault, [FromRoute] ulong proposalId,
                                                                                     [FromBody] VaultProposalWithdrawVoteQuoteRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new CreateWithdrawVaultProposalVoteQuoteCommand(vault, proposalId, request.Amount, _context.Wallet), cancellationToken);
        var quote = _mapper.Map<TransactionQuoteResponseModel>(response);
        return Ok(quote);
    }

    /// <summary>Get Vault Proposal Vote</summary>
    /// <remarks>Retrieves vault proposal vote details.</remarks>
    /// <param name="vault">Address of the vault.</param>
    /// <param name="proposalId">Id of the proposal in the vault.</param>
    /// <param name="voter">Address of the voter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Vault proposal vote details.</returns>
    [HttpGet("{vault}/proposals/{proposalId}/votes/{voter}")]
    public async Task<ActionResult<VaultProposalVoteResponseModel>> GetVote([FromRoute] Address vault, [FromRoute] ulong proposalId, [FromRoute] Address voter,
                                                                            CancellationToken cancellationToken)
    {
        var dto = await _mediator.Send(new GetVaultProposalVoteByVaultAddressPublicIdAndVoterQuery(vault, proposalId, voter), cancellationToken);
        var response = _mapper.Map<VaultProposalVoteResponseModel>(dto);
        return Ok(response);
    }

    /// <summary>Get Vault Proposal Votes</summary>
    /// <remarks>Retrieves vault proposal votes.</remarks>
    /// <param name="vault">Address of the vault.</param>
    /// <param name="filters">Filter parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Vault proposal vote paging results.</returns>
    [HttpGet("{vault}/votes")]
    public async Task<ActionResult<VaultProposalVotesResponseModel>> GetVaultProposalVotes([FromRoute] Address vault,
                                                                                           [FromQuery] VaultProposalVoteFilterParameters filters,
                                                                                           CancellationToken cancellationToken)
    {
        var vaults = await _mediator.Send(new GetVaultProposalVotesWithFilterQuery(vault, filters.BuildCursor()), cancellationToken);
        return Ok(_mapper.Map<VaultProposalVotesResponseModel>(vaults));
    }
}
