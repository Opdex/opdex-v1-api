using AutoMapper;
using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults.ProposalCertificates;
using Opdex.Platform.Application.Abstractions.Queries.Vaults.Proposals;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Vaults;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Assemblers;

public class VaultCertificateDtoAssembler: IModelAssembler<VaultCertificate, VaultCertificateDto>
{
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public VaultCertificateDtoAssembler(IMapper mapper, IMediator mediator)
    {
        _mapper = mapper;
        _mediator = mediator;
    }

    public async Task<VaultCertificateDto> Assemble(VaultCertificate certificate)
    {
        var certificateDto = _mapper.Map<VaultCertificateDto>(certificate);

        var vault = await _mediator.Send(new RetrieveVaultByIdQuery(certificate.VaultId), CancellationToken.None);
        var governanceToken = await _mediator.Send(new RetrieveTokenByIdQuery(vault.TokenId), CancellationToken.None);
        certificateDto.Amount = certificate.Amount.ToDecimal(governanceToken.Decimals);

        var proposalCertificates = await _mediator.Send(new RetrieveVaultProposalCertificatesByCertificateIdQuery(certificate.Id));

        var proposals = await Task.WhenAll(proposalCertificates
                                               .Select(proposalCertificate =>
                                                           _mediator.Send(new RetrieveVaultProposalByIdQuery(proposalCertificate.ProposalId))));

        certificateDto.Proposals = proposals.Select(proposal => proposal.PublicId);

        return certificateDto;
    }
}
