using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryCommands.Markets.Quotes;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Controllers;
using Opdex.Platform.WebApi.Models;
using Opdex.Platform.WebApi.Models.Requests.Markets;
using Opdex.Platform.WebApi.Models.Responses.Transactions;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Controllers.MarketsControllerTests
{
    public class CollectFeesQuoteTests
    {
        private readonly Mock<IApplicationContext> _applicationContextMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly MarketsController _controller;

        public CollectFeesQuoteTests()
        {
            _applicationContextMock = new Mock<IApplicationContext>();
            _mapperMock = new Mock<IMapper>();
            _mediatorMock = new Mock<IMediator>();

            _controller = new MarketsController(_mediatorMock.Object, _mapperMock.Object, _applicationContextMock.Object);
        }

        [Fact]
        public async Task CollectFeesQuote_CreateCollectStandardMarketFeesTransactionQuoteCommand_Send()
        {
            // Arrange
            Address walletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
            _applicationContextMock.Setup(callTo => callTo.Wallet).Returns(walletAddress);

            Address market = "PR71udY85pAcNcitdDfzQevp6Zar9DizHM";

            var request = new CollectMarketFeesQuoteRequest
            {
                Token = "PNvzq4pxJ5v3pp9kDaZyifKNspGD79E4qM",
                Amount = 40
            };
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _controller.CollectFeesQuote(market, request, cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<CreateCollectStandardMarketFeesTransactionQuoteCommand>(command
                => command.Market == market
                && command.WalletAddress == walletAddress
                && command.Token == request.Token
                && command.Amount == request.Amount
            ), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task CollectFeesQuote_Result_Map()
        {
            // Arrange
            Address walletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
            _applicationContextMock.Setup(callTo => callTo.Wallet).Returns(walletAddress);

            var quote = new TransactionQuoteDto();
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CreateCollectStandardMarketFeesTransactionQuoteCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(quote);

            var request = new CollectMarketFeesQuoteRequest
            {
                Token = "PNvzq4pxJ5v3pp9kDaZyifKNspGD79E4qM",
                Amount = 40
            };

            // Act
            try
            {
                await _controller.CollectFeesQuote("PR71udY85pAcNcitdDfzQevp6Zar9DizHM", request, CancellationToken.None);
            }
            catch (Exception) { }

            // Assert
            _mapperMock.Verify(callTo => callTo.Map<TransactionQuoteResponseModel>(quote), Times.Once);
        }

        [Fact]
        public async Task CollectFeesQuote_Success_ReturnOk()
        {
            // Arrange
            Address walletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
            _applicationContextMock.Setup(callTo => callTo.Wallet).Returns(walletAddress);

            var responseModel = new TransactionQuoteResponseModel();
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CreateCollectStandardMarketFeesTransactionQuoteCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new TransactionQuoteDto());
            _mapperMock.Setup(callTo => callTo.Map<TransactionQuoteResponseModel>(It.IsAny<TransactionQuoteDto>())).Returns(responseModel);

            var request = new CollectMarketFeesQuoteRequest
            {
                Token = "PNvzq4pxJ5v3pp9kDaZyifKNspGD79E4qM",
                Amount = 40
            };

            // Act
            var response = await _controller.CollectFeesQuote("PR71udY85pAcNcitdDfzQevp6Zar9DizHM", request, CancellationToken.None);

            // Act
            response.Result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)response.Result).Value.Should().Be(responseModel);
        }
    }
}
