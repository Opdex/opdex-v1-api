using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.VaultGovernances.Pledges;
using Opdex.Platform.Infrastructure.Data.Handlers.VaultGovernances.Pledges;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.VaultGovernances.Pledges;

public class SelectVaultProposalPledgeByVaultIdAndProposalIdAndPledgerQueryHandlerTests
{
    private readonly Mock<IDbContext> _dbContext;
    private readonly SelectVaultProposalPledgeByVaultIdAndProposalIdAndPledgerQueryHandler _handler;

    public SelectVaultProposalPledgeByVaultIdAndProposalIdAndPledgerQueryHandlerTests()
    {
        var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();

        _dbContext = new Mock<IDbContext>();
        _handler = new SelectVaultProposalPledgeByVaultIdAndProposalIdAndPledgerQueryHandler(_dbContext.Object, mapper);
    }

    [Fact]
    public async Task SelectVaultProposalPledgeById_Success()
    {
        const ulong vaultGovernanceId = 10;
        const ulong proposalId = 11;
        Address pledger = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u";

        var expectedEntity = new VaultProposalPledgeEntity
        {
            Id = 100,
            VaultGovernanceId = vaultGovernanceId,
            ProposalId = proposalId,
            Pledger = pledger,
            Pledge = 5,
            Balance = 3,
            CreatedBlock = 1,
            ModifiedBlock = 2
        };

        var command = new SelectVaultProposalPledgeByVaultIdAndProposalIdAndPledgerQuery(vaultGovernanceId, proposalId, pledger);

        _dbContext.Setup(db => db.ExecuteFindAsync<VaultProposalPledgeEntity>(It.Is<DatabaseQuery>(q => q.Sql.Contains("vault_proposal_pledge"))))
            .Returns(() => Task.FromResult(expectedEntity));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Id.Should().Be(expectedEntity.Id);
        result.VaultGovernanceId.Should().Be(expectedEntity.VaultGovernanceId);
        result.ProposalId.Should().Be(expectedEntity.ProposalId);
        result.Pledger.Should().Be(expectedEntity.Pledger);
        result.Pledge.Should().Be(expectedEntity.Pledge);
        result.Balance.Should().Be(expectedEntity.Balance);
        result.CreatedBlock.Should().Be(expectedEntity.CreatedBlock);
        result.ModifiedBlock.Should().Be(expectedEntity.ModifiedBlock);
    }

    [Fact]
    public void SelectVaultProposalPledgeByVaultIdAndProposalIdAndPledgerQuery_Throws_NotFoundException()
    {
        const ulong vaultGovernanceId = 10;
        const ulong proposalId = 11;
        Address pledger = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u";

        var command = new SelectVaultProposalPledgeByVaultIdAndProposalIdAndPledgerQuery(vaultGovernanceId, proposalId, pledger);

        _dbContext.Setup(db => db.ExecuteFindAsync<VaultProposalPledgeEntity>(It.IsAny<DatabaseQuery>()))
            .Returns(() => Task.FromResult<VaultProposalPledgeEntity>(null));

        _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage($"{nameof(VaultProposalPledge)} not found.");
    }

    [Fact]
    public async Task SelectVaultProposalPledgeByVaultIdAndProposalIdAndPledgerQuery_ReturnsNull()
    {
        const ulong vaultGovernanceId = 10;
        const ulong proposalId = 11;
        Address pledger = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u";
        const bool findOrThrow = false;

        var command = new SelectVaultProposalPledgeByVaultIdAndProposalIdAndPledgerQuery(vaultGovernanceId, proposalId, pledger, findOrThrow);

        _dbContext.Setup(db => db.ExecuteFindAsync<VaultProposalPledgeEntity>(It.IsAny<DatabaseQuery>()))
            .Returns(() => Task.FromResult<VaultProposalPledgeEntity>(null));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeNull();
    }
}