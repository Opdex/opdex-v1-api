using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Governances;
using Opdex.Platform.Application.Abstractions.EntryCommands.Governances;
using Opdex.Platform.Application.Abstractions.Queries.Governances;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.EntryHandlers.Governances;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Governances;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Governances
{
    public class CreateMiningGovernanceCommandHandlerTests
    {
        private readonly Mock<IMediator> _mediator;
        private readonly CreateMiningGovernanceCommandHandler _handler;

        public CreateMiningGovernanceCommandHandlerTests()
        {
            _mediator = new Mock<IMediator>();
            _handler = new CreateMiningGovernanceCommandHandler(_mediator.Object);
        }

        [Fact]
        public void CreateMiningGovernanceCommand_InvalidDeployer_ThrowsArgumentNullException()
        {
            // Arrange
            const ulong blockHeight = 10;

            // Act
            void Act() => new CreateMiningGovernanceCommand(null, blockHeight, false);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Contains("Governance address must be provided.");
        }

        [Fact]
        public void CreateMiningGovernanceCommand_InvalidBlockHeight_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            Address governance = "PH1iT1GLsMroh6zXXNMU9EjmivLgqqARwm";

            // Act
            void Act() => new CreateMiningGovernanceCommand(governance, 0, true);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Contains("Block height must be greater than zero.");
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task CreateMiningGovernanceCommand_Sends_RetrieveMiningGovernanceByAddressQuery(bool isUpdate)
        {
            // Arrange
            Address governance = "PH1iT1GLsMroh6zXXNMU9EjmivLgqqARwm";
            const ulong blockHeight = 10;

            // Act
            try
            {
                await _handler.Handle(new CreateMiningGovernanceCommand(governance, blockHeight, isUpdate), CancellationToken.None);
            } catch { }

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveMiningGovernanceByAddressQuery>(q => q.Address == governance &&
                                                                                                      q.FindOrThrow == isUpdate),
                                                   It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateMiningGovernanceCommand_Returns_NoUpdates()
        {
            // Arrange
            Address governance = "PH1iT1GLsMroh6zXXNMU9EjmivLgqqARwm";
            const ulong blockHeight = 10;
            const bool isUpdate = true;

            var expectedGovernance = new MiningGovernance(1, "PT1GLsMroh6zXXNMU9EjmivLgqqARwmH1i", 2, 100, 200, 4, 300, 3, 11);

            _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveMiningGovernanceByAddressQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedGovernance);

            // Act
            var result = await _handler.Handle(new CreateMiningGovernanceCommand(governance, blockHeight, isUpdate), CancellationToken.None);

            // Assert
            result.Should().Be(expectedGovernance.Id);
            _mediator.Verify(callTo => callTo.Send(It.IsAny<RetrieveMiningGovernanceContractSummaryByAddressQuery>(),
                                                   It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CreateMiningGovernanceCommand_Sends_RetrieveMiningGovernanceContractSummaryByAddressQuery()
        {
            // Arrange
            Address governance = "PH1iT1GLsMroh6zXXNMU9EjmivLgqqARwm";
            const ulong blockHeight = 10;
            const bool isUpdate = true;

            _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveMiningGovernanceByAddressQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new MiningGovernance(1, "PT1GLsMroh6zXXNMU9EjmivLgqqARwmH1i", 2, 100, 200, 4, 300, 3, 4));

            // Act
            try
            {
                await _handler.Handle(new CreateMiningGovernanceCommand(governance, blockHeight, isUpdate), CancellationToken.None);
            } catch { }

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveMiningGovernanceContractSummaryByAddressQuery>(q => q.Governance == governance &&
                                                                                                                     q.BlockHeight == blockHeight),
                                                   It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateMiningGovernanceCommand_Sends_RetrieveTokenByAddressQuery()
        {
            // Arrange
            Address governance = "PH1iT1GLsMroh6zXXNMU9EjmivLgqqARwm";
            Address governanceToken = "PoARwmh6zXXH1iT1GLsMrNMU9EjmivLgqq";
            const ulong blockHeight = 10;
            const bool isUpdate = false;

            var expectedSummary = new MiningGovernanceContractSummary(governance, governanceToken, 1, 2, 3, 4);

            _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveMiningGovernanceContractSummaryByAddressQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedSummary);

            // Act
            try
            {
                await _handler.Handle(new CreateMiningGovernanceCommand(governance, blockHeight, isUpdate), CancellationToken.None);
            } catch { }

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveTokenByAddressQuery>(q => q.Address == governanceToken),
                                                   It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateMiningGovernanceCommand_Sends_MakeMiningGovernanceCommand()
        {
            // Arrange
            Address governance = "PH1iT1GLsMroh6zXXNMU9EjmivLgqqARwm";
            const ulong blockHeight = 10;
            const bool isUpdate = true;

            var existingGovernance = new MiningGovernance(1, governance, 2, 100, 200, 4, 300, 3, 4);
            var expectedSummary = new MiningGovernanceContractSummary(governance, "PoARwmh6zXXH1iT1GLsMrNMU9EjmivLgqq", 1, 2, 3, 4);

            _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveMiningGovernanceByAddressQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingGovernance);

            _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveMiningGovernanceContractSummaryByAddressQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedSummary);

            // Act
            try
            {
                await _handler.Handle(new CreateMiningGovernanceCommand(governance, blockHeight, isUpdate), CancellationToken.None);
            } catch { }

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.Is<MakeMiningGovernanceCommand>(q => q.MiningGovernance.Id == existingGovernance.Id &&
                                                                                           q.MiningGovernance.Address == governance &&
                                                                                           q.MiningGovernance.MiningDuration == expectedSummary.MiningDuration &&
                                                                                           q.MiningGovernance.MiningPoolsFunded == expectedSummary.MiningPoolsFunded &&
                                                                                           q.MiningGovernance.NominationPeriodEnd == expectedSummary.NominationPeriodEnd &&
                                                                                           q.MiningGovernance.MiningPoolReward == expectedSummary.MiningPoolReward &&
                                                                                           q.BlockHeight == blockHeight &&
                                                                                           q.Rewind == false),
                                                   It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
