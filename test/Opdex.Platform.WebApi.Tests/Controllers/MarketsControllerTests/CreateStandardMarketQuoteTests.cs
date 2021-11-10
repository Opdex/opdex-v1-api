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
    public class CreateStandardMarketQuoteTests
    {
        private readonly Mock<IApplicationContext> _applicationContextMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly MarketsController _controller;

        public CreateStandardMarketQuoteTests()
        {
            _applicationContextMock = new Mock<IApplicationContext>();
            _mapperMock = new Mock<IMapper>();
            _mediatorMock = new Mock<IMediator>();
            _controller = new MarketsController(_mediatorMock.Object, _mapperMock.Object, _applicationContextMock.Object);
        }

        [Fact]
        public async Task CreateStandardMarketQuote_CreateCreateStandardMarketTransactionQuoteCommand_Send()
        {
            // Arrange
            Address walletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
            _applicationContextMock.Setup(callTo => callTo.Wallet).Returns(walletAddress);

            var request = new CreateStandardMarketQuoteRequest
            {
                MarketOwner = "PUFLuoW2K4PgJZ4nt5fEUHfvQXyQWKG9hm",
                TransactionFee = 5,
                AuthLiquidityProviders = true,
                AuthTraders = false,
                AuthPoolCreators = false,
                EnableMarketFee = true
            };
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _controller.CreateStandardMarketQuote(request, cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<CreateCreateStandardMarketTransactionQuoteCommand>(command
                => command.WalletAddress == walletAddress
                && command.Owner == request.MarketOwner
                && command.TransactionFee == request.TransactionFee
                && command.AuthLiquidityProviders == request.AuthLiquidityProviders
                && command.AuthTraders == request.AuthTraders
                && command.AuthPoolCreators == request.AuthPoolCreators
                && command.EnableMarketFee == request.EnableMarketFee
            ), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task CreateStandardMarketQuote_Result_Map()
        {
            // Arrange
            Address walletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
            _applicationContextMock.Setup(callTo => callTo.Wallet).Returns(walletAddress);

            var quote = new TransactionQuoteDto();
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CreateCreateStandardMarketTransactionQuoteCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(quote);

            var request = new CreateStandardMarketQuoteRequest
            {
                MarketOwner = "PUFLuoW2K4PgJZ4nt5fEUHfvQXyQWKG9hm",
                TransactionFee = 5,
                AuthLiquidityProviders = true,
                AuthTraders = false,
                AuthPoolCreators = false,
                EnableMarketFee = true
            };

            // Act
            try
            {
                await _controller.CreateStandardMarketQuote(request, CancellationToken.None);
            }
            catch (Exception) { }

            // Assert
            _mapperMock.Verify(callTo => callTo.Map<TransactionQuoteResponseModel>(quote), Times.Once);
        }

        [Fact]
        public async Task CreateStandardMarketQuote_Success_ReturnOk()
        {
            // Arrange
            Address walletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
            _applicationContextMock.Setup(callTo => callTo.Wallet).Returns(walletAddress);

            var responseModel = new TransactionQuoteResponseModel();
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CreateCreateStandardMarketTransactionQuoteCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new TransactionQuoteDto());
            _mapperMock.Setup(callTo => callTo.Map<TransactionQuoteResponseModel>(It.IsAny<TransactionQuoteDto>())).Returns(responseModel);

            var request = new CreateStandardMarketQuoteRequest
            {
                MarketOwner = "PUFLuoW2K4PgJZ4nt5fEUHfvQXyQWKG9hm",
                TransactionFee = 5,
                AuthLiquidityProviders = true,
                AuthTraders = false,
                AuthPoolCreators = false,
                EnableMarketFee = true
            };

            // Act
            var response = await _controller.CreateStandardMarketQuote(request, CancellationToken.None);

            // Act
            response.Result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)response.Result).Value.Should().Be(responseModel);
        }
    }
}
