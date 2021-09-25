using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Governances;
using Opdex.Platform.Application.Abstractions.EntryCommands.Governances;
using Opdex.Platform.Application.Abstractions.Queries.Governances;
using Opdex.Platform.Application.Abstractions.Queries.Governances.Nominations;
using Opdex.Platform.Application.EntryHandlers.Governances;
using Opdex.Platform.Domain.Models.Governances;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Governances
{
    public class CreateRewindMiningGovernancesAndNominationsCommandHandlerTests
    {
        private readonly Mock<IMediator> _mediator;
        private readonly CreateRewindMiningGovernancesAndNominationsCommandHandler _handler;

        public CreateRewindMiningGovernancesAndNominationsCommandHandlerTests()
        {
            _mediator = new Mock<IMediator>();
            _handler = new CreateRewindMiningGovernancesAndNominationsCommandHandler(_mediator.Object, Mock.Of<ILogger<CreateRewindMiningGovernancesAndNominationsCommandHandler>>());
        }

        [Fact]
        public void CreateRewindMiningGovernancesAndNominationsCommand_InvalidRewindHeight_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            const ulong rewindHeight = 0;

            // Act
            void Act() => new CreateRewindMiningGovernancesAndNominationsCommand(rewindHeight);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Contains("Rewind height must be greater than zero.");
        }

        [Fact]
        public async Task CreateRewindMiningGovernancesAndNominationsCommand_Sends_RetrieveMiningGovernancesByModifiedBlockQuery()
        {
            // Arrange
            const ulong rewindHeight = 10;

            // Act
            try
            {
                await _handler.Handle(new CreateRewindMiningGovernancesAndNominationsCommand(rewindHeight), CancellationToken.None);
            }
            catch { }

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveMiningGovernancesByModifiedBlockQuery>(q => q.BlockHeight == rewindHeight),
                                                   It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateRewindMiningGovernancesAndNominationsCommand_Sends_MakeMiningGovernanceCommand()
        {
            // Arrange
            const ulong rewindHeight = 10;

            var governances = new List<MiningGovernance>
            {
                new MiningGovernance(1, "PT1GLsMroh6zXXNMU9EjmivLgqqARwmH1i", 2, 100, 200, 4, 300, 3, 4),
                new MiningGovernance(1, "PU9EjmivLgqqARwmH1iT1GLsMroh6zXXNM", 3, 200, 300, 8, 400, 5, 6),
                new MiningGovernance(1, "PARwm9EjmivLgqqH1Mroh6zXXNMUiT1GLs", 4, 300, 400, 12, 500, 7, 8),
                new MiningGovernance(1, "PEjmivT1GLs9LgqqARwmH1iM6zXXNMUroh", 5, 400, 500, 16, 600, 9, 10),
            };

            _mediator.Setup(callTo => callTo.Send(It.Is<RetrieveMiningGovernancesByModifiedBlockQuery>(q => q.BlockHeight == rewindHeight),
                                                  It.IsAny<CancellationToken>())).ReturnsAsync(governances);

            // Act
            await _handler.Handle(new CreateRewindMiningGovernancesAndNominationsCommand(rewindHeight), CancellationToken.None);

            // Assert
            foreach (var governance in governances)
            {
                _mediator.Verify(callTo => callTo.Send(It.Is<MakeMiningGovernanceCommand>(q => q.BlockHeight == rewindHeight &&
                                                                                               q.MiningGovernance == governance &&
                                                                                               q.RefreshMiningPoolReward == true &&
                                                                                               q.RefreshMiningPoolsFunded == true &&
                                                                                               q.RefreshNominationPeriodEnd == true),
                                                       It.IsAny<CancellationToken>()), Times.Once);
            }
        }

        [Fact]
        public async Task CreateRewindMiningGovernancesAndNominationsCommand_Sends_MakeGovernanceNominationsCommand()
        {
            // Arrange
            const ulong rewindHeight = 10;

            var governances = new List<MiningGovernance>
            {
                new MiningGovernance(1, "PT1GLsMroh6zXXNMU9EjmivLgqqARwmH1i", 2, 100, 200, 4, 300, 3, 4),
                new MiningGovernance(1, "PU9EjmivLgqqARwmH1iT1GLsMroh6zXXNM", 3, 200, 300, 8, 400, 5, 6),
                new MiningGovernance(1, "PARwm9EjmivLgqqH1Mroh6zXXNMUiT1GLs", 4, 300, 400, 12, 500, 7, 8),
                new MiningGovernance(1, "PEjmivT1GLs9LgqqARwmH1iM6zXXNMUroh", 5, 400, 500, 16, 600, 9, 10),
            };

            _mediator.Setup(callTo => callTo.Send(It.Is<RetrieveMiningGovernancesByModifiedBlockQuery>(q => q.BlockHeight == rewindHeight),
                                                  It.IsAny<CancellationToken>())).ReturnsAsync(governances);

            foreach (var governance in governances)
            {
                _mediator.Setup(callTo => callTo.Send(It.Is<MakeMiningGovernanceCommand>(q => q.MiningGovernance.Id == governance.Id), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(governance.Id);
            }

            // Act
            await _handler.Handle(new CreateRewindMiningGovernancesAndNominationsCommand(rewindHeight), CancellationToken.None);

            // Assert
            foreach (var governance in governances)
            {
                _mediator.Verify(callTo => callTo.Send(It.Is<MakeGovernanceNominationsCommand>(q => q.BlockHeight == rewindHeight &&
                                                                                                    q.Governance == governance),
                                                       It.IsAny<CancellationToken>()), Times.Once);
            }
        }
    }
}
