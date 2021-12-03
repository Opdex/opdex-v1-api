using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.MiningGovernances;
using Opdex.Platform.Application.Abstractions.EntryCommands.MiningGovernances;
using Opdex.Platform.Application.Abstractions.Queries.MiningGovernances;
using Opdex.Platform.Application.Abstractions.Queries.MiningGovernances.Nominations;
using Opdex.Platform.Application.EntryHandlers.MiningGovernances;
using Opdex.Platform.Domain.Models.MiningGovernances;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.MiningGovernances;

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

        var miningGovernances = new List<MiningGovernance>
        {
            new MiningGovernance(1, "PT1GLsMroh6zXXNMU9EjmivLgqqARwmH1i", 2, 100, 200, 4, 300, 3, 4),
            new MiningGovernance(1, "PU9EjmivLgqqARwmH1iT1GLsMroh6zXXNM", 3, 200, 300, 8, 400, 5, 6),
            new MiningGovernance(1, "PARwm9EjmivLgqqH1Mroh6zXXNMUiT1GLs", 4, 300, 400, 12, 500, 7, 8),
            new MiningGovernance(1, "PEjmivT1GLs9LgqqARwmH1iM6zXXNMUroh", 5, 400, 500, 16, 600, 9, 10),
        };

        _mediator.Setup(callTo => callTo.Send(It.Is<RetrieveMiningGovernancesByModifiedBlockQuery>(q => q.BlockHeight == rewindHeight),
                                              It.IsAny<CancellationToken>())).ReturnsAsync(miningGovernances);

        // Act
        await _handler.Handle(new CreateRewindMiningGovernancesAndNominationsCommand(rewindHeight), CancellationToken.None);

        // Assert
        foreach (var miningGovernance in miningGovernances)
        {
            _mediator.Verify(callTo => callTo.Send(It.Is<MakeMiningGovernanceCommand>(q => q.BlockHeight == rewindHeight &&
                                                                                           q.MiningGovernance == miningGovernance &&
                                                                                           q.RefreshMiningPoolReward == true &&
                                                                                           q.RefreshMiningPoolsFunded == true &&
                                                                                           q.RefreshNominationPeriodEnd == true),
                                                   It.IsAny<CancellationToken>()), Times.Once);
        }
    }

    [Fact]
    public async Task CreateRewindMiningGovernancesAndNominationsCommand_Sends_MakeMiningGovernanceNominationsCommand()
    {
        // Arrange
        const ulong rewindHeight = 10;

        var miningGovernances = new List<MiningGovernance>
        {
            new MiningGovernance(1, "PT1GLsMroh6zXXNMU9EjmivLgqqARwmH1i", 2, 100, 200, 4, 300, 3, 4),
            new MiningGovernance(1, "PU9EjmivLgqqARwmH1iT1GLsMroh6zXXNM", 3, 200, 300, 8, 400, 5, 6),
            new MiningGovernance(1, "PARwm9EjmivLgqqH1Mroh6zXXNMUiT1GLs", 4, 300, 400, 12, 500, 7, 8),
            new MiningGovernance(1, "PEjmivT1GLs9LgqqARwmH1iM6zXXNMUroh", 5, 400, 500, 16, 600, 9, 10),
        };

        _mediator.Setup(callTo => callTo.Send(It.Is<RetrieveMiningGovernancesByModifiedBlockQuery>(q => q.BlockHeight == rewindHeight),
                                              It.IsAny<CancellationToken>())).ReturnsAsync(miningGovernances);

        foreach (var miningGovernance in miningGovernances)
        {
            _mediator.Setup(callTo => callTo.Send(It.Is<MakeMiningGovernanceCommand>(q => q.MiningGovernance.Id == miningGovernance.Id), It.IsAny<CancellationToken>()))
                .ReturnsAsync(miningGovernance.Id);
        }

        // Act
        await _handler.Handle(new CreateRewindMiningGovernancesAndNominationsCommand(rewindHeight), CancellationToken.None);

        // Assert
        foreach (var miningGovernance in miningGovernances)
        {
            _mediator.Verify(callTo => callTo.Send(It.Is<MakeMiningGovernanceNominationsCommand>(q => q.BlockHeight == rewindHeight &&
                                                                                                      q.MiningGovernance == miningGovernance),
                                                   It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}