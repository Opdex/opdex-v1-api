using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.Governances;
using Opdex.Platform.Application.Abstractions.Models.Governances;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.WebApi.Controllers;
using Opdex.Platform.WebApi.Models;
using Opdex.Platform.WebApi.Models.Responses;
using Opdex.Platform.WebApi.Models.Responses.Governances;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Controllers
{
    public class GovernancesControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IApplicationContext> _applicationContextMock;

        private readonly GovernancesController _controller;

        public GovernancesControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _mapperMock = new Mock<IMapper>();
            _applicationContextMock = new Mock<IApplicationContext>();

            _controller = new GovernancesController(_mediatorMock.Object, _mapperMock.Object, _applicationContextMock.Object, new OpdexConfiguration());
        }

        [Fact]
        public async Task GetGovernances_CursorProvidedNotBase64_Return422ValidationError()
        {
            // Arrange
            // Act
            var response = await _controller.GetGovernances(default, default, default, "NOT_BASE_64_****", CancellationToken.None);

            // Assert
            response.Result.Should().BeOfType<ValidationErrorProblemDetailsResult>();
        }

        [Fact]
        public async Task GetGovernances_CursorProvidedNotValidCursor_Return422ValidationError()
        {
            // Arrange
            // Act
            var response = await _controller.GetGovernances(default, default, default, "Tk9UX1ZBTElE", CancellationToken.None);

            // Assert
            response.Result.Should().BeOfType<ValidationErrorProblemDetailsResult>();
        }

        [Fact]
        public async Task GetGovernances_GetMiningGovernancesWithFilterQuery_Send()
        {
            // Arrange
            var sortDirection = SortDirectionType.ASC;
            var limit = 10U;
            var minedToken = "P8zHy2c8Nydkh2r6Wv6K6kacxkDcZyfaLy";
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _controller.GetGovernances(minedToken, sortDirection, limit, null, cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<GetMiningGovernancesWithFilterQuery>(query => query.Cursor.MinedToken == minedToken
                                                                                                  && query.Cursor.IsFirstRequest
                                                                                                  && query.Cursor.SortDirection == sortDirection
                                                                                                  && query.Cursor.Limit == limit), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task GetGovernances_Result_ReturnOk()
        {
            // Arrange
            var stakingPositions = new MiningGovernancesResponseModel();

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<GetMiningGovernancesWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new MiningGovernancesDto());
            _mapperMock.Setup(callTo => callTo.Map<MiningGovernancesResponseModel>(It.IsAny<MiningGovernancesDto>())).Returns(stakingPositions);

            // Act
            var response = await _controller.GetGovernances("P8zHy2c8Nydkh2r6Wv6K6kacxkDcZyfaLy", SortDirectionType.ASC, 10, null, CancellationToken.None);

            // Assert
            response.Result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)response.Result).Value.Should().Be(stakingPositions);
        }
    }
}
