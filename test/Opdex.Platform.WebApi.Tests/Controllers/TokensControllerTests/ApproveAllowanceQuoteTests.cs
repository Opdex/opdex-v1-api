using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryCommands.Tokens.Quotes;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Controllers;
using Opdex.Platform.WebApi.Models;
using Opdex.Platform.WebApi.Models.Requests.Tokens;
using Opdex.Platform.WebApi.Models.Responses.Transactions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Controllers.TokensControllerTests;

public class ApproveAllowanceQuoteTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IApplicationContext> _contextMock;
    private readonly TokensController _controller;

    public ApproveAllowanceQuoteTests()
    {
        _mapperMock = new Mock<IMapper>();
        _mediatorMock = new Mock<IMediator>();
        _contextMock = new Mock<IApplicationContext>();

        _controller = new TokensController(_mediatorMock.Object, _mapperMock.Object, _contextMock.Object);
    }

    [Fact]
    public async Task ApproveAllowance_CreateApproveAllowanceTransactionQuoteCommand_Send()
    {
        // Arrange
        Address walletAddress = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u";
        Address token = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDda3";
        Address spender = "PXfJ6u1yzNPDda3GZPZpB4iW4LHVEPMKeh";
        FixedDecimal amount = FixedDecimal.Parse("1.1");

        var request = new ApproveAllowanceQuoteRequest { Spender = spender, Amount = amount };
        var cancellationToken = new CancellationTokenSource().Token;

        _contextMock.Setup(get => get.Wallet).Returns(walletAddress);

        // Act
        await _controller.ApproveAllowance(token, request, cancellationToken);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<CreateApproveAllowanceTransactionQuoteCommand>(query => query.Token == token &&
                                                                                                                 query.WalletAddress == walletAddress &&
                                                                                                                 query.Spender == spender &&
                                                                                                                 query.Amount == amount),
                                                   cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ApproveAllowance_CreateApproveAllowanceTransactionQuoteCommandResponse_Map()
    {
        // Arrange
        Address walletAddress = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u";
        Address token = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDda3";
        Address spender = "PXfJ6u1yzNPDda3GZPZpB4iW4LHVEPMKeh";
        FixedDecimal amount = FixedDecimal.Parse("1.1");

        var request = new ApproveAllowanceQuoteRequest { Spender = spender, Amount = amount };
        var cancellationToken = new CancellationTokenSource().Token;
        var dto = new TransactionQuoteDto();

        _contextMock.Setup(get => get.Wallet).Returns(walletAddress);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CreateApproveAllowanceTransactionQuoteCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(dto);

        // Act
        await _controller.ApproveAllowance(token, request, cancellationToken);

        // Assert
        _mapperMock.Verify(callTo => callTo.Map<TransactionQuoteResponseModel>(It.IsAny<TransactionQuoteDto>()), Times.Once);
    }

    [Fact]
    public async Task ApproveAllowance_CreateApproveAllowanceTransactionQuoteCommandResponse_ReturnOk()
    {
        // Arrange
        Address walletAddress = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u";
        Address token = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDda3";
        Address spender = "PXfJ6u1yzNPDda3GZPZpB4iW4LHVEPMKeh";
        FixedDecimal amount = FixedDecimal.Parse("1.1");

        var request = new ApproveAllowanceQuoteRequest { Spender = spender, Amount = amount };
        var cancellationToken = new CancellationTokenSource().Token;
        var quoteResponse = new TransactionQuoteResponseModel();

        _contextMock.Setup(get => get.Wallet).Returns(walletAddress);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CreateApproveAllowanceTransactionQuoteCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(new TransactionQuoteDto());
        _mapperMock.Setup(callTo => callTo.Map<TransactionQuoteResponseModel>(It.IsAny<TransactionQuoteDto>())).Returns(quoteResponse);

        // Act
        var response = await _controller.ApproveAllowance(token, request, cancellationToken);

        // Act
        response.Should().BeOfType<OkObjectResult>();
        ((OkObjectResult)response).Value.Should().Be(quoteResponse);
    }
}