using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Auth;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Auth;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Auth;
using Opdex.Platform.Infrastructure.Data.Handlers.Auth;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Auth;

public class SelectAuthSuccessByConnectionIdQueryHandlerTests
{
    private readonly Mock<IDbContext> _dbContext;
    private readonly SelectAuthSuccessByConnectionIdQueryHandler _handler;
    private readonly Mock<IMapper> _mapperMock;

    public SelectAuthSuccessByConnectionIdQueryHandlerTests()
    {
        _dbContext = new Mock<IDbContext>();
        _mapperMock = new Mock<IMapper>();
        _handler = new SelectAuthSuccessByConnectionIdQueryHandler(_dbContext.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_Query_Limit1()
    {
        // Arrange
        var query = new SelectAuthSuccessByConnectionIdQuery("jwf892jfieurwnfqjkr3n2ogfn3wklfnekrlf");

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _dbContext.Verify(callTo => callTo.ExecuteFindAsync<It.IsAnyType>(
            It.Is<DatabaseQuery>(q => q.Sql.EndsWith("LIMIT 1;"))), Times.Once);
    }

    [Fact]
    public async Task Handle_Query_FilterByConnectionId()
    {
        // Arrange
        var query = new SelectAuthSuccessByConnectionIdQuery("jwf892jfieurwnfqjkr3n2ogfn3wklfnekrlf");

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _dbContext.Verify(callTo => callTo.ExecuteFindAsync<It.IsAnyType>(
            It.Is<DatabaseQuery>(q => q.Sql.Contains("ConnectionId = @ConnectionId"))), Times.Once);
    }

    [Fact]
    public async Task Handle_NoResult_ReturnNull()
    {
        // Arrange
        var query = new SelectAuthSuccessByConnectionIdQuery("jwf892jfieurwnfqjkr3n2ogfn3wklfnekrlf");
        _dbContext.Setup(callTo => callTo.ExecuteFindAsync<AuthSuccessEntity>(It.IsAny<DatabaseQuery>()))
            .ReturnsAsync((AuthSuccessEntity)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().Be(null);
    }

    [Fact]
    public async Task Handle_Result_ReturnMapped()
    {
        // Arrange
        var connectionId = "jwf892jfieurwnfqjkr3n2ogfn3wklfnekrlf";
        var authSuccessEntity = new AuthSuccessEntity
        {
            ConnectionId = connectionId,
            Signer = new Address("PAe1RRxnRVZtbS83XQ4soyjwJUDSjaJAKZ"),
            Expiry = DateTime.UtcNow.AddMinutes(3)
        };
        var query = new SelectAuthSuccessByConnectionIdQuery(connectionId);
        _dbContext.Setup(callTo => callTo.ExecuteFindAsync<AuthSuccessEntity>(It.IsAny<DatabaseQuery>()))
            .ReturnsAsync(authSuccessEntity);

        var authSuccess = new AuthSuccess(connectionId, new Address("PAe1RRxnRVZtbS83XQ4soyjwJUDSjaJAKZ"), DateTime.UtcNow.AddMinutes(3));
        _mapperMock.Setup(callTo => callTo.Map<AuthSuccess>(authSuccessEntity)).Returns(authSuccess);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().Be(authSuccess);
    }
}
