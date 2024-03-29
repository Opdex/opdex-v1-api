using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Domain.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults;
using Opdex.Platform.Infrastructure.Data.Handlers.Vaults;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Vaults;

public class SelectVaultByIdQueryHandlerTests
{
    private readonly Mock<IDbContext> _dbContext;
    private readonly SelectVaultByIdQueryHandler _handler;

    public SelectVaultByIdQueryHandlerTests()
    {
        var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();

        _dbContext = new Mock<IDbContext>();
        _handler = new SelectVaultByIdQueryHandler(_dbContext.Object, mapper);
    }

    [Fact]
    public async Task Handle_Query_Limit1()
    {
        // Arrange
        var query = new SelectVaultByIdQuery(5, false);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _dbContext.Verify(callTo => callTo.ExecuteFindAsync<It.IsAnyType>(
            It.Is<DatabaseQuery>(q => q.Sql.EndsWith("LIMIT 1;"))), Times.Once);
    }

    [Fact]
    public async Task SelectVaultById_Success()
    {
        const ulong id = 10;

        var expectedEntity = new VaultEntity
        {
            Id = id,
            TokenId = 11,
            Address = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u",
            UnassignedSupply = 233,
            ProposedSupply = 100,
            VestingDuration = 19,
            TotalPledgeMinimum = 33,
            TotalVoteMinimum = 9876,
            CreatedBlock = 1,
            ModifiedBlock = 2
        };

        var command = new SelectVaultByIdQuery(id);

        _dbContext.Setup(db => db.ExecuteFindAsync<VaultEntity>(It.Is<DatabaseQuery>(q => q.Sql.Contains("vault"))))
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
    public async Task SelectVaultById_Throws_NotFoundException()
    {
        const ulong id = 10;

        var command = new SelectVaultByIdQuery(id);

        _dbContext.Setup(db => db.ExecuteFindAsync<VaultEntity>(It.IsAny<DatabaseQuery>()))
            .Returns(() => Task.FromResult<VaultEntity>(null));

        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage($"{nameof(Vault)} not found.");
    }

    [Fact]
    public async Task SelectVaultById_ReturnsNull()
    {
        const ulong id = 10;
        const bool findOrThrow = false;

        var command = new SelectVaultByIdQuery(id, findOrThrow);

        _dbContext.Setup(db => db.ExecuteFindAsync<VaultEntity>(It.IsAny<DatabaseQuery>()))
            .Returns(() => Task.FromResult<VaultEntity>(null));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeNull();
    }
}
