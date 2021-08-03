using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.Addresses;
using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.WebApi.Controllers;
using Opdex.Platform.WebApi.Models.Responses.Wallet;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Controllers
{
    public class WalletControllerTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IMediator> _mediatorMock;

        private readonly WalletController _controller;

        public WalletControllerTests()
        {
            _mapperMock = new Mock<IMapper>();
            _mediatorMock = new Mock<IMediator>();

            _controller = new WalletController(_mapperMock.Object, _mediatorMock.Object);
        }

        [Fact]
        public async Task GetApprovedAllowanceForToken_GetAddressAllowanceForTokenQuery_Send()
        {
            // Arrange
            var owner = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
            var spender = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXl";
            var token = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _controller.GetApprovedAllowances(owner, spender, token, cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<GetAddressAllowancesApprovedByOwnerQuery>(
                query => query.Owner == owner
                      && query.Spender == spender
                      && query.Token == token
            ), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task GetApprovedAllowanceForToken_GetAddressAllowanceForTokenQuery_ReturnMapped()
        {
            // Arrange
            var addressAllowancesDto = new List<AddressAllowanceDto> { new AddressAllowanceDto() };
            var approvedAllowancesResponseModel = new List<ApprovedAllowanceResponseModel> { new ApprovedAllowanceResponseModel { Allowance = "500000" } };

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<GetAddressAllowancesApprovedByOwnerQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(addressAllowancesDto);
            _mapperMock.Setup(callTo => callTo.Map<IEnumerable<ApprovedAllowanceResponseModel>>(addressAllowancesDto)).Returns(approvedAllowancesResponseModel);

            // Act
            var response = await _controller.GetApprovedAllowances("PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXl", "", "", CancellationToken.None);

            // Assert
            response.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)response).Value.Should().BeEquivalentTo(approvedAllowancesResponseModel);
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
            var addressAllowanceDto = new AddressAllowanceDto { Allowance = "500000", Owner = owner, Spender = spender, Token = token };
            var approvedAllowanceResponseModel = new ApprovedAllowanceResponseModel { Allowance = "500000", Owner = owner, Spender = spender, Token = token };

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
