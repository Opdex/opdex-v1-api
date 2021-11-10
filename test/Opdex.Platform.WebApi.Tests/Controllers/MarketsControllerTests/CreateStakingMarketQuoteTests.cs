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
    public class CreateStakingMarketQuoteTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IApplicationContext> _contextMock;
        private readonly MarketsController _controller;

        public CreateStakingMarketQuoteTests()
        {
            _mapperMock = new Mock<IMapper>();
            _mediatorMock = new Mock<IMediator>();
            _contextMock = new Mock<IApplicationContext>();
            _controller = new MarketsController(_mediatorMock.Object, _mapperMock.Object, _contextMock.Object);
        }

        [Fact]
        public async Task CreateStakingMarketQuoteRequest_CreateCreateStakingMarketTransactionQuoteCommand_Send()
        {
            // Arrange
            Address walletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
            _contextMock.Setup(callTo => callTo.Wallet).Returns(walletAddress);

            var request = new CreateStakingMarketQuoteRequest
            {
                StakingToken = "PR71udY85pAcNcitdDfzQevp6Zar9DizHM"
            };
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _controller.CreateStakingMarketQuote(request, cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<CreateCreateStakingMarketTransactionQuoteCommand>(command
                => command.WalletAddress == walletAddress
                && command.StakingToken == request.StakingToken
            ), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task CreateStakingMarketQuote_Result_Map()
        {
            // Arrange
            Address walletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
            _contextMock.Setup(callTo => callTo.Wallet).Returns(walletAddress);

            var quote = new TransactionQuoteDto();
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CreateCreateStakingMarketTransactionQuoteCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(quote);

            var request = new CreateStakingMarketQuoteRequest
            {
                StakingToken = "PR71udY85pAcNcitdDfzQevp6Zar9DizHM"
            };

            // Act
            try
            {
                await _controller.CreateStakingMarketQuote(request, CancellationToken.None);
            }
            catch (Exception) { }

            // Assert
            _mapperMock.Verify(callTo => callTo.Map<TransactionQuoteResponseModel>(quote), Times.Once);
        }

        [Fact]
        public async Task CreateStakingMarketQuote_Success_ReturnOk()
        {
            // Arrange
            Address walletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
            _contextMock.Setup(callTo => callTo.Wallet).Returns(walletAddress);

            var responseModel = new TransactionQuoteResponseModel();
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CreateCreateStakingMarketTransactionQuoteCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new TransactionQuoteDto());
            _mapperMock.Setup(callTo => callTo.Map<TransactionQuoteResponseModel>(It.IsAny<TransactionQuoteDto>())).Returns(responseModel);

            var request = new CreateStakingMarketQuoteRequest
            {
                StakingToken = "PR71udY85pAcNcitdDfzQevp6Zar9DizHM"
            };

            // Act
            var response = await _controller.CreateStakingMarketQuote(request, CancellationToken.None);

            // Act
            response.Result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)response.Result).Value.Should().Be(responseModel);
        }
    }
}
