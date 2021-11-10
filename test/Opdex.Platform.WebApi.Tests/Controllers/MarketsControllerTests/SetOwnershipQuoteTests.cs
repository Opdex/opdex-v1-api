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
    public class SetOwnershipQuoteTests
    {
        private readonly Mock<IApplicationContext> _applicationContextMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly MarketsController _controller;

        public SetOwnershipQuoteTests()
        {
            _applicationContextMock = new Mock<IApplicationContext>();
            _mapperMock = new Mock<IMapper>();
            _mediatorMock = new Mock<IMediator>();
            _controller = new MarketsController(_mediatorMock.Object, _mapperMock.Object, _applicationContextMock.Object);
        }

        [Fact]
        public async Task SetOwnershipQuote_CreateSetStandardMarketOwnershipTransactionQuoteCommand_Send()
        {
            // Arrange
            Address walletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
            _applicationContextMock.Setup(callTo => callTo.Wallet).Returns(walletAddress);

            Address market = "PR71udY85pAcNcitdDfzQevp6Zar9DizHM";
            var request = new SetMarketOwnerQuoteRequest
            {
                Owner = "PUFLuoW2K4PgJZ4nt5fEUHfvQXyQWKG9hm"
            };
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _controller.SetOwnershipQuote(market, request, cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<CreateSetStandardMarketOwnershipTransactionQuoteCommand>(command
                => command.Market == market
                && command.WalletAddress == walletAddress
                && command.NewOwner == request.Owner
            ), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task SetOwnershipQuote_Result_Map()
        {
            // Arrange
            Address walletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
            _applicationContextMock.Setup(callTo => callTo.Wallet).Returns(walletAddress);

            var quote = new TransactionQuoteDto();
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CreateSetStandardMarketOwnershipTransactionQuoteCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(quote);

            var request = new SetMarketOwnerQuoteRequest
            {
                Owner = "PUFLuoW2K4PgJZ4nt5fEUHfvQXyQWKG9hm"
            };

            // Act
            try
            {
                await _controller.SetOwnershipQuote("PR71udY85pAcNcitdDfzQevp6Zar9DizHM", request, CancellationToken.None);
            }
            catch (Exception) { }

            // Assert
            _mapperMock.Verify(callTo => callTo.Map<TransactionQuoteResponseModel>(quote), Times.Once);
        }

        [Fact]
        public async Task SetOwnershipQuote_Success_ReturnOk()
        {
            // Arrange
            Address walletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
            _applicationContextMock.Setup(callTo => callTo.Wallet).Returns(walletAddress);

            var responseModel = new TransactionQuoteResponseModel();
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CreateSetStandardMarketOwnershipTransactionQuoteCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new TransactionQuoteDto());
            _mapperMock.Setup(callTo => callTo.Map<TransactionQuoteResponseModel>(It.IsAny<TransactionQuoteDto>())).Returns(responseModel);

            var request = new SetMarketOwnerQuoteRequest
            {
                Owner = "PUFLuoW2K4PgJZ4nt5fEUHfvQXyQWKG9hm"
            };

            // Act
            var response = await _controller.SetOwnershipQuote("PR71udY85pAcNcitdDfzQevp6Zar9DizHM", request, CancellationToken.None);

            // Act
            response.Result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)response.Result).Value.Should().Be(responseModel);
        }
    }
}
