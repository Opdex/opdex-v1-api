using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryCommands.Addresses.Balances;
using Opdex.Platform.Application.Abstractions.EntryQueries.Addresses.Allowances;
using Opdex.Platform.Application.Abstractions.EntryQueries.Addresses.Balances;
using Opdex.Platform.Application.Abstractions.EntryQueries.Addresses.Mining;
using Opdex.Platform.Application.Abstractions.EntryQueries.Addresses.Staking;
using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Controllers;
using Opdex.Platform.WebApi.Models.Requests.Wallets;
using Opdex.Platform.WebApi.Models.Responses.Wallet;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Controllers
{
    public class WalletsControllerTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IMediator> _mediatorMock;

        private readonly WalletsController _controller;

        public WalletsControllerTests()
        {
            _mapperMock = new Mock<IMapper>();
            _mediatorMock = new Mock<IMediator>();

            _controller = new WalletsController(_mapperMock.Object, _mediatorMock.Object);
        }

        [Fact]
        public async Task GetAddressBalances_GetAddressBalancesWithFilterQuery_Send()
        {
            // Arrange
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _controller.GetAddressBalances("P8zHy2c8Nydkh2r6Wv6K6kacxkDcZyfaLy", new AddressBalanceFilterParameters(), cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<GetAddressBalancesWithFilterQuery>(query => query.Cursor != null), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task GetAddressBalances_Result_ReturnOk()
        {
            // Arrange
            var addressBalances = new AddressBalancesResponseModel();
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<GetAddressBalancesWithFilterQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new AddressBalancesDto());
            _mapperMock.Setup(callTo => callTo.Map<AddressBalancesResponseModel>(It.IsAny<AddressBalancesDto>())).Returns(addressBalances);

            // Act
            var response = await _controller.GetAddressBalances("P8zHy2c8Nydkh2r6Wv6K6kacxkDcZyfaLy", new AddressBalanceFilterParameters(), CancellationToken.None);

            // Assert
            response.Result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)response.Result).Value.Should().Be(addressBalances);
        }

        [Fact]
        public async Task GetAddressBalanceByToken_GetAddressBalanceByTokenQuery_Send()
        {
            // Arrange
            var address = "P8zHy2c8Nydkh2r6Wv6K6kacxkDcZyfaLy";
            var token = "PBWhPbobijB21xv6DY75zaRpaLCvVZWLN5";
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _controller.GetAddressBalanceByToken(address, token, cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(
                It.Is<GetAddressBalanceByTokenQuery>(query => query.WalletAddress == address && query.TokenAddress == token),
                cancellationToken), Times.Once);
        }

        [Fact]
        public async Task GetAddressBalanceByToken_Result_ReturnOk()
        {
            // Arrange
            var tokenBalance = new AddressBalanceResponseModel();

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<GetAddressBalanceByTokenQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new AddressBalanceDto());
            _mapperMock.Setup(callTo => callTo.Map<AddressBalanceResponseModel>(It.IsAny<AddressBalanceDto>())).Returns(tokenBalance);

            // Act
            var response = await _controller.GetAddressBalanceByToken("P8zHy2c8Nydkh2r6Wv6K6kacxkDcZyfaLy", "PBWhPbobijB21xv6DY75zaRpaLCvVZWLN5", CancellationToken.None);

            // Act
            response.Result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)response.Result).Value.Should().Be(tokenBalance);
        }

        [Fact]
        public async Task RefreshAddressBalance_CreateRefreshAddressBalanceCommand_Send()
        {
            // Arrange
            var walletAddress = new Address("P8zHy2c8Nydkh2r6Wv6K6kacxkDcZyfaLy");
            var tokenAddress = new Address("PBWhPbobijB21xv6DY75zaRpaLCvVZWLN5");

            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _controller.RefreshAddressBalance(walletAddress, tokenAddress, cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<CreateRefreshAddressBalanceCommand>(command => command.Wallet == walletAddress
                                                                                                         && command.Token == tokenAddress), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task RefreshAddressBalance_Result_ReturnOk()
        {
            // Arrange
            var walletAddress = new Address("P8zHy2c8Nydkh2r6Wv6K6kacxkDcZyfaLy");
            var tokenAddress = new Address("PBWhPbobijB21xv6DY75zaRpaLCvVZWLN5");

            var addressBalanceResponse = new AddressBalanceResponseModel();
            var addressBalanceDto = new AddressBalanceDto();
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CreateRefreshAddressBalanceCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(addressBalanceDto);
            _mapperMock.Setup(callTo => callTo.Map<AddressBalanceResponseModel>(addressBalanceDto)).Returns(addressBalanceResponse);

            // Act
            var response = await _controller.RefreshAddressBalance(walletAddress, tokenAddress, CancellationToken.None);

            // Assert

            response.Result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)response.Result).Value.Should().Be(addressBalanceResponse);
        }

        [Fact]
        public async Task GetMiningPositions_GetMiningPositionsWithFilterQuery_Send()
        {
            // Arrange
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _controller.GetMiningPositions("P8zHy2c8Nydkh2r6Wv6K6kacxkDcZyfaLy", new MiningPositionFilterParameters(), cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<GetMiningPositionsWithFilterQuery>(query => query.Cursor != null), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task GetMiningPositions_Result_ReturnOk()
        {
            // Arrange
            var miningPositions = new MiningPositionsResponseModel();
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<GetMiningPositionsWithFilterQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new MiningPositionsDto());
            _mapperMock.Setup(callTo => callTo.Map<MiningPositionsResponseModel>(It.IsAny<MiningPositionsDto>())).Returns(miningPositions);

            // Act
            var response = await _controller.GetMiningPositions("P8zHy2c8Nydkh2r6Wv6K6kacxkDcZyfaLy", new MiningPositionFilterParameters(), CancellationToken.None);

            // Assert
            response.Result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)response.Result).Value.Should().Be(miningPositions);
        }

        [Fact]
        public async Task GetStakingPositions_GetStakingPositionsWithFilterQuery_Send()
        {
            // Arrange
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _controller.GetStakingPositions("P8zHy2c8Nydkh2r6Wv6K6kacxkDcZyfaLy", new StakingPositionFilterParameters(), cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<GetStakingPositionsWithFilterQuery>(query => query.Cursor != null), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task GetStakingPositions_Result_ReturnOk()
        {
            // Arrange
            var stakingPositions = new StakingPositionsResponseModel();
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<GetStakingPositionsWithFilterQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new StakingPositionsDto());
            _mapperMock.Setup(callTo => callTo.Map<StakingPositionsResponseModel>(It.IsAny<StakingPositionsDto>())).Returns(stakingPositions);

            // Act
            var response = await _controller.GetStakingPositions("P8zHy2c8Nydkh2r6Wv6K6kacxkDcZyfaLy", new StakingPositionFilterParameters(), CancellationToken.None);

            // Assert
            response.Result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)response.Result).Value.Should().Be(stakingPositions);
        }

        [Fact]
        public async Task GetStakingPositionByPool_GetStakingPositionByPoolQuery_Send()
        {
            // Arrange
            var address = "P8zHy2c8Nydkh2r6Wv6K6kacxkDcZyfaLy";
            var liquidityPool = "PBWhPbobijB21xv6DY75zaRpaLCvVZWLN5";
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _controller.GetStakingPositionByPool(address, liquidityPool, cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(
                It.Is<GetStakingPositionByPoolQuery>(query => query.Address == address && query.LiquidityPoolAddress == liquidityPool),
                cancellationToken), Times.Once);
        }

        [Fact]
        public async Task GetStakingPositionByPool_Result_ReturnOk()
        {
            // Arrange
            var stakingPosition = new StakingPositionResponseModel();

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<GetStakingPositionByPoolQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new StakingPositionDto());
            _mapperMock.Setup(callTo => callTo.Map<StakingPositionResponseModel>(It.IsAny<StakingPositionDto>())).Returns(stakingPosition);

            // Act
            var response = await _controller.GetStakingPositionByPool("P8zHy2c8Nydkh2r6Wv6K6kacxkDcZyfaLy", "PBWhPbobijB21xv6DY75zaRpaLCvVZWLN5", CancellationToken.None);

            // Act
            response.Result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)response.Result).Value.Should().Be(stakingPosition);
        }

        [Fact]
        public async Task GetMiningPositionByPool_GetMiningPositionByPoolQuery_Send()
        {
            // Arrange
            var address = "P8zHy2c8Nydkh2r6Wv6K6kacxkDcZyfaLy";
            var miningPool = "PBWhPbobijB21xv6DY75zaRpaLCvVZWLN5";
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _controller.GetMiningPositionByPool(address, miningPool, cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(
                It.Is<GetMiningPositionByPoolQuery>(query => query.Address == address && query.MiningPoolAddress == miningPool),
                cancellationToken), Times.Once);
        }

        [Fact]
        public async Task GetMiningPositionByPool_Result_ReturnOk()
        {
            // Arrange
            var miningPosition = new MiningPositionResponseModel();

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<GetMiningPositionByPoolQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new MiningPositionDto());
            _mapperMock.Setup(callTo => callTo.Map<MiningPositionResponseModel>(It.IsAny<MiningPositionDto>())).Returns(miningPosition);

            // Act
            var response = await _controller.GetMiningPositionByPool("P8zHy2c8Nydkh2r6Wv6K6kacxkDcZyfaLy", "PBWhPbobijB21xv6DY75zaRpaLCvVZWLN5", CancellationToken.None);

            // Act
            response.Result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)response.Result).Value.Should().Be(miningPosition);
        }

        [Fact]
        public async Task GetAllowance_GetAddressAllowanceQuery_Send()
        {
            // Arrange
            const string owner = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
            const string spender = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXl";
            const string token = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _controller.GetAllowance(owner, token, spender, cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<GetAddressAllowanceQuery>(
                query => query.Owner == owner
                      && query.Spender == spender
                      && query.Token == token
            ), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task GetAllowance_GetAddressAllowanceQuery_ReturnMapped()
        {
            // Arrange
            const string owner = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
            const string spender = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXl";
            const string token = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            var addressAllowanceDto = new AddressAllowanceDto { Allowance = FixedDecimal.Parse("500000"), Owner = owner, Spender = spender, Token = token };
            var approvedAllowanceResponseModel = new ApprovedAllowanceResponseModel { Allowance = FixedDecimal.Parse("500000"), Owner = owner, Spender = spender, Token = token };

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<GetAddressAllowanceQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(addressAllowanceDto);

            _mapperMock.Setup(callTo => callTo.Map<ApprovedAllowanceResponseModel>(addressAllowanceDto))
                .Returns(approvedAllowanceResponseModel);

            // Act
            var response = await _controller.GetAllowance(owner, token, spender, CancellationToken.None);

            // Assert
            response.Result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)response.Result).Value.Should().Be(approvedAllowanceResponseModel);
        }
    }
}
