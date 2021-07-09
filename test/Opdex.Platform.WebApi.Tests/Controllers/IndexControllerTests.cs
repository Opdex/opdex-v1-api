using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Blocks;
using Opdex.Platform.Application.Abstractions.EntryCommands.Blocks;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.WebApi.Controllers;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Controllers
{
    // public class IndexControllerTests
    // {
    //     private readonly Mock<IMediator> _mediator;
    //     private readonly Mock<IHostingEnvironment> _hostingEnvironment;
    //     private readonly IndexController _controller;
    //
    //     public IndexControllerTests()
    //     {
    //         _mediator = new Mock<IMediator>();
    //         _hostingEnvironment = new Mock<IHostingEnvironment>();
    //         _controller = new IndexController(_mediator.Object, new NullLogger<IndexController>(), _hostingEnvironment.Object);
    //     }
    //
    //     [Fact]
    //     public async Task ProcessLatestBlocks_Send_MakeIndexerLockCommand()
    //     {
    //         // Arrange
    //         var token = new CancellationTokenSource().Token;
    //
    //         // Act
    //         await _controller.ProcessLatestBlocks(token);
    //
    //         // Assert
    //         _mediator.Verify(callTo => callTo.Send(It.IsAny<MakeIndexerLockCommand>(), token), Times.Once);
    //     }
    //
    //     [Fact]
    //     public async Task ProcessLatestBlocks_MakeIndexerLockFails_DoNotSendProcessLatestBlocksCommand()
    //     {
    //         // Arrange
    //         _mediator.Setup(callTo => callTo.Send(It.IsAny<MakeIndexerLockCommand>(), It.IsAny<CancellationToken>()))
    //                  .ThrowsAsync(new IndexingAlreadyRunningException());
    //
    //         // Act
    //         try
    //         {
    //             await _controller.ProcessLatestBlocks(CancellationToken.None);
    //         }
    //         catch (IndexingAlreadyRunningException) { }
    //
    //         // Assert
    //         _mediator.Verify(callTo => callTo.Send(It.IsAny<ProcessLatestBlocksCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    //     }
    //
    //     [Fact]
    //     public async Task ProcessLatestBlocks_MakeIndexerLockSuccessful_SendProcessLatestBlocksCommand()
    //     {
    //         // Arrange
    //         var isDevelopEnv = true;
    //         _hostingEnvironment.Setup(callTo => callTo.EnvironmentName).Returns("Development");
    //         var token = new CancellationTokenSource().Token;
    //
    //         // Act
    //         await _controller.ProcessLatestBlocks(token);
    //
    //         // Assert
    //         _mediator.Verify(callTo => callTo.Send(It.Is<ProcessLatestBlocksCommand>(command => command.IsDevelopEnv == isDevelopEnv), token), Times.Once);
    //     }
    //
    //     [Fact]
    //     public async Task ProcessLatestBlocks_MakeIndexerLockFails_DoNotSendMakeIndexerUnlockCommand()
    //     {
    //         // Arrange
    //         _mediator.Setup(callTo => callTo.Send(It.IsAny<MakeIndexerLockCommand>(), It.IsAny<CancellationToken>()))
    //                  .ThrowsAsync(new IndexingAlreadyRunningException());
    //
    //         // Act
    //         try
    //         {
    //             await _controller.ProcessLatestBlocks(CancellationToken.None);
    //         }
    //         catch (IndexingAlreadyRunningException) { }
    //
    //         // Assert
    //         _mediator.Verify(callTo => callTo.Send(It.IsAny<MakeIndexerUnlockCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    //     }
    //
    //     [Fact]
    //     public async Task ProcessLatestBlocks_MakeIndexerLockSuccessful_AlwaysSendMakeIndexerUnlockCommand()
    //     {
    //         // Arrange
    //         _mediator.Setup(callTo => callTo.Send(It.IsAny<ProcessLatestBlocksCommand>(), It.IsAny<CancellationToken>()))
    //                  .ThrowsAsync(new Exception("Something went wrong!"));
    //
    //         // Act
    //         try
    //         {
    //             await _controller.ProcessLatestBlocks(new CancellationTokenSource().Token);
    //         }
    //         catch (Exception) { }
    //
    //         // Assert
    //         _mediator.Verify(callTo => callTo.Send(It.IsAny<MakeIndexerUnlockCommand>(), CancellationToken.None), Times.Once);
    //     }
    // }
}
