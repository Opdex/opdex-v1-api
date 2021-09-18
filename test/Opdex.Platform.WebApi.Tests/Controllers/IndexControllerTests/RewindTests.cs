using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Indexer;
using Opdex.Platform.Application.Abstractions.EntryCommands.Blocks;
using Opdex.Platform.Application.Abstractions.EntryQueries.Admins;
using Opdex.Platform.Application.Abstractions.Models.Admins;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Controllers;
using Opdex.Platform.WebApi.Models;
using Opdex.Platform.WebApi.Models.Requests.Index;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Controllers.IndexControllerTests
{
    public class RewindTests
    {
        private readonly Mock<IMediator> _mediator;
        private readonly Mock<IApplicationContext> _context;
        private readonly IndexController _controller;

        public RewindTests()
        {
            _mediator = new Mock<IMediator>();
            _context = new Mock<IApplicationContext>();

            var opdexConfiguration = new OpdexConfiguration { Network = NetworkType.DEVNET };

            _controller = new IndexController(_mediator.Object, opdexConfiguration, _context.Object);
        }

        [Fact]
        public async Task Rewind_Sends_GetAdminByAddressQuery()
        {
            // Arrange
            var request = new RewindRequest { Block = 10 };

            _context.Setup(callTo => callTo.Wallet).Returns(new Address("PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk"));

            // Act
            try
            {
                await _controller.Rewind(request, CancellationToken.None);
            }
            catch { }

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.IsAny<GetAdminByAddressQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Rewind_Returns_Unauthorized()
        {
            // Arrange
            var request = new RewindRequest { Block = 10 };
            var admin = new AdminDto { Id = 1, Address = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk" };

            _context.Setup(callTo => callTo.Wallet).Returns(admin.Address);

            // Act
            var response = await _controller.Rewind(request, CancellationToken.None);

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.IsAny<GetAdminByAddressQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediator.Verify(callTo => callTo.Send(It.IsAny<MakeIndexerLockCommand>(), CancellationToken.None), Times.Never);

            response.Should().BeOfType<UnauthorizedResult>();
        }

        [Fact]
        public async Task Rewind_Sends_MakeIndexerLockCommand()
        {
            // Arrange
            var request = new RewindRequest { Block = 10 };
            var admin = new AdminDto { Id = 1, Address = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk" };

            _context.Setup(callTo => callTo.Wallet).Returns(admin.Address);
            _mediator.Setup(callTo => callTo.Send(It.IsAny<GetAdminByAddressQuery>(), CancellationToken.None)).ReturnsAsync(admin);

            // Act
            try
            {
                await _controller.Rewind(request, CancellationToken.None);
            }
            catch { }

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.IsAny<MakeIndexerLockCommand>(), CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task Rewind_Sends_CreateRewindToBlockCommand_Success_MakeIndexerUnlockCommand()
        {
            // Arrange
            var request = new RewindRequest { Block = 10 };
            var admin = new AdminDto { Id = 1, Address = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk" };

            _context.Setup(callTo => callTo.Wallet).Returns(admin.Address);
            _mediator.Setup(callTo => callTo.Send(It.IsAny<GetAdminByAddressQuery>(), CancellationToken.None)).ReturnsAsync(admin);
            _mediator.Setup(m => m.Send(It.IsAny<MakeIndexerLockCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Unit.Value);

            // Act
            try
            {
                var response = await _controller.Rewind(request, CancellationToken.None);
            }
            catch (Exception) { }

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.Is<CreateRewindToBlockCommand>(q => q.Block == request.Block), CancellationToken.None), Times.Once);
            _mediator.Verify(callTo => callTo.Send(It.IsAny<MakeIndexerUnlockCommand>(), CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task Rewind_RewindToBlockReturnsFalse_ThrowException()
        {
            // Arrange
            var request = new RewindRequest { Block = 10 };
            var admin = new AdminDto { Id = 1, Address = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk" };

            _context.Setup(callTo => callTo.Wallet).Returns(admin.Address);
            _mediator.Setup(callTo => callTo.Send(It.IsAny<GetAdminByAddressQuery>(), CancellationToken.None)).ReturnsAsync(admin);
            _mediator.Setup(m => m.Send(It.IsAny<MakeIndexerLockCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Unit.Value);

            _mediator.Setup(callTo => callTo.Send(It.IsAny<CreateRewindToBlockCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

            // Act
            Task Act() => _controller.Rewind(request, CancellationToken.None);

            // Assert
            await Assert.ThrowsAnyAsync<Exception>(Act);
        }

        [Fact]
        public async Task Rewind_RewindToBlockReturnsTrue_Return204NoContent()
        {
            // Arrange
            var request = new RewindRequest { Block = 10 };
            var admin = new AdminDto { Id = 1, Address = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk" };

            _context.Setup(callTo => callTo.Wallet).Returns(admin.Address);
            _mediator.Setup(callTo => callTo.Send(It.IsAny<GetAdminByAddressQuery>(), CancellationToken.None)).ReturnsAsync(admin);
            _mediator.Setup(m => m.Send(It.IsAny<MakeIndexerLockCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Unit.Value);

            _mediator.Setup(callTo => callTo.Send(It.IsAny<CreateRewindToBlockCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

            // Act
            var response = await _controller.Rewind(request, CancellationToken.None);

            // Assert
            response.As<StatusCodeResult>().StatusCode.Should().Be(StatusCodes.Status204NoContent);
        }

        [Fact]
        public async Task Rewind_Sends_CreateRewindToBlockCommand_ThrowsInvalidDataException_MakeIndexerUnlockCommand()
        {
            // Arrange
            var request = new RewindRequest { Block = 10 };
            var admin = new AdminDto { Id = 1, Address = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk" };

            _context.Setup(callTo => callTo.Wallet).Returns(admin.Address);
            _mediator.Setup(callTo => callTo.Send(It.IsAny<GetAdminByAddressQuery>(), CancellationToken.None)).ReturnsAsync(admin);
            _mediator.Setup(m => m.Send(It.IsAny<MakeIndexerLockCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Unit.Value);
            _mediator.Setup(m => m.Send(It.IsAny<CreateRewindToBlockCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidDataException("block", "Exception message."));

            // Act
            try
            {
                await _controller.Rewind(request, CancellationToken.None);
            }
            catch { }

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.Is<CreateRewindToBlockCommand>(q => q.Block == request.Block), CancellationToken.None), Times.Once);
            _mediator.Verify(callTo => callTo.Send(It.IsAny<MakeIndexerUnlockCommand>(), CancellationToken.None), Times.Once);
        }
    }
}
