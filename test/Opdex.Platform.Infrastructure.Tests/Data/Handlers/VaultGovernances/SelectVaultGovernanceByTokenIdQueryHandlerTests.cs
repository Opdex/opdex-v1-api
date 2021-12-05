using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Domain.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.VaultGovernances;
using Opdex.Platform.Infrastructure.Data.Handlers.VaultGovernances;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.VaultGovernances;

public class SelectVaultGovernanceByTokenIdQueryHandlerTests
{
    private readonly Mock<IDbContext> _dbContext;
    private readonly SelectVaultGovernanceByTokenIdQueryHandler _handler;

    public SelectVaultGovernanceByTokenIdQueryHandlerTests()
    {
        var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();

        _dbContext = new Mock<IDbContext>();
        _handler = new SelectVaultGovernanceByTokenIdQueryHandler(_dbContext.Object, mapper);
    }

    [Fact]
    public async Task SelectVaultGovernanceByTokenId_Success()
    {
        const ulong tokenId = 10;

        var expectedEntity = new VaultGovernanceEntity
        {
            Id = 10,
            TokenId = tokenId,
            Address = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u",
            UnassignedSupply = 233,
            ProposedSupply = 100,
            VestingDuration = 19,
            TotalPledgeMinimum = 33,
            TotalVoteMinimum = 9876,
            CreatedBlock = 1,
            ModifiedBlock = 2
        };

        var command = new SelectVaultGovernanceByTokenIdQuery(tokenId);

        _dbContext.Setup(db => db.ExecuteFindAsync<VaultGovernanceEntity>(It.Is<DatabaseQuery>(q => q.Sql.Contains("vault_governance") &&
                                                                                                    q.Sql.Contains("TokenId = @TokenId"))))
            .Returns(() => Task.FromResult(expectedEntity));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Id.Should().Be(expectedEntity.Id);
        result.TokenId.Should().Be(expectedEntity.TokenId);
        result.Address.Should().Be(expectedEntity.Address);
        result.UnassignedSupply.Should().Be(expectedEntity.UnassignedSupply);
        result.ProposedSupply.Should().Be(expectedEntity.ProposedSupply);
        result.VestingDuration.Should().Be(expectedEntity.VestingDuration);
        result.TotalPledgeMinimum.Should().Be(expectedEntity.TotalPledgeMinimum);
        result.TotalVoteMinimum.Should().Be(expectedEntity.TotalVoteMinimum);
        result.CreatedBlock.Should().Be(expectedEntity.CreatedBlock);
        result.ModifiedBlock.Should().Be(expectedEntity.ModifiedBlock);
    }

    [Fact]
    public void SelectVaultGovernanceByTokenId_Throws_NotFoundException()
    {
        const ulong tokenId = 10;

        var command = new SelectVaultGovernanceByTokenIdQuery(tokenId);

        _dbContext.Setup(db => db.ExecuteFindAsync<VaultGovernanceEntity>(It.IsAny<DatabaseQuery>()))
            .Returns(() => Task.FromResult<VaultGovernanceEntity>(null));

        _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage($"{nameof(VaultGovernance)} not found.");
    }

    [Fact]
    public async Task SelectVaultGovernanceByTokenId_ReturnsNull()
    {
        const ulong tokenId = 10;
        const bool findOrThrow = false;

        var command = new SelectVaultGovernanceByTokenIdQuery(tokenId, findOrThrow);

        _dbContext.Setup(db => db.ExecuteFindAsync<VaultGovernanceEntity>(It.IsAny<DatabaseQuery>()))
            .Returns(() => Task.FromResult<VaultGovernanceEntity>(null));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeNull();
    }
}
