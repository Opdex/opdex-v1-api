using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Governances;
using Opdex.Platform.Application.Abstractions.Queries.Governances.Nominations;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.MiningPools;
using Opdex.Platform.Application.Handlers.Governances.Nominations;
using Opdex.Platform.Domain.Models.Governances;
using Opdex.Platform.Domain.Models.LiquidityPools;
using Opdex.Platform.Domain.Models.MiningPools;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Governances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Handlers.Governances
{
    public class MakeGovernanceNominationsCommandHandlerTests
    {
        private readonly Mock<IMediator> _mediator;
        private readonly MakeGovernanceNominationsCommandHandler _handler;

        public MakeGovernanceNominationsCommandHandlerTests()
        {
            _mediator = new Mock<IMediator>();
            _handler = new MakeGovernanceNominationsCommandHandler(_mediator.Object);
        }

        [Fact]
        public void MakeGovernanceNominationsCommand_InvalidGovernance_ThrowsArgumentNullException()
        {
            // Arrange
            const ulong blockHeight = 10;

            // Act
            void Act() => new MakeGovernanceNominationsCommand(null, blockHeight);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Governance must be provided.");
        }

        [Fact]
        public void MakeGovernanceNominationsCommand_InvalidBlockHeight_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            var governance = new MiningGovernance(1, "PT1GLsMroh6zXXNMU9EjmivLgqqARwmH1i", 2, 100, 200, 4, 300, 3, 4);
            const ulong blockHeight = 0;

            // Act
            void Act() => new MakeGovernanceNominationsCommand(governance, blockHeight);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Block height must be greater than zero.");
        }

        [Fact]
        public async Task MakeGovernanceNominationsCommand_Sends_RetrieveActiveGovernanceNominationsByGovernanceIdQuery()
        {
            // Arrange
            var governance = new MiningGovernance(1, "PT1GLsMroh6zXXNMU9EjmivLgqqARwmH1i", 2, 100, 200, 4, 300, 3, 4);
            const ulong blockHeight = 10;

            // Act
            try
            {
                await _handler.Handle(new MakeGovernanceNominationsCommand(governance, blockHeight), CancellationToken.None);
            } catch {}

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveActiveGovernanceNominationsByGovernanceIdQuery>(q => q.GovernanceId == governance.Id),
                                                   It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task MakeGovernanceNominationsCommand_Sends_CallCirrusGetGovernanceNominationsSummaryQuery()
        {
            // Arrange
            var governance = new MiningGovernance(1, "PT1GLsMroh6zXXNMU9EjmivLgqqARwmH1i", 2, 100, 200, 4, 300, 3, 4);
            const ulong blockHeight = 10;

            var expectedDbNominations = new List<MiningGovernanceNomination>
            {
                new MiningGovernanceNomination(1, 1, 2, 3, true, 11, 3, 4),
                new MiningGovernanceNomination(2, 1, 3, 4, true, 12, 3, 4),
                new MiningGovernanceNomination(3, 1, 4, 5, true, 13, 3, 4),
                new MiningGovernanceNomination(4, 1, 5, 6, true, 14, 3, 4)
            };

            _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveActiveGovernanceNominationsByGovernanceIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDbNominations);

            // Act
            try
            {
                await _handler.Handle(new MakeGovernanceNominationsCommand(governance, blockHeight), CancellationToken.None);
            } catch {}

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.Is<CallCirrusGetGovernanceNominationsSummaryQuery>(q => q.Governance == governance.Address &&
                                                                                                              q.BlockHeight == blockHeight),
                                                   It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task MakeGovernanceNominationsCommand_Sends_RetrieveLiquidityPoolByAddressQuery()
        {
            // Arrange
            var governance = new MiningGovernance(1, "PT1GLsMroh6zXXNMU9EjmivLgqqARwmH1i", 2, 100, 200, 4, 300, 3, 4);
            const ulong blockHeight = 10;

            var expectedDbNominations = new List<MiningGovernanceNomination>
            {
                new MiningGovernanceNomination(1, 1, 2, 3, true, 11, 3, 4),
                new MiningGovernanceNomination(2, 1, 3, 4, true, 12, 3, 4),
                new MiningGovernanceNomination(3, 1, 4, 5, true, 13, 3, 4),
                new MiningGovernanceNomination(4, 1, 5, 6, true, 14, 3, 4)
            };

            var expectedCirrusNominations = new List<GovernanceContractNominationSummary>
            {
                new GovernanceContractNominationSummary("PU9EMroh6zXXNMjmivLgqqARwmH1iT1GLs", 123),
                new GovernanceContractNominationSummary("PjmivLgqqARwmH1iT1GLsU9EMroh6zXXNM", 456),
                new GovernanceContractNominationSummary("PH1iT1GLsU9EMroh6zXXNMjmivLgqqARwm", 789),
                new GovernanceContractNominationSummary("PARwmH1iT1GLsU9EMroh6zXXNMjmivLgqq", 999)
            };

            _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveActiveGovernanceNominationsByGovernanceIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDbNominations);

            _mediator.Setup(callTo => callTo.Send(It.IsAny<CallCirrusGetGovernanceNominationsSummaryQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedCirrusNominations);

            // Act
            try
            {
                await _handler.Handle(new MakeGovernanceNominationsCommand(governance, blockHeight), CancellationToken.None);
            } catch {}

            // Assert
            foreach (var nomination in expectedCirrusNominations)
            {
                _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveLiquidityPoolByAddressQuery>(q => q.Address == nomination.LiquidityPool &&
                                                                                                       q.FindOrThrow == true),
                                                       It.IsAny<CancellationToken>()), Times.Once);
            }
        }

        [Fact]
        public async Task MakeGovernanceNominationsCommand_Sends_RetrieveMiningPoolByLiquidityPoolIdQuery()
        {
            // Arrange
            var governance = new MiningGovernance(1, "PT1GLsMroh6zXXNMU9EjmivLgqqARwmH1i", 2, 100, 200, 4, 300, 3, 4);
            const ulong blockHeight = 10;

            var expectedDbNominations = new List<MiningGovernanceNomination>
            {
                new MiningGovernanceNomination(1, 1, 2, 3, true, 11, 3, 4),
                new MiningGovernanceNomination(2, 1, 3, 4, true, 12, 3, 4),
                new MiningGovernanceNomination(3, 1, 4, 5, true, 13, 3, 4),
                new MiningGovernanceNomination(4, 1, 5, 6, true, 14, 3, 4)
            };

            var expectedCirrusNominations = new List<GovernanceContractNominationSummary>
            {
                new GovernanceContractNominationSummary("PU9EMroh6zXXNMjmivLgqqARwmH1iT1GLs", 123),
                new GovernanceContractNominationSummary("PjmivLgqqARwmH1iT1GLsU9EMroh6zXXNM", 456),
                new GovernanceContractNominationSummary("PH1iT1GLsU9EMroh6zXXNMjmivLgqqARwm", 789),
                new GovernanceContractNominationSummary("PARwmH1iT1GLsU9EMroh6zXXNMjmivLgqq", 999)
            };

            var expectedLiquidityPools = new List<LiquidityPool>
            {
                new LiquidityPool(2, "PU9EMroh6zXXNMjmivLgqqARwmH1iT1GLs", 20, 21, 1, 3, 4),
                new LiquidityPool(3, "PjmivLgqqARwmH1iT1GLsU9EMroh6zXXNM", 22, 23, 1, 3, 4),
                new LiquidityPool(4, "PH1iT1GLsU9EMroh6zXXNMjmivLgqqARwm", 24, 25, 1, 3, 4),
                new LiquidityPool(5, "PARwmH1iT1GLsU9EMroh6zXXNMjmivLgqq", 26, 27, 1, 3, 4)
            };

            _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveActiveGovernanceNominationsByGovernanceIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDbNominations);

            _mediator.Setup(callTo => callTo.Send(It.IsAny<CallCirrusGetGovernanceNominationsSummaryQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedCirrusNominations);

            foreach (var nomination in expectedCirrusNominations)
            {
                _mediator.Setup(callTo => callTo.Send(It.Is<RetrieveLiquidityPoolByAddressQuery>(q => q.Address == nomination.LiquidityPool),
                                                      It.IsAny<CancellationToken>()))
                    .ReturnsAsync(expectedLiquidityPools.Single(lp => lp.Address == nomination.LiquidityPool));
            }

            // Act
            try
            {
                await _handler.Handle(new MakeGovernanceNominationsCommand(governance, blockHeight), CancellationToken.None);
            } catch {}

            // Assert
            foreach (var liquidityPool in expectedLiquidityPools)
            {
                _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveMiningPoolByLiquidityPoolIdQuery>(q => q.LiquidityPoolId == liquidityPool.Id &&
                                                                                                            q.FindOrThrow == true),
                                                       It.IsAny<CancellationToken>()), Times.Once);
            }
        }

        [Fact]
        public async Task MakeGovernanceNominationsCommand_Sends_MakeMiningGovernanceNominationCommand()
        {
            // Arrange
            var governance = new MiningGovernance(1, "PT1GLsMroh6zXXNMU9EjmivLgqqARwmH1i", 2, 100, 200, 4, 300, 3, 4);
            const ulong blockHeight = 10;

            var expectedDbNominations = new List<MiningGovernanceNomination>
            {
                new MiningGovernanceNomination(1, 1, 2, 3, true, 11, 3, 4),
                new MiningGovernanceNomination(2, 1, 3, 4, true, 12, 3, 4),
                new MiningGovernanceNomination(3, 1, 4, 5, true, 13, 3, 4),
                new MiningGovernanceNomination(4, 1, 5, 6, true, 14, 3, 4)
            };

            var expectedCirrusNominations = new List<GovernanceContractNominationSummary>
            {
                new GovernanceContractNominationSummary("PU9EMroh6zXXNMjmivLgqqARwmH1iT1GLs", 123),
                new GovernanceContractNominationSummary("PjmivLgqqARwmH1iT1GLsU9EMroh6zXXNM", 456),
                new GovernanceContractNominationSummary("PH1iT1GLsU9EMroh6zXXNMjmivLgqqARwm", 789),
                new GovernanceContractNominationSummary("PARwmH1iT1GLsU9EMroh6zXXNMjmivLgqq", 999)
            };

            var expectedLiquidityPools = new List<LiquidityPool>
            {
                new LiquidityPool(4, "PU9EMroh6zXXNMjmivLgqqARwmH1iT1GLs", 20, 21, 1, 3, 4),
                new LiquidityPool(5, "PjmivLgqqARwmH1iT1GLsU9EMroh6zXXNM", 22, 23, 1, 3, 4),
                new LiquidityPool(6, "PH1iT1GLsU9EMroh6zXXNMjmivLgqqARwm", 24, 25, 1, 3, 4),
                new LiquidityPool(7, "PARwmH1iT1GLsU9EMroh6zXXNMjmivLgqq", 26, 27, 1, 3, 4)
            };

            var expectedMiningPools = new List<MiningPool>
            {
                new MiningPool(5, 4, "PivLgqqARwmH1iT1GLsU9EMroh6zXXNMjm", 100, 200, 300, 3, 4),
                new MiningPool(6, 5, "P1GLsU9EMroh6zXXNMjmivLgqqARwmH1iT", 100, 200, 300, 3, 4),
                new MiningPool(7, 6, "Ph6zXXNMjmivLgqqARwmH1iT1GLsU9EMro", 100, 200, 300, 3, 4),
                new MiningPool(8, 7, "Proh6zXXNMjmivLgqqARwmH1iT1GLsU9EM", 100, 200, 300, 3, 4),
            };

            var expectedNominations = new List<MiningGovernanceNomination>
            {
                new MiningGovernanceNomination(3, 1, 4, 5, true, 123, 3, blockHeight),
                new MiningGovernanceNomination(4, 1, 5, 6, true, 456, 3, blockHeight),
                new MiningGovernanceNomination(5, 1, 6, 7, true, 789, 2, blockHeight),
                new MiningGovernanceNomination(1, 7, 8, true, 999, blockHeight),
            };

            var updatedFalseNominations = new List<MiningGovernanceNomination>
            {
                new MiningGovernanceNomination(1, 1, 2, 3, false, 11, 3, blockHeight),
                new MiningGovernanceNomination(2, 1, 3, 4, false, 12, 3, blockHeight)
            };

            _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveActiveGovernanceNominationsByGovernanceIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDbNominations);

            _mediator.Setup(callTo => callTo.Send(It.IsAny<CallCirrusGetGovernanceNominationsSummaryQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedCirrusNominations);

            foreach (var nomination in expectedCirrusNominations)
            {
                _mediator.Setup(callTo => callTo.Send(It.Is<RetrieveLiquidityPoolByAddressQuery>(q => q.Address == nomination.LiquidityPool),
                                                      It.IsAny<CancellationToken>()))
                    .ReturnsAsync(expectedLiquidityPools.Single(lp => lp.Address == nomination.LiquidityPool));
            }

            foreach (var liquidityPool in expectedLiquidityPools)
            {
                _mediator.Setup(callTo => callTo.Send(It.Is<RetrieveMiningPoolByLiquidityPoolIdQuery>(q => q.LiquidityPoolId == liquidityPool.Id),
                                                      It.IsAny<CancellationToken>()))
                    .ReturnsAsync(expectedMiningPools.Single(mp => mp.LiquidityPoolId == liquidityPool.Id));
            }

            foreach (var nomination in expectedNominations)
            {
                var dbNomination = expectedDbNominations.SingleOrDefault(nom => nom.LiquidityPoolId == nomination.LiquidityPoolId);

                if (dbNomination == null && nomination.Id > 0)
                {
                    _mediator.Setup(callTo => callTo.Send(It.Is<RetrieveMiningGovernanceNominationByLiquidityAndMiningPoolIdQuery>(q => q.LiquidityPoolId == nomination.LiquidityPoolId),
                                                          It.IsAny<CancellationToken>()))
                        .ReturnsAsync(nomination);
                }

                _mediator.Setup(callTo => callTo.Send(It.Is<MakeMiningGovernanceNominationCommand>(q => q.Nomination.Id == nomination.Id),
                                                      It.IsAny<CancellationToken>()))
                    .ReturnsAsync(() => nomination.Id == 0 ? 6 : nomination.Id);
            }

            // Act
            try
            {
                await _handler.Handle(new MakeGovernanceNominationsCommand(governance, blockHeight), CancellationToken.None);
            } catch {}

            // Assert
            foreach (var nomination in expectedNominations)
            {
                _mediator.Verify(callTo => callTo.Send(It.Is<MakeMiningGovernanceNominationCommand>(q => q.Nomination.Id == nomination.Id &&
                                                                                                         q.Nomination.GovernanceId == nomination.GovernanceId &&
                                                                                                         q.Nomination.LiquidityPoolId == nomination.LiquidityPoolId &&
                                                                                                         q.Nomination.MiningPoolId == nomination.MiningPoolId &&
                                                                                                         q.Nomination.Weight == nomination.Weight &&
                                                                                                         q.Nomination.IsNominated == true &&
                                                                                                         q.Nomination.CreatedBlock == nomination.CreatedBlock &&
                                                                                                         q.Nomination.ModifiedBlock == nomination.ModifiedBlock),
                                                       It.IsAny<CancellationToken>()), Times.Once);
            }

            foreach (var nomination in updatedFalseNominations)
            {
                _mediator.Verify(callTo => callTo.Send(It.Is<MakeMiningGovernanceNominationCommand>(q => q.Nomination.Id == nomination.Id &&
                                                                                                         q.Nomination.GovernanceId == nomination.GovernanceId &&
                                                                                                         q.Nomination.LiquidityPoolId == nomination.LiquidityPoolId &&
                                                                                                         q.Nomination.MiningPoolId == nomination.MiningPoolId &&
                                                                                                         q.Nomination.Weight == nomination.Weight &&
                                                                                                         q.Nomination.IsNominated == false &&
                                                                                                         q.Nomination.CreatedBlock == nomination.CreatedBlock &&
                                                                                                         q.Nomination.ModifiedBlock == nomination.ModifiedBlock),
                                                       It.IsAny<CancellationToken>()), Times.Once);
            }
        }
    }
}
