using AutoMapper;
using Moq;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Blocks;
using Opdex.Platform.Infrastructure.Data.Handlers.Blocks;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Blocks;

public class SelectBlocksWithFilterQueryHandlerTests
{
    private readonly Mock<IDbContext> _dbContext;
    private readonly SelectBlocksWithFilterQueryHandler _handler;

    public SelectBlocksWithFilterQueryHandlerTests()
    {
        var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();

        _dbContext = new Mock<IDbContext>();
        _handler = new SelectBlocksWithFilterQueryHandler(_dbContext.Object, mapper);
    }

    [Fact]
    public async Task SelectBlocksWithFilter_ByCursor_NextASC()
    {
        // Arrange
        const SortDirectionType direction = SortDirectionType.ASC;
        const uint limit = 10;

        var cursor = new BlocksCursor(direction, limit, PagingDirection.Forward, 50);

        var command = new SelectBlocksWithFilterQuery(cursor);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _dbContext.Verify(callTo =>
                              callTo.ExecuteQueryAsync<BlockEntity>(
                                  It.Is<DatabaseQuery>(q => q.Sql.Contains("Height > @Height") &&
                                                            q.Sql.Contains($"ORDER BY Height {direction}") &&
                                                            q.Sql.Contains($"LIMIT {limit + 1}"))), Times.Once);
    }

    [Fact]
    public async Task SelectBlocksWithFilter_ByCursor_NextDESC()
    {
        // Arrange
        const SortDirectionType direction = SortDirectionType.DESC;
        const uint limit = 10;

        var cursor = new BlocksCursor(direction, limit, PagingDirection.Forward, 50);

        var command = new SelectBlocksWithFilterQuery(cursor);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _dbContext.Verify(callTo =>
                              callTo.ExecuteQueryAsync<BlockEntity>(
                                  It.Is<DatabaseQuery>(q => q.Sql.Contains("Height < @Height") &&
                                                            q.Sql.Contains($"ORDER BY Height {direction}") &&
                                                            q.Sql.Contains($"LIMIT {limit + 1}"))), Times.Once);
    }

    [Fact]
    public async Task SelectBlocksWithFilter_ByCursor_PreviousDESC()
    {
        // Arrange
        const SortDirectionType requestDirection = SortDirectionType.DESC;
        const uint limit = 10;

        var cursor = new BlocksCursor(requestDirection, limit, PagingDirection.Backward, 50);

        var command = new SelectBlocksWithFilterQuery(cursor);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _dbContext.Verify(callTo =>
                              callTo.ExecuteQueryAsync<BlockEntity>(
                                  It.Is<DatabaseQuery>(q => q.Sql.Contains("Height > @Height") &&
                                                            q.Sql.Contains($"ORDER BY Height {SortDirectionType.ASC}") &&
                                                            q.Sql.Contains($"LIMIT {limit + 1}"))), Times.Once);
    }

    [Fact]
    public async Task SelectBlocksWithFilter_ByCursor_PreviousASC()
    {
        // Arrange
        const SortDirectionType requestDirection = SortDirectionType.ASC;
        const uint limit = 10;

        var cursor = new BlocksCursor(requestDirection, limit, PagingDirection.Backward, 50);

        var command = new SelectBlocksWithFilterQuery(cursor);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _dbContext.Verify(callTo =>
                              callTo.ExecuteQueryAsync<BlockEntity>(
                                  It.Is<DatabaseQuery>(q => q.Sql.Contains("Height < @Height") &&
                                                            q.Sql.Contains($"ORDER BY Height {SortDirectionType.DESC}") &&
                                                            q.Sql.Contains($"LIMIT {limit + 1}"))), Times.Once);
    }
}
