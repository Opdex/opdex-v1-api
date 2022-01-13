using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Transactions;
using Opdex.Platform.Application.Abstractions.EntryCommands.Tokens.Quotes;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Application.EntryHandlers.Tokens.Quotes;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Constants.SmartContracts.Tokens;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Domain.Models.Transactions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Tokens.Quotes;

public class CreateDistributeTokensTransactionQuoteCommandHandlerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IModelAssembler<TransactionQuote, TransactionQuoteDto>> _assemblerMock;
    private readonly CreateDistributeTokensTransactionQuoteCommandHandler _handler;
    private readonly OpdexConfiguration _config;

    public CreateDistributeTokensTransactionQuoteCommandHandlerTests()
    {
        _config = new OpdexConfiguration { ApiUrl = "https://dev-api.opdex.com", WalletTransactionCallback = "/transactions" };
        _mediatorMock = new Mock<IMediator>();
        _assemblerMock = new Mock<IModelAssembler<TransactionQuote, TransactionQuoteDto>>();
        _handler = new CreateDistributeTokensTransactionQuoteCommandHandler(_assemblerMock.Object, _mediatorMock.Object, _config);
    }

    [Fact]
    public void CreateDistributeTokensTransactionQuoteCommand_InvalidToken_ThrowArgumentNullException()
    {
        // Arrange
        Address token = Address.Empty;
        Address walletAddress = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";

        // Act
        void Act() => new CreateDistributeTokensTransactionQuoteCommand(token, walletAddress);

        // Assert
        Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Token address must be provided.");
    }

    [Fact]
    public async Task CreateDistributeTokensTransactionQuoteCommand_Sends_RetrieveTokenByAddressQuery()
    {
        // Arrange
        Address token = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
        Address walletAddress = "Pz633jsy1XTenAyWcdTKU64jVFCDoHJgUK";

        var command = new CreateDistributeTokensTransactionQuoteCommand(token, walletAddress);
        var cancellationToken = new CancellationTokenSource().Token;

        // Act
        try
        {
            await _handler.Handle(command, cancellationToken);
        }
        catch { }

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveTokenByAddressQuery>(p => p.Address == token &&
                                                                                           p.FindOrThrow == true),
                                                   cancellationToken), Times.Once);
    }

    [Fact]
    public async Task CreateDistributeTokensTransactionQuoteCommand_Sends_MakeTransactionQuoteCommand()
    {
        // Arrange
        Address token = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
        Address walletAddress = "Pz633jsy1XTenAyWcdTKU64jVFCDoHJgUK";

        var command = new CreateDistributeTokensTransactionQuoteCommand(token, walletAddress);
        var cancellationToken = new CancellationTokenSource().Token;

        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), cancellationToken))
            .ReturnsAsync(new Token(1, token, "Bitcoin", "BTC", 8, 100_000_000, 0, 9, 10));

        // Act
        try
        {
            await _handler.Handle(command, cancellationToken);
        }
        catch { }

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<MakeTransactionQuoteCommand>(c => c.QuoteRequest.Sender == walletAddress
                                                                                           && c.QuoteRequest.To == token
                                                                                           && c.QuoteRequest.Amount == FixedDecimal.Zero
                                                                                           && c.QuoteRequest.Method == StakingTokenConstants.Methods.Distribute
                                                                                           && c.QuoteRequest.Callback == _config.WalletTransactionCallback
                                                                                           && !c.QuoteRequest.Parameters.Any()),
                                                   It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateDistributeTokensTransactionQuoteCommand_Assembles_TransactionQuoteDto()
    {
        // Arrange
        Address token = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
        Address walletAddress = "Pz633jsy1XTenAyWcdTKU64jVFCDoHJgUK";

        var command = new CreateDistributeTokensTransactionQuoteCommand(token, walletAddress);
        var cancellationToken = new CancellationTokenSource().Token;

        var expectedRequest = new TransactionQuoteRequest(walletAddress, token, FixedDecimal.Zero,
                                                          StandardTokenConstants.Methods.Approve, _config.WalletTransactionCallback);

        var expectedQuote = new TransactionQuote(null, null, 23800, null, expectedRequest);

        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), cancellationToken))
            .ReturnsAsync(new Token(1, token, "Bitcoin", "BTC", 8, 100_000_000, 0, 9, 10));

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
