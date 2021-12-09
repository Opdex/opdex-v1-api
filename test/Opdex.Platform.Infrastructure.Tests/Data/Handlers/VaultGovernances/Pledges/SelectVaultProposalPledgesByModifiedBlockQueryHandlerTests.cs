using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.VaultGovernances.Pledges;
using Opdex.Platform.Infrastructure.Data.Handlers.VaultGovernances.Pledges;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.VaultGovernances.Pledges;

public class SelectVaultProposalPledgesByModifiedBlockQueryHandlerTests
{
    private readonly Mock<IDbContext> _dbContext;
    private readonly SelectVaultProposalPledgesByModifiedBlockQueryHandler _handler;

    public SelectVaultProposalPledgesByModifiedBlockQueryHandlerTests()
    {
        var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();

        _dbContext = new Mock<IDbContext>();
        _handler = new SelectVaultProposalPledgesByModifiedBlockQueryHandler(_dbContext.Object, mapper);
    }

    [Fact]
    public async Task SelectVaultProposalPledgeById_Success()
    {
        const ulong modifiedBlock = 2;

        var expectedList = new List<VaultProposalPledgeEntity> {
            new()
            {
                Id = 100,
                VaultGovernanceId = 123,
                ProposalId = 11,
                Pledger = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u",
                Pledge = 5,
                Balance = 3,
                CreatedBlock = 1,
                ModifiedBlock = modifiedBlock
            }
        };

        var command = new SelectVaultProposalPledgesByModifiedBlockQuery(modifiedBlock);

        _dbContext.Setup(db => db.ExecuteQueryAsync<VaultProposalPledgeEntity>(It.Is<DatabaseQuery>(q => q.Sql.Contains("vault_proposal_pledge") &&
                                                                                                         q.Sql.Contains("ModifiedBlock = @ModifiedBlock"))))
            .ReturnsAsync(expectedList);

        var results = await _handler.Handle(command, CancellationToken.None);
        var resultsList = results.ToList();

        for (var i = 0; i < resultsList.Count; i++)
        {
            resultsList[i].Id.Should().Be(expectedList[i].Id);
            resultsList[i].VaultGovernanceId.Should().Be(expectedList[i].VaultGovernanceId);
            resultsList[i].ProposalId.Should().Be(expectedList[i].ProposalId);
            resultsList[i].Pledger.Should().Be(expectedList[i].Pledger);
            resultsList[i].Pledge.Should().Be(expectedList[i].Pledge);
            resultsList[i].Balance.Should().Be(expectedList[i].Balance);
            resultsList[i].CreatedBlock.Should().Be(expectedList[i].CreatedBlock);
            resultsList[i].ModifiedBlock.Should().Be(expectedList[i].ModifiedBlock);
        }
    }

    [Fact]
    public async Task SelectVaultProposalPledgesByModifiedBlockQuery_ReturnsEmpty()
    {
        const ulong modifiedBlock = 2;
        var command = new SelectVaultProposalPledgesByModifiedBlockQuery(modifiedBlock);

        _dbContext.Setup(db => db.ExecuteQueryAsync<VaultProposalPledgeEntity>(It.IsAny<DatabaseQuery>()))
            .ReturnsAsync(Enumerable.Empty<VaultProposalPledgeEntity>());

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeEmpty();
    }
}
