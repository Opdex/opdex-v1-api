using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Blocks;
using Opdex.Platform.Infrastructure.Data.Handlers.Blocks;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Blocks;

public class SelectBlockByHeightQueryHandlerTests
{
    private readonly Mock<IDbContext> _dbContext;
    private readonly SelectBlockByHeightQueryHandler _handler;

    public SelectBlockByHeightQueryHandlerTests()
    {
        var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();

        _dbContext = new Mock<IDbContext>();
        _handler = new SelectBlockByHeightQueryHandler(_dbContext.Object, mapper);
    }

    [Fact]
    public async Task Handle_Query_Limit1()
    {
        // Arrange
        var query = new SelectBlockByHeightQuery(50000, false);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _dbContext.Verify(callTo => callTo.ExecuteFindAsync<It.IsAnyType>(
            It.Is<DatabaseQuery>(q => q.Sql.EndsWith("LIMIT 1;"))), Times.Once);
    }

    [Fact]
    public async Task SelectBlockByHeight_Success()
    {
        // Arrange
        const ulong height = 500000;

        var block = new BlockEntity
        {
            Height = height,
            Hash = new Sha256(345843095842095824),
            MedianTime = DateTime.UtcNow.AddMinutes(-2),
            Time = DateTime.UtcNow
        };

        _dbContext.Setup(db => db.ExecuteFindAsync<BlockEntity>(It.IsAny<DatabaseQuery>()))
            .ReturnsAsync(block);

        // Act
        var result = await _handler.Handle(new SelectBlockByHeightQuery(height), CancellationToken.None);

        // Assert
        _dbContext.Verify(callTo => callTo.ExecuteFindAsync<BlockEntity>(It.IsAny<DatabaseQuery>()), Times.Once);
        result.Height.Should().Be(block.Height);
        result.Hash.Should().Be(block.Hash);
        result.Time.Should().Be(block.Time);
        result.MedianTime.Should().Be(block.MedianTime);
    }

    [Fact]
    public async Task SelectBlockByHeight_Throws_NotFoundException()
    {
        // Arrange
        var command = new SelectBlockByHeightQuery(500000);

        _dbContext.Setup(db => db.ExecuteFindAsync<BlockEntity>(It.IsAny<DatabaseQuery>()))
            .ReturnsAsync((BlockEntity)null);

        // Act
        // Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage($"Block not found.");
    }

    [Fact]
    public async Task SelectBlockByHeight_ReturnsNull()
    {
        // Arrange
        var command = new SelectBlockByHeightQuery(50000, false);

        _dbContext.Setup(db => db.ExecuteFindAsync<BlockEntity>(It.IsAny<DatabaseQuery>()))
            .ReturnsAsync((BlockEntity)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _dbContext.Verify(callTo => callTo.ExecuteFindAsync<BlockEntity>(It.IsAny<DatabaseQuery>()), Times.Once);
        result.Should().BeNull();
    }
}
