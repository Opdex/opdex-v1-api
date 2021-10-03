using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Vaults;
using Opdex.Platform.Application.Abstractions.EntryCommands.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Application.EntryHandlers.Vaults;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Vaults;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Vaults
{
    public class CreateVaultCommandHandlerTests
    {
        private readonly Mock<IMediator> _mediator;
        private readonly CreateVaultCommandHandler _handler;

        public CreateVaultCommandHandlerTests()
        {
            _mediator = new Mock<IMediator>();
            _handler = new CreateVaultCommandHandler(_mediator.Object);
        }

        [Fact]
        public void CreateVaultCommand_InvalidVault_ThrowsArgumentNullException()
        {
            // Arrange
            const long tokenId = 1;
            Address owner = "Poh6zXXNMU9EjmivLgqqARwmH1iT1GLsMr";
            const ulong blockHeight = 10;

            // Act
            void Act() => new CreateVaultCommand(null, tokenId, owner, blockHeight);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Contains("Vault address must be provided.");
        }

        [Fact]
        public void CreateVaultCommand_InvalidOwner_ThrowsArgumentNullException()
        {
            // Arrange
            Address vault = "PH1iT1GLsMroh6zXXNMU9EjmivLgqqARwm";
            const long tokenId = 1;
            const ulong blockHeight = 10;

            // Act
            void Act() => new CreateVaultCommand(vault, tokenId, null, blockHeight);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Contains("Owner address must be provided.");
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void CreateVaultCommand_InvalidTokenId_ThrowsArgumentOutOfRangeException(long tokenId)
        {
            // Arrange
            Address vault = "PH1iT1GLsMroh6zXXNMU9EjmivLgqqARwm";
            Address owner = "Poh6zXXNMU9EjmivLgqqARwmH1iT1GLsMr";
            const ulong blockHeight = 10;

            // Act
            void Act() => new CreateVaultCommand(vault, tokenId, owner, blockHeight);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Contains("Token Id must be greater than zero.");
        }

        [Fact]
        public void CreateVaultCommand_InvalidBlockHeight_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            Address vault = "PH1iT1GLsMroh6zXXNMU9EjmivLgqqARwm";
            const long tokenId = 1;
            Address owner = "Poh6zXXNMU9EjmivLgqqARwmH1iT1GLsMr";

            // Act
            void Act() => new CreateVaultCommand(vault, tokenId, owner, 0);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Contains("Block height must be greater than zero.");
        }

        [Fact]
        public async Task CreateVaultCommand_Sends_RetrieveVaultByAddressQuery()
        {
            // Arrange
            Address vault = "PH1iT1GLsMroh6zXXNMU9EjmivLgqqARwm";
            const long tokenId = 1;
            Address owner = "Poh6zXXNMU9EjmivLgqqARwmH1iT1GLsMr";
            const ulong blockHeight = 10;

            // Act
            try
            {
                await _handler.Handle(new CreateVaultCommand(vault, tokenId, owner, blockHeight), CancellationToken.None);
            }
            catch { }

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveVaultByAddressQuery>(q => q.Vault == vault &&
                                                                                           q.FindOrThrow == false),
                                                   It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateVaultCommand_Returns_VaultExists()
        {
            // Arrange
            Address vault = "PH1iT1GLsMroh6zXXNMU9EjmivLgqqARwm";
            const long tokenId = 1;
            Address owner = "Poh6zXXNMU9EjmivLgqqARwmH1iT1GLsMr";
            const ulong blockHeight = 10;
            const long vaultId = 100;

            var expectedVault = new Vault(vaultId, vault, tokenId, Address.Empty, owner, 0ul, UInt256.Zero, blockHeight, blockHeight);

            _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultByAddressQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedVault);

            // Act
            var response = await _handler.Handle(new CreateVaultCommand(vault, tokenId, owner, blockHeight), CancellationToken.None);

            // Assert
            response.Should().Be(vaultId);
            _mediator.Verify(callTo => callTo.Send(It.IsAny<MakeVaultCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CreateVaultCommand_Sends_MakeVaultCommand()
        {
            // Arrange
            Address vault = "PH1iT1GLsMroh6zXXNMU9EjmivLgqqARwm";
            const long tokenId = 1;
            Address owner = "Poh6zXXNMU9EjmivLgqqARwmH1iT1GLsMr";
            const ulong blockHeight = 10;

            // Act
            await _handler.Handle(new CreateVaultCommand(vault, tokenId, owner, blockHeight), CancellationToken.None);

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.Is<MakeVaultCommand>(q => q.Vault.Id == 0 &&
                                                                                q.Refresh == false), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
