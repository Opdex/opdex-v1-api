using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.VaultGovernances.Votes;
using Opdex.Platform.Infrastructure.Data.Handlers.VaultGovernances.Votes;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.VaultGovernances.Votes;

public class SelectVaultProposalVotesByModifiedBlockQueryHandlerTests
{
    private readonly Mock<IDbContext> _dbContext;
    private readonly SelectVaultProposalVotesByModifiedBlockQueryHandler _handler;

    public SelectVaultProposalVotesByModifiedBlockQueryHandlerTests()
    {
        var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();

        _dbContext = new Mock<IDbContext>();
        _handler = new SelectVaultProposalVotesByModifiedBlockQueryHandler(_dbContext.Object, mapper);
    }

    [Fact]
    public async Task SelectVaultProposalVoteById_Success()
    {
        const ulong modifiedBlock = 2;

        var expectedList = new List<VaultProposalVoteEntity> {
            new()
            {
                Id = 100,
                VaultGovernanceId = 123,
                ProposalId = 11,
                Voter = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u",
                Vote = 5,
                InFavor = true,
                Balance = 3,
                CreatedBlock = 1,
                ModifiedBlock = modifiedBlock
            }
        };

        var command = new SelectVaultProposalVotesByModifiedBlockQuery(modifiedBlock);

        _dbContext.Setup(db => db.ExecuteQueryAsync<VaultProposalVoteEntity>(It.Is<DatabaseQuery>(q => q.Sql.Contains("vault_proposal_vote") &&
                                                                                                       q.Sql.Contains("ModifiedBlock = @ModifiedBlock"))))
            .ReturnsAsync(expectedList);

        var results = await _handler.Handle(command, CancellationToken.None);
        var resultsList = results.ToList();

        for (var i = 0; i < resultsList.Count; i++)
        {
            resultsList[i].Id.Should().Be(expectedList[i].Id);
            resultsList[i].VaultGovernanceId.Should().Be(expectedList[i].VaultGovernanceId);
            resultsList[i].ProposalId.Should().Be(expectedList[i].ProposalId);
            resultsList[i].Voter.Should().Be(expectedList[i].Voter);
            resultsList[i].Vote.Should().Be(expectedList[i].Vote);
            resultsList[i].Balance.Should().Be(expectedList[i].Balance);
            resultsList[i].CreatedBlock.Should().Be(expectedList[i].CreatedBlock);
            resultsList[i].ModifiedBlock.Should().Be(expectedList[i].ModifiedBlock);
        }
    }

    [Fact]
    public async Task SelectVaultProposalVotesByModifiedBlockQuery_ReturnsEmpty()
    {
        const ulong modifiedBlock = 2;
        var command = new SelectVaultProposalVotesByModifiedBlockQuery(modifiedBlock);

        _dbContext.Setup(db => db.ExecuteQueryAsync<VaultProposalVoteEntity>(It.IsAny<DatabaseQuery>()))
            .ReturnsAsync(Enumerable.Empty<VaultProposalVoteEntity>());

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeEmpty();
    }
}
