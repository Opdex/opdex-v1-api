using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.VaultGovernances;
using Opdex.Platform.Application.Abstractions.EntryCommands.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances;
using Opdex.Platform.Application.EntryHandlers.VaultGovernances;
using Opdex.Platform.Domain.Models.VaultGovernances;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.VaultGovernances;

public class CreateRewindVaultGovernancesCommandHandlerTests
{
    private readonly Mock<IMediator> _mediator;
    private readonly CreateRewindVaultGovernancesCommandHandler _handler;

    public CreateRewindVaultGovernancesCommandHandlerTests()
    {
        _mediator = new Mock<IMediator>();
        _handler = new CreateRewindVaultGovernancesCommandHandler(_mediator.Object,
                                                                  Mock.Of<ILogger<CreateRewindVaultGovernancesCommandHandler>>());
    }

    [Fact]
    public void CreateRewindVaultGovernancesCommand_InvalidRewindHeight_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        const ulong rewindHeight = 0;

        // Act
        void Act() => new CreateRewindVaultGovernancesCommand(rewindHeight);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Contains("Rewind height must be greater than zero.");
    }

    [Fact]
    public async Task CreateRewindVaultGovernancesCommand_Sends_RetrieveVaultGovernancesByModifiedBlockQuery()
    {
        // Arrange
        const ulong rewindHeight = 10;

        // Act
        try
        {
            await _handler.Handle(new CreateRewindVaultGovernancesCommand(rewindHeight), CancellationToken.None);
        }
        catch { }

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveVaultGovernancesByModifiedBlockQuery>(q => q.BlockHeight == rewindHeight),
                                               It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateRewindVaultGovernancesCommand_Sends_MakeVaultGovernanceCommand()
    {
        // Arrange
        const ulong rewindHeight = 12;

        var vaults = new List<VaultGovernance>
        {
            new(1, "PXXNMivLgqqART1GLsMroh6zwmH1iU9Ejm", 2, 3, 4, 5, 6, 7, 8, 9),
            new(2, "PXXNsMroh6zwmH1iU9EjmMivLgqqART1GL", 3, 4, 5, 6, 7, 8, 9, 10),
            new(3, "PXXroh6zwmH1iU9NMivLgqqART1GLsMEjm", 4, 5, 6, 7, 8, 9, 10, 11),
            new(4, "Proh6zwmH1iU9EjmXXNMivLgqqART1GLsM", 5, 6, 7, 8, 9, 10, 11, 12)
        };

        _mediator.Setup(callTo => callTo.Send(It.Is<RetrieveVaultGovernancesByModifiedBlockQuery>(q => q.BlockHeight == rewindHeight),
                                              It.IsAny<CancellationToken>())).ReturnsAsync(vaults);

        // Act
        await _handler.Handle(new CreateRewindVaultGovernancesCommand(rewindHeight), CancellationToken.None);

        // Assert
        foreach (var vault in vaults)
        {
            _mediator.Verify(callTo => callTo.Send(It.Is<MakeVaultGovernanceCommand>(q => q.BlockHeight == rewindHeight &&
                                                                                          q.Vault == vault &&
                                                                                          q.RefreshUnassignedSupply == true &&
                                                                                          q.RefreshProposedSupply == true &&
                                                                                          q.RefreshTotalPledgeMinimum == true &&
                                                                                          q.RefreshTotalVoteMinimum == true),
                                                   It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
