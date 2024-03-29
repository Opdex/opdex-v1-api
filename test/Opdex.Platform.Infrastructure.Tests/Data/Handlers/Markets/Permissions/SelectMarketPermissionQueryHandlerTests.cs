using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets.Permissions;
using Opdex.Platform.Infrastructure.Data.Handlers.Markets.Permissions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Markets.Permissions;

public class SelectMarketPermissionQueryHandlerTests
{
    private readonly Mock<IDbContext> _dbContext;
    private readonly SelectMarketPermissionQueryHandler _handler;

    public SelectMarketPermissionQueryHandlerTests()
    {
        var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();

        _dbContext = new Mock<IDbContext>();
        _handler = new SelectMarketPermissionQueryHandler(_dbContext.Object, mapper);
    }

    [Fact]
    public async Task Handle_Query_Limit1()
    {
        // Arrange
        var query = new SelectMarketPermissionQuery(5, "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", MarketPermissionType.Provide, false);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _dbContext.Verify(callTo => callTo.ExecuteFindAsync<It.IsAnyType>(
            It.Is<DatabaseQuery>(q => q.Sql.EndsWith("LIMIT 1;"))), Times.Once);
    }

    [Fact]
    public async Task Handle_AnySelectQuery_ExecuteFind()
    {
        // Arrange
        var cancellationToken = new CancellationTokenSource().Token;

        // Act
        var result = await _handler.Handle(
            new SelectMarketPermissionQuery(5, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", MarketPermissionType.Provide, false),
            cancellationToken);

        // Assert
        _dbContext.Verify(callTo => callTo.ExecuteFindAsync<MarketPermissionEntity>(
                              It.Is<DatabaseQuery>(query => query.Token == cancellationToken)), Times.Once);
    }

    [Fact]
    public async Task Handle_NoResult_ReturnNull()
    {
        // Arrange
        _dbContext.Setup(callTo => callTo.ExecuteFindAsync<MarketPermissionEntity>(It.IsAny<DatabaseQuery>()))
            .ReturnsAsync((MarketPermissionEntity)null);

        // Act
        var result = await _handler.Handle(
            new SelectMarketPermissionQuery(5, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", MarketPermissionType.Provide, false),
            default);

        // Assert
        result.Should().Be(null);
    }

    [Fact]
    public async Task Handle_FoundEntity_ReturnMapped()
    {
        // Arrange
        var permission = MarketPermissionType.Trade;
        var entity = new MarketPermissionEntity
        {
            Id = 5,
            MarketId = 10,
            User = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj",
            Permission = (int)permission,
            Blame = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk",
            CreatedBlock = 500,
            ModifiedBlock = 505
        };

        _dbContext.Setup(callTo => callTo.ExecuteFindAsync<MarketPermissionEntity>(It.IsAny<DatabaseQuery>()))
            .ReturnsAsync(entity);

        // Act
        var result = await _handler.Handle(
            new SelectMarketPermissionQuery(5, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", MarketPermissionType.Provide, false),
            default);

        // Assert
        result.Id.Should().Be(entity.Id);
        result.MarketId.Should().Be(entity.MarketId);
        result.User.Should().Be(entity.User);
        result.Permission.Should().Be(permission);
        result.IsAuthorized.Should().Be(entity.IsAuthorized);
        result.Blame.Should().Be(entity.Blame);
        result.CreatedBlock.Should().Be(entity.CreatedBlock);
        result.ModifiedBlock.Should().Be(entity.ModifiedBlock);
    }
}
