using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults;
using Opdex.Platform.Infrastructure.Data.Handlers.Vaults;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Vaults;

public class SelectVaultByAddressQueryHandlerTests
{
    private readonly Mock<IDbContext> _dbContext;
    private readonly SelectVaultByAddressQueryHandler _handler;

    public SelectVaultByAddressQueryHandlerTests()
    {
        var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();

        _dbContext = new Mock<IDbContext>();
        _handler = new SelectVaultByAddressQueryHandler(_dbContext.Object, mapper);
    }

    [Fact]
    public async Task SelectVaultByAddress_Success()
    {
        Address address = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u";

        var expectedEntity = new VaultEntity
        {
            Id = 1012,
            TokenId = 11,
            Address = address,
            UnassignedSupply = 233,
            ProposedSupply = 100,
            VestingDuration = 19,
            TotalPledgeMinimum = 33,
            TotalVoteMinimum = 9876,
            CreatedBlock = 1,
            ModifiedBlock = 2
        };

        var command = new SelectVaultByAddressQuery(address);

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
    public void SelectVaultByAddress_Throws_NotFoundException()
    {
        Address address = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u";

        var command = new SelectVaultByAddressQuery(address);

        _dbContext.Setup(db => db.ExecuteFindAsync<VaultEntity>(It.IsAny<DatabaseQuery>()))
            .Returns(() => Task.FromResult<VaultEntity>(null));

        _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage($"{nameof(Vault)} not found.");
    }

    [Fact]
    public async Task SelectVaultByAddress_ReturnsNull()
    {
        Address address = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u";
        const bool findOrThrow = false;

        var command = new SelectVaultByAddressQuery(address, findOrThrow);

        _dbContext.Setup(db => db.ExecuteFindAsync<VaultEntity>(It.IsAny<DatabaseQuery>()))
            .Returns(() => Task.FromResult<VaultEntity>(null));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeNull();
    }
}
