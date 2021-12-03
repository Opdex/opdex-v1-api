using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Deployers;
using Opdex.Platform.Application.Abstractions.Queries.Deployers;
using Opdex.Platform.Application.Handlers.Deployers;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Deployers;
using Opdex.Platform.Domain.Models.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Deployers;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Handlers.Deployers;

public class MakeDeployerCommandHandlerTests
{
    private readonly Mock<IMediator> _mediator;
    private readonly MakeDeployerCommandHandler _handler;

    public MakeDeployerCommandHandlerTests()
    {
        _mediator = new Mock<IMediator>();
        _handler = new MakeDeployerCommandHandler(_mediator.Object);
    }

    [Fact]
    public void MakeDeployerCommand_InvalidDeployer_ThrowsArgumentNullException()
    {
        // Arrange
        const ulong blockHeight = 10;

        // Act
        void Act() => new MakeDeployerCommand(null, blockHeight);

        // Assert
        Assert.Throws<ArgumentNullException>(Act).Message.Contains("Deployer must be provided.");
    }

    [Fact]
    public void MakeDeployerCommand_InvalidBlockHeight_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var deployer = new Deployer(1, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", Address.Empty, "PMUARwmH1iT1GLsMroh6zXXN9EjmivLgqq", true, 3, 4);

        // Act
        void Act() => new MakeDeployerCommand(deployer, 0);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Contains("Block height must be greater than zero.");
    }

    [Theory]
    [InlineData(false, false, false)]
    [InlineData(false, true, true)]
    [InlineData(true, false, true)]
    [InlineData(true, true, true)]
    public async Task MakeDeployerCommand_Sends_RetrieveDeployerContractSummaryByAddressQuery(bool refreshPendingOwner,
                                                                                              bool refreshOwner,
                                                                                              bool expected)
    {
        // Arrange
        const ulong blockHeight = 20;
        var stakingTokenId = refreshOwner ? 0 : 1;
        var deployer = new Deployer(1, "PH1iT1GLsMroh6zXXNMU9EjmivLgqqARwm", Address.Empty, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", true, 10, 11);

        // Act
        try
        {
            await _handler.Handle(new MakeDeployerCommand(deployer, blockHeight,
                                                          refreshPendingOwner: refreshPendingOwner,
                                                          refreshOwner: refreshOwner), CancellationToken.None);
        }
        catch { }

        // Assert
        var times = expected ? Times.Once() : Times.Never();

        _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveDeployerContractSummaryQuery>(q => q.Deployer == deployer.Address &&
                                                                                                q.BlockHeight == blockHeight &&
                                                                                                q.IncludePendingOwner == refreshPendingOwner &&
                                                                                                q.IncludeOwner == refreshOwner),
                                               It.IsAny<CancellationToken>()), times);
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public async Task MakeDeployerCommand_Sends_PersistDeployerCommand(bool refreshPendingOwner,
                                                                       bool refreshOwner)
    {
        // Arrange
        const ulong blockHeight = 20;
        var stakingTokenId = refreshOwner ? 0 : 1;

        Address currentPendingOwner = Address.Empty;
        Address updatedPendingOwner = "PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh";

        Address currentOwner = "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN";
        Address updatedOwner = refreshOwner ? "PRwmH1iT1GLsMroh6zXXNMU9EjmivLgqqA" : currentOwner;

        var deployer = new Deployer(1, "PH1iT1GLsMroh6zXXNMU9EjmivLgqqARwm", Address.Empty, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", true, 10, 11);

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveDeployerContractSummaryQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                var summary = new DeployerContractSummary(blockHeight);

                if (refreshPendingOwner) summary.SetPendingOwner(new SmartContractMethodParameter(updatedPendingOwner));
                if (refreshOwner) summary.SetOwner(new SmartContractMethodParameter(updatedOwner));

                return summary;
            });

        // Act
        try
        {
            await _handler.Handle(new MakeDeployerCommand(deployer, blockHeight,
                                                          refreshPendingOwner: refreshPendingOwner,
                                                          refreshOwner: refreshOwner), CancellationToken.None);
        }
        catch { }

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<PersistDeployerCommand>(q => q.Deployer == deployer),
                                               It.IsAny<CancellationToken>()), Times.Once);
    }
}