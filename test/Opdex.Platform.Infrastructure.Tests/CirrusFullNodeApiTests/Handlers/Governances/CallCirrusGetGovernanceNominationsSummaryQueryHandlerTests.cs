using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Governances;
using Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.Governances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.CirrusFullNodeApiTests.Handlers.Governances
{
    public class CallCirrusGetGovernanceNominationsSummaryQueryHandlerTests
    {
        private readonly Mock<ISmartContractsModule> _smartContractsModuleMock;
        private readonly CallCirrusGetGovernanceNominationsSummaryQueryHandler _handler;

        public CallCirrusGetGovernanceNominationsSummaryQueryHandlerTests()
        {
            var logger = NullLogger<CallCirrusGetGovernanceNominationsSummaryQueryHandler>.Instance;
            _smartContractsModuleMock = new Mock<ISmartContractsModule>();
            _handler = new CallCirrusGetGovernanceNominationsSummaryQueryHandler(_smartContractsModuleMock.Object, logger);
        }

        [Fact]
        public void CallCirrusGetGovernanceNominationsSummaryQuery_InvalidGovernance_ThrowsArgumentNullException()
        {
            // Arrange
            Address governance = Address.Empty;
            const ulong blockHeight = 10;

            // Act
            void Act() => new CallCirrusGetGovernanceNominationsSummaryQuery(governance, blockHeight);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Governance address must be provided.");
        }

        [Fact]
        public void CallCirrusGetGovernanceNominationsSummaryQuery_InvalidBlockHeight_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            Address governance = "PU9EjmivLgqqARwmH1iT1GLsMroh6zXXNM";
            const ulong blockHeight = 0;

            // Act
            void Act() => new CallCirrusGetGovernanceNominationsSummaryQuery(governance, blockHeight);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Block height must be greater than zero.");
        }

        [Fact]
        public async Task CallCirrusGetGovernanceNominationsSummaryQuery_Sends_LocalCallAsync()
        {
            // Arrange
            Address governance = "PU9EjmivLgqqARwmH1iT1GLsMroh6zXXNM";
            const ulong blockHeight = 10;

            // Act
            try
            {
                await _handler.Handle(new CallCirrusGetGovernanceNominationsSummaryQuery(governance, blockHeight), CancellationToken.None);
            } catch {}

            // Assert
            _smartContractsModuleMock.Verify(callTo => callTo.LocalCallAsync(It.Is<LocalCallRequestDto>(q => q.Amount == FixedDecimal.Zero &&
                                                                                                             q.MethodName == "get_Nominations" &&
                                                                                                             q.ContractAddress == governance),
                                                                             It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CallCirrusGetGovernanceNominationsSummaryQuery_Returns_GovernanceContractNominationSummaries()
        {
            // Arrange
            Address governance = "PU9EjmivLgqqARwmH1iT1GLsMroh6zXXNM";
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
            var response = await _handler.Handle(new CallCirrusGetGovernanceNominationsSummaryQuery(governance, blockHeight), CancellationToken.None);

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
