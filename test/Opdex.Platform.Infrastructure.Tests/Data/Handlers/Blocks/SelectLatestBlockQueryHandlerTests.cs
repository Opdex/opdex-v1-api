using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Blocks;
using Opdex.Platform.Infrastructure.Data.Handlers.Blocks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Blocks;

public class SelectLatestBlockQueryHandlerTests
{
    private readonly Mock<IDbContext> _dbContext;
    private readonly SelectLatestBlockQueryHandler _handler;

    public SelectLatestBlockQueryHandlerTests()
    {
        var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();

        _dbContext = new Mock<IDbContext>();
        _handler = new SelectLatestBlockQueryHandler(_dbContext.Object, mapper);
    }

    [Fact]
    public async Task Handle_Query_Limit1()
    {
        // Arrange
        var query = new SelectLatestBlockQuery(false);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _dbContext.Verify(callTo => callTo.ExecuteFindAsync<It.IsAnyType>(
            It.Is<DatabaseQuery>(q => q.Sql.EndsWith("LIMIT 1;"))), Times.Once);
    }

    [Fact]
    public async Task SelectLatestBlock_Success()
    {
        var expectedEntity = new BlockEntity
        {
            Hash = new Sha256(543548579837345),
            Height = 1235,
            Time = DateTime.Now,
            MedianTime = DateTime.Now
        };

        var command = new SelectLatestBlockQuery();

        _dbContext.Setup(db => db.ExecuteFindAsync<BlockEntity>(It.IsAny<DatabaseQuery>()))
            .Returns(() => Task.FromResult(expectedEntity));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Hash.Should().Be(expectedEntity.Hash);
        result.Height.Should().Be(expectedEntity.Height);
        result.Time.Should().Be(expectedEntity.Time);
        result.MedianTime.Should().Be(expectedEntity.MedianTime);
    }
}
