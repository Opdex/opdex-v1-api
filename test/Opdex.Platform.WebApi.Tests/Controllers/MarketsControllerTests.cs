using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryCommands.Markets;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.WebApi.Controllers;
using Opdex.Platform.WebApi.Models;
using Opdex.Platform.WebApi.Models.Requests.Markets;
using Opdex.Platform.WebApi.Models.Responses.Transactions;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Controllers
{
    public class MarketsControllerTests
    {
        private readonly Mock<IApplicationContext> _applicationContextMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly MarketsController _controller;

        public MarketsControllerTests()
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

        [Fact]
        public async Task ClaimOwnershipQuote_CreateClaimStandardMarketOwnershipTransactionQuoteCommand_Send()
        {
            // Arrange
            Address walletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
            _applicationContextMock.Setup(callTo => callTo.Wallet).Returns(walletAddress);

            Address market = "PR71udY85pAcNcitdDfzQevp6Zar9DizHM";
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _controller.ClaimOwnershipQuote(market, cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<CreateClaimStandardMarketOwnershipTransactionQuoteCommand>(command
                => command.Market == market
                && command.WalletAddress == walletAddress
            ), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task ClaimOwnershipQuote_Result_Map()
        {
            // Arrange
            Address walletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
            _applicationContextMock.Setup(callTo => callTo.Wallet).Returns(walletAddress);

            var quote = new TransactionQuoteDto();
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CreateClaimStandardMarketOwnershipTransactionQuoteCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(quote);

            // Act
            try
            {
                await _controller.ClaimOwnershipQuote("PR71udY85pAcNcitdDfzQevp6Zar9DizHM", CancellationToken.None);
            }
            catch (Exception) { }

            // Assert
            _mapperMock.Verify(callTo => callTo.Map<TransactionQuoteResponseModel>(quote), Times.Once);
        }

        [Fact]
        public async Task ClaimOwnershipQuote_Success_ReturnOk()
        {
            // Arrange
            Address walletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
            _applicationContextMock.Setup(callTo => callTo.Wallet).Returns(walletAddress);

            var responseModel = new TransactionQuoteResponseModel();
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CreateClaimStandardMarketOwnershipTransactionQuoteCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new TransactionQuoteDto());
            _mapperMock.Setup(callTo => callTo.Map<TransactionQuoteResponseModel>(It.IsAny<TransactionQuoteDto>())).Returns(responseModel);

            // Act
            var response = await _controller.ClaimOwnershipQuote("PR71udY85pAcNcitdDfzQevp6Zar9DizHM", CancellationToken.None);

            // Act
            response.Result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)response.Result).Value.Should().Be(responseModel);
        }


        [Fact]
        public async Task SetPermissionsQuote_CreateSetStandardMarketPermissionsTransactionQuoteCommand_Send()
        {
            // Arrange
            Address walletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
            _applicationContextMock.Setup(callTo => callTo.Wallet).Returns(walletAddress);

            Address market = "PR71udY85pAcNcitdDfzQevp6Zar9DizHM";
            Address user = "P8zHy2c8Nydkh2r6Wv6K6kacxkDcZyfaLy";

            var request = new SetMarketPermissionsQuoteRequest
            {
                Permission = Permissions.Provide,
                Authorize = true
            };
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _controller.SetPermissionsQuote(market, user, request, cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<CreateSetStandardMarketPermissionsTransactionQuoteCommand>(command
                => command.Market == market
                && command.WalletAddress == walletAddress
                && command.User == user
                && command.Permission == request.Permission
                && command.Authorize == request.Authorize
            ), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task SetPermissionsQuote_Result_Map()
        {
            // Arrange
            Address walletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
            _applicationContextMock.Setup(callTo => callTo.Wallet).Returns(walletAddress);

            var quote = new TransactionQuoteDto();
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CreateSetStandardMarketPermissionsTransactionQuoteCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(quote);

            var request = new SetMarketPermissionsQuoteRequest
            {
                Permission = Permissions.Provide,
                Authorize = true
            };

            // Act
            try
            {
                await _controller.SetPermissionsQuote("PR71udY85pAcNcitdDfzQevp6Zar9DizHM", "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV",
                                                      request, CancellationToken.None);
            }
            catch (Exception) { }

            // Assert
            _mapperMock.Verify(callTo => callTo.Map<TransactionQuoteResponseModel>(quote), Times.Once);
        }

        [Fact]
        public async Task SetPermissionsQuote_Success_ReturnOk()
        {
            // Arrange
            Address walletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
            _applicationContextMock.Setup(callTo => callTo.Wallet).Returns(walletAddress);

            var responseModel = new TransactionQuoteResponseModel();
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CreateSetStandardMarketPermissionsTransactionQuoteCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new TransactionQuoteDto());
            _mapperMock.Setup(callTo => callTo.Map<TransactionQuoteResponseModel>(It.IsAny<TransactionQuoteDto>())).Returns(responseModel);

            var request = new SetMarketPermissionsQuoteRequest
            {
                Permission = Permissions.Provide,
                Authorize = true
            };

            // Act
            var response = await _controller.SetPermissionsQuote("PR71udY85pAcNcitdDfzQevp6Zar9DizHM", "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV",
                                                                 request, CancellationToken.None);

            // Act
            response.Result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)response.Result).Value.Should().Be(responseModel);
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

        [Fact]
        public async Task CreateStakingMarketQuoteRequest_CreateCreateStakingMarketTransactionQuoteCommand_Send()
        {
            // Arrange
            Address walletAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
            _applicationContextMock.Setup(callTo => callTo.Wallet).Returns(walletAddress);

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
            _applicationContextMock.Setup(callTo => callTo.Wallet).Returns(walletAddress);

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
            _applicationContextMock.Setup(callTo => callTo.Wallet).Returns(walletAddress);

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
