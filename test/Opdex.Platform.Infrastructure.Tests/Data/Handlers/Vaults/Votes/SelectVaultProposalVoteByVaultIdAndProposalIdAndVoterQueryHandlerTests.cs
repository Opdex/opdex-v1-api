using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults.Votes;
using Opdex.Platform.Infrastructure.Data.Handlers.Vaults.Votes;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Vaults.Votes;

public class SelectVaultProposalVoteByVaultIdAndProposalIdAndVoterQueryHandlerTests
{
    private readonly Mock<IDbContext> _dbContext;
    private readonly SelectVaultProposalVoteByVaultIdAndProposalIdAndVoterQueryHandler _handler;

    public SelectVaultProposalVoteByVaultIdAndProposalIdAndVoterQueryHandlerTests()
    {
        var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();

        _dbContext = new Mock<IDbContext>();
        _handler = new SelectVaultProposalVoteByVaultIdAndProposalIdAndVoterQueryHandler(_dbContext.Object, mapper);
    }

    [Fact]
    public async Task SelectVaultProposalVoteById_Success()
    {
        const ulong vaultId = 10;
        const ulong proposalId = 11;
        Address voter = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u";

        var expectedEntity = new VaultProposalVoteEntity
        {
            Id = 100,
            VaultId = vaultId,
            ProposalId = proposalId,
            Voter = voter,
            Vote = 5,
            InFavor = true,
            Balance = 3,
            CreatedBlock = 1,
            ModifiedBlock = 2
        };

        var command = new SelectVaultProposalVoteByVaultIdAndProposalIdAndVoterQuery(vaultId, proposalId, voter);

        _dbContext.Setup(db => db.ExecuteFindAsync<VaultProposalVoteEntity>(It.Is<DatabaseQuery>(q => q.Sql.Contains("vault_proposal_vote"))))
            .Returns(() => Task.FromResult(expectedEntity));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Id.Should().Be(expectedEntity.Id);
        result.VaultId.Should().Be(expectedEntity.VaultId);
        result.ProposalId.Should().Be(expectedEntity.ProposalId);
        result.Voter.Should().Be(expectedEntity.Voter);
        result.Vote.Should().Be(expectedEntity.Vote);
        result.Balance.Should().Be(expectedEntity.Balance);
        result.CreatedBlock.Should().Be(expectedEntity.CreatedBlock);
        result.ModifiedBlock.Should().Be(expectedEntity.ModifiedBlock);
    }

    [Fact]
    public void SelectVaultProposalVoteByVaultIdAndProposalIdAndVoterQuery_Throws_NotFoundException()
    {
        const ulong vaultId = 10;
        const ulong proposalId = 11;
        Address voter = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u";

        var command = new SelectVaultProposalVoteByVaultIdAndProposalIdAndVoterQuery(vaultId, proposalId, voter);

        _dbContext.Setup(db => db.ExecuteFindAsync<VaultProposalVoteEntity>(It.IsAny<DatabaseQuery>()))
            .Returns(() => Task.FromResult<VaultProposalVoteEntity>(null));

        _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage($"{nameof(VaultProposalVote)} not found.");
    }

    [Fact]
    public async Task SelectVaultProposalVoteByVaultIdAndProposalIdAndVoterQuery_ReturnsNull()
    {
        const ulong vaultId = 10;
        const ulong proposalId = 11;
        Address voter = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u";
        const bool findOrThrow = false;

        var command = new SelectVaultProposalVoteByVaultIdAndProposalIdAndVoterQuery(vaultId, proposalId, voter, findOrThrow);

        _dbContext.Setup(db => db.ExecuteFindAsync<VaultProposalVoteEntity>(It.IsAny<DatabaseQuery>()))
            .Returns(() => Task.FromResult<VaultProposalVoteEntity>(null));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeNull();
    }
}
