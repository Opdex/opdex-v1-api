using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Auth;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Auth;
using Opdex.Platform.Infrastructure.Data.Handlers.Auth;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Auth;

public class PersistAuthSuccessQueryHandlerTests
{
    private readonly Mock<IDbContext> _dbContext;
    private readonly PersistAuthSuccessCommandHandler _handler;

    public PersistAuthSuccessQueryHandlerTests()
    {
        _dbContext = new Mock<IDbContext>();
        var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();
        _handler = new PersistAuthSuccessCommandHandler(_dbContext.Object, mapper, new NullLogger<PersistAuthSuccessCommandHandler>());
    }

    [Fact]
    public async Task Insert_AuthSuccess_CorrectTable()
    {
        // Arrange
        var authSuccess = new AuthSuccess("CONNECTION_ID", new Address("PAe1RRxnRVZtbS83XQ4soyjwJUDSjaJAKZ"), DateTime.UtcNow);
        var command = new PersistAuthSuccessCommand(authSuccess);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _dbContext.Verify(callTo => callTo.ExecuteCommandAsync(
            It.Is<DatabaseQuery>(q => q.Sql.StartsWith("INSERT INTO auth_success"))), Times.Once);
    }

    [Fact]
    public async Task Handle_Success_ReturnTrue()
    {
        // Arrange
        var authSuccess = new AuthSuccess("CONNECTION_ID", new Address("PAe1RRxnRVZtbS83XQ4soyjwJUDSjaJAKZ"), DateTime.UtcNow);
        var command = new PersistAuthSuccessCommand(authSuccess);

        _dbContext.Setup(db => db.ExecuteCommandAsync(It.IsAny<DatabaseQuery>())).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Fail_ReturnFalse()
    {
        // Arrange
        var authSuccess = new AuthSuccess("CONNECTION_ID", new Address("PAe1RRxnRVZtbS83XQ4soyjwJUDSjaJAKZ"), DateTime.UtcNow);
        var command = new PersistAuthSuccessCommand(authSuccess);

        _dbContext.Setup(db => db.ExecuteCommandAsync(It.IsAny<DatabaseQuery>())).ReturnsAsync(0);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }
}
