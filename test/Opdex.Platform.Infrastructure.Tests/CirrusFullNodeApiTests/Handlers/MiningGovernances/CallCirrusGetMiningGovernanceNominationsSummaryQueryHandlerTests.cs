using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.MiningGovernances;
using Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.MiningGovernances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.CirrusFullNodeApiTests.Handlers.MiningGovernances
{
    public class CallCirrusGetMiningGovernanceNominationsSummaryQueryHandlerTests
    {
        private readonly Mock<ISmartContractsModule> _smartContractsModuleMock;
        private readonly CallCirrusGetMiningGovernanceNominationsSummaryQueryHandler _handler;

        public CallCirrusGetMiningGovernanceNominationsSummaryQueryHandlerTests()
        {
            _smartContractsModuleMock = new Mock<ISmartContractsModule>();
            _handler = new CallCirrusGetMiningGovernanceNominationsSummaryQueryHandler(_smartContractsModuleMock.Object);
        }

        [Fact]
        public void CallCirrusGetMiningGovernanceNominationsSummaryQuery_InvalidGovernance_ThrowsArgumentNullException()
        {
            // Arrange
            Address miningGovernance = Address.Empty;
            const ulong blockHeight = 10;

            // Act
            void Act() => new CallCirrusGetMiningGovernanceNominationsSummaryQuery(miningGovernance, blockHeight);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Mining governance address must be provided.");
        }

        [Fact]
        public void CallCirrusGetMiningGovernanceNominationsSummaryQuery_InvalidBlockHeight_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            Address miningGovernance = "PU9EjmivLgqqARwmH1iT1GLsMroh6zXXNM";
            const ulong blockHeight = 0;

            // Act
            void Act() => new CallCirrusGetMiningGovernanceNominationsSummaryQuery(miningGovernance, blockHeight);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Block height must be greater than zero.");
        }

        [Fact]
        public async Task CallCirrusGetMiningGovernanceNominationsSummaryQuery_Sends_LocalCallAsync()
        {
            // Arrange
            Address miningGovernance = "PU9EjmivLgqqARwmH1iT1GLsMroh6zXXNM";
            const ulong blockHeight = 10;

            // Act
            try
            {
                await _handler.Handle(new CallCirrusGetMiningGovernanceNominationsSummaryQuery(miningGovernance, blockHeight), CancellationToken.None);
            }
            catch { }

            // Assert
            _smartContractsModuleMock.Verify(callTo => callTo.LocalCallAsync(It.Is<LocalCallRequestDto>(q => q.Amount == FixedDecimal.Zero &&
                                                                                                             q.MethodName == "get_Nominations" &&
                                                                                                             q.ContractAddress == miningGovernance),
                                                                             It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CallCirrusGetMiningGovernanceNominationsSummaryQuery_Returns_MiningGovernanceContractNominationSummaries()
        {
            // Arrange
            Address miningGovernance = "PU9EjmivLgqqARwmH1iT1GLsMroh6zXXNM";
            const ulong blockHeight = 10;

            var expectedNominations = new List<NominationResponse>
            {
                new NominationResponse{ StakingPool = "PU9EMroh6zXXNMjmivLgqqARwmH1iT1GLs", Weight = 123 },
                new NominationResponse{ StakingPool = "PjmivLgqqARwmH1iT1GLsU9EMroh6zXXNM", Weight = 456 },
                new NominationResponse{ StakingPool = "PH1iT1GLsU9EMroh6zXXNMjmivLgqqARwm", Weight = 789 },
                new NominationResponse{ StakingPool = "PARwmH1iT1GLsU9EMroh6zXXNMjmivLgqq", Weight = 999 }
            };

            var expectedResponse = new LocalCallResponseDto { Return = expectedNominations };

            _smartContractsModuleMock.Setup(callTo => callTo.LocalCallAsync(It.IsAny<LocalCallRequestDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var response = await _handler.Handle(new CallCirrusGetMiningGovernanceNominationsSummaryQuery(miningGovernance, blockHeight), CancellationToken.None);

            // Assert
            foreach (var nomination in response)
            {
                var expected = expectedNominations.Single(nom => nom.StakingPool == nomination.LiquidityPool);

                nomination.StakingWeight.Should().Be(expected.Weight);
            }
        }

        private sealed class NominationResponse
        {
            public Address StakingPool { get; set; }
            public UInt256 Weight { get; set; }
        }
    }
}
