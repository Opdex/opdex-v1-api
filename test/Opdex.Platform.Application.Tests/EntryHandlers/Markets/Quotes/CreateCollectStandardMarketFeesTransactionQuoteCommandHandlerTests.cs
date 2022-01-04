using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Transactions;
using Opdex.Platform.Application.Abstractions.EntryCommands.Markets.Quotes;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Application.EntryHandlers.Markets.Quotes;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Constants.SmartContracts.Markets;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Domain.Models.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Markets.Quotes;

public class CreateCollectStandardMarketFeesTransactionQuoteCommandHandlerTests
{
    private readonly Mock<IModelAssembler<TransactionQuote, TransactionQuoteDto>> _assemblerMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly OpdexConfiguration _config;
    private readonly CreateCollectStandardMarketFeesTransactionQuoteCommandHandler _handler;

    public CreateCollectStandardMarketFeesTransactionQuoteCommandHandlerTests()
    {
        _assemblerMock = new Mock<IModelAssembler<TransactionQuote, TransactionQuoteDto>>();
        _mediatorMock = new Mock<IMediator>();
        _config = new OpdexConfiguration { ApiUrl = "https://dev-api.opdex.com", WalletTransactionCallback = "/transactions" };
        _handler = new CreateCollectStandardMarketFeesTransactionQuoteCommandHandler(_assemblerMock.Object, _mediatorMock.Object, _config);
    }

    [Fact]
    public async Task Handle_RetrieveMarketByAddressQuery_FindOrThrow()
    {
        // Arrange
        Address market = "PEkFDLUw1aLjYCWoJ1jRehNfTXjgWuZsX3";
        var command = new CreateCollectStandardMarketFeesTransactionQuoteCommand(market, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy", 50);
        var cancellationToken = new CancellationTokenSource().Token;

        // Act
        try
        {
            await _handler.Handle(command, cancellationToken);
        }
        catch (Exception) { }

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveMarketByAddressQuery>(query => query.Address == market
                                                                                                && query.FindOrThrow), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_RetrieveTokenByAddressQuery_Send()
    {
        // Arrange
        Address token = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
        var command = new CreateCollectStandardMarketFeesTransactionQuoteCommand("PEkFDLUw1aLjYCWoJ1jRehNfTXjgWuZsX3", "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", token, 50);
        var cancellationToken = new CancellationTokenSource().Token;

        // Act
        try
        {
            await _handler.Handle(command, cancellationToken);
        }
        catch (Exception) { }

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveTokenByAddressQuery>(query => query.Address == token
                                                                                               && !query.FindOrThrow), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_TokenNotFound_ThrowInvalidDataException()
    {
        // Arrange
        Address token = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
        var command = new CreateCollectStandardMarketFeesTransactionQuoteCommand("PEkFDLUw1aLjYCWoJ1jRehNfTXjgWuZsX3", "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", token, 50);

        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync((Token)null);

        // Act
        async Task Act() => await _handler.Handle(command, CancellationToken.None);

        // Assert
        var exception = await Assert.ThrowsAsync<InvalidDataException>(Act);
        exception.PropertyName.Should().Be("Token");
        exception.Message.Should().Be("Token address is not known.");
    }

    [Fact]
    public async Task CreateCollectStandardMarketFeesTransactionQuoteCommand_Sends_MakeTransactionQuoteCommand()
    {
        // Arrange
        Address market = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
        Address owner = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
        Address tokenAddress = "PNEPCzpKSXns3jWtVfkF7WJeZKdNeEZTBK";
        FixedDecimal amount = 50;

        FixedDecimal crsToSend = FixedDecimal.Zero;

        var command = new CreateCollectStandardMarketFeesTransactionQuoteCommand(market, owner, tokenAddress, amount);
        var cancellationToken = new CancellationTokenSource().Token;

        var token = new Token(5, "PNEPCzpKSXns3jWtVfkF7WJeZKdNeEZTBK", false, "Opdex", "dODX", 8, 10000000000, 50000000000000, 9, 10);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(token);

        var expectedParameters = new List<TransactionQuoteRequestParameter>
        {
            new TransactionQuoteRequestParameter("Token", new SmartContractMethodParameter(command.Token)),
            new TransactionQuoteRequestParameter("Amount", new SmartContractMethodParameter(amount.ToSatoshis(token.Decimals)))
        };

        // Act
        try
        {
            await _handler.Handle(command, cancellationToken);
        }
        catch { }

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<MakeTransactionQuoteCommand>(c => c.QuoteRequest.Sender == owner
                                                                                           && c.QuoteRequest.To == market
                                                                                           && c.QuoteRequest.Amount == crsToSend
                                                                                           && c.QuoteRequest.Method == StandardMarketConstants.Methods.CollectMarketFees
                                                                                           && c.QuoteRequest.Callback != null
                                                                                           && c.QuoteRequest.Parameters.All(p => expectedParameters.Select(e => e.Value).Contains(p.Value))),
                                                   cancellationToken), Times.Once);
    }

    [Fact]
    public async Task CreateCollectStandardMarketFeesTransactionQuoteCommand_Assembles_TransactionQuoteDto()
    {
        // Arrange
        Address market = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
        Address owner = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
        Address tokenAddress = "PNEPCzpKSXns3jWtVfkF7WJeZKdNeEZTBK";
        FixedDecimal amount = 50;

        var command = new CreateCollectStandardMarketFeesTransactionQuoteCommand(market, owner, tokenAddress, amount);
        var cancellationToken = new CancellationTokenSource().Token;

        var token = new Token(5, "PNEPCzpKSXns3jWtVfkF7WJeZKdNeEZTBK", false, "Opdex", "dODX", 8, 10000000000, 500000000000, 9, 10);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(token);

        var expectedRequest = new TransactionQuoteRequest(market, owner, FixedDecimal.Zero, StandardMarketConstants.Methods.CollectMarketFees, _config.WalletTransactionCallback);

        var expectedQuote = new TransactionQuote("PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjQf", null, 23800, null, expectedRequest);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<MakeTransactionQuoteCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(expectedQuote);

        // Act
        try
        {
            await _handler.Handle(command, CancellationToken.None);
        }
        catch { }

        // Assert
        _assemblerMock.Verify(callTo => callTo.Assemble(expectedQuote), Times.Once);
    }
}
