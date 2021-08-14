using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.Addresses;
using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.WebApi.Controllers;
using Opdex.Platform.WebApi.Models.Responses;
using Opdex.Platform.WebApi.Models.Responses.Wallet;
using System.Linq;
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
        public async Task GetMiningPositions_CursorNotProvidedNullIncludeZeroAmounts_IncludeZeroAmountsFalse()
        {
            // Arrange
            // Act
            await _controller.GetMiningPositions("P8zHy2c8Nydkh2r6Wv6K6kacxkDcZyfaLy", Enumerable.Empty<string>(), Enumerable.Empty<string>(), null, SortDirectionType.DESC, 10, null, CancellationToken.None);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<GetMiningPositionsWithFilterQuery>(query => query.Cursor.IncludeZeroAmounts == false), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetMiningPositions_CursorProvidedNotBase64_Return422ValidationError()
        {
            // Arrange
            // Act
            var response = await _controller.GetMiningPositions(default, default, default, default, default, default, "NOT_BASE_64_****", CancellationToken.None);

            // Assert
            response.Result.Should().BeOfType<ValidationErrorProblemDetailsResult>();
        }

        [Fact]
        public async Task GetMiningPositions_CursorProvidedNotValidCursor_Return422ValidationError()
        {
            // Arrange
            // Act
            var response = await _controller.GetMiningPositions(default, default, default, default, default, default, "Tk9UX1ZBTElE", CancellationToken.None);

            // Assert
            response.Result.Should().BeOfType<ValidationErrorProblemDetailsResult>();
        }

        [Fact]
        public async Task GetMiningPositions_GetMiningPositionsWithFilterQuery_Send()
        {
            // Arrange
            var liquidityPools = Enumerable.Empty<string>();
            var miningPools = new string[] { "PB5gG1wRsTpST5mFLkgxwauamhW28i1LRe", "PXRNXAEYkCjMJpqdgdRG4FzbguG4GcdZuN" };
            var includeZeroBalances = true;
            var sortDirection = SortDirectionType.ASC;
            var limit = 10U;
            var address = "P8zHy2c8Nydkh2r6Wv6K6kacxkDcZyfaLy";
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _controller.GetMiningPositions(address, liquidityPools, miningPools, includeZeroBalances, sortDirection, limit, null, cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<GetMiningPositionsWithFilterQuery>(query => query.Address == address
                                                                                                      && query.Cursor.IsFirstRequest
                                                                                                      && query.Cursor.LiquidityPools.SequenceEqual(liquidityPools)
                                                                                                      && query.Cursor.MiningPools.SequenceEqual(miningPools)
                                                                                                      && query.Cursor.IncludeZeroAmounts == includeZeroBalances
                                                                                                      && query.Cursor.SortDirection == sortDirection
                                                                                                      && query.Cursor.Limit == limit), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task GetMiningPositions_Result_ReturnOk()
        {
            // Arrange
            var miningPositions = new MiningPositionsResponseModel();

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<GetMiningPositionsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new MiningPositionsDto());
            _mapperMock.Setup(callTo => callTo.Map<MiningPositionsResponseModel>(It.IsAny<MiningPositionsDto>())).Returns(miningPositions);

            // Act
            var response = await _controller.GetMiningPositions("P8zHy2c8Nydkh2r6Wv6K6kacxkDcZyfaLy", Enumerable.Empty<string>(), Enumerable.Empty<string>(), false, SortDirectionType.ASC, 10, null, CancellationToken.None);

            // Assert
            response.Result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)response.Result).Value.Should().Be(miningPositions);
        }

        [Fact]
        public async Task GetStakingPositions_CursorNotProvidedNullIncludeZeroAmounts_IncludeZeroAmountsFalse()
        {
            // Arrange
            // Act
            await _controller.GetStakingPositions("P8zHy2c8Nydkh2r6Wv6K6kacxkDcZyfaLy", Enumerable.Empty<string>(), null, SortDirectionType.DESC, 10, null, CancellationToken.None);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<GetStakingPositionsWithFilterQuery>(query => query.Cursor.IncludeZeroAmounts == false), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetStakingPositions_CursorProvidedNotBase64_Return422ValidationError()
        {
            // Arrange
            // Act
            var response = await _controller.GetStakingPositions(default, default, default, default, default, "NOT_BASE_64_****", CancellationToken.None);

            // Assert
            response.Result.Should().BeOfType<ValidationErrorProblemDetailsResult>();
        }

        [Fact]
        public async Task GetStakingPositions_CursorProvidedNotValidCursor_Return422ValidationError()
        {
            // Arrange
            // Act
            var response = await _controller.GetStakingPositions(default, default, default, default, default, "Tk9UX1ZBTElE", CancellationToken.None);

            // Assert
            response.Result.Should().BeOfType<ValidationErrorProblemDetailsResult>();
        }

        [Fact]
        public async Task GetStakingPositions_GetStakingPositionsWithFilterQuery_Send()
        {
            // Arrange
            var liquidityPools = Enumerable.Empty<string>();
            var includeZeroBalances = true;
            var sortDirection = SortDirectionType.ASC;
            var limit = 10U;
            var address = "P8zHy2c8Nydkh2r6Wv6K6kacxkDcZyfaLy";
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _controller.GetStakingPositions(address, liquidityPools, includeZeroBalances, sortDirection, limit, null, cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<GetStakingPositionsWithFilterQuery>(query => query.Address == address
                                                                                                       && query.Cursor.IsFirstRequest
                                                                                                       && query.Cursor.LiquidityPools.SequenceEqual(liquidityPools)
                                                                                                       && query.Cursor.IncludeZeroAmounts == includeZeroBalances
                                                                                                       && query.Cursor.SortDirection == sortDirection
                                                                                                       && query.Cursor.Limit == limit), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task GetStakingPositions_Result_ReturnOk()
        {
            // Arrange
            var stakingPositions = new StakingPositionsResponseModel();

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<GetStakingPositionsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new StakingPositionsDto());
            _mapperMock.Setup(callTo => callTo.Map<StakingPositionsResponseModel>(It.IsAny<StakingPositionsDto>())).Returns(stakingPositions);

            // Act
            var response = await _controller.GetStakingPositions("P8zHy2c8Nydkh2r6Wv6K6kacxkDcZyfaLy", Enumerable.Empty<string>(), false, SortDirectionType.ASC, 10, null, CancellationToken.None);

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
