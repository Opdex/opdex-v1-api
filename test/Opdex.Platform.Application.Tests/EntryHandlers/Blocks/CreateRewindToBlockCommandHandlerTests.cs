using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Blocks;
using Opdex.Platform.Application.Abstractions.EntryCommands.Addresses.Balances;
using Opdex.Platform.Application.Abstractions.EntryCommands.Blocks;
using Opdex.Platform.Application.Abstractions.EntryCommands.Deployers;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Application.EntryHandlers.Blocks;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Domain.Models.Blocks;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Blocks
{
    public class CreateRewindToBlockCommandHandlerTests
    {
        private readonly CreateRewindToBlockCommandHandler _handler;
        private readonly Mock<IMediator> _mediator;

        public CreateRewindToBlockCommandHandlerTests()
        {
            _mediator = new Mock<IMediator>();
            _handler = new CreateRewindToBlockCommandHandler(_mediator.Object);
        }

        [Fact]
        public void CreateRewindToBlockCommand_InvalidBlock_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            // Act
            static void Act() => new CreateRewindToBlockCommand(0);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Contains("Block number must be greater than 0.");
        }

        [Fact]
        public async Task CreateRewindToBlockCommand_Sends_RetrieveBlockByHeightQuery()
        {
            // Arrange
            const ulong block = 10;

            // Act
            try
            {
                await _handler.Handle(new CreateRewindToBlockCommand(block), CancellationToken.None);
            }
            catch { }

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveBlockByHeightQuery>(q => q.Height == block), CancellationToken.None));
        }

        [Fact]
        public void CreateRewindToBlockCommand_InvalidBlock_ThrowsInvalidDataException()
        {
            // Arrange
            const ulong block = 10;
            _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveBlockByHeightQuery>(), CancellationToken.None)).ReturnsAsync(() => null);

            // Act
            // Assert
            _handler.Invoking(h => h.Handle(new CreateRewindToBlockCommand(block), CancellationToken.None))
                .Should().Throw<InvalidDataException>()
                .WithMessage("Unable to find a block by the provided block number.");
        }

        [Fact]
        public async Task CreateRewindToBlockCommand_Sends_MakeRewindToBlockCommand()
        {
            // Arrange
            var block = new Block(10, "BlockHash", DateTime.UtcNow, DateTime.UtcNow);

            _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveBlockByHeightQuery>(), CancellationToken.None)).ReturnsAsync(block);

            // Act
            try
            {
                await _handler.Handle(new CreateRewindToBlockCommand(block.Height), CancellationToken.None);
            }
            catch { }

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.Is<MakeRewindToBlockCommand>(q => q.Block == block.Height), CancellationToken.None));
        }

        [Fact]
        public async Task CreateRewindToBlockCommand_Sends_CreateRewindAddressBalancesCommand()
        {
            // Arrange
            var block = new Block(10, "BlockHash", DateTime.UtcNow, DateTime.UtcNow);

            _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveBlockByHeightQuery>(), CancellationToken.None)).ReturnsAsync(block);
            _mediator.Setup(m => m.Send(It.IsAny<MakeRewindToBlockCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

            // Act
            try
            {
                await _handler.Handle(new CreateRewindToBlockCommand(block.Height), CancellationToken.None);
            }
            catch { }

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.Is<CreateRewindAddressBalancesCommand>(q => q.RewindHeight == block.Height), CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task CreateRewindToBlockCommand_Sends_CreateRewindDeployersCommand()
        {
            // Arrange
            var block = new Block(10, "BlockHash", DateTime.UtcNow, DateTime.UtcNow);

            _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveBlockByHeightQuery>(), CancellationToken.None)).ReturnsAsync(block);
            _mediator.Setup(m => m.Send(It.IsAny<MakeRewindToBlockCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
            _mediator.Setup(m => m.Send(It.IsAny<CreateRewindAddressBalancesCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

            // Act
            try
            {
                await _handler.Handle(new CreateRewindToBlockCommand(block.Height), CancellationToken.None);
            }
            catch { }

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.Is<CreateRewindDeployersCommand>(q => q.RewindHeight == block.Height), CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task CreateRewindToBlockCommand_Success()
        {
            // Arrange
            var block = new Block(10, "BlockHash", DateTime.UtcNow, DateTime.UtcNow);
            var command = new CreateRewindToBlockCommand(10);

            _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveBlockByHeightQuery>(), CancellationToken.None)).ReturnsAsync(block);
            _mediator.Setup(m => m.Send(It.IsAny<MakeRewindToBlockCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
            _mediator.Setup(m => m.Send(It.IsAny<CreateRewindAddressBalancesCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
            _mediator.Setup(m => m.Send(It.IsAny<CreateRewindDeployersCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

            // Act
            var response = await _handler.Handle(command, CancellationToken.None);

            // Assert
            response.Should().BeTrue();
        }

        [Fact]
        public async Task CreateRewindToBlockCommand_Fail()
        {
            // Arrange
            var block = new Block(10, "BlockHash", DateTime.UtcNow, DateTime.UtcNow);
            var command = new CreateRewindToBlockCommand(10);

            _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveBlockByHeightQuery>(), CancellationToken.None)).ReturnsAsync(block);
            _mediator.Setup(m => m.Send(It.IsAny<MakeRewindToBlockCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

            // Act
            var response = await _handler.Handle(command, CancellationToken.None);

            // Assert
            response.Should().BeFalse();
        }
    }
}
