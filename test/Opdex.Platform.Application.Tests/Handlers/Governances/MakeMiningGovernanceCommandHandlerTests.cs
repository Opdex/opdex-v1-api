using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Governances;
using Opdex.Platform.Application.Abstractions.Queries.Governances;
using Opdex.Platform.Application.Handlers.Governances;
using Opdex.Platform.Domain.Models.Governances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Governances;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Handlers.Governances
{
    public class MakeMiningGovernanceCommandHandlerTests
    {
        private readonly Mock<IMediator> _mediator;
        private readonly MakeMiningGovernanceCommandHandler _handler;

        public MakeMiningGovernanceCommandHandlerTests()
        {
            _mediator = new Mock<IMediator>();
            _handler = new MakeMiningGovernanceCommandHandler(_mediator.Object);
        }

        [Fact]
        public void MakeMiningGovernanceCommand_InvalidMiningGovernance_ThrowsArgumentNullException()
        {
            // Arrange
            const ulong blockHeight = 10;

            // Act
            void Act() => new MakeMiningGovernanceCommand(null, blockHeight, false);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Contains("Mining governance address must be provided.");
        }

        [Fact]
        public void MakeMiningGovernanceCommand_InvalidBlockHeight_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            var governance = new MiningGovernance(1, "PH1iT1GLsMroh6zXXNMU9EjmivLgqqARwm", 2, 100, 200, 4, 300, 3, 4);

            // Act
            void Act() => new MakeMiningGovernanceCommand(governance, 0, true);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Contains("Block height must be greater than zero.");
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task MakeMiningGovernanceCommand_Sends_RetrieveMiningGovernanceContractSummaryByAddressQuery(bool rewind)
        {
            // Arrange
            var governance = new MiningGovernance(1, "PH1iT1GLsMroh6zXXNMU9EjmivLgqqARwm", 2, 100, 200, 4, 300, 3, 4);
            const ulong blockHeight = 10;

            // Act
            try
            {
                await _handler.Handle(new MakeMiningGovernanceCommand(governance, blockHeight, rewind), CancellationToken.None);
            } catch { }

            // Assert
            Times times = rewind ? Times.Once() : Times.Never();

            _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveMiningGovernanceContractSummaryByAddressQuery>(q => q.Governance == governance.Address &&
                                                                                                                q.BlockHeight == blockHeight),
                                                   It.IsAny<CancellationToken>()), times);
        }

        [Fact]
        public async Task MakeMiningGovernanceCommand_Sends_PersistMiningGovernanceCommand_Rewind()
        {
            // Arrange
            var governance = new MiningGovernance(1, "PH1iT1GLsMroh6zXXNMU9EjmivLgqqARwm", 2, 100, 200, 4, 300, 3, 4);
            const ulong blockHeight = 10;

            var expectedSummary = new MiningGovernanceContractSummary(governance.Address, "PoARwmh6zXXH1iT1GLsMrNMU9EjmivLgqq", 1, 2, 3, 4);

            _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveMiningGovernanceContractSummaryByAddressQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedSummary);

            // Act
            await _handler.Handle(new MakeMiningGovernanceCommand(governance, blockHeight, rewind: true), CancellationToken.None);

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.Is<PersistMiningGovernanceCommand>(q => q.MiningGovernance.Id == governance.Id &&
                                                                                              q.MiningGovernance.Address == governance.Address &&
                                                                                              q.MiningGovernance.MiningDuration == expectedSummary.MiningDuration &&
                                                                                              q.MiningGovernance.MiningPoolsFunded == expectedSummary.MiningPoolsFunded &&
                                                                                              q.MiningGovernance.NominationPeriodEnd == expectedSummary.NominationPeriodEnd &&
                                                                                              q.MiningGovernance.MiningPoolReward == expectedSummary.MiningPoolReward),
                                                   It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task MakeMiningGovernanceCommand_Sends_PersistMiningGovernanceCommand_NoRewind()
        {
            // Arrange
            var governance = new MiningGovernance(1, "PH1iT1GLsMroh6zXXNMU9EjmivLgqqARwm", 2, 100, 200, 4, 300, 3, 4);
            const ulong blockHeight = 10;

            // Act
            await _handler.Handle(new MakeMiningGovernanceCommand(governance, blockHeight, rewind: false), CancellationToken.None);

            // Assert

            _mediator.Verify(callTo => callTo.Send(It.Is<PersistMiningGovernanceCommand>(q => q.MiningGovernance.Id == governance.Id &&
                                                                                              q.MiningGovernance.Address == governance.Address &&
                                                                                              q.MiningGovernance.MiningDuration == governance.MiningDuration &&
                                                                                              q.MiningGovernance.MiningPoolsFunded == governance.MiningPoolsFunded &&
                                                                                              q.MiningGovernance.NominationPeriodEnd == governance.NominationPeriodEnd &&
                                                                                              q.MiningGovernance.MiningPoolReward == governance.MiningPoolReward),
                                                   It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
