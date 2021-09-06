using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Blocks;
using Opdex.Platform.Application.Handlers.Blocks;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Blocks;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Handlers.Blocks
{
    public class MakeRewindToBlockCommandHandlerTests
    {
        private readonly MakeRewindToBlockCommandHandler _handler;
        private readonly Mock<IMediator> _mediator;

        public MakeRewindToBlockCommandHandlerTests()
        {
            _mediator = new Mock<IMediator>();
            _handler = new MakeRewindToBlockCommandHandler(_mediator.Object);
        }

        [Fact]
        public async Task MakeRewindToBlockCommand_Sends_ExecuteRewindToBlockCommand()
        {
            // Arrange
            const ulong block = 10;

            // Act
            try
            {
                await _handler.Handle(new MakeRewindToBlockCommand(block), CancellationToken.None);
            }
            catch { }

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.Is<ExecuteRewindToBlockCommand>(q => q.Block == block), CancellationToken.None));
        }

        [Fact]
        public async Task MakeRewindToBlockCommand_Success()
        {
            // Arrange
            var command = new MakeRewindToBlockCommand(10);

            _mediator.Setup(m => m.Send(It.IsAny<ExecuteRewindToBlockCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

            // Act
            var response = await _handler.Handle(command, CancellationToken.None);

            // Assert
            response.Should().BeTrue();
        }

        [Fact]
        public async Task MakeRewindToBlockCommand_Fail()
        {
            // Arrange
            var command = new MakeRewindToBlockCommand(10);

            _mediator.Setup(m => m.Send(It.IsAny<ExecuteRewindToBlockCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

            // Act
            var response = await _handler.Handle(command, CancellationToken.None);

            // Assert
            response.Should().BeFalse();
        }
    }
}
