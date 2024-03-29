using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses.Balances;
using Opdex.Platform.Infrastructure.Data.Handlers.Addresses.Balances;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Addresses.Balances;

public class SelectAddressBalanceByOwnerAndTokenIdQueryHandlerTests
{
    private readonly Mock<IDbContext> _dbContext;
    private readonly SelectAddressBalanceByOwnerAndTokenIdQueryHandler _handler;

    public SelectAddressBalanceByOwnerAndTokenIdQueryHandlerTests()
    {
        var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();

        _dbContext = new Mock<IDbContext>();
        _handler = new SelectAddressBalanceByOwnerAndTokenIdQueryHandler(_dbContext.Object, mapper);
    }

    [Fact]
    public async Task Handle_Query_Limit1()
    {
        // Arrange
        var query = new SelectAddressBalanceByOwnerAndTokenIdQuery("PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", 5, false);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _dbContext.Verify(callTo => callTo.ExecuteFindAsync<It.IsAnyType>(
            It.Is<DatabaseQuery>(q => q.Sql.EndsWith("LIMIT 1;"))), Times.Once);
    }

    [Fact]
    public void SelectAddressBalanceByOwnerAndTokenId_ThrowsArgumentNullException_InvalidOwner()
    {
        // Arrange
        Address owner = Address.Empty;
        const ulong tokenId = 2;

        void Act() => new SelectAddressBalanceByOwnerAndTokenIdQuery(owner, tokenId);

        // Assert
        Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Owner must be provided.");
    }

    [Fact]
    public void SelectAddressBalanceByOwnerAndTokenId_ThrowsArgumentNullException_InvalidTokenId()
    {
        // Arrange
        const ulong tokenId = 0;
        const string owner = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";

        // Act
        void Act() => new SelectAddressBalanceByOwnerAndTokenIdQuery(owner, tokenId);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("TokenId must be provided.");
    }

    [Fact]
    public async Task SelectAddressBalanceByOwnerAndTokenId_Success()
    {
        // Arrange
        const ulong tokenId = 2;
        const string owner = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";

        var expectedEntity = new AddressBalanceEntity
        {
            Id = 1,
            Owner = owner,
            TokenId = tokenId,
            Balance = 100,
            CreatedBlock = 1,
            ModifiedBlock = 2
        };

        _dbContext.Setup(db => db.ExecuteFindAsync<AddressBalanceEntity>(It.IsAny<DatabaseQuery>()))
            .Returns(() => Task.FromResult(expectedEntity));

        // Act
        var result = await _handler.Handle(new SelectAddressBalanceByOwnerAndTokenIdQuery(owner, tokenId), CancellationToken.None);

        // Assert
        _dbContext.Verify(callTo => callTo.ExecuteFindAsync<AddressBalanceEntity>(It.IsAny<DatabaseQuery>()), Times.Once);

        result.Id.Should().Be(expectedEntity.Id);
        result.Owner.Should().Be(expectedEntity.Owner);
        result.TokenId.Should().Be(expectedEntity.TokenId);
        result.Balance.Should().Be(expectedEntity.Balance);
        result.CreatedBlock.Should().Be(expectedEntity.CreatedBlock);
        result.ModifiedBlock.Should().Be(expectedEntity.ModifiedBlock);
    }

    [Fact]
    public async Task SelectAddressBalanceByOwnerAndTokenId_Throws_NotFoundException()
    {
        // Arrange
        const ulong tokenId = 2;
        const string owner = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";

        var command = new SelectAddressBalanceByOwnerAndTokenIdQuery(owner, tokenId);

        _dbContext.Setup(db => db.ExecuteFindAsync<AddressBalanceEntity>(It.IsAny<DatabaseQuery>()))
            .Returns(() => Task.FromResult<AddressBalanceEntity>(null));

        // Act
        // Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage($"Balance not found.");
    }

    [Fact]
    public async Task SelectAddressBalanceByOwnerAndTokenId_ReturnsNull()
    {
        // Arrange
        const ulong tokenId = 2;
        const string owner = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
        const bool findOrThrow = false;

        var command = new SelectAddressBalanceByOwnerAndTokenIdQuery(owner, tokenId, findOrThrow);

        _dbContext.Setup(db => db.ExecuteFindAsync<AddressBalanceEntity>(It.IsAny<DatabaseQuery>()))
            .Returns(() => Task.FromResult<AddressBalanceEntity>(null));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeNull();

        _dbContext.Verify(callTo => callTo.ExecuteFindAsync<AddressBalanceEntity>(It.IsAny<DatabaseQuery>()), Times.Once);
    }
}
