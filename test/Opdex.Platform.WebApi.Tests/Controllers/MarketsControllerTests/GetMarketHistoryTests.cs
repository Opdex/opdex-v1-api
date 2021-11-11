using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.Markets;
using Opdex.Platform.Application.Abstractions.Models.Markets;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.WebApi.Controllers;
using Opdex.Platform.WebApi.Models;
using Opdex.Platform.WebApi.Models.Requests.Markets;
using Opdex.Platform.WebApi.Models.Responses.Markets;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Controllers.MarketsControllerTests
{
    public class GetMarketHistoryTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IApplicationContext> _contextMock;
        private readonly MarketsController _controller;

        public GetMarketHistoryTests()
        {
            _mapperMock = new Mock<IMapper>();
            _mediatorMock = new Mock<IMediator>();
            _contextMock = new Mock<IApplicationContext>();
            _controller = new MarketsController(_mediatorMock.Object, _mapperMock.Object, _contextMock.Object);
        }

        [Fact]
        public async Task GetMarketHistory_GetMarketSnapshotsWithFilterQuery_Send()
        {
            // Arrange
            Address market = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u";
            var filters = new MarketSnapshotFilterParameters
            {
                StartDateTime = DateTime.UtcNow.AddDays(-5),
                EndDateTime = DateTime.UtcNow
            };

            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _controller.GetMarketHistory(market, filters, cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<GetMarketSnapshotsWithFilterQuery>(query => query.Cursor.StartTime == filters.StartDateTime
                                                                                                     && query.Cursor.EndTime == filters.EndDateTime
                                                                                                     && query.Cursor.Interval == Interval.OneDay
                                                                                                     && query.Cursor.IsFirstRequest
                                                                                                     && query.Market == market),
                                                       cancellationToken), Times.Once);
        }

        [Fact]
        public async Task GetMarketHistory_MarketDto_Map()
        {
            // Arrange
            Address market = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u";
            var filters = new MarketSnapshotFilterParameters
            {
                StartDateTime = DateTime.UtcNow.AddDays(-5),
                EndDateTime = DateTime.UtcNow
            };

            var dto = new MarketSnapshotsDto();

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<GetMarketSnapshotsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(dto);

            // Act
            await _controller.GetMarketHistory(market, filters, CancellationToken.None);

            // Assert
            _mapperMock.Verify(callTo => callTo.Map<MarketSnapshotsResponseModel>(It.IsAny<MarketSnapshotsDto>()), Times.Once);
        }

        [Fact]
        public async Task GetMarketHistory_ReturnOk()
        {
            // Arrange
            Address market = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u";
            var filters = new MarketSnapshotFilterParameters
            {
                StartDateTime = DateTime.UtcNow.AddDays(-5),
                EndDateTime = DateTime.UtcNow
            };

            var dto = new MarketSnapshotsDto();
            var marketResponse = new MarketSnapshotsResponseModel();

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<GetMarketSnapshotsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(dto);
            _mapperMock.Setup(callTo => callTo.Map<MarketSnapshotsResponseModel>(It.IsAny<MarketSnapshotsDto>())).Returns(marketResponse);

            // Act
            var response = await _controller.GetMarketHistory(market, filters, CancellationToken.None);

            // Act
            response.Result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)response.Result).Value.Should().Be(marketResponse);
        }
    }
}
