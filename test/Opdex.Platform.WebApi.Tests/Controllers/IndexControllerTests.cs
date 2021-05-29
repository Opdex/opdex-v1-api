using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryCommands.Blocks;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Blocks;
using Opdex.Platform.WebApi.Controllers;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Controllers
{
    public class IndexControllerTests
    {
        private readonly Mock<IMediator> _mediator;
        private readonly Mock<IHostingEnvironment> _hostingEnvironment;
        private readonly IndexController _controller;

        public IndexControllerTests()
        {
            _mediator = new Mock<IMediator>();
            _hostingEnvironment = new Mock<IHostingEnvironment>();
            _controller = new IndexController(_mediator.Object, new NullLogger<IndexController>(), _hostingEnvironment.Object);
        }

        [Fact]
        public async Task ProcessLatestBlocks_Send_PersistIndexerLockCommand()
        {
            // Arrange
            var token = new CancellationTokenSource().Token;

            // Act
            try
            {
                await _controller.ProcessLatestBlocks(token);
            }
            catch (Exception) { }

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.IsAny<PersistIndexerLockCommand>(), token), Times.Once);
        }

        [Fact]
        public async Task ProcessLatestBlocks_CannotPersistIndexerLock_ThrowIndexerAlreadyRunningException()
        {
            // Arrange
            _mediator.Setup(callTo => callTo.Send(It.IsAny<PersistIndexerLockCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(false);

            // Act
            Task Act() => _controller.ProcessLatestBlocks(default);

            // Assert
            await Assert.ThrowsAsync<IndexingAlreadyRunningException>(Act);
        }

        [Fact]
        public async Task ProcessLatestBlocks_PersistIndexerLockSuccessfully_SendIndexLatestBlocksCommand()
        {
            // Arrange
            _mediator.Setup(callTo => callTo.Send(It.IsAny<PersistIndexerLockCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(true);

            // Act
            await _controller.ProcessLatestBlocks(new CancellationTokenSource().Token);

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.IsAny<ProcessLatestBlocksCommand>(), CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task ProcessLatestBlocks_PersistIndexerUnlockCommand_AlwaysSend()
        {
            // Arrange
            _mediator.Setup(callTo => callTo.Send(It.IsAny<PersistIndexerLockCommand>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(true);
            _mediator.Setup(callTo => callTo.Send(It.IsAny<ProcessLatestBlocksCommand>(), It.IsAny<CancellationToken>()))
                     .ThrowsAsync(new Exception("Something went wrong!"));

            // Act
            try
            {
                await _controller.ProcessLatestBlocks(new CancellationTokenSource().Token);
            }
            catch (Exception) { }

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.IsAny<PersistIndexerUnlockCommand>(), CancellationToken.None), Times.Once);
        }
    }
}