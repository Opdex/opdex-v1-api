using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults;
using Opdex.Platform.Infrastructure.Data.Handlers.Vaults;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Vaults;

public class SelectVaultsByModifiedBlockQueryHandlerTests
{
    private readonly Mock<IDbContext> _dbContext;
    private readonly SelectVaultsByModifiedBlockQueryHandler _handler;

    public SelectVaultsByModifiedBlockQueryHandlerTests()
    {
        var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();

        _dbContext = new Mock<IDbContext>();
        _handler = new SelectVaultsByModifiedBlockQueryHandler(_dbContext.Object, mapper);
    }

    [Fact]
    public async Task SelectVaultById_Success()
    {
        const ulong modifiedBlock = 2;

        var expectedList = new List<VaultEntity> {
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

        var command = new SelectVaultsByModifiedBlockQuery(modifiedBlock);

        _dbContext.Setup(db => db.ExecuteQueryAsync<VaultEntity>(It.Is<DatabaseQuery>(q => q.Sql.Contains("vault") &&
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
    public async Task SelectVaultsByModifiedBlockQuery_ReturnsEmpty()
    {
        const ulong modifiedBlock = 2;
        var command = new SelectVaultsByModifiedBlockQuery(modifiedBlock);

        _dbContext.Setup(db => db.ExecuteQueryAsync<VaultEntity>(It.IsAny<DatabaseQuery>()))
            .ReturnsAsync(Enumerable.Empty<VaultEntity>());

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeEmpty();
    }
}
