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

public class CallCirrusGetVaultProposalVoteSummaryByProposalIdAndVoterQueryHandlerTests
{
    private readonly Mock<ISmartContractsModule> _smartContractsModuleMock;
    private readonly CallCirrusGetVaultProposalVoteSummaryByProposalIdAndVoterQueryHandler _handler;

    public CallCirrusGetVaultProposalVoteSummaryByProposalIdAndVoterQueryHandlerTests()
    {
        _smartContractsModuleMock = new Mock<ISmartContractsModule>();
        _handler = new CallCirrusGetVaultProposalVoteSummaryByProposalIdAndVoterQueryHandler(_smartContractsModuleMock.Object);
    }

    [Fact]
    public void CallCirrusGetVaultProposalVoteSummaryByProposalIdAndVoterQuery_InvalidVault_ThrowsArgumentNullException()
    {
        // Arrange
        Address vault = Address.Empty;
        Address voter = "PU9EjH1iT1GLsMroh6zXXNMmivLgqqARwm";
        const ulong proposalId = 1ul;
        const ulong blockHeight = 10;

        // Act
        void Act() => new CallCirrusGetVaultProposalVoteSummaryByProposalIdAndVoterQuery(vault, proposalId, voter, blockHeight);

        // Assert
        Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Vault address must be provided.");
    }

    [Fact]
    public void CallCirrusGetVaultProposalVoteSummaryByProposalIdAndVoterQuery_InvalidProposalId_ThrowsArgumentNullException()
    {
        // Arrange
        Address vault = "PU9EjmivLgqqARwmH1iT1GLsMroh6zXXNM";
        Address voter = "PU9EjH1iT1GLsMroh6zXXNMmivLgqqARwm";
        const ulong proposalId = 0ul;
        const ulong blockHeight = 10;

        // Act
        void Act() => new CallCirrusGetVaultProposalVoteSummaryByProposalIdAndVoterQuery(vault, proposalId, voter, blockHeight);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("ProposalId must be greater than zero.");
    }

    [Fact]
    public void CallCirrusGetVaultProposalVoteSummaryByProposalIdAndVoterQuery_InvalidBlockHeight_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        Address vault = "PU9EjmivLgqqARwmH1iT1GLsMroh6zXXNM";
        Address voter = "PU9EjH1iT1GLsMroh6zXXNMmivLgqqARwm";
        const ulong proposalId = 1ul;
        const ulong blockHeight = 0;

        // Act
        void Act() => new CallCirrusGetVaultProposalVoteSummaryByProposalIdAndVoterQuery(vault, proposalId, voter, blockHeight);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Block height must be greater than zero.");
    }

    [Fact]
    public void CallCirrusGetVaultProposalVoteSummaryByProposalIdAndVoterQuery_InvalidVoter_ThrowsArgumentNullException()
    {
        // Arrange
        Address vault = "PU9EjmivLgqqARwmH1iT1GLsMroh6zXXNM";
        Address voter = Address.Empty;
        const ulong proposalId = 1ul;
        const ulong blockHeight = 10;

        // Act
        void Act() => new CallCirrusGetVaultProposalVoteSummaryByProposalIdAndVoterQuery(vault, proposalId, voter, blockHeight);

        // Assert
        Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Voter address must be provided.");
    }

    [Fact]
    public async Task CallCirrusGetVaultProposalVoteSummaryByProposalIdAndVoterQuery_Sends_LocalCallAsync()
    {
        // Arrange
        Address vault = "PU9EjmivLgqqARwmH1iT1GLsMroh6zXXNM";
        Address voter = "PU9EjH1iT1GLsMroh6zXXNMmivLgqqARwm";
        const ulong proposalId = 1ul;
        const ulong blockHeight = 10;

        var parameters = new[] { new SmartContractMethodParameter(proposalId), new SmartContractMethodParameter(voter) };

        // Act
        try
        {
            await _handler.Handle(new CallCirrusGetVaultProposalVoteSummaryByProposalIdAndVoterQuery(vault, proposalId, voter, blockHeight), CancellationToken.None);
        }
        catch { }

        // Assert
        _smartContractsModuleMock.Verify(callTo => callTo.LocalCallAsync(It.Is<LocalCallRequestDto>(q => q.Amount == FixedDecimal.Zero &&
                                                                                                         q.MethodName == VaultConstants.Methods.GetProposalVote &&
                                                                                                         q.Parameters.All(p => parameters.Contains(p)) &&
                                                                                                         q.ContractAddress == vault &&
                                                                                                         q.BlockHeight == blockHeight),
                                                                         It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CallCirrusGetVaultProposalVoteSummaryByProposalIdAndVoterQuery_Returns_VaultProposalVoteSummary()
    {
        // Voter
        Address vault = "PU9EjmivLgqqARwmH1iT1GLsMroh6zXXNM";
        Address voter = "PU9EjH1iT1GLsMroh6zXXNMmivLgqqARwm";

        const ulong proposalId = 1ul;
        const ulong blockHeight = 10;

        var expectedSummary = new ProposalVoteResponse {
            Amount = 100,
            InFavor = true
        };

        var expectedResponse = new LocalCallResponseDto { Return = expectedSummary };

        _smartContractsModuleMock.Setup(callTo => callTo.LocalCallAsync(It.IsAny<LocalCallRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var response = await _handler.Handle(new CallCirrusGetVaultProposalVoteSummaryByProposalIdAndVoterQuery(vault, proposalId, voter, blockHeight), CancellationToken.None);

        // Assert
        response.Amount.Should().Be(expectedSummary.Amount);
        response.InFavor.Should().Be(expectedSummary.InFavor);
    }

    private sealed class ProposalVoteResponse
    {
        public ulong Amount { get; set; }
        public bool InFavor { get; set; }
    }
}
