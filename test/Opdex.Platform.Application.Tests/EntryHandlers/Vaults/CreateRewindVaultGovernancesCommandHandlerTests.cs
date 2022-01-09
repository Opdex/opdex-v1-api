using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Vaults;
using Opdex.Platform.Application.Abstractions.EntryCommands.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Application.EntryHandlers.Vaults;
using Opdex.Platform.Domain.Models.Vaults;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Vaults;

public class CreateRewindVaultsCommandHandlerTests
{
    private readonly Mock<IMediator> _mediator;
    private readonly CreateRewindVaultsCommandHandler _handler;

    public CreateRewindVaultsCommandHandlerTests()
    {
        _mediator = new Mock<IMediator>();
        _handler = new CreateRewindVaultsCommandHandler(_mediator.Object,
                                                                  Mock.Of<ILogger<CreateRewindVaultsCommandHandler>>());
    }

    [Fact]
    public void CreateRewindVaultsCommand_InvalidRewindHeight_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        const ulong rewindHeight = 0;

        // Act
        void Act() => new CreateRewindVaultsCommand(rewindHeight);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Contains("Rewind height must be greater than zero.");
    }

    [Fact]
    public async Task CreateRewindVaultsCommand_Sends_RetrieveVaultsByModifiedBlockQuery()
    {
        // Arrange
        const ulong rewindHeight = 10;

        // Act
        try
        {
            await _handler.Handle(new CreateRewindVaultsCommand(rewindHeight), CancellationToken.None);
        }
        catch { }

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveVaultsByModifiedBlockQuery>(q => q.BlockHeight == rewindHeight),
                                               It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateRewindVaultsCommand_Sends_MakeVaultCommand()
    {
        // Arrange
        const ulong rewindHeight = 12;

        var vaults = new List<Vault>
        {
            new(1, "PXXNMivLgqqART1GLsMroh6zwmH1iU9Ejm", 2, 3, 4, 5, 6, 7, 8, 9),
            new(2, "PXXNsMroh6zwmH1iU9EjmMivLgqqART1GL", 3, 4, 5, 6, 7, 8, 9, 10),
            new(3, "PXXroh6zwmH1iU9NMivLgqqART1GLsMEjm", 4, 5, 6, 7, 8, 9, 10, 11),
            new(4, "Proh6zwmH1iU9EjmXXNMivLgqqART1GLsM", 5, 6, 7, 8, 9, 10, 11, 12)
        };

        _mediator.Setup(callTo => callTo.Send(It.Is<RetrieveVaultsByModifiedBlockQuery>(q => q.BlockHeight == rewindHeight),
                                              It.IsAny<CancellationToken>())).ReturnsAsync(vaults);

        // Act
        await _handler.Handle(new CreateRewindVaultsCommand(rewindHeight), CancellationToken.None);

        // Assert
        foreach (var vault in vaults)
        {
            _mediator.Verify(callTo => callTo.Send(It.Is<MakeVaultCommand>(q => q.BlockHeight == rewindHeight &&
                                                                                          q.Vault == vault &&
                                                                                          q.RefreshUnassignedSupply == true &&
                                                                                          q.RefreshProposedSupply == true &&
                                                                                          q.RefreshTotalPledgeMinimum == true &&
                                                                                          q.RefreshTotalVoteMinimum == true),
                                                   It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
