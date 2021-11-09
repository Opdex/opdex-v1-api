using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.Tokens.Snapshots;
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

namespace Opdex.Platform.WebApi.Tests.Controllers.TokensControllerTests
{
    public class GetTokenHistoryTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IApplicationContext> _contextMock;
        private readonly TokensController _controller;

        public GetTokenHistoryTests()
        {
            _mapperMock = new Mock<IMapper>();
            _mediatorMock = new Mock<IMediator>();
            _contextMock = new Mock<IApplicationContext>();

            _controller = new TokensController(_mediatorMock.Object, _mapperMock.Object, _contextMock.Object);
        }

        [Fact]
        public async Task GetToken_GetTokenSnapshotsWithFilterQuery_Send()
        {
            // Arrange
            Address token = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u";
            var filters = new SnapshotFilterParameters
            {
                StartDateTime = DateTime.UtcNow.AddDays(-5),
                EndDateTime = DateTime.UtcNow,
                Interval = Interval.OneDay
            };

            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _controller.GetTokenHistory(token, filters, cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<GetTokenSnapshotsWithFilterQuery>(query => query.Cursor.StartTime == filters.StartDateTime
                                                                                                     && query.Cursor.EndTime == filters.EndDateTime
                                                                                                     && query.Cursor.Interval == filters.Interval
                                                                                                     && query.Cursor.IsFirstRequest
                                                                                                     && query.Token == token),
                                                       cancellationToken), Times.Once);
        }

        [Fact]
        public async Task GetToken_TokenDto_Map()
        {
            // Arrange
            Address token = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u";
            var filters = new SnapshotFilterParameters
            {
                StartDateTime = DateTime.UtcNow.AddDays(-5),
                EndDateTime = DateTime.UtcNow,
                Interval = Interval.OneDay
            };

            var dto = new TokenSnapshotsDto();

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<GetTokenSnapshotsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(dto);

            // Act
            await _controller.GetTokenHistory(token, filters, CancellationToken.None);

            // Assert
            _mapperMock.Verify(callTo => callTo.Map<TokenSnapshotsResponseModel>(It.IsAny<TokenSnapshotsDto>()), Times.Once);
        }

        [Fact]
        public async Task GetToken_ReturnOk()
        {
            // Arrange
            Address token = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u";
            var filters = new SnapshotFilterParameters
            {
                StartDateTime = DateTime.UtcNow.AddDays(-5),
                EndDateTime = DateTime.UtcNow,
                Interval = Interval.OneDay
            };

            var dto = new TokenSnapshotsDto();
            var tokenResponse = new TokenSnapshotsResponseModel();

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<GetTokenSnapshotsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(dto);
            _mapperMock.Setup(callTo => callTo.Map<TokenSnapshotsResponseModel>(It.IsAny<TokenSnapshotsDto>())).Returns(tokenResponse);

            // Act
            var response = await _controller.GetTokenHistory(token, filters, CancellationToken.None);

            // Act
            response.Result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)response.Result).Value.Should().Be(tokenResponse);
        }
    }
}
