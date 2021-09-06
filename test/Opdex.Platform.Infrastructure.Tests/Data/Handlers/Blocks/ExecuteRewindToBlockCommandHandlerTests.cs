using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Blocks;
using Opdex.Platform.Infrastructure.Data.Handlers.Blocks;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Blocks
{
    public class ExecuteRewindToBlockCommandHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly ExecuteRewindToBlockCommandHandler _handler;

        public ExecuteRewindToBlockCommandHandlerTests()
        {
            var logger = new NullLogger<ExecuteRewindToBlockCommandHandler>();

            _dbContext = new Mock<IDbContext>();
            _handler = new ExecuteRewindToBlockCommandHandler(_dbContext.Object, logger);
        }

        [Fact]
        public void ExecuteRewindToBlockCommand_InvalidBlock_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            // Act
            static void Act() => new ExecuteRewindToBlockCommand(0);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Contains("Block number must be greater than 0.");
        }

        [Fact]
        public async Task ExecuteRewindToBlockCommand_Executes_StoredProcedure()
        {
            // Arrange
            var command = new ExecuteRewindToBlockCommand(10);

            // Act
            try
            {
                await _handler.Handle(command, CancellationToken.None);
            } catch { }

            // Assert
            _dbContext.Verify(callTo => callTo.ExecuteCommandAsync(It.Is<DatabaseQuery>(q => q.Parameters != null &&
                                                                                             q.Type == CommandType.StoredProcedure &&
                                                                                             q.Sql == "RewindToBlock")), Times.Once);
        }

        [Fact]
        public async Task ExecuteRewindToBlockCommand_Success()
        {
            // Arrange
            var command = new ExecuteRewindToBlockCommand(10);

            _dbContext.Setup(db => db.ExecuteCommandAsync(It.IsAny<DatabaseQuery>())).ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ExecuteRewindToBlockCommand_Fail()
        {
            // Arrange
            var command = new ExecuteRewindToBlockCommand(10);

            _dbContext.Setup(db => db.ExecuteCommandAsync(It.IsAny<DatabaseQuery>())).ThrowsAsync(new Exception("Some Issue"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeFalse();
        }
    }
}
