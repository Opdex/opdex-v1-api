using FluentAssertions;
using Moq;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Indexer;
using Opdex.Platform.Infrastructure.Data.Handlers.Indexer;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Indexer
{
    public class PersistIndexerUnlockCommandHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly PersistIndexerUnlockCommandHandler _handler;

        public PersistIndexerUnlockCommandHandlerTests()
        {
            _dbContext = new Mock<IDbContext>();
            _handler = new PersistIndexerUnlockCommandHandler(_dbContext.Object);
        }

        [Fact]
        public async Task PersistIndexerUnlock_ExecuteCommand()
        {
            // Arrange
            var token = new CancellationTokenSource().Token;

            // Act
            var result = await _handler.Handle(new PersistIndexerUnlockCommand(), default);

            // Assert
            _dbContext.Verify(callTo => callTo.ExecuteCommandAsync(It.Is<DatabaseQuery>(q => q.Token == default)), Times.Once);
        }

        [Fact]
        public async Task PersistIndexerUnlock_Failure_ReturnFalse()
        {
            // Arrange
            _dbContext.Setup(db => db.ExecuteCommandAsync(It.IsAny<DatabaseQuery>()))
                      .ReturnsAsync(0);

            // Act
            var result = await _handler.Handle(new PersistIndexerUnlockCommand(), default);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task PersistIndexerUnlock_Success_ReturnTrue()
        {
            // Arrange
            _dbContext.Setup(db => db.ExecuteCommandAsync(It.IsAny<DatabaseQuery>()))
                      .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(new PersistIndexerUnlockCommand(), default);

            // Assert
            result.Should().BeTrue();
        }
    }
}
