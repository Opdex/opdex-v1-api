using FluentAssertions;
using Moq;
using Opdex.Platform.Common.Constants.SmartContracts;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Vaults;
using Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.Vaults;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.CirrusFullNodeApiTests.Handlers.Vaults;

public class CallCirrusGetVaultProposalPledgeByProposalIdAndPledgerQueryHandlerTests
{
     private readonly Mock<ISmartContractsModule> _smartContractsModuleMock;
    private readonly CallCirrusGetVaultProposalPledgeByProposalIdAndPledgerQueryHandler _handler;

    public CallCirrusGetVaultProposalPledgeByProposalIdAndPledgerQueryHandlerTests()
    {
        _smartContractsModuleMock = new Mock<ISmartContractsModule>();
        _handler = new CallCirrusGetVaultProposalPledgeByProposalIdAndPledgerQueryHandler(_smartContractsModuleMock.Object);
    }

    [Fact]
    public void CallCirrusGetVaultProposalPledgeByProposalIdAndPledgerQuery_InvalidVault_ThrowsArgumentNullException()
    {
        // Arrange
        Address vault = Address.Empty;
        Address pledger = "PU9EjH1iT1GLsMroh6zXXNMmivLgqqARwm";
        const ulong proposalId = 1ul;
        const ulong blockHeight = 10;

        // Act
        void Act() => new CallCirrusGetVaultProposalPledgeByProposalIdAndPledgerQuery(vault, proposalId, pledger, blockHeight);

        // Assert
        Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Vault address must be provided.");
    }

    [Fact]
    public void CallCirrusGetVaultProposalPledgeByProposalIdAndPledgerQuery_InvalidProposalId_ThrowsArgumentNullException()
    {
        // Arrange
        Address vault = "PU9EjmivLgqqARwmH1iT1GLsMroh6zXXNM";
        Address pledger = "PU9EjH1iT1GLsMroh6zXXNMmivLgqqARwm";
        const ulong proposalId = 0ul;
        const ulong blockHeight = 10;

        // Act
        void Act() => new CallCirrusGetVaultProposalPledgeByProposalIdAndPledgerQuery(vault, proposalId, pledger, blockHeight);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("ProposalId must be greater than zero.");
    }

    [Fact]
    public void CallCirrusGetVaultProposalPledgeByProposalIdAndPledgerQuery_InvalidBlockHeight_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        Address vault = "PU9EjmivLgqqARwmH1iT1GLsMroh6zXXNM";
        Address pledger = "PU9EjH1iT1GLsMroh6zXXNMmivLgqqARwm";
        const ulong proposalId = 1ul;
        const ulong blockHeight = 0;

        // Act
        void Act() => new CallCirrusGetVaultProposalPledgeByProposalIdAndPledgerQuery(vault, proposalId, pledger, blockHeight);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Block height must be greater than zero.");
    }

    [Fact]
    public void CallCirrusGetVaultProposalPledgeByProposalIdAndPledgerQuery_InvalidPledger_ThrowsArgumentNullException()
    {
        // Arrange
        Address vault = "PU9EjmivLgqqARwmH1iT1GLsMroh6zXXNM";
        Address pledger = Address.Empty;
        const ulong proposalId = 1ul;
        const ulong blockHeight = 10;

        // Act
        void Act() => new CallCirrusGetVaultProposalPledgeByProposalIdAndPledgerQuery(vault, proposalId, pledger, blockHeight);

        // Assert
        Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Pledger address must be provided.");
    }

    [Fact]
    public async Task CallCirrusGetVaultProposalPledgeByProposalIdAndPledgerQuery_Sends_LocalCallAsync()
    {
        // Arrange
        Address vault = "PU9EjmivLgqqARwmH1iT1GLsMroh6zXXNM";
        Address pledger = "PU9EjH1iT1GLsMroh6zXXNMmivLgqqARwm";
        const ulong proposalId = 1ul;
        const ulong blockHeight = 10;

        var parameters = new[] { new SmartContractMethodParameter(proposalId), new SmartContractMethodParameter(pledger) };

        // Act
        try
        {
            await _handler.Handle(new CallCirrusGetVaultProposalPledgeByProposalIdAndPledgerQuery(vault, proposalId, pledger, blockHeight), CancellationToken.None);
        }
        catch { }

        // Assert
        _smartContractsModuleMock.Verify(callTo => callTo.LocalCallAsync(It.Is<LocalCallRequestDto>(q => q.Amount == FixedDecimal.Zero &&
                                                                                                         q.MethodName == VaultGovernanceConstants.Methods.GetProposalPledge &&
                                                                                                         q.Parameters.All(p => parameters.Contains(p)) &&
                                                                                                         q.ContractAddress == vault &&
                                                                                                         q.BlockHeight == blockHeight),
                                                                         It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CallCirrusGetVaultProposalPledgeByProposalIdAndPledgerQuery_Returns_VaultProposalPledgeSummary()
    {
        // Arrange
        Address vault = "PU9EjmivLgqqARwmH1iT1GLsMroh6zXXNM";
        Address pledger = "PU9EjH1iT1GLsMroh6zXXNMmivLgqqARwm";

        const ulong proposalId = 1ul;
        const ulong blockHeight = 10;

        const ulong expected = 100;

        var expectedResponse = new LocalCallResponseDto { Return = expected };

        _smartContractsModuleMock.Setup(callTo => callTo.LocalCallAsync(It.IsAny<LocalCallRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var response = await _handler.Handle(new CallCirrusGetVaultProposalPledgeByProposalIdAndPledgerQuery(vault, proposalId, pledger, blockHeight), CancellationToken.None);

        // Assert
        response.Should().Be(expected);
    }
}
