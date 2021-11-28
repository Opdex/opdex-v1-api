using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.MiningGovernances;
using Opdex.Platform.Application.Abstractions.EntryCommands.MiningGovernances;
using Opdex.Platform.Application.Abstractions.Queries.MiningGovernances;
using Opdex.Platform.Application.EntryHandlers.MiningGovernances;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.MiningGovernances;
using Opdex.Platform.Domain.Models.Transactions;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.MiningGovernances
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
            const ulong stakingTokenId = 2;

            // Act
            void Act() => new CreateMiningGovernanceCommand(null, stakingTokenId, blockHeight);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Contains("Mining governance address must be provided.");
        }

        [Theory]
        [InlineData(0)]
        public void CreateMiningGovernanceCommand_InvalidStakingTokenId_ThrowsArgumentOutOfRangeException(ulong stakingTokenId)
        {
            // Arrange
            Address miningGovernance = "PH1iT1GLsMroh6zXXNMU9EjmivLgqqARwm";
            const ulong blockHeight = 10;

            // Act
            void Act() => new CreateMiningGovernanceCommand(miningGovernance, stakingTokenId, blockHeight);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Contains("Staking token id must be greater than zero.");
        }

        [Fact]
        public void CreateMiningGovernanceCommand_InvalidBlockHeight_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            Address miningGovernance = "PH1iT1GLsMroh6zXXNMU9EjmivLgqqARwm";
            const ulong stakingTokenId = 2;
            const ulong blockHeight = 0;

            // Act
            void Act() => new CreateMiningGovernanceCommand(miningGovernance, stakingTokenId, blockHeight);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Contains("Block height must be greater than zero.");
        }

        [Fact]
        public async Task CreateMiningGovernanceCommand_Sends_RetrieveMiningGovernanceByAddressQuery()
        {
            // Arrange
            Address miningGovernance = "PH1iT1GLsMroh6zXXNMU9EjmivLgqqARwm";
            const ulong blockHeight = 10;
            const ulong stakingTokenId = 2;

            // Act
            try
            {
                await _handler.Handle(new CreateMiningGovernanceCommand(miningGovernance, stakingTokenId, blockHeight), CancellationToken.None);
            }
            catch { }

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveMiningGovernanceByAddressQuery>(q => q.Address == miningGovernance &&
                                                                                                      q.FindOrThrow == false),
                                                   It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateMiningGovernanceCommand_Returns_NoUpdates()
        {
            // Arrange
            Address miningGovernance = "PH1iT1GLsMroh6zXXNMU9EjmivLgqqARwm";
            const ulong blockHeight = 10;
            const ulong stakingTokenId = 2;

            var expectedMiningGovernance = new MiningGovernance(1, "PT1GLsMroh6zXXNMU9EjmivLgqqARwmH1i", 2, 100, 200, 4, 300, 3, 11);

            _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveMiningGovernanceByAddressQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedMiningGovernance);

            // Act
            var result = await _handler.Handle(new CreateMiningGovernanceCommand(miningGovernance, stakingTokenId, blockHeight), CancellationToken.None);

            // Assert
            result.Should().Be(expectedMiningGovernance.Id);
            _mediator.Verify(callTo => callTo.Send(It.IsAny<RetrieveMiningGovernanceContractSummaryByAddressQuery>(),
                                                   It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CreateMiningGovernanceCommand_Sends_RetrieveMiningGovernanceContractSummaryByAddressQuery()
        {
            // Arrange
            Address miningGovernance = "PH1iT1GLsMroh6zXXNMU9EjmivLgqqARwm";
            const ulong blockHeight = 10;
            const ulong stakingTokenId = 2;

            // Act
            try
            {
                await _handler.Handle(new CreateMiningGovernanceCommand(miningGovernance, stakingTokenId, blockHeight), CancellationToken.None);
            }
            catch { }

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveMiningGovernanceContractSummaryByAddressQuery>(q => q.MiningGovernance == miningGovernance &&
                                                                                                                     q.BlockHeight == blockHeight &&
                                                                                                                     q.IncludeMiningDuration == true &&
                                                                                                                     q.IncludeMinedToken == false &&
                                                                                                                     q.IncludeMiningPoolReward == false &&
                                                                                                                     q.IncludeMiningPoolsFunded == false &&
                                                                                                                     q.IncludeNominationPeriodEnd == false),
                                                   It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateMiningGovernanceCommand_Sends_MakeMiningGovernanceCommand()
        {
            // Arrange
            Address miningGovernance = "PH1iT1GLsMroh6zXXNMU9EjmivLgqqARwm";
            const ulong blockHeight = 10;
            const ulong stakingTokenId = 2;
            const ulong miningDuration = 5;

            var expectedSummary = new MiningGovernanceContractSummary(blockHeight);
            expectedSummary.SetMiningDuration(new SmartContractMethodParameter(miningDuration));

            _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveMiningGovernanceContractSummaryByAddressQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedSummary);

            // Act
            await _handler.Handle(new CreateMiningGovernanceCommand(miningGovernance, stakingTokenId, blockHeight), CancellationToken.None);

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.Is<MakeMiningGovernanceCommand>(q => q.MiningGovernance.Id == 0 &&
                                                                                           q.MiningGovernance.Address == miningGovernance &&
                                                                                           q.MiningGovernance.MiningDuration == miningDuration &&
                                                                                           q.MiningGovernance.TokenId == stakingTokenId &&
                                                                                           q.BlockHeight == blockHeight &&
                                                                                           q.RefreshMiningPoolReward == false &&
                                                                                           q.RefreshMiningPoolsFunded == false &&
                                                                                           q.RefreshNominationPeriodEnd == false &&
                                                                                           q.RefreshMinedToken == false &&
                                                                                           q.RefreshMiningDuration == false),
                                                   It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
