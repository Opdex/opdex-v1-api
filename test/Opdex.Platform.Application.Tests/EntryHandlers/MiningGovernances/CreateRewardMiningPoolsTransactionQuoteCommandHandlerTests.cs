using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Transactions;
using Opdex.Platform.Application.Abstractions.EntryCommands.MiningGovernances;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Application.Abstractions.Queries.MiningGovernances;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Application.EntryHandlers.MiningGovernances;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Constants.SmartContracts;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Transactions;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.MiningGovernances;

public class CreateRewardMiningPoolsTransactionQuoteCommandHandlerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IModelAssembler<TransactionQuote, TransactionQuoteDto>> _assemblerMock;
    private readonly CreateRewardMiningPoolsTransactionQuoteCommandHandler _handler;
    private readonly OpdexConfiguration _config;

    public CreateRewardMiningPoolsTransactionQuoteCommandHandlerTests()
    {
        _config = new OpdexConfiguration { ApiUrl = "https://dev-api.opdex.com", WalletTransactionCallback = "/transactions" };
        _mediatorMock = new Mock<IMediator>();
        _assemblerMock = new Mock<IModelAssembler<TransactionQuote, TransactionQuoteDto>>();
        _handler = new CreateRewardMiningPoolsTransactionQuoteCommandHandler(_assemblerMock.Object, _mediatorMock.Object, _config);
    }

    [Fact]
    public void CreateRewardMiningPoolsTransactionQuoteCommand_InvalidGovernance_ThrowArgumentNullException()
    {
        // Arrange
        Address miningGovernance = Address.Empty;
        Address walletAddress = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
        const bool fullDistribution = true;

        // Act
        void Act() => new CreateRewardMiningPoolsTransactionQuoteCommand(miningGovernance, walletAddress, fullDistribution);

        // Assert
        Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Mining governance address must be provided.");
    }

    [Fact]
    public async Task CreateRewardMiningPoolsTransactionQuoteCommand_Sends_RetrieveMiningGovernanceByAddressQuery()
    {
        // Arrange
        Address walletAddress = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
        Address miningGovernance = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
        const bool fullDistribution = true;

        var command = new CreateRewardMiningPoolsTransactionQuoteCommand(miningGovernance, walletAddress, fullDistribution);
        var cancellationToken = new CancellationTokenSource().Token;

        // Act
        try
        {
            await _handler.Handle(command, cancellationToken);
        }
        catch { }

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveMiningGovernanceByAddressQuery>(p => p.Address == miningGovernance
                                                                                                      && p.FindOrThrow == true), cancellationToken), Times.Once);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task CreateRewardMiningPoolsTransactionQuoteCommand_Sends_MakeTransactionQuoteCommand(bool fullDistribution)
    {
        // Arrange
        Address walletAddress = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
        Address miningGovernance = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
        FixedDecimal crsToSend = FixedDecimal.Zero;
        var methodName = fullDistribution ? MiningGovernanceConstants.Methods.RewardMiningPools : MiningGovernanceConstants.Methods.RewardMiningPool;

        var command = new CreateRewardMiningPoolsTransactionQuoteCommand(miningGovernance, walletAddress, fullDistribution);
        var cancellationToken = new CancellationTokenSource().Token;
        // Act
        try
        {
            await _handler.Handle(command, cancellationToken);
        }
        catch { }

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.IsAny<RetrieveMiningGovernanceByAddressQuery>(), cancellationToken), Times.Once);
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<MakeTransactionQuoteCommand>(c => c.QuoteRequest.Sender == walletAddress
                                                                                           && c.QuoteRequest.To == miningGovernance
                                                                                           && c.QuoteRequest.Amount == crsToSend
                                                                                           && c.QuoteRequest.Method == methodName
                                                                                           && c.QuoteRequest.Callback != null),
                                                   It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateRewardMiningPoolsTransactionQuoteCommand_Assembles_TransactionQuoteDto()
    {
        // Arrange
        Address walletAddress = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
        Address miningGovernance = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
        FixedDecimal crsToSend = FixedDecimal.Zero;
        const bool fullDistribution = true;
        const string methodName = MiningGovernanceConstants.Methods.RewardMiningPools;

        var command = new CreateRewardMiningPoolsTransactionQuoteCommand(miningGovernance, walletAddress, fullDistribution);
        var cancellationToken = new CancellationTokenSource().Token;

        var expectedRequest = new TransactionQuoteRequest(walletAddress, miningGovernance, crsToSend, methodName, _config.WalletTransactionCallback);

        var expectedQuote = new TransactionQuote(null, null, 23800, null, expectedRequest);

        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<MakeTransactionQuoteCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedQuote);

        // Act
        try
        {
            await _handler.Handle(command, cancellationToken);
        }
        catch { }

        // Assert
        _assemblerMock.Verify(callTo => callTo.Assemble(expectedQuote), Times.Once);
        _mediatorMock.Verify(callTo => callTo.Send(It.IsAny<RetrieveMiningGovernanceByAddressQuery>(), cancellationToken), Times.Once);
    }
}