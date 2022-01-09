using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Vaults;
using Opdex.Platform.Application.Abstractions.EntryCommands.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Application.EntryHandlers.Vaults;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Transactions;
using Opdex.Platform.Domain.Models.Vaults;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Vaults;

public class CreateVaultGovernanceCommandHandlerTests
{
    private readonly Mock<IMediator> _mediator;
    private readonly CreateVaultGovernanceCommandHandler _handler;

    public CreateVaultGovernanceCommandHandlerTests()
    {
        _mediator = new Mock<IMediator>();
        _handler = new CreateVaultGovernanceCommandHandler(_mediator.Object);
    }

    [Fact]
    public void CreateVaultGovernanceCommand_InvalidVault_ThrowsArgumentNullException()
    {
        // Arrange
        const  ulong tokenId = 1;
        const ulong blockHeight = 10;

        // Act
        void Act() => new CreateVaultGovernanceCommand(null, tokenId, blockHeight);

        // Assert
        Assert.Throws<ArgumentNullException>(Act).Message.Contains("Vault address must be provided.");
    }

    [Theory]
    [InlineData(0)]
    public void CreateVaultGovernanceCommand_InvalidTokenId_ThrowsArgumentOutOfRangeException(ulong tokenId)
    {
        // Arrange
        Address vault = "PH1iT1GLsMroh6zXXNMU9EjmivLgqqARwm";
        const ulong blockHeight = 10;

        // Act
        void Act() => new CreateVaultGovernanceCommand(vault, tokenId, blockHeight);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Contains("Token Id must be greater than zero.");
    }

    [Fact]
    public void CreateVaultGovernanceCommand_InvalidBlockHeight_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        Address vault = "PH1iT1GLsMroh6zXXNMU9EjmivLgqqARwm";
        const  ulong tokenId = 1;

        // Act
        void Act() => new CreateVaultGovernanceCommand(vault, tokenId, 0);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Contains("Block height must be greater than zero.");
    }

    [Fact]
    public async Task CreateVaultGovernanceCommand_Sends_RetrieveVaultGovernanceByAddressQuery()
    {
        // Arrange
        Address vault = "PH1iT1GLsMroh6zXXNMU9EjmivLgqqARwm";
        const  ulong tokenId = 1;
        const ulong blockHeight = 10;

        // Act
        try
        {
            await _handler.Handle(new CreateVaultGovernanceCommand(vault, tokenId, blockHeight), CancellationToken.None);
        }
        catch { }

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveVaultGovernanceByAddressQuery>(q => q.Vault == vault &&
                                                                                       q.FindOrThrow == false),
                                               It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateVaultGovernanceCommand_Returns_VaultExists()
    {
        // Arrange
        Address vault = "PH1iT1GLsMroh6zXXNMU9EjmivLgqqARwm";
        const  ulong tokenId = 1;
        const ulong blockHeight = 10;
        const ulong vaultId = 100;

        var expectedVault = new VaultGovernance(vaultId, vault, tokenId, 1, 2, 3, 4, 5, 6, 7);

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernanceByAddressQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedVault);

        // Act
        var response = await _handler.Handle(new CreateVaultGovernanceCommand(vault, tokenId, blockHeight), CancellationToken.None);

        // Assert
        response.Should().Be(vaultId);
        _mediator.Verify(callTo => callTo.Send(It.IsAny<MakeVaultGovernanceCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateVaultGovernanceCommand_Sends_RetrieveVaultGovernanceContractSummaryQuery()
    {
        // Arrange
        Address vault = "PH1iT1GLsMroh6zXXNMU9EjmivLgqqARwm";
        const  ulong tokenId = 1;
        const ulong blockHeight = 10;

        // Act
        try
        {
            await _handler.Handle(new CreateVaultGovernanceCommand(vault, tokenId, blockHeight), CancellationToken.None);
        }
        catch { }

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveVaultGovernanceContractSummaryQuery>(q => q.VaultGovernance == vault &&
                                                                                                       q.BlockHeight == blockHeight &&
                                                                                                       q.IncludeVestingDuration == true &&
                                                                                                       q.IncludeTotalPledgeMinimum == true &&
                                                                                                       q.IncludeTotalVoteMinimum == true),
                                               It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateVaultGovernanceCommand_Sends_MakeVaultGovernanceCommand()
    {
        // Arrange
        Address vault = "PH1iT1GLsMroh6zXXNMU9EjmivLgqqARwm";
        const  ulong tokenId = 1;
        const ulong blockHeight = 10;
        const ulong vestingDuration = 10ul;
        const ulong totalPledge = 20ul;
        const ulong totalVote = 30ul;

        var expectedSummary = new VaultGovernanceContractSummary(blockHeight);
        expectedSummary.SetVestingDuration(new SmartContractMethodParameter(vestingDuration));
        expectedSummary.SetTotalPledgeMinimum(new SmartContractMethodParameter(totalPledge));
        expectedSummary.SetTotalVoteMinimum(new SmartContractMethodParameter(totalVote));

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultGovernanceContractSummaryQuery>(),
                                              It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedSummary);

        // Act
        await _handler.Handle(new CreateVaultGovernanceCommand(vault, tokenId, blockHeight), CancellationToken.None);

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<MakeVaultGovernanceCommand>(q => q.Vault.Id == 0 &&
                                                                                      q.Vault.Address == vault &&
                                                                                      q.Vault.TokenId == tokenId &&
                                                                                      q.Vault.VestingDuration == vestingDuration &&
                                                                                      q.Vault.TotalPledgeMinimum == totalPledge &&
                                                                                      q.Vault.TotalVoteMinimum == totalVote &&
                                                                                      q.BlockHeight == blockHeight &&
                                                                                      q.Refresh == false),
                                               It.IsAny<CancellationToken>()), Times.Once);
    }
}
