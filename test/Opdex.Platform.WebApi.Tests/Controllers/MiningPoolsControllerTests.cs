using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.MiningPools;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.Models.MiningPools;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.MiningPools;
using Opdex.Platform.WebApi.Controllers;
using Opdex.Platform.WebApi.Models;
using Opdex.Platform.WebApi.Models.Responses;
using Opdex.Platform.WebApi.Models.Responses.Pools;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Controllers
{
    public class MiningPoolsControllerTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IApplicationContext> _contextMock;
        private readonly MiningPoolsController _controller;

        public MiningPoolsControllerTests()
        {
            _mapperMock = new Mock<IMapper>();
            _mediatorMock = new Mock<IMediator>();
            _contextMock = new Mock<IApplicationContext>();

            _controller = new MiningPoolsController(_mapperMock.Object, _mediatorMock.Object, _contextMock.Object);
        }

        [Fact]
        public async Task GetMiningPools_CursorProvidedNotBase64_Return422ValidationError()
        {
            // Arrange
            // Act
            var response = await _controller.GetMiningPools(default, default, default, default, "NOT_BASE_64_****", CancellationToken.None);

            // Assert
            response.Result.Should().BeOfType<ValidationErrorProblemDetailsResult>();
        }

        [Fact]
        public async Task GetMiningPools_CursorProvidedNotValidCursor_Return422ValidationError()
        {
            // Arrange
            // Act
            var response = await _controller.GetMiningPools(default, default, default, default, "Tk9UX1ZBTElE", CancellationToken.None);

            // Assert
            response.Result.Should().BeOfType<ValidationErrorProblemDetailsResult>();
        }

        [Fact]
        public async Task GetMiningPools_GetMiningPoolsWithFilterQuery_Send()
        {
            // Arrange
            var sortDirection = SortDirectionType.ASC;
            var limit = 10U;
            var liquidityPools = new Address[] { "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", "P9zt2HJGqPZRWDsDmeYVSGD4DHYM5NPwtm" };
            var miningStatus = MiningStatusFilter.Inactive;
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _controller.GetMiningPools(liquidityPools, miningStatus, sortDirection, limit, null, cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<GetMiningPoolsWithFilterQuery>(query => query.Cursor.IsFirstRequest
                                                                                                  && query.Cursor.LiquidityPools.SequenceEqual(liquidityPools)
                                                                                                  && query.Cursor.MiningStatus == miningStatus
                                                                                                  && query.Cursor.SortDirection == sortDirection
                                                                                                  && query.Cursor.Limit == limit), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task GetMiningPools_Result_ReturnOk()
        {
            // Arrange
            var vaults = new MiningPoolsResponseModel();

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<GetMiningPoolsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new MiningPoolsDto());
            _mapperMock.Setup(callTo => callTo.Map<MiningPoolsResponseModel>(It.IsAny<MiningPoolsDto>())).Returns(vaults);

            // Act
            var response = await _controller.GetMiningPools(Enumerable.Empty<Address>(), default, SortDirectionType.ASC, 10, null, CancellationToken.None);

            // Assert
            response.Result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)response.Result).Value.Should().Be(vaults);
        }

        [Fact]
        public async Task GetMiningPool_GetMiningPoolByAddressQuery_Send()
        {
            // Arrange
            var miningPool = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u";
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _controller.GetMiningPool(miningPool, cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<GetMiningPoolByAddressQuery>(query => query.MiningPool == miningPool), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task GetMiningPool_GetMiningPoolByAddressQueryResponse_Map()
        {
            // Arrange
            var dto = new MiningPoolDto();

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<GetMiningPoolByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(dto);

            // Act
            await _controller.GetMiningPool("PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", CancellationToken.None);

            // Assert
            _mapperMock.Verify(callTo => callTo.Map<MiningPoolResponseModel>(dto), Times.Once);
        }

        [Fact]
        public async Task GetMiningPool_GetMiningPoolByAddressQueryResponse_ReturnOk()
        {
            // Arrange
            var miningPoolDetails = new MiningPoolResponseModel();

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<GetMiningPoolByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new MiningPoolDto());
            _mapperMock.Setup(callTo => callTo.Map<MiningPoolResponseModel>(It.IsAny<MiningPoolDto>())).Returns(miningPoolDetails);

            // Act
            var response = await _controller.GetMiningPool("PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", CancellationToken.None);

            // Act
            response.Result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)response.Result).Value.Should().Be(miningPoolDetails);
        }
    }
}
