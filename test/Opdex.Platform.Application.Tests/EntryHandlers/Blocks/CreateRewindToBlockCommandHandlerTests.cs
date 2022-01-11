using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Blocks;
using Opdex.Platform.Application.Abstractions.EntryCommands;
using Opdex.Platform.Application.Abstractions.EntryCommands.Addresses.Balances;
using Opdex.Platform.Application.Abstractions.EntryCommands.Addresses.Mining;
using Opdex.Platform.Application.Abstractions.EntryCommands.Addresses.Staking;
using Opdex.Platform.Application.Abstractions.EntryCommands.Blocks;
using Opdex.Platform.Application.Abstractions.EntryCommands.Deployers;
using Opdex.Platform.Application.Abstractions.EntryCommands.MiningGovernances;
using Opdex.Platform.Application.Abstractions.EntryCommands.Markets;
using Opdex.Platform.Application.Abstractions.EntryCommands.Markets.Permissions;
using Opdex.Platform.Application.Abstractions.EntryCommands.MiningPools;
using Opdex.Platform.Application.Abstractions.EntryCommands.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Application.EntryHandlers.Blocks;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Blocks;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Blocks;

public class CreateRewindToBlockCommandHandlerTests
{
    private readonly CreateRewindToBlockCommandHandler _handler;
    private readonly Mock<IMediator> _mediator;

    public CreateRewindToBlockCommandHandlerTests()
    {
        _mediator = new Mock<IMediator>();
        _handler = new CreateRewindToBlockCommandHandler(_mediator.Object, Mock.Of<ILogger<CreateRewindToBlockCommandHandler>>());
    }

    [Fact]
    public void CreateRewindToBlockCommand_InvalidBlock_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        // Act
        static void Act() => new CreateRewindToBlockCommand(0);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Contains("Block number must be greater than 0.");
    }

    [Fact]
    public async Task CreateRewindToBlockCommand_Sends_RetrieveBlockByHeightQuery()
    {
        // Arrange
        const ulong block = 10;

        // Act
        try
        {
            await _handler.Handle(new CreateRewindToBlockCommand(block), CancellationToken.None);
        }
        catch { }

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveBlockByHeightQuery>(q => q.Height == block), CancellationToken.None));
    }

    [Fact]
    public void CreateRewindToBlockCommand_InvalidBlock_ThrowsInvalidDataException()
    {
        // Arrange
        const ulong block = 10;
        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveBlockByHeightQuery>(), CancellationToken.None)).ReturnsAsync(() => null);

        // Act
        // Assert
        _handler.Invoking(h => h.Handle(new CreateRewindToBlockCommand(block), CancellationToken.None))
            .Should().ThrowAsync<InvalidDataException>()
            .WithMessage("Unable to find a block by the provided block number.");
    }

    [Fact]
    public async Task CreateRewindToBlockCommand_Sends_RetrieveLatestBlockQuery()
    {
        // Arrange
        var block = new Block(10, Sha256.Parse("18236e42c337ee0b8a23df39523a904853ac9a1e42120a5086420ecf9c79b147"), DateTime.UtcNow, DateTime.UtcNow);
        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveBlockByHeightQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(block);

        // Act
        try
        {
            await _handler.Handle(new CreateRewindToBlockCommand(block.Height), CancellationToken.None);
        }
        catch { }

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.IsAny<RetrieveLatestBlockQuery>(), CancellationToken.None));
    }

    [Fact]
    public void CreateRewindToBlockCommand_RequestExceedsMaximumRewind_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var rewindBlock = new Block(10, Sha256.Parse("18236e42c337ee0b8a23df39523a904853ac9a1e42120a5086420ecf9c79b147"), DateTime.UtcNow, DateTime.UtcNow);
        var latestBlock = new Block(20_000, Sha256.Parse("142120a5086420ecf9c79b148236e42c337ee0b8a23df39523a904853ac9a1e7"), DateTime.UtcNow, DateTime.UtcNow);
        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveBlockByHeightQuery>(), CancellationToken.None)).ReturnsAsync(rewindBlock);
        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveLatestBlockQuery>(), CancellationToken.None)).ReturnsAsync(latestBlock);

        // Act
        // Assert
        _handler.Invoking(h => h.Handle(new CreateRewindToBlockCommand(rewindBlock.Height), CancellationToken.None))
            .Should().ThrowAsync<ArgumentOutOfRangeException>()
            .WithMessage("*Rewind request exceeds maximum rewind limit.*");
    }

    [Fact]
    public async Task CreateRewindToBlockCommand_Sends_MakeRewindToBlockCommand()
    {
        // Arrange
        var rewindBlock = new Block(10, Sha256.Parse("18236e42c337ee0b8a23df39523a904853ac9a1e42120a5086420ecf9c79b147"), DateTime.UtcNow, DateTime.UtcNow);
        var latestBlock = new Block(1000, Sha256.Parse("142120a5086420ecf9c79b148236e42c337ee0b8a23df39523a904853ac9a1e7"), DateTime.UtcNow, DateTime.UtcNow);

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveBlockByHeightQuery>(), CancellationToken.None)).ReturnsAsync(rewindBlock);
        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveLatestBlockQuery>(), CancellationToken.None)).ReturnsAsync(latestBlock);

        // Act
        try
        {
            await _handler.Handle(new CreateRewindToBlockCommand(rewindBlock.Height), CancellationToken.None);
        }
        catch { }

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<MakeRewindToBlockCommand>(q => q.Block == rewindBlock.Height), It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task CreateRewindToBlockCommand_Sends_CreateRewindAddressBalancesCommand()
    {
        // Arrange
        var rewindBlock = new Block(10, Sha256.Parse("18236e42c337ee0b8a23df39523a904853ac9a1e42120a5086420ecf9c79b147"), DateTime.UtcNow, DateTime.UtcNow);
        var latestBlock = new Block(1000, Sha256.Parse("142120a5086420ecf9c79b148236e42c337ee0b8a23df39523a904853ac9a1e7"), DateTime.UtcNow, DateTime.UtcNow);

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveBlockByHeightQuery>(), CancellationToken.None)).ReturnsAsync(rewindBlock);
        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveLatestBlockQuery>(), CancellationToken.None)).ReturnsAsync(latestBlock);
        _mediator.Setup(m => m.Send(It.IsAny<MakeRewindToBlockCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

        // Act
        try
        {
            await _handler.Handle(new CreateRewindToBlockCommand(rewindBlock.Height), CancellationToken.None);
        }
        catch { }

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<CreateRewindAddressBalancesCommand>(q => q.RewindHeight == rewindBlock.Height), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task CreateRewindToBlockCommand_Sends_CreateRewindDeployersCommand()
    {
        // Arrange
        var rewindBlock = new Block(10, Sha256.Parse("18236e42c337ee0b8a23df39523a904853ac9a1e42120a5086420ecf9c79b147"), DateTime.UtcNow, DateTime.UtcNow);
        var latestBlock = new Block(1000, Sha256.Parse("142120a5086420ecf9c79b148236e42c337ee0b8a23df39523a904853ac9a1e7"), DateTime.UtcNow, DateTime.UtcNow);

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveBlockByHeightQuery>(), CancellationToken.None)).ReturnsAsync(rewindBlock);
        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveLatestBlockQuery>(), CancellationToken.None)).ReturnsAsync(latestBlock);
        _mediator.Setup(m => m.Send(It.IsAny<MakeRewindToBlockCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _mediator.Setup(m => m.Send(It.IsAny<CreateRewindAddressBalancesCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

        // Act
        try
        {
            await _handler.Handle(new CreateRewindToBlockCommand(rewindBlock.Height), CancellationToken.None);
        }
        catch { }

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<CreateRewindDeployersCommand>(q => q.RewindHeight == rewindBlock.Height), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_MakeRewindToBlockCommandFailure_DoNotRefreshStaleRecords()
    {
        // Arrange
        var rewindBlock = new Block(10, Sha256.Parse("18236e42c337ee0b8a23df39523a904853ac9a1e42120a5086420ecf9c79b147"), DateTime.UtcNow, DateTime.UtcNow);
        var latestBlock = new Block(1000, Sha256.Parse("142120a5086420ecf9c79b148236e42c337ee0b8a23df39523a904853ac9a1e7"), DateTime.UtcNow, DateTime.UtcNow);
        var command = new CreateRewindToBlockCommand(10);

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveBlockByHeightQuery>(), CancellationToken.None)).ReturnsAsync(rewindBlock);
        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveLatestBlockQuery>(), CancellationToken.None)).ReturnsAsync(latestBlock);
        _mediator.Setup(m => m.Send(It.IsAny<MakeRewindToBlockCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

        // Act
        var response = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.IsAny<CreateRewindAddressBalancesCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        response.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_RefreshStaleRecordsFailure_IgnoreAndContinue()
    {
        // Arrange
        var rewindBlock = new Block(10, Sha256.Parse("18236e42c337ee0b8a23df39523a904853ac9a1e42120a5086420ecf9c79b147"), DateTime.UtcNow, DateTime.UtcNow);
        var latestBlock = new Block(1000, Sha256.Parse("142120a5086420ecf9c79b148236e42c337ee0b8a23df39523a904853ac9a1e7"), DateTime.UtcNow, DateTime.UtcNow);
        var command = new CreateRewindToBlockCommand(10);

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveBlockByHeightQuery>(), CancellationToken.None)).ReturnsAsync(rewindBlock);
        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveLatestBlockQuery>(), CancellationToken.None)).ReturnsAsync(latestBlock);
        _mediator.Setup(m => m.Send(It.IsAny<MakeRewindToBlockCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _mediator.Setup(callTo => callTo.Send(It.IsAny<CreateRewindAddressBalancesCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<CreateRewindStakingPositionsCommand>(c => c.RewindHeight == command.Block), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task CreateRewindToBlockCommand_Success()
    {
        // Arrange
        var rewindBlock = new Block(10, Sha256.Parse("18236e42c337ee0b8a23df39523a904853ac9a1e42120a5086420ecf9c79b147"), DateTime.UtcNow, DateTime.UtcNow);
        var latestBlock = new Block(1000, Sha256.Parse("142120a5086420ecf9c79b148236e42c337ee0b8a23df39523a904853ac9a1e7"), DateTime.UtcNow, DateTime.UtcNow);
        var command = new CreateRewindToBlockCommand(10);

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveBlockByHeightQuery>(), CancellationToken.None)).ReturnsAsync(rewindBlock);
        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveLatestBlockQuery>(), CancellationToken.None)).ReturnsAsync(latestBlock);
        _mediator.Setup(m => m.Send(It.IsAny<MakeRewindToBlockCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _mediator.Setup(m => m.Send(It.IsAny<CreateRewindAddressBalancesCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _mediator.Setup(m => m.Send(It.IsAny<CreateRewindMiningPositionsCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _mediator.Setup(m => m.Send(It.IsAny<CreateRewindStakingPositionsCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _mediator.Setup(m => m.Send(It.IsAny<CreateRewindDeployersCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _mediator.Setup(m => m.Send(It.IsAny<CreateRewindMiningGovernancesAndNominationsCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _mediator.Setup(m => m.Send(It.IsAny<CreateRewindMiningPoolsCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _mediator.Setup(m => m.Send(It.IsAny<CreateRewindMarketsCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _mediator.Setup(m => m.Send(It.IsAny<CreateRewindMarketPermissionsCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _mediator.Setup(m => m.Send(It.IsAny<CreateRewindVaultsCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _mediator.Setup(m => m.Send(It.IsAny<CreateRewindVaultProposalsCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _mediator.Setup(m => m.Send(It.IsAny<CreateRewindVaultProposalPledgesCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _mediator.Setup(m => m.Send(It.IsAny<CreateRewindVaultProposalVotesCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _mediator.Setup(m => m.Send(It.IsAny<CreateRewindVaultCertificatesCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _mediator.Setup(m => m.Send(It.IsAny<CreateRewindSnapshotsCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

        // Act
        var response = await _handler.Handle(command, CancellationToken.None);

        // Assert
        response.Should().BeTrue();
    }
}
