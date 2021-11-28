using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.LiquidityPools.Snapshots;
using Opdex.Platform.Application.Abstractions.Models.LiquidityPools.Snapshots;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.WebApi.Controllers;
using Opdex.Platform.WebApi.Models;
using Opdex.Platform.WebApi.Models.Requests;
using Opdex.Platform.WebApi.Models.Responses.LiquidityPools.Snapshots;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Controllers.LiquidityPoolsControllerTests
{
    public class GetLiquidityPoolHistoryTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IApplicationContext> _contextMock;
        private readonly LiquidityPoolsController _controller;

        public GetLiquidityPoolHistoryTests()
        {
            _mapperMock = new Mock<IMapper>();
            _mediatorMock = new Mock<IMediator>();
            _contextMock = new Mock<IApplicationContext>();
            _controller = new LiquidityPoolsController(_mediatorMock.Object, _mapperMock.Object, _contextMock.Object);
        }

        [Fact]
        public async Task GetLiquidityPoolHistory_GetLiquidityPoolSnapshotsWithFilterQuery_Send()
        {
            // Arrange
            Address liquidityPool = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u";
            var filters = new SnapshotFilterParameters
            {
                StartDateTime = DateTime.UtcNow.AddDays(-5),
                EndDateTime = DateTime.UtcNow,
                Interval = Interval.OneDay
            };

            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _controller.GetLiquidityPoolHistory(liquidityPool, filters, cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<GetLiquidityPoolSnapshotsWithFilterQuery>(query => query.Cursor.StartTime == filters.StartDateTime
                                                                                                     && query.Cursor.EndTime == filters.EndDateTime
                                                                                                     && query.Cursor.Interval == filters.Interval
                                                                                                     && query.Cursor.IsFirstRequest
                                                                                                     && query.LiquidityPool == liquidityPool),
                                                       cancellationToken), Times.Once);
        }

        [Fact]
        public async Task GetLiquidityPoolHistory_LiquidityPoolDto_Map()
        {
            // Arrange
            Address liquidityPool = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u";
            var filters = new SnapshotFilterParameters
            {
                StartDateTime = DateTime.UtcNow.AddDays(-5),
                EndDateTime = DateTime.UtcNow,
                Interval = Interval.OneDay
            };

            var dto = new LiquidityPoolSnapshotsDto();

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<GetLiquidityPoolSnapshotsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(dto);

            // Act
            await _controller.GetLiquidityPoolHistory(liquidityPool, filters, CancellationToken.None);

            // Assert
            _mapperMock.Verify(callTo => callTo.Map<LiquidityPoolSnapshotsResponseModel>(It.IsAny<LiquidityPoolSnapshotsDto>()), Times.Once);
        }

        [Fact]
        public async Task GetLiquidityPoolHistory_ReturnOk()
        {
            // Arrange
            Address liquidityPool = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u";
            var filters = new SnapshotFilterParameters
            {
                StartDateTime = DateTime.UtcNow.AddDays(-5),
                EndDateTime = DateTime.UtcNow,
                Interval = Interval.OneDay
            };

            var dto = new LiquidityPoolSnapshotsDto();
            var liquidityPoolResponse = new LiquidityPoolSnapshotsResponseModel();

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<GetLiquidityPoolSnapshotsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(dto);
            _mapperMock.Setup(callTo => callTo.Map<LiquidityPoolSnapshotsResponseModel>(It.IsAny<LiquidityPoolSnapshotsDto>())).Returns(liquidityPoolResponse);

            // Act
            var response = await _controller.GetLiquidityPoolHistory(liquidityPool, filters, CancellationToken.None);

            // Act
            response.Result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)response.Result).Value.Should().Be(liquidityPoolResponse);
        }
    }
}
