using FluentAssertions;
using Moq;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Indexer;
using Opdex.Platform.Infrastructure.Data.Handlers.Indexer;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Indexer
{
    public class PersistIndexerLockCommandHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly PersistIndexerLockCommandHandler _handler;

        public PersistIndexerLockCommandHandlerTests()
        {
            _dbContext = new Mock<IDbContext>();

            var opdexConfiguration = new OpdexConfiguration();

            _handler = new PersistIndexerLockCommandHandler(_dbContext.Object, opdexConfiguration);
        }

        [Fact]
        public async Task PersistIndexerLock_ExecuteCommand()
        {
            // Arrange
            var token = CancellationToken.None;

            // Act
            var result = await _handler.Handle(new PersistIndexerLockCommand(IndexLockReason.Deploy), token);

            // Assert
            _dbContext.Verify(callTo => callTo.ExecuteCommandAsync(It.Is<DatabaseQuery>(q => q.Token == token)), Times.Once);
        }

        [Fact]
        public async Task PersistIndexerLock_Failure_ReturnFalse()
        {
            // Arrange
            var token = CancellationToken.None;
            _dbContext.Setup(db => db.ExecuteCommandAsync(It.IsAny<DatabaseQuery>()))
                      .ReturnsAsync(0);

            // Act
            var result = await _handler.Handle(new PersistIndexerLockCommand(IndexLockReason.Deploy), token);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task PersistIndexerLock_Success_ReturnTrue()
        {
            // Arrange
            var token = CancellationToken.None;
            _dbContext.Setup(db => db.ExecuteCommandAsync(It.IsAny<DatabaseQuery>()))
                      .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(new PersistIndexerLockCommand(IndexLockReason.Deploy), token);

            // Assert
            result.Should().BeTrue();
        }
    }
}
