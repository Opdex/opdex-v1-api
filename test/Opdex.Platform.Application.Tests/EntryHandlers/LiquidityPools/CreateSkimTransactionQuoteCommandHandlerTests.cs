using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Transactions;
using Opdex.Platform.Application.Abstractions.EntryCommands.LiquidityPools.Quotes;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Application.EntryHandlers.LiquidityPools.Quotes;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Constants.SmartContracts;
using Opdex.Platform.Common.Constants.SmartContracts.LiquidityPools;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.LiquidityPools;

public class CreateSkimTransactionQuoteCommandHandlerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IModelAssembler<TransactionQuote, TransactionQuoteDto>> _assemblerMock;
    private readonly CreateSkimTransactionQuoteCommandHandler _handler;
    private readonly OpdexConfiguration _config;
    const string MethodName = LiquidityPoolConstants.Methods.Skim;

    public CreateSkimTransactionQuoteCommandHandlerTests()
    {
        _config = new OpdexConfiguration {ApiUrl = "https://dev-api.opdex.com", WalletTransactionCallback = "/transactions"};
        _mediatorMock = new Mock<IMediator>();
        _assemblerMock = new Mock<IModelAssembler<TransactionQuote, TransactionQuoteDto>>();
        _handler = new CreateSkimTransactionQuoteCommandHandler(_assemblerMock.Object, _mediatorMock.Object, _config);
    }

    [Fact]
    public void CreateSkimTransactionQuoteCommand_InvalidLiquidityPool_ThrowArgumentNullException()
    {
        // Arrange
        Address liquidityPool = Address.Empty;
        Address walletAddress = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
        Address recipient = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGJcuwA";

        // Act
        void Act() => new CreateSkimTransactionQuoteCommand(liquidityPool, walletAddress, recipient);

        // Assert
        Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Liquidity pool must be provided.");
    }

    [Fact]
    public void CreateSkimTransactionQuoteCommand_InvalidRecipient_ThrowArgumentNullException()
    {
        // Arrange
        Address walletAddress = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
        Address liquidityPool = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
        Address recipient = null;

        // Act
        void Act() => new CreateSkimTransactionQuoteCommand(liquidityPool, walletAddress, recipient);

        // Assert
        Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Recipient must be provided.");
    }

    [Fact]
    public async Task CreateSkimTransactionQuoteCommand_Sends_RetrieveLiquidityPoolByAddressQuery()
    {
        // Arrange
        Address walletAddress = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
        Address liquidityPool = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
        Address recipient = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGJcuwA";

        var command = new CreateSkimTransactionQuoteCommand(liquidityPool, walletAddress, recipient);
        var cancellationToken = new CancellationTokenSource().Token;

        // Act
        try
        {
            await _handler.Handle(command, cancellationToken);
        }
        catch { }

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveLiquidityPoolByAddressQuery>(c => c.Address == liquidityPool && c.FindOrThrow == true),
                                                   It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateSkimTransactionQuoteCommand_Sends_MakeTransactionQuoteCommand()
    {
        // Arrange
        Address walletAddress = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
        Address liquidityPool = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
        Address recipient = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGJcuwA";
        FixedDecimal crsToSend = FixedDecimal.Zero;

        var command = new CreateSkimTransactionQuoteCommand(liquidityPool, walletAddress, recipient);
        var cancellationToken = new CancellationTokenSource().Token;

        var expectedParameters = new List<TransactionQuoteRequestParameter>
        {
            new TransactionQuoteRequestParameter("Recipient", recipient)
        };

        // Act
        try
        {
            await _handler.Handle(command, cancellationToken);
        }
        catch { }

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<MakeTransactionQuoteCommand>(c => c.QuoteRequest.Sender == walletAddress
                                                                                           && c.QuoteRequest.To == liquidityPool
                                                                                           && c.QuoteRequest.Amount == crsToSend
                                                                                           && c.QuoteRequest.Method == MethodName
                                                                                           && c.QuoteRequest.Callback != null
                                                                                           && c.QuoteRequest.Parameters
                                                                                               .All(p => expectedParameters
                                                                                                        .Select(e => e.Value)
                                                                                                        .Contains(p.Value))),
                                                   It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateSkimTransactionQuoteCommand_Assembles_TransactionQuoteDto()
    {
        // Arrange
        Address walletAddress = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
        Address liquidityPool = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
        Address recipient = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGJcuwA";
        FixedDecimal crsToSend = FixedDecimal.Zero;

        var command = new CreateSkimTransactionQuoteCommand(liquidityPool, walletAddress, recipient);
        var cancellationToken = new CancellationTokenSource().Token;

        var expectedParameters = new List<TransactionQuoteRequestParameter>
        {
            new TransactionQuoteRequestParameter("Recipient", recipient)
        };

        var expectedRequest = new TransactionQuoteRequest(walletAddress, liquidityPool, crsToSend, MethodName, _config.WalletTransactionCallback, expectedParameters);

        var expectedQuote = new TransactionQuote("PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjQf", null, 23800, null, expectedRequest);

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
    }
}