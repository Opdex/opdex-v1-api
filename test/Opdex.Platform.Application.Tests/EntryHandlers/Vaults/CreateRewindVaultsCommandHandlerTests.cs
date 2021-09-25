using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Vaults;
using Opdex.Platform.Application.Abstractions.EntryCommands.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Application.EntryHandlers.Vaults;
using Opdex.Platform.Domain.Models.Vaults;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Vaults
{
    public class CreateRewindVaultsCommandHandlerTests
    {
        private readonly Mock<IMediator> _mediator;
        private readonly CreateRewindVaultsCommandHandler _handler;

        public CreateRewindVaultsCommandHandlerTests()
        {
            _mediator = new Mock<IMediator>();
            _handler = new CreateRewindVaultsCommandHandler(_mediator.Object, Mock.Of<ILogger<CreateRewindVaultsCommandHandler>>());
        }

        [Fact]
        public void CreateRewindVaultsCommand_InvalidRewindHeight_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            const ulong rewindHeight = 0;

            // Act
            void Act() => new CreateRewindVaultsCommand(rewindHeight);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Contains("Rewind height must be greater than zero.");
        }

        [Fact]
        public async Task CreateRewindVaultsCommand_Sends_RetrieveVaultsByModifiedBlockQuery()
        {
            // Arrange
            const ulong rewindHeight = 10;

            // Act
            try
            {
                await _handler.Handle(new CreateRewindVaultsCommand(rewindHeight), CancellationToken.None);
            }
            catch { }

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveVaultsByModifiedBlockQuery>(q => q.BlockHeight == rewindHeight),
                                                   It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateRewindVaultsCommand_Sends_MakeVaultCommand()
        {
            // Arrange
            const ulong rewindHeight = 10;

            var vaults = new List<Vault>
            {
                new Vault(1, "PXXNMivLgqqART1GLsMroh6zwmH1iU9Ejm", 2, "PT1GLsMroh6zXXNMU9EjmivLgqqARwmH1i", 3ul, 4, 5, 6),
                new Vault(2, "PXXNsMroh6zwmH1iU9EjmMivLgqqART1GL", 3, "PivLgqqARwmH1iT1GLsMroh6zXXNMU9Ejm", 4ul, 5, 6, 7),
                new Vault(3, "PXXroh6zwmH1iU9NMivLgqqART1GLsMEjm", 4, "P6zXXNMU9EjmivLgqqARwmH1iT1GLsMroh", 5ul, 6, 7, 8),
                new Vault(4, "Proh6zwmH1iU9EjmXXNMivLgqqART1GLsM", 5, "PTivLgqqARwmH1i1GLsMroh6zXXNMU9Ejm", 6ul, 7, 8, 9)
            };

            _mediator.Setup(callTo => callTo.Send(It.Is<RetrieveVaultsByModifiedBlockQuery>(q => q.BlockHeight == rewindHeight),
                                                  It.IsAny<CancellationToken>())).ReturnsAsync(vaults);

            // Act
            await _handler.Handle(new CreateRewindVaultsCommand(rewindHeight), CancellationToken.None);

            // Assert
            foreach (var vault in vaults)
            {
                _mediator.Verify(callTo => callTo.Send(It.Is<MakeVaultCommand>(q => q.BlockHeight == rewindHeight &&
                                                                                       q.Vault == vault &&
                                                                                       q.RefreshOwner == true &&
                                                                                       q.RefreshGenesis == true &&
                                                                                       q.RefreshSupply == true),
                                                       It.IsAny<CancellationToken>()), Times.Once);
            }
        }
    }
}
