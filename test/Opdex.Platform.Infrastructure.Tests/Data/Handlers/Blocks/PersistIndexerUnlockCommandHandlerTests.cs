using System.Threading;
using System.Threading.Tasks;
using Moq;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Blocks;
using Opdex.Platform.Infrastructure.Data.Handlers.Blocks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Blocks
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
            var result = await _handler.Handle(new PersistIndexerUnlockCommand(), token);

            // Assert
            _dbContext.Verify(callTo => callTo.ExecuteCommandAsync(It.Is<DatabaseQuery>(q => q.Token == token)), Times.Once);
        }
    }
}