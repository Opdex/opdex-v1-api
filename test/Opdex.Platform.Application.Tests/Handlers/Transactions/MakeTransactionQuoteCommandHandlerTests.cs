using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Transactions;
using Opdex.Platform.Application.Handlers.Transactions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.Tokens;
using Opdex.Platform.Domain.Models.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Commands;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Handlers.Transactions;

public class MakeTransactionQuoteCommandHandlerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly MakeTransactionQuoteCommandHandler _handler;

    public MakeTransactionQuoteCommandHandlerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _handler = new MakeTransactionQuoteCommandHandler(_mediatorMock.Object);
    }

    [Fact]
    public void MakeTransactionQuoteCommandHandler_InvalidQuoteRequest_ThrowArgumentNullException()
    {
        // Arrange
        // Act
        void Act() => new MakeTransactionQuoteCommand(null);

        // Assert
        Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Transaction quote request must be provided.");
    }

    [Fact]
    public async Task MakeTransactionQuoteCommandHandler_Sends_CallCirrusLocalCallSmartContractMethodCommand()
    {
        // Arrange
        Address sender = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
        Address to = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
        FixedDecimal amount = FixedDecimal.Parse("1.1");
        const string method = "Swap";
        const string callback = "https://dev-api.opdex.com/transactions";

        var parameters = new List<TransactionQuoteRequestParameter>()
        {
            new TransactionQuoteRequestParameter("Amount", UInt256.Parse("10"))
        };

        var cancellationToken = new CancellationTokenSource().Token;
        var request = new TransactionQuoteRequest(sender, to, amount, method, callback, parameters);

        // Act
        try
        {
            await _handler.Handle(new MakeTransactionQuoteCommand(request), cancellationToken);
        }
        catch (Exception) { }

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<CallCirrusLocalCallSmartContractMethodCommand>(requestDto => requestDto.QuoteRequest == request),
                                                   It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task MakeTransactionQuoteCommandHandler_Returns_TransactionQuote_Success()
    {

        // Arrange
        const string result = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
        const uint gasUsed = 10000;

        Address sender = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
        Address to = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
        FixedDecimal amount = FixedDecimal.Zero;
        const string method = "Swap";
        const string callback = "https://dev-api.opdex.com/transactions";

        var quoteRequest = new TransactionQuoteRequest(sender, to, amount, method, callback);

        dynamic txLog = new ExpandoObject();
        txLog.owner = "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN";
        txLog.spender = "PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh";
        txLog.amount = "1";
        txLog.oldAmount = "0";

        var logs = new List<TransactionLog> { new ApprovalLog(txLog, "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy", 1) };

        var quote = new TransactionQuote(result, null, gasUsed, logs, quoteRequest);

        _mediatorMock
            .Setup(callTo => callTo.Send(It.IsAny<CallCirrusLocalCallSmartContractMethodCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(quote);

        // Act
        var response = await _handler.Handle(new MakeTransactionQuoteCommand(quoteRequest), CancellationToken.None);

        // Assert
        response.Error.Should().BeNull();
        response.Request.Should().BeEquivalentTo(quoteRequest);
        response.Result.Should().Be(result);
        response.GasUsed.Should().Be(gasUsed);
        response.Logs.Should().BeEquivalentTo(logs);
    }
}