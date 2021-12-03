using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.MiningGovernances;
using Opdex.Platform.Application.Handlers.MiningGovernances.Nominations;
using Opdex.Platform.Domain.Models.MiningGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.MiningGovernances;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Handlers.MiningGovernances;

public class MakeMiningGovernanceNominationCommandHandlerTests
{
    private readonly Mock<IMediator> _mediator;
    private readonly MakeMiningGovernanceNominationCommandHandler _handler;

    public MakeMiningGovernanceNominationCommandHandlerTests()
    {
        _mediator = new Mock<IMediator>();
        _handler = new MakeMiningGovernanceNominationCommandHandler(_mediator.Object);
    }

    [Fact]
    public void MakeMiningGovernanceNominationCommand_InvalidNomination_ThrowsArgumentNullException()
    {
        // Arrange
        // Act
        void Act() => new MakeMiningGovernanceNominationCommand(null);

        // Assert
        Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Nomination must be provided.");
    }

    [Fact]
    public async Task MakeMiningGovernanceNominationCommand_Sends_PersistMiningGovernanceNominationCommand()
    {
        // Arrange
        var nomination = new MiningGovernanceNomination(1, 1, 2, 3, true, 11, 3, 4);

        // Act
        await _handler.Handle(new MakeMiningGovernanceNominationCommand(nomination), CancellationToken.None);

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<PersistMiningGovernanceNominationCommand>(q => q.Nomination.Id == nomination.Id &&
                                                                                                    q.Nomination.MiningGovernanceId == nomination.MiningGovernanceId &&
                                                                                                    q.Nomination.LiquidityPoolId == nomination.LiquidityPoolId &&
                                                                                                    q.Nomination.MiningPoolId == nomination.MiningPoolId &&
                                                                                                    q.Nomination.Weight == nomination.Weight &&
                                                                                                    q.Nomination.IsNominated == nomination.IsNominated &&
                                                                                                    q.Nomination.CreatedBlock == nomination.CreatedBlock &&
                                                                                                    q.Nomination.ModifiedBlock == nomination.ModifiedBlock),
                                               It.IsAny<CancellationToken>()), Times.Once);
    }
}