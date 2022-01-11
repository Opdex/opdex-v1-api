using FluentAssertions;
using Moq;
using Opdex.Platform.Common.Constants.SmartContracts;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Transactions;
using Opdex.Platform.Domain.Models.Vaults;
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

public class CallCirrusGetVaultProposalSummaryByProposalIdQueryHandlerTests
{
    private readonly Mock<ISmartContractsModule> _smartContractsModuleMock;
    private readonly CallCirrusGetVaultProposalSummaryByProposalIdQueryHandler _handler;

    public CallCirrusGetVaultProposalSummaryByProposalIdQueryHandlerTests()
    {
        _smartContractsModuleMock = new Mock<ISmartContractsModule>();
        _handler = new CallCirrusGetVaultProposalSummaryByProposalIdQueryHandler(_smartContractsModuleMock.Object);
    }

    [Fact]
    public void CallCirrusGetVaultProposalSummaryByProposalIdQuery_InvalidVault_ThrowsArgumentNullException()
    {
        // Arrange
        Address vault = Address.Empty;
        const ulong proposalId = 1ul;
        const ulong blockHeight = 10;

        // Act
        void Act() => new CallCirrusGetVaultProposalSummaryByProposalIdQuery(vault, proposalId, blockHeight);

        // Assert
        Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Vault address must be provided.");
    }

    [Fact]
    public void CallCirrusGetVaultProposalSummaryByProposalIdQuery_InvalidProposalId_ThrowsArgumentNullException()
    {
        // Arrange
        Address vault = "PU9EjmivLgqqARwmH1iT1GLsMroh6zXXNM";
        const ulong proposalId = 0ul;
        const ulong blockHeight = 10;

        // Act
        void Act() => new CallCirrusGetVaultProposalSummaryByProposalIdQuery(vault, proposalId, blockHeight);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("ProposalId must be greater than zero.");
    }

    [Fact]
    public void CallCirrusGetVaultProposalSummaryByProposalIdQuery_InvalidBlockHeight_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        Address vault = "PU9EjmivLgqqARwmH1iT1GLsMroh6zXXNM";
        const ulong proposalId = 1ul;
        const ulong blockHeight = 0;

        // Act
        void Act() => new CallCirrusGetVaultProposalSummaryByProposalIdQuery(vault, proposalId, blockHeight);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Block height must be greater than zero.");
    }

    [Fact]
    public async Task CallCirrusGetVaultProposalSummaryByProposalIdQuery_Sends_LocalCallAsync()
    {
        // Arrange
        Address vault = "PU9EjmivLgqqARwmH1iT1GLsMroh6zXXNM";
        const ulong proposalId = 1ul;
        const ulong blockHeight = 10;

        var parameters = new[] { new SmartContractMethodParameter(proposalId) };

        // Act
        try
        {
            await _handler.Handle(new CallCirrusGetVaultProposalSummaryByProposalIdQuery(vault, proposalId, blockHeight), CancellationToken.None);
        }
        catch { }

        // Assert
        _smartContractsModuleMock.Verify(callTo => callTo.LocalCallAsync(It.Is<LocalCallRequestDto>(q => q.Amount == FixedDecimal.Zero &&
                                                                                                         q.MethodName == VaultConstants.Methods.GetProposal &&
                                                                                                         q.Parameters.All(p => parameters.Contains(p)) &&
                                                                                                         q.ContractAddress == vault &&
                                                                                                         q.BlockHeight == blockHeight),
                                                                         It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CallCirrusGetVaultProposalSummaryByProposalIdQuery_Returns_VaultProposalSummary()
    {
        // Arrange
        Address vault = "PU9EjmivLgqqARwmH1iT1GLsMroh6zXXNM";
        const ulong proposalId = 1ul;
        const ulong blockHeight = 10;

        var expectedSummary = new ProposalResponse {
            Creator = "PXNMjmivLgqqARwmH1iT1GLU9EsMroh6zX",
            Amount = 100,
            Wallet = "PU9EsMroh6zXXNMjmivLgqqARwmH1iT1GL",
            Type = 1,
            Status = 2,
            Expiration = 3,
            YesAmount = 4,
            NoAmount = 5,
            PledgeAmount = 6
        };

        var expectedResponse = new LocalCallResponseDto { Return = expectedSummary };

        _smartContractsModuleMock.Setup(callTo => callTo.LocalCallAsync(It.IsAny<LocalCallRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var response = await _handler.Handle(new CallCirrusGetVaultProposalSummaryByProposalIdQuery(vault, proposalId, blockHeight), CancellationToken.None);

        // Assert
        response.Creator.Should().Be(expectedSummary.Creator);
        response.Amount.Should().Be(expectedSummary.Amount);
        response.Wallet.Should().Be(expectedSummary.Wallet);
        response.Type.Should().Be((VaultProposalType)expectedSummary.Type);
        response.Status.Should().Be((VaultProposalStatus)expectedSummary.Status);
        response.Expiration.Should().Be(expectedSummary.Expiration);
        response.YesAmount.Should().Be(expectedSummary.YesAmount);
        response.NoAmount.Should().Be(expectedSummary.NoAmount);
        response.PledgeAmount.Should().Be(expectedSummary.PledgeAmount);
    }

    private sealed class ProposalResponse
    {
        public Address Creator { get; set; }
        public UInt256 Amount { get; set; }
        public Address Wallet { get; set; }
        public byte Type { get; set; }
        public byte Status { get; set; }
        public ulong Expiration { get; set; }
        public ulong YesAmount { get; set; }
        public ulong NoAmount { get; set; }
        public ulong PledgeAmount { get; set; }
    }
}
