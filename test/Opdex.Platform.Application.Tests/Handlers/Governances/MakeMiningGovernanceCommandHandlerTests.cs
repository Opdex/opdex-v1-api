using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Governances;
using Opdex.Platform.Application.Abstractions.Queries.Governances;
using Opdex.Platform.Application.Handlers.Governances;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Governances;
using Opdex.Platform.Domain.Models.Transactions;
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
            void Act() => new MakeMiningGovernanceCommand(null, blockHeight);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Contains("Mining governance address must be provided.");
        }

        [Fact]
        public void MakeMiningGovernanceCommand_InvalidBlockHeight_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            var governance = new MiningGovernance(1, "PH1iT1GLsMroh6zXXNMU9EjmivLgqqARwm", 2, 100, 200, 4, 300, 3, 4);

            // Act
            void Act() => new MakeMiningGovernanceCommand(governance, 0);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Contains("Block height must be greater than zero.");
        }

        [Theory]
        [InlineData(false, false, true, true, false, true)]
        [InlineData(false, true, false, true, true, true)]
        [InlineData(true, false, false, true, false, true)]
        [InlineData(true, true, false, true, false, true)]
        [InlineData(true, false, true, true, false, true)]
        [InlineData(true, true, true, true, false, true)]
        [InlineData(false, false, false, false, false, false)]
        public async Task MakeMiningGovernanceCommand_Sends_RetrieveMiningGovernanceContractSummaryByAddressQuery(bool refreshNominationPeriodEnd,
                                                                                                                  bool refreshMiningPoolsFunded,
                                                                                                                  bool refreshMiningPoolReward,
                                                                                                                  bool refreshMiningDuration,
                                                                                                                  bool refreshMinedToken,
                                                                                                                  bool expected)
        {
            // Arrange
            var governance = new MiningGovernance(1, "PH1iT1GLsMroh6zXXNMU9EjmivLgqqARwm", 2, 100, 200, 4, 300, 3, 4);
            const ulong blockHeight = 10;

            // Act
            try
            {
                await _handler.Handle(new MakeMiningGovernanceCommand(governance, blockHeight,
                                                                      refreshMinedToken: refreshMinedToken,
                                                                      refreshMiningPoolReward: refreshMiningPoolReward,
                                                                      refreshNominationPeriodEnd: refreshNominationPeriodEnd,
                                                                      refreshMiningPoolsFunded: refreshMiningPoolsFunded,
                                                                      refreshMiningDuration: refreshMiningDuration), CancellationToken.None);
            } catch { }

            // Assert
            var times = expected ? Times.Once() : Times.Never();

            _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveMiningGovernanceContractSummaryByAddressQuery>(q => q.Governance == governance.Address &&
                                                                                                                q.BlockHeight == blockHeight &&
                                                                                                                q.IncludeNominationPeriodEnd == refreshNominationPeriodEnd &&
                                                                                                                q.IncludeMiningPoolsFunded == refreshMiningPoolsFunded &&
                                                                                                                q.IncludeMiningPoolReward == refreshMiningPoolReward &&
                                                                                                                q.IncludeMiningDuration == refreshMiningDuration &&
                                                                                                                q.IncludeMinedToken == refreshMinedToken),
                                                   It.IsAny<CancellationToken>()), times);
        }

        [Theory]
        [InlineData(false, false, true, true)]
        [InlineData(false, true, false, true)]
        [InlineData(true, false, false, true)]
        [InlineData(true, true, false, true)]
        [InlineData(true, false, true, true)]
        [InlineData(true, true, true, true)]
        [InlineData(false, false, false, false)]
        public async Task MakeMiningGovernanceCommand_Sends_PersistMiningGovernanceCommand_Rewind(bool refreshNominationPeriodEnd,
                                                                                                  bool refreshMiningPoolsFunded,
                                                                                                  bool refreshMiningPoolReward,
                                                                                                  bool refreshMiningDuration)
        {
            const ulong currentNominationPeriodEnd = 100ul;
            const ulong updatedNominationPeriodEnd = 200ul;

            const ulong currentMiningDuration = 0ul;
            const ulong updatedMiningDuration = 100ul;

            UInt256 currentReward = 150;
            UInt256 updatedReward = 100;

            const uint currentPoolsFunded = 4;
            const uint updatedPoolsFunded = 8;

            const ulong blockHeight = 10;
            const ulong createdBlock = 10;
            const ulong modifiedBlock = blockHeight;

            // Arrange
            var governance = new MiningGovernance(1, "PH1iT1GLsMroh6zXXNMU9EjmivLgqqARwm", 2, currentNominationPeriodEnd,
                                                  currentMiningDuration, currentPoolsFunded, currentReward, createdBlock, createdBlock);



            _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveMiningGovernanceContractSummaryByAddressQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() =>
                {
                    var summary = new MiningGovernanceContractSummary(blockHeight);

                    if (refreshNominationPeriodEnd) summary.SetNominationPeriodEnd(new SmartContractMethodParameter((updatedNominationPeriodEnd)));
                    if (refreshMiningPoolsFunded) summary.SetMiningPoolsFunded(new SmartContractMethodParameter((updatedPoolsFunded)));
                    if (refreshMiningPoolReward) summary.SetMiningPoolReward(new SmartContractMethodParameter((updatedReward)));
                    if (refreshMiningDuration) summary.SetMiningDuration(new SmartContractMethodParameter((updatedMiningDuration)));

                    return summary;
                });

            // Act
            await _handler.Handle(new MakeMiningGovernanceCommand(governance, blockHeight,
                                                                  refreshMiningPoolReward: refreshMiningPoolReward,
                                                                  refreshNominationPeriodEnd: refreshNominationPeriodEnd,
                                                                  refreshMiningPoolsFunded: refreshMiningPoolsFunded,
                                                                  refreshMiningDuration: refreshMiningDuration), CancellationToken.None);

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.Is<PersistMiningGovernanceCommand>(q => q.MiningGovernance.Id == governance.Id &&
                                                                                              q.MiningGovernance.Address == governance.Address &&
                                                                                              q.MiningGovernance.MiningDuration == (refreshMiningDuration ? updatedMiningDuration : currentMiningDuration) &&
                                                                                              q.MiningGovernance.MiningPoolsFunded == (refreshMiningPoolsFunded ? updatedPoolsFunded : currentPoolsFunded) &&
                                                                                              q.MiningGovernance.NominationPeriodEnd == (refreshNominationPeriodEnd ? updatedNominationPeriodEnd : currentNominationPeriodEnd) &&
                                                                                              q.MiningGovernance.MiningPoolReward == (refreshMiningPoolReward ? updatedReward : currentReward) &&
                                                                                              q.MiningGovernance.ModifiedBlock == modifiedBlock),
                                                   It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
