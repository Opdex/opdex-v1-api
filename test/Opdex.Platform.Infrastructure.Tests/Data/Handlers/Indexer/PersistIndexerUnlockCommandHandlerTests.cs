using Castle.Core.Logging;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using MySqlConnector;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Indexer;
using Opdex.Platform.Infrastructure.Data;
using Opdex.Platform.Infrastructure.Data.Handlers.Indexer;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Indexer;

public class PersistIndexerUnlockCommandHandlerTests
{
    private readonly Mock<IDbContext> _dbContextMock;
    private readonly Mock<ILogger<PersistIndexerUnlockCommandHandler>> _loggerMock;
    private readonly PersistIndexerUnlockCommandHandlerWrapper _handler;

    public PersistIndexerUnlockCommandHandlerTests()
    {
        _dbContextMock = new Mock<IDbContext>();
        _loggerMock = new Mock<ILogger<PersistIndexerUnlockCommandHandler>>();
        var opdexConfiguration = new OpdexConfiguration();
        _handler = new PersistIndexerUnlockCommandHandlerWrapper(_dbContextMock.Object, _loggerMock.Object, opdexConfiguration);
    }

    [Fact]
    public async Task PersistIndexerUnlock_ExecuteCommand()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();

        // Act
        await _handler.Handle(new PersistIndexerUnlockCommand(), cancellationTokenSource.Token);

        // Assert
        _dbContextMock.Verify(callTo => callTo.ExecuteCommandAsync(It.Is<DatabaseQuery>(q => q.Token == CancellationToken.None)), Times.Once);
    }

    [Fact]
    public async Task Handle_ThrownException_LogCritical()
    {
        // Arrange
        var exception = new StubMysqlException();
        _dbContextMock.Setup(callTo => callTo.ExecuteCommandAsync(It.IsAny<DatabaseQuery>())).ThrowsAsync(exception);

        // Act
        await _handler.Handle(new PersistIndexerUnlockCommand(), CancellationToken.None);

        // Assert
        _loggerMock.Verify(
               x => x.Log(
                   LogLevel.Critical,
                   It.IsAny<EventId>(),
                   It.Is<It.IsAnyType>((o, t) => string.Equals("Failed to unlock indexer.", o.ToString(), StringComparison.InvariantCultureIgnoreCase)),
                   exception,
                   It.IsAny<Func<It.IsAnyType, Exception, string>>()),
               Times.Once);
    }

    [Fact]
    public async Task Handle_NoAffectedRows_LogCritical()
    {
        // Arrange
        _dbContextMock.Setup(callTo => callTo.ExecuteCommandAsync(It.IsAny<DatabaseQuery>())).ReturnsAsync(0);

        // Act
        await _handler.Handle(new PersistIndexerUnlockCommand(), CancellationToken.None);

        // Assert
        _loggerMock.Verify(
               x => x.Log(
                   LogLevel.Critical,
                   It.IsAny<EventId>(),
                   It.Is<It.IsAnyType>((o, t) => string.Equals("Failed to unlock indexer.", o.ToString(), StringComparison.InvariantCultureIgnoreCase)),
                   It.IsAny<NoRowsAffectedException>(),
                   It.IsAny<Func<It.IsAnyType, Exception, string>>()),
               Times.Once);
    }

    [Fact]
    public async Task Handle_OneAffectedRow_NoCriticalLog()
    {
        // Arrange
        _dbContextMock.Setup(callTo => callTo.ExecuteCommandAsync(It.IsAny<DatabaseQuery>())).ReturnsAsync(1);

        // Act
        await _handler.Handle(new PersistIndexerUnlockCommand(), CancellationToken.None);

        // Assert
        _loggerMock.Verify(
               x => x.Log(
                   LogLevel.Critical,
                   It.IsAny<EventId>(),
                   It.Is<It.IsAnyType>((o, t) => true),
                   It.IsAny<NoRowsAffectedException>(),
                   It.IsAny<Func<It.IsAnyType, Exception, string>>()),
               Times.Never);
    }

    public class PersistIndexerUnlockCommandHandlerWrapper : PersistIndexerUnlockCommandHandler
    {
        public PersistIndexerUnlockCommandHandlerWrapper(IDbContext context, ILogger<PersistIndexerUnlockCommandHandler> logger, OpdexConfiguration opdexConfiguration) : base(context, logger, opdexConfiguration)
        {
        }

        public new async Task Handle(PersistIndexerUnlockCommand command, CancellationToken cancellationToken)
        {
            await base.Handle(command, cancellationToken);
        }
    }

    class StubMysqlException : Exception
    {
    }
}