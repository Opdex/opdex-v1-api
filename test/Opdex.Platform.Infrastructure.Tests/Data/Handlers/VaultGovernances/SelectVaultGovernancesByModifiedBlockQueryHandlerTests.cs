using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.VaultGovernances;
using Opdex.Platform.Infrastructure.Data.Handlers.VaultGovernances;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.VaultGovernances;

public class SelectVaultGovernancesByModifiedBlockQueryHandlerTests
{
    private readonly Mock<IDbContext> _dbContext;
    private readonly SelectVaultGovernancesByModifiedBlockQueryHandler _handler;

    public SelectVaultGovernancesByModifiedBlockQueryHandlerTests()
    {
        var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();

        _dbContext = new Mock<IDbContext>();
        _handler = new SelectVaultGovernancesByModifiedBlockQueryHandler(_dbContext.Object, mapper);
    }

    [Fact]
    public async Task SelectVaultGovernanceById_Success()
    {
        const ulong modifiedBlock = 2;

        var expectedList = new List<VaultGovernanceEntity> {
            new()
            {
                Id = 10,
                TokenId = 2,
                Address = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u",
                UnassignedSupply = 233,
                ProposedSupply = 100,
                VestingDuration = 19,
                TotalPledgeMinimum = 33,
                TotalVoteMinimum = 9876,
                CreatedBlock = 1,
                ModifiedBlock = modifiedBlock
            }
        };

        var command = new SelectVaultGovernancesByModifiedBlockQuery(modifiedBlock);

        _dbContext.Setup(db => db.ExecuteQueryAsync<VaultGovernanceEntity>(It.Is<DatabaseQuery>(q => q.Sql.Contains("vault") &&
                                                                                                     q.Sql.Contains("ModifiedBlock = @ModifiedBlock"))))
            .ReturnsAsync(expectedList);

        var results = await _handler.Handle(command, CancellationToken.None);
        var resultsList = results.ToList();

        for (var i = 0; i < resultsList.Count; i++)
        {
            resultsList[i].Id.Should().Be(expectedList[i].Id);
            resultsList[i].TokenId.Should().Be(expectedList[i].TokenId);
            resultsList[i].Address.Should().Be(expectedList[i].Address);
            resultsList[i].UnassignedSupply.Should().Be(expectedList[i].UnassignedSupply);
            resultsList[i].ProposedSupply.Should().Be(expectedList[i].ProposedSupply);
            resultsList[i].VestingDuration.Should().Be(expectedList[i].VestingDuration);
            resultsList[i].TotalPledgeMinimum.Should().Be(expectedList[i].TotalPledgeMinimum);
            resultsList[i].TotalVoteMinimum.Should().Be(expectedList[i].TotalVoteMinimum);
            resultsList[i].CreatedBlock.Should().Be(expectedList[i].CreatedBlock);
            resultsList[i].ModifiedBlock.Should().Be(expectedList[i].ModifiedBlock);
        }
    }

    [Fact]
    public async Task SelectVaultGovernancesByModifiedBlockQuery_ReturnsEmpty()
    {
        const ulong modifiedBlock = 2;
        var command = new SelectVaultGovernancesByModifiedBlockQuery(modifiedBlock);

        _dbContext.Setup(db => db.ExecuteQueryAsync<VaultGovernanceEntity>(It.IsAny<DatabaseQuery>()))
            .ReturnsAsync(Enumerable.Empty<VaultGovernanceEntity>());

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeEmpty();
    }
}
