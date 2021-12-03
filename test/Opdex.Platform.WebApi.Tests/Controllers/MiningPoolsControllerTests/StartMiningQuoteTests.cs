using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryCommands.MiningPools.Quotes;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models;
using Opdex.Platform.WebApi.Models.Requests.MiningPools;
using Opdex.Platform.WebApi.Models.Responses.Transactions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Controllers.MiningPoolsControllerTests;

public class StartMiningQuoteTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IApplicationContext> _contextMock;
    private readonly WebApi.Controllers.MiningPoolsController _controller;

    public StartMiningQuoteTests()
    {
        _mapperMock = new Mock<IMapper>();
        _mediatorMock = new Mock<IMediator>();
        _contextMock = new Mock<IApplicationContext>();

        _controller = new WebApi.Controllers.MiningPoolsController(_mapperMock.Object, _mediatorMock.Object, _contextMock.Object);
    }

    [Fact]
    public async Task StartMining_CreateStartMiningTransactionQuoteCommand_Send()
    {
        // Arrange
        Address walletAddress = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u";
        Address miningPool = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDda3";
        var miningQuoteRequest = new MiningQuote { Amount = FixedDecimal.Parse("1.00") };
        var cancellationToken = new CancellationTokenSource().Token;

        _contextMock.Setup(get => get.Wallet).Returns(walletAddress);

        // Act
        await _controller.StartMining(miningPool.ToString(), miningQuoteRequest, cancellationToken);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<CreateStartMiningTransactionQuoteCommand>(query => query.MiningPool == miningPool
                                                                                                            && query.WalletAddress == walletAddress), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task StartMining_CreateStartMiningTransactionQuoteCommandResponse_Map()
    {
        // Arrange
        var dto = new TransactionQuoteDto();
        Address walletAddress = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u";
        Address miningPool = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDda3";
        var miningQuoteRequest = new MiningQuote { Amount = FixedDecimal.Parse("1.00") };
        var cancellationToken = new CancellationTokenSource().Token;

        _contextMock.Setup(get => get.Wallet).Returns(walletAddress);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CreateStartMiningTransactionQuoteCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(dto);

        // Act
        await _controller.StartMining(miningPool.ToString(), miningQuoteRequest, cancellationToken);

        // Assert
        _mapperMock.Verify(callTo => callTo.Map<TransactionQuoteResponseModel>(dto), Times.Once);
    }

    [Fact]
    public async Task StartMining_CreateStartMiningTransactionQuoteCommandResponse_ReturnOk()
    {
        // Arrange
        var quoteResponse = new TransactionQuoteResponseModel();
        Address walletAddress = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u";
        Address miningPool = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDda3";
        var miningQuoteRequest = new MiningQuote { Amount = FixedDecimal.Parse("1.00") };
        var cancellationToken = new CancellationTokenSource().Token;

        _contextMock.Setup(get => get.Wallet).Returns(walletAddress);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CreateStartMiningTransactionQuoteCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(new TransactionQuoteDto());
        _mapperMock.Setup(callTo => callTo.Map<TransactionQuoteResponseModel>(It.IsAny<TransactionQuoteDto>())).Returns(quoteResponse);

        // Act
        var response = await _controller.StartMining(miningPool.ToString(), miningQuoteRequest, cancellationToken);

        // Act
        response.Result.Should().BeOfType<OkObjectResult>();
        ((OkObjectResult)response.Result).Value.Should().Be(quoteResponse);
    }
}