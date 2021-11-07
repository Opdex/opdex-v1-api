using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.MarketTokens;
using Opdex.Platform.Application.Abstractions.Models.Tokens;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.WebApi.Controllers;
using Opdex.Platform.WebApi.Models;
using Opdex.Platform.WebApi.Models.Requests;
using Opdex.Platform.WebApi.Models.Responses.Tokens;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Controllers.MarketTokensControllerTests
{
    public class GetTokenHistoryTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IApplicationContext> _contextMock;
        private readonly MarketTokensController _controller;

        public GetTokenHistoryTests()
        {
            _mapperMock = new Mock<IMapper>();
            _mediatorMock = new Mock<IMediator>();
            _contextMock = new Mock<IApplicationContext>();

            _controller = new MarketTokensController(_mediatorMock.Object, _mapperMock.Object, _contextMock.Object);
        }

        [Fact]
        public async Task GetMarketToken_GetMarketTokenSnapshotsWithFilterQuery_Send()
        {
            // Arrange
            Address market = "fJ6u1yzNPDw7uPGZPZpB4iW4LHVEPMKehX";
            Address token = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u";
            var filters = new SnapshotFilterParameters
            {
                StartTime = DateTime.UtcNow.AddDays(-5),
                EndTime = DateTime.UtcNow,
                Interval = Interval.OneDay
            };

            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _controller.GetMarketTokenHistory(market, token, filters, cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<GetMarketTokenSnapshotsWithFilterQuery>(query => query.Cursor.StartTime == filters.StartTime
                                                                                                     && query.Cursor.EndTime == filters.EndTime
                                                                                                     && query.Cursor.Interval == filters.Interval
                                                                                                     && query.Cursor.IsFirstRequest
                                                                                                     && query.Token == token),
                                                       cancellationToken), Times.Once);
        }

        [Fact]
        public async Task GetMarketToken_MarketTokenDto_Map()
        {
            // Arrange
            Address market = "fJ6u1yzNPDw7uPGZPZpB4iW4LHVEPMKehX";
            Address token = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u";
            var filters = new SnapshotFilterParameters
            {
                StartTime = DateTime.UtcNow.AddDays(-5),
                EndTime = DateTime.UtcNow,
                Interval = Interval.OneDay
            };

            var dto = new TokenSnapshotsDto();

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<GetMarketTokenSnapshotsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(dto);

            // Act
            await _controller.GetMarketTokenHistory(market, token, filters, CancellationToken.None);

            // Assert
            _mapperMock.Verify(callTo => callTo.Map<TokenSnapshotsResponseModel>(It.IsAny<TokenSnapshotsDto>()), Times.Once);
        }

        [Fact]
        public async Task GetMarketToken_ReturnOk()
        {
            // Arrange
            Address market = "fJ6u1yzNPDw7uPGZPZpB4iW4LHVEPMKehX";
            Address token = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u";
            var filters = new SnapshotFilterParameters
            {
                StartTime = DateTime.UtcNow.AddDays(-5),
                EndTime = DateTime.UtcNow,
                Interval = Interval.OneDay
            };

            var dto = new TokenSnapshotsDto();
            var tokenResponse = new TokenSnapshotsResponseModel();

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<GetMarketTokenSnapshotsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(dto);
            _mapperMock.Setup(callTo => callTo.Map<TokenSnapshotsResponseModel>(It.IsAny<TokenSnapshotsDto>())).Returns(tokenResponse);

            // Act
            var response = await _controller.GetMarketTokenHistory(market, token, filters, CancellationToken.None);

            // Act
            response.Result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)response.Result).Value.Should().Be(tokenResponse);
        }
    }
}
