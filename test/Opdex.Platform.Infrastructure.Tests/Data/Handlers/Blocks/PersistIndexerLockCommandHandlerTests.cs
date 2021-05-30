using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Blocks;
using Opdex.Platform.Infrastructure.Data.Handlers.Blocks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Blocks
{
    public class PersistIndexerLockCommandHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly PersistIndexerLockCommandHandler _handler;

        public PersistIndexerLockCommandHandlerTests()
        {
            _dbContext = new Mock<IDbContext>();
            _handler = new PersistIndexerLockCommandHandler(_dbContext.Object);
        }

        [Fact]
        public async Task PersistIndexerLock_ExecuteCommand()
        {
            // Arrange
            var token = new CancellationTokenSource().Token;

            // Act
            var result = await _handler.Handle(new PersistIndexerLockCommand(), token);

            // Assert
            _dbContext.Verify(callTo => callTo.ExecuteCommandAsync(It.Is<DatabaseQuery>(q => q.Token == token)), Times.Once);
        }

        [Fact]
        public async Task PersistIndexerLock_Failure_ReturnFalse()
        {
            // Arrange
            _dbContext.Setup(db => db.ExecuteCommandAsync(It.IsAny<DatabaseQuery>()))
                      .ReturnsAsync(0);

            // Act
            var result = await _handler.Handle(new PersistIndexerLockCommand(), default);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task PersistIndexerLock_Success_ReturnTrue()
        {
            // Arrange
            _dbContext.Setup(db => db.ExecuteCommandAsync(It.IsAny<DatabaseQuery>()))
                      .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(new PersistIndexerLockCommand(), default);

            // Assert
            result.Should().BeTrue();
        }
    }
}