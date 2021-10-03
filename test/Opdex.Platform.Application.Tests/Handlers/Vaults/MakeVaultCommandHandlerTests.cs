using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Application.Handlers.Vaults;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Transactions;
using Opdex.Platform.Domain.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Vaults;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Handlers.Vaults
{
    public class MakeVaultCommandHandlerTests
    {
        private readonly Mock<IMediator> _mediator;
        private readonly MakeVaultCommandHandler _handler;

        public MakeVaultCommandHandlerTests()
        {
            _mediator = new Mock<IMediator>();
            _handler = new MakeVaultCommandHandler(_mediator.Object);
        }

        [Fact]
        public void MakeVaultCommand_InvalidVault_ThrowsArgumentNullException()
        {
            // Arrange
            const ulong blockHeight = 10;

            // Act
            void Act() => new MakeVaultCommand(null, blockHeight);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Contains("Vault must be provided.");
        }

        [Fact]
        public void MakeVaultCommand_InvalidBlockHeight_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            var vault = new Vault(1, "PXXNMivLgqqART1GLsMroh6zwmH1iU9Ejm", 2, Address.Empty, "PT1GLsMroh6zXXNMU9EjmivLgqqARwmH1i", 3ul, 4, 5, 6);

            // Act
            void Act() => new MakeVaultCommand(vault, 0);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Contains("Block height must be greater than zero.");
        }

        [Theory]
        [InlineData(true, false, false, true, true)]
        [InlineData(true, false, true, false, true)]
        [InlineData(false, true, false, false, true)]
        [InlineData(true, true, true, false, true)]
        [InlineData(false, true, false, true, true)]
        [InlineData(true, true, true, true, true)]
        [InlineData(false, false, false, false, false)]
        public async Task MakeVaultCommand_Sends_RetrieveVaultContractSummaryQuery(bool refreshPendingOwner,
                                                                                   bool refreshOwner,
                                                                                   bool refreshSupply,
                                                                                   bool refreshGenesis,
                                                                                   bool expected)
        {
            // Arrange
            var vault = new Vault(1, "PXXNMivLgqqART1GLsMroh6zwmH1iU9Ejm", 2, Address.Empty, "PT1GLsMroh6zXXNMU9EjmivLgqqARwmH1i", 3ul, 4, 5, 6);
            const ulong blockHeight = 10;

            // Act
            try
            {
                await _handler.Handle(new MakeVaultCommand(vault, blockHeight,
                                                           refreshGenesis: refreshGenesis,
                                                           refreshPendingOwner: refreshPendingOwner,
                                                           refreshOwner: refreshOwner,
                                                           refreshSupply: refreshSupply), CancellationToken.None);
            }
            catch { }

            // Assert
            var times = expected ? Times.Once() : Times.Never();

            _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveVaultContractSummaryQuery>(q => q.Vault == vault.Address &&
                                                                                                   q.BlockHeight == blockHeight &&
                                                                                                   q.IncludeGenesis == refreshGenesis &&
                                                                                                   q.IncludePendingOwner == refreshPendingOwner &&
                                                                                                   q.IncludeOwner == refreshOwner &&
                                                                                                   q.IncludeSupply == refreshSupply &&
                                                                                                   q.IncludeLockedToken == false),
                                                   It.IsAny<CancellationToken>()), times);
        }

        [Theory]
        [InlineData(false, false, false, true)]
        [InlineData(false, false, true, false)]
        [InlineData(false, true, false, false)]
        [InlineData(true, false, false, false)]
        [InlineData(true, true, false, false)]
        [InlineData(true, true, true, false)]
        [InlineData(true, true, false, true)]
        [InlineData(false, true, true, false)]
        [InlineData(false, true, true, true)]
        [InlineData(false, true, false, true)]
        [InlineData(false, false, true, true)]
        [InlineData(true, true, true, true)]
        [InlineData(false, false, false, false)]
        public async Task MakeVaultCommand_Sends_PersistVaultCommand(bool refreshPendingOwner, bool refreshOwner, bool refreshSupply, bool refreshGenesis)
        {
            // Arrange
            Address currentPendingOwner = Address.Empty;
            Address newPendingOwner = "PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh";

            Address currentOwner = "PT1GLsMroh6zXXNMU9EjmivLgqqARwmH1i";
            Address newOwner = "PRwmH1iT1GLsMroh6zXXNMU9EjmivLgqqA";

            UInt256 currentSupply = 10;
            UInt256 newSupply = 15;

            const ulong genesis = 0;
            const ulong newGenesis = 10;

            var vault = new Vault(1, "PXXNMivLgqqART1GLsMroh6zwmH1iU9Ejm", 2, currentPendingOwner, currentOwner, genesis, currentSupply, 5, 6);
            const ulong blockHeight = 10;

            _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultContractSummaryQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() =>
                {
                    var summary = new VaultContractSummary(blockHeight);

                    if (refreshPendingOwner) summary.SetPendingOwner(new SmartContractMethodParameter(newPendingOwner));
                    if (refreshOwner) summary.SetOwner(new SmartContractMethodParameter(newOwner));
                    if (refreshSupply) summary.SetUnassignedSupply(new SmartContractMethodParameter(newSupply));
                    if (refreshGenesis) summary.SetGenesis(new SmartContractMethodParameter(newGenesis));

                    return summary;
                });

            // Act
            await _handler.Handle(new MakeVaultCommand(vault, blockHeight,
                                                       refreshGenesis: refreshGenesis,
                                                       refreshPendingOwner: refreshPendingOwner,
                                                       refreshOwner: refreshOwner,
                                                       refreshSupply: refreshSupply), CancellationToken.None);

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.Is<PersistVaultCommand>(q => q.Vault.Id == vault.Id &&
                                                                                   q.Vault.Address == vault.Address &&
                                                                                   q.Vault.PendingOwner == (refreshPendingOwner ? newPendingOwner : currentPendingOwner) &&
                                                                                   q.Vault.Owner == (refreshOwner ? newOwner : currentOwner) &&
                                                                                   q.Vault.Genesis == (refreshGenesis ? newGenesis : genesis) &&
                                                                                   q.Vault.UnassignedSupply == (refreshSupply ? newSupply : currentSupply)),
                                                   It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
