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
using Opdex.Platform.WebApi.Models.Responses.Transactions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Controllers.TokensControllerTests;

public class DistributeQuoteTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IApplicationContext> _contextMock;
    private readonly TokensController _controller;

    public DistributeQuoteTests()
    {
        _mapperMock = new Mock<IMapper>();
        _mediatorMock = new Mock<IMediator>();
        _contextMock = new Mock<IApplicationContext>();

        _controller = new TokensController(_mediatorMock.Object, _mapperMock.Object, _contextMock.Object);
    }

    [Fact]
    public async Task Distribute_CreateDistributeTokensTransactionQuoteCommand_Send()
    {
        // Arrange
        Address walletAddress = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u";
        Address token = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDda3";

        var cancellationToken = new CancellationTokenSource().Token;

        _contextMock.Setup(get => get.Wallet).Returns(walletAddress);

        // Act
        await _controller.Distribute(token, cancellationToken);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<CreateDistributeTokensTransactionQuoteCommand>(query => query.Token == token &&
                                                                                                                 query.WalletAddress == walletAddress),
                                                   cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Distribute_CreateDistributeTokensTransactionQuoteCommandResponse_Map()
    {
        // Arrange
        Address walletAddress = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u";
        Address token = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDda3";

        var cancellationToken = new CancellationTokenSource().Token;
        var dto = new TransactionQuoteDto();

        _contextMock.Setup(get => get.Wallet).Returns(walletAddress);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CreateDistributeTokensTransactionQuoteCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(dto);

        // Act
        await _controller.Distribute(token, cancellationToken);

        // Assert
        _mapperMock.Verify(callTo => callTo.Map<TransactionQuoteResponseModel>(It.IsAny<TransactionQuoteDto>()), Times.Once);
    }

    [Fact]
    public async Task Distribute_CreateDistributeTokensTransactionQuoteCommandResponse_ReturnOk()
    {
        // Arrange
        Address walletAddress = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u";
        Address token = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDda3";

        var cancellationToken = new CancellationTokenSource().Token;
        var quoteResponse = new TransactionQuoteResponseModel();

        _contextMock.Setup(get => get.Wallet).Returns(walletAddress);
        _mediatorMock
            .Setup(callTo => callTo.Send(It.IsAny<CreateDistributeTokensTransactionQuoteCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TransactionQuoteDto());
        _mapperMock.Setup(callTo => callTo.Map<TransactionQuoteResponseModel>(It.IsAny<TransactionQuoteDto>())).Returns(quoteResponse);

        // Act
        var response = await _controller.Distribute(token, cancellationToken);

        // Act
        response.Should().BeOfType<OkObjectResult>();
        ((OkObjectResult)response).Value.Should().Be(quoteResponse);
    }
}