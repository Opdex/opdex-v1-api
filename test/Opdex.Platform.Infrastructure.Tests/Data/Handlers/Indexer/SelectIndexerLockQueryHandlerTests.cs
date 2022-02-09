using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Indexer;
using Opdex.Platform.Infrastructure.Data.Handlers.Indexer;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Indexer;

public class SelectIndexerLockQueryHandlerTests
{
    private readonly Mock<IDbContext> _dbContext;
    private readonly SelectIndexerLockQueryHandler _handler;

    public SelectIndexerLockQueryHandlerTests()
    {
        _dbContext = new Mock<IDbContext>();
        _handler = new SelectIndexerLockQueryHandler(_dbContext.Object);
    }

    [Fact]
    public async Task Handle_Query_Limit1()
    {
        // Arrange
        var query = new SelectIndexerLockQuery();

        // Act
        try
        {
            await _handler.Handle(query, CancellationToken.None);
        } catch(NotFoundException) {}

        // Assert
        _dbContext.Verify(callTo => callTo.ExecuteFindAsync<It.IsAnyType>(
            It.Is<DatabaseQuery>(q => q.Sql.EndsWith("LIMIT 1;"))), Times.Once);
    }

    [Fact]
    public async Task SelectIndexLock_Success()
    {
        var expectedEntity = new IndexLockEntity
        {
            Available = true,
            Locked = false,
            ModifiedDate = DateTime.Now
        };

        var command = new SelectIndexerLockQuery();

        _dbContext.Setup(db => db.ExecuteFindAsync<IndexLockEntity>(It.IsAny<DatabaseQuery>()))
            .Returns(() => Task.FromResult(expectedEntity));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Available.Should().Be(expectedEntity.Available);
        result.Locked.Should().Be(expectedEntity.Locked);
        result.ModifiedDate.Should().Be(expectedEntity.ModifiedDate);
    }

    [Fact]
    public void SelectIndexLock_Throws_NotFoundException()
    {
        var command = new SelectIndexerLockQuery();

        _dbContext.Setup(db => db.ExecuteFindAsync<IndexLockEntity>(It.IsAny<DatabaseQuery>()))
            .Returns(() => Task.FromResult<IndexLockEntity>(null));

        _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage($"{nameof(IndexLock)} not found.");
    }
}
