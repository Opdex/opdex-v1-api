using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Opdex.Platform.Application.Abstractions.EntryCommands.VaultGovernances;
using Opdex.Platform.Application.Abstractions.EntryQueries.VaultGovernances;
using Opdex.Platform.Application.Abstractions.EntryQueries.VaultGovernances.Certificates;
using Opdex.Platform.Application.Abstractions.EntryQueries.VaultGovernances.Pledges;
using Opdex.Platform.Application.Abstractions.EntryQueries.VaultGovernances.Proposals;
using Opdex.Platform.Application.Abstractions.EntryQueries.VaultGovernances.Votes;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models;
using Opdex.Platform.WebApi.Models.Requests.VaultGovernances;
using Opdex.Platform.WebApi.Models.Responses.Transactions;
using Opdex.Platform.WebApi.Models.Responses.VaultGovernances;
using Opdex.Platform.WebApi.Models.Responses.Vaults;
using System.Threading.Tasks;
using System;
using System.Threading;

namespace Opdex.Platform.WebApi.Controllers;

[ApiController]
[Route("vault-governances")]
public class VaultGovernancesController : ControllerBase
{
    private readonly IApplicationContext _context;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public VaultGovernancesController(IApplicationContext context, IMapper mapper, IMediator mediator)
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
    /// <response code="200">Vault results returned.</response>
    /// <response code="400">The request is not valid.</response>
    [HttpGet]
    public async Task<ActionResult<VaultGovernancesResponseModel>> GetVaults([FromQuery] VaultGovernanceFilterParameters filters, CancellationToken cancellationToken)
    {
        var vaults = await _mediator.Send(new GetVaultGovernancesWithFilterQuery(filters.BuildCursor()), cancellationToken);
        return Ok(_mapper.Map<VaultGovernancesResponseModel>(vaults));
    }

    /// <summary>Get Vault</summary>
    /// <remarks>Retrieves vault details.</remarks>
    /// <param name="address">Address of the vault.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Vault details.</returns>
    /// <response code="200">Vault details found.</response>
    /// <response code="400">The request is not valid.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Vault not found.</response>
    [HttpGet("{address}")]
    public async Task<ActionResult<VaultGovernanceResponseModel>> GetVault([FromRoute] Address address, CancellationToken cancellationToken)
    {
        var dto = await _mediator.Send(new GetVaultGovernanceByAddressQuery(address), cancellationToken);
        var response = _mapper.Map<VaultGovernanceResponseModel>(dto);
        return Ok(response);
    }

    /// <summary>Get Vault Certificates</summary>
    /// <remarks>Retrieves vault certificates.</remarks>
    /// <param name="address">Address of the vault.</param>
    /// <param name="filters">Filter parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Vault certificate paging results.</returns>
    /// <response code="200">Vault certificate results returned.</response>
    /// <response code="400">The request is not valid.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Vault not found.</response>
    [HttpGet("{address}/certificates")]
    public async Task<ActionResult<VaultCertificatesResponseModel>> GetCertificates([FromRoute] Address address, [FromQuery] VaultGovernanceCertificateFilterParameters filters, CancellationToken cancellationToken)
    {
        var certificates = await _mediator.Send(new GetVaultGovernanceCertificatesWithFilterQuery(address, filters.BuildCursor()), cancellationToken);
        return Ok(_mapper.Map<VaultCertificatesResponseModel>(certificates));
    }

    /// <summary>Quote Redeem Vault Certificate</summary>
    /// <remarks>Quotes redeeming a vault certificate.</remarks>
    /// <param name="address">Address of the vault.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Redeem vault certificate quote.</returns>
    /// <response code="200">Redeem vault certificate quote created.</response>
    /// <response code="400">The request is not valid.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Vault not found.</response>
    [HttpPost("{address}/certificates/redeem")]
    [Authorize]
    public async Task<ActionResult<TransactionQuoteResponseModel>> QuoteRedeemCertificate([FromRoute] Address address, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new CreateRedeemVaultCertificateQuoteCommand(address, _context.Wallet), cancellationToken);
        var quote = _mapper.Map<TransactionQuoteResponseModel>(response);
        return Ok(quote);
    }

    /// <summary>Get Vault Proposal Pledges</summary>
    /// <remarks>Retrieves vault proposal pledges.</remarks>
    /// <param name="address">Address of the vault.</param>
    /// <param name="filters">Filter parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Vault proposal pledge paging results.</returns>
    /// <response code="200">Vault proposal pledge results returned.</response>
    /// <response code="400">The request is not valid.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Vault not found.</response>
    [HttpGet("{address}/pledges")]
    public async Task<ActionResult<VaultProposalPledgesResponseModel>> GetVaultProposalPledges([FromRoute] Address address,
        [FromQuery] VaultProposalPledgeFilterParameters filters, CancellationToken cancellationToken)
    {
        var vaults = await _mediator.Send(new GetVaultProposalPledgesWithFilterQuery(address, filters.BuildCursor()), cancellationToken);
        return Ok(_mapper.Map<VaultProposalPledgesResponseModel>(vaults));
    }

    /// <summary>Get Vault Proposals</summary>
    /// <remarks>Retrieves vault proposals.</remarks>
    /// <param name="address">Address of the vault.</param>
    /// <param name="filters">Filter parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Vault proposal paging results.</returns>
    /// <response code="200">Vault proposal results returned.</response>
    /// <response code="400">The request is not valid.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Vault not found.</response>
    [HttpGet("{address}/proposals")]
    public async Task<ActionResult<VaultProposalsResponseModel>> GetVaultProposals([FromRoute] Address address, [FromQuery] VaultProposalFilterParameters filters,
                                                                                   CancellationToken cancellationToken)
    {
        var vaults = await _mediator.Send(new GetVaultProposalsWithFilterQuery(address, filters.BuildCursor()), cancellationToken);
        return Ok(_mapper.Map<VaultProposalsResponseModel>(vaults));
    }

    /// <summary>Quote Propose Create Certificate</summary>
    /// <remarks>Quotes making a vault proposal to create a certificate.</remarks>
    /// <param name="address">Address of the vault.</param>
    /// <param name="request">Vault proposal details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Vault proposal quote.</returns>
    /// <response code="200">Vault proposal quote created.</response>
    /// <response code="400">The request is not valid.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Vault not found.</response>
    [HttpPost("{address}/proposals/create-certificate")]
    [Authorize]
    public async Task<ActionResult<TransactionQuoteResponseModel>> QuoteProposeCreateCertificate([FromRoute] Address address,
                                                                                                 [FromBody] CreateCertificateVaultProposalQuoteRequest request,
                                                                                                 CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new CreateVaultProposalCreateCertificateQuoteCommand(address, request.Owner, request.Amount, request.Description, _context.Wallet), cancellationToken);
        var quote = _mapper.Map<TransactionQuoteResponseModel>(response);
        return Ok(quote);
    }

    /// <summary>Quote Propose Revoke Certificate</summary>
    /// <remarks>Quotes making a vault proposal to revoke a certificate.</remarks>
    /// <param name="address">Address of the vault.</param>
    /// <param name="request">Vault proposal details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Vault proposal quote.</returns>
    /// <response code="200">Vault proposal quote created.</response>
    /// <response code="400">The request is not valid.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Vault not found.</response>
    [HttpPost("{address}/proposals/revoke-certificate")]
    [Authorize]
    public async Task<ActionResult<TransactionQuoteResponseModel>> QuoteProposeRevokeCertificate([FromRoute] Address address,
                                                                                                 [FromBody] RevokeCertificateVaultProposalQuoteRequest request,
                                                                                                 CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new CreateVaultProposalRevokeCertificateQuoteCommand(address, request.Owner, request.Description, _context.Wallet), cancellationToken);
        var quote = _mapper.Map<TransactionQuoteResponseModel>(response);
        return Ok(quote);
    }

    /// <summary>Quote Propose Change Minimum Pledge</summary>
    /// <remarks>Quotes making a vault proposal to change the minimum pledge amount.</remarks>
    /// <param name="address">Address of the vault.</param>
    /// <param name="request">Vault proposal details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Vault proposal quote.</returns>
    /// <response code="200">Vault proposal quote created.</response>
    /// <response code="400">The request is not valid.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Vault not found.</response>
    [HttpPost("{address}/proposals/minimum-pledge")]
    [Authorize]
    public async Task<ActionResult<TransactionQuoteResponseModel>> QuoteProposeMinimumPledge([FromRoute] Address address,
                                                                                             [FromBody] MinimumPledgeVaultProposalQuoteRequest request,
                                                                                             CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new CreateVaultProposalMinimumPledgeQuoteCommand(address, request.Amount, request.Description, _context.Wallet), cancellationToken);
        var quote = _mapper.Map<TransactionQuoteResponseModel>(response);
        return Ok(quote);
    }

    /// <summary>Quote Propose Change Minimum Vote</summary>
    /// <remarks>Quotes making a vault proposal to change the minimum vote amount.</remarks>
    /// <param name="address">Address of the vault.</param>
    /// <param name="request">Vault proposal details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Vault proposal quote.</returns>
    /// <response code="200">Vault proposal quote created.</response>
    /// <response code="400">The request is not valid.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Vault not found.</response>
    [HttpPost("{address}/proposals/minimum-vote")]
    [Authorize]
    public async Task<ActionResult<TransactionQuoteResponseModel>> QuoteProposeMinimumVote([FromRoute] Address address, [FromBody] MinimumVoteVaultProposalQuoteRequest request,
                                                                                           CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new CreateVaultProposalMinimumVoteQuoteCommand(address, request.Amount, request.Description, _context.Wallet), cancellationToken);
        var quote = _mapper.Map<TransactionQuoteResponseModel>(response);
        return Ok(quote);
    }

    /// <summary>Get Vault Proposal</summary>
    /// <remarks>Retrieves vault proposal details.</remarks>
    /// <param name="address">Address of the vault.</param>
    /// <param name="proposalId">Id of the proposal in the vault.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Vault proposal details.</returns>
    /// <response code="200">Vault proposal details found.</response>
    /// <response code="400">The request is not valid.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Either vault or proposal not found.</response>
    [HttpGet("{address}/proposals/{proposalId}")]
    public async Task<ActionResult<VaultProposalResponseModel>> GetProposal([FromRoute] Address address, [FromRoute] ulong proposalId, CancellationToken cancellationToken)
    {
        var dto = await _mediator.Send(new GetVaultProposalByVaultAddressAndPublicIdQuery(address, proposalId), cancellationToken);
        var response = _mapper.Map<VaultProposalResponseModel>(dto);
        return Ok(response);
    }

    /// <summary>Quote Complete Vault Proposal</summary>
    /// <remarks>Quotes completing a vault proposal.</remarks>
    /// <param name="address">Address of the vault.</param>
    /// <param name="proposalId">Id of the proposal in the vault.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Vault proposal completion quote.</returns>
    /// <response code="200">Vault proposal completion quote created.</response>
    /// <response code="400">The request is not valid.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Either vault or proposal not found.</response>
    [HttpPost("{address}/proposals/{proposalId}/complete")]
    [Authorize]
    public async Task<ActionResult<TransactionQuoteResponseModel>> QuoteCompleteProposal([FromRoute] Address address, [FromRoute] ulong proposalId, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new CreateCompleteVaultProposalQuoteCommand(address, proposalId, _context.Wallet), cancellationToken);
        var quote = _mapper.Map<TransactionQuoteResponseModel>(response);
        return Ok(quote);
    }

    /// <summary>Quote Vault Proposal Pledge</summary>
    /// <remarks>Quotes a vault proposal pledge.</remarks>
    /// <param name="address">Address of the vault.</param>
    /// <param name="proposalId">Id of the proposal in the vault.</param>
    /// <param name="request">Vault proposal pledge request details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Vault proposal pledge quote.</returns>
    /// <response code="200">Vault proposal pledge quote created.</response>
    /// <response code="400">The request is not valid.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Either vault or proposal not found.</response>
    [HttpPost("{address}/proposals/{proposalId}/pledges")]
    [Authorize]
    public async Task<ActionResult<TransactionQuoteResponseModel>> QuotePledge([FromRoute] Address address, [FromRoute] ulong proposalId,
                                                                               [FromBody] VaultProposalPledgeQuoteRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new CreateVaultProposalPledgeQuoteCommand(address, proposalId, request.Amount, _context.Wallet), cancellationToken);
        var quote = _mapper.Map<TransactionQuoteResponseModel>(response);
        return Ok(quote);
    }

    /// <summary>Quote Withdraw Vault Proposal Pledge</summary>
    /// <remarks>Quotes withdrawing a vault proposal pledge.</remarks>
    /// <param name="address">Address of the vault.</param>
    /// <param name="proposalId">Id of the proposal in the vault.</param>
    /// <param name="request">Vault proposal withdraw pledge request details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Vault proposal withdraw pledge quote.</returns>
    /// <response code="200">Vault proposal withdraw pledge quote created.</response>
    /// <response code="400">The request is not valid.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Either vault or proposal not found.</response>
    [HttpPost("{address}/proposals/{proposalId}/pledges/withdraw")]
    [Authorize]
    public async Task<ActionResult<TransactionQuoteResponseModel>> QuoteWithdrawPledge([FromRoute] Address address, [FromRoute] ulong proposalId,
                                                                                       [FromBody] VaultProposalWithdrawPledgeQuoteRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new CreateWithdrawVaultProposalPledgeQuoteCommand(address, proposalId, request.Amount, _context.Wallet), cancellationToken);
        var quote = _mapper.Map<TransactionQuoteResponseModel>(response);
        return Ok(quote);
    }

    /// <summary>Get Vault Proposal Pledge</summary>
    /// <remarks>Retrieves vault proposal pledge details.</remarks>
    /// <param name="address">Address of the vault.</param>
    /// <param name="proposalId">Id of the proposal in the vault.</param>
    /// <param name="pledger">Address of the pledger.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Vault proposal pledge details.</returns>
    /// <response code="200">Vault proposal pledge details found.</response>
    /// <response code="400">The request is not valid.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Either vault, proposal or pledger not found.</response>
    [HttpGet("{address}/proposals/{proposalId}/pledges/{pledger}")]
    public async Task<ActionResult<VaultProposalPledgeResponseModel>> GetPledge([FromRoute] Address address, [FromRoute] ulong proposalId, [FromRoute] Address pledger,
                                                                                CancellationToken cancellationToken)
    {
        var dto = await _mediator.Send(new GetVaultProposalPledgeByVaultAddressPublicIdAndPledgerQuery(address, proposalId, pledger), cancellationToken);
        var response = _mapper.Map<VaultProposalPledgeResponseModel>(dto);
        return Ok(response);
    }

    /// <summary>Quote Vault Proposal Vote</summary>
    /// <remarks>Quotes a vault proposal vote.</remarks>
    /// <param name="address">Address of the vault.</param>
    /// <param name="proposalId">Id of the proposal in the vault.</param>
    /// <param name="request">Vault proposal vote request details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Vault proposal vote quote.</returns>
    /// <response code="200">Vault proposal vote quote created.</response>
    /// <response code="400">The request is not valid.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Either vault or proposal not found.</response>
    [HttpPost("{address}/proposals/{proposalId}/votes")]
    [Authorize]
    public async Task<ActionResult<TransactionQuoteResponseModel>> QuoteVote([FromRoute] Address address, [FromRoute] ulong proposalId,
                                                                             [FromBody] VaultProposalVoteQuoteRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new CreateVaultProposalVoteQuoteCommand(address, proposalId, request.Amount, request.InFavor, _context.Wallet), cancellationToken);
        var quote = _mapper.Map<TransactionQuoteResponseModel>(response);
        return Ok(quote);
    }

    /// <summary>Quote Withdraw Vault Proposal Vote</summary>
    /// <remarks>Quotes withdrawing a vault proposal vote.</remarks>
    /// <param name="address">Address of the vault.</param>
    /// <param name="proposalId">Id of the proposal in the vault.</param>
    /// <param name="request">Vault proposal withdraw vote request details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Vault proposal withdraw vote quote.</returns>
    /// <response code="200">Vault proposal withdraw vote quote created.</response>
    /// <response code="400">The request is not valid.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Either vault or proposal not found.</response>
    [HttpPost("{address}/proposals/{proposalId}/votes/withdraw")]
    [Authorize]
    public async Task<ActionResult<TransactionQuoteResponseModel>> QuoteWithdrawVote([FromRoute] Address address, [FromRoute] ulong proposalId,
                                                                                     [FromBody] VaultProposalWithdrawVoteQuoteRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new CreateWithdrawVaultProposalVoteQuoteCommand(address, proposalId, request.Amount, _context.Wallet), cancellationToken);
        var quote = _mapper.Map<TransactionQuoteResponseModel>(response);
        return Ok(quote);
    }

    /// <summary>Get Vault Proposal Vote</summary>
    /// <remarks>Retrieves vault proposal vote details.</remarks>
    /// <param name="address">Address of the vault.</param>
    /// <param name="proposalId">Id of the proposal in the vault.</param>
    /// <param name="voter">Address of the voter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Vault proposal vote details.</returns>
    /// <response code="200">Vault proposal vote details found.</response>
    /// <response code="400">The request is not valid.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Either vault, proposal or voter not found.</response>
    [HttpGet("{address}/proposals/{proposalId}/votes/{voter}")]
    public async Task<ActionResult<VaultProposalVoteResponseModel>> GetVote([FromRoute] Address address, [FromRoute] ulong proposalId, [FromRoute] Address voter,
                                                                            CancellationToken cancellationToken)
    {
        var dto = await _mediator.Send(new GetVaultProposalVoteByVaultAddressPublicIdAndVoterQuery(address, proposalId, voter), cancellationToken);
        var response = _mapper.Map<VaultProposalVoteResponseModel>(dto);
        return Ok(response);
    }

    /// <summary>Get Vault Proposal Votes</summary>
    /// <remarks>Retrieves vault proposal votes.</remarks>
    /// <param name="address">Address of the vault.</param>
    /// <param name="filters">Filter parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Vault proposal vote paging results.</returns>
    /// <response code="200">Vault proposal vote results returned.</response>
    /// <response code="400">The request is not valid.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Vault not found.</response>
    [HttpGet("{address}/votes")]
    public async Task<ActionResult<VaultProposalVotesResponseModel>> GetVaultProposalVotes([FromRoute] Address address,
                                                                                           [FromQuery] VaultProposalVoteFilterParameters filters,
                                                                                           CancellationToken cancellationToken)
    {
        var vaults = await _mediator.Send(new GetVaultProposalVotesWithFilterQuery(address, filters.BuildCursor()), cancellationToken);
        return Ok(_mapper.Map<VaultProposalVotesResponseModel>(vaults));
    }
}
