using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.MiningGovernances;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Tokens;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Assemblers;

public class MinedTokenDistributionScheduleDtoAssembler
{
    private readonly IMediator _mediator;
    private readonly TokenBase _token;

    public MinedTokenDistributionScheduleDtoAssembler(IMediator mediator, TokenBase token)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _token = token ?? throw new ArgumentNullException(nameof(token));
    }

    public async Task<MinedTokenDistributionScheduleDto> Assemble(TokenDistribution distribution)
    {
        var vault = await _mediator.Send(new RetrieveVaultByTokenIdQuery(distribution.TokenId), CancellationToken.None);
        var miningGovernance = await _mediator.Send(new RetrieveMiningGovernanceByTokenIdQuery(distribution.TokenId), CancellationToken.None);

        return new MinedTokenDistributionScheduleDto
        {
            Vault = vault.Address,
            MiningGovernance = miningGovernance.Address,
            NextDistributionBlock = distribution.NextDistributionBlock,
            Previous = new MinedTokenDistributionItemDto
            {
                Vault = distribution.VaultDistribution.ToDecimal(_token.Decimals),
                MiningGovernance = distribution.MiningGovernanceDistribution.ToDecimal(_token.Decimals),
                Block = distribution.DistributionBlock
            }
        };
    }
}
