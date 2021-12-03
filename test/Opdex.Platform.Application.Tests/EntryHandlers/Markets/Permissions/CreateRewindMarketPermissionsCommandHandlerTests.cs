using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryCommands.Markets.Permissions;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Markets.Permissions;
using Opdex.Platform.Application.EntryHandlers.Markets.Permissions;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Markets;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Markets.Permissions;

public class CreateRewindMarketPermissionsCommandHandlerTests
{
    private readonly Mock<IMediator> _mediator;
    private readonly CreateRewindMarketPermissionsCommandHandler _handler;

    public CreateRewindMarketPermissionsCommandHandlerTests()
    {
        _mediator = new Mock<IMediator>();
        _handler = new CreateRewindMarketPermissionsCommandHandler(_mediator.Object, Mock.Of<ILogger<CreateRewindMarketPermissionsCommandHandler>>());
    }

    [Fact]
    public void CreateRewindMarketPermissionsCommand_InvalidRewindHeight_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        const ulong rewindHeight = 0;

        // Act
        void Act() => new CreateRewindMarketPermissionsCommand(rewindHeight);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Contains("Rewind height must be greater than zero.");
    }

    [Fact]
    public async Task CreateRewindMarketPermissionsCommand_Sends_RetrieveMarketPermissionsByModifiedBlockQuery()
    {
        // Arrange
        const ulong rewindHeight = 10;

        // Act
        try
        {
            await _handler.Handle(new CreateRewindMarketPermissionsCommand(rewindHeight), CancellationToken.None);
        }
        catch { }

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveMarketPermissionsByModifiedBlockQuery>(q => q.BlockHeight == rewindHeight),
                                               It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateRewindMarketPermissionsCommand_Sends_RetrieveMarketByIdQuery()
    {
        // Arrange
        const ulong rewindHeight = 10;
        const ulong marketId = 2;

        var permissions = new List<MarketPermission>
        {
            new MarketPermission(1, marketId, "PzwmH1iU9EjmXXNMivLgqqART1GLsMroh6", MarketPermissionType.Provide,
                                 true, "PivLgqqART1GLsMroh6zwmH1iU9EjmXXNM", 10, rewindHeight)
        };

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketPermissionsByModifiedBlockQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(permissions);

        // Act
        try
        {
            await _handler.Handle(new CreateRewindMarketPermissionsCommand(rewindHeight), CancellationToken.None);
        }
        catch { }

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveMarketByIdQuery>(q => q.MarketId == marketId),
                                               It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateRewindMarketPermissionsCommand_Sends_RetrieveMarketContractPermissionSummaryQuery()
    {
        // Arrange
        const ulong rewindHeight = 10;
        const ulong marketId = 3;
        var market = new Market(marketId, "PH1iT1GLsMroh6zXXNMU9EjmivLgqqARwm", 2, 3, Address.Empty, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", false, false, false, 3, true, 10, 11);

        var certificates = new List<MarketPermission>
        {
            new MarketPermission(1, marketId, "PzwmH1iU9EjmXXNMivLgqqART1GLsMroh6", MarketPermissionType.Provide,
                                 true, "PivLgqqART1GLsMroh6zwmH1iU9EjmXXNM", 10, rewindHeight),
            new MarketPermission(2, marketId, "PXNMivLsMroh6LgzwmH1iU9EjmXqqART1G", MarketPermissionType.Trade,
                                 true, "PLjmXXiU9EivLgqRT1GMsMroh6qANzwmH1", 8, rewindHeight)
        };

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketPermissionsByModifiedBlockQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(certificates);

        _mediator.Setup(callTo => callTo.Send(It.Is<RetrieveMarketByIdQuery>(q => q.MarketId == marketId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(market);

        // Act
        try
        {
            await _handler.Handle(new CreateRewindMarketPermissionsCommand(rewindHeight), CancellationToken.None);
        }
        catch { }

        // Assert
        foreach (var certificate in certificates)
        {
            _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveMarketContractPermissionSummaryQuery>(q => q.Market == market.Address &&
                                                                                                            q.Wallet == certificate.User &&
                                                                                                            q.PermissionType == certificate.Permission &&
                                                                                                            q.BlockHeight == rewindHeight),
                                                   It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}