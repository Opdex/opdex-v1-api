using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.Governances;
using Opdex.Platform.Application.Abstractions.Models.Governances;
using Opdex.Platform.WebApi.Controllers;
using Opdex.Platform.WebApi.Models;
using Opdex.Platform.WebApi.Models.Requests.Governances;
using Opdex.Platform.WebApi.Models.Responses.Governances;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Controllers.GovernancesControllerTests
{
    public class GetGovernancesTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IApplicationContext> _applicationContextMock;

        private readonly GovernancesController _controller;

        public GetGovernancesTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _mapperMock = new Mock<IMapper>();
            _applicationContextMock = new Mock<IApplicationContext>();

            _controller = new GovernancesController(_mediatorMock.Object, _mapperMock.Object, _applicationContextMock.Object);
        }

        [Fact]
        public async Task GetGovernances_GetMiningGovernancesWithFilterQuery_Send()
        {
            // Arrange
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _controller.GetGovernances(new GovernanceFilterParameters(), cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<GetMiningGovernancesWithFilterQuery>(query => query.Cursor != null), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task GetGovernances_Result_ReturnOk()
        {
            // Arrange
            var governances = new MiningGovernancesResponseModel();

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<GetMiningGovernancesWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new MiningGovernancesDto());
            _mapperMock.Setup(callTo => callTo.Map<MiningGovernancesResponseModel>(It.IsAny<MiningGovernancesDto>())).Returns(governances);

            // Act
            var response = await _controller.GetGovernances(new GovernanceFilterParameters(), CancellationToken.None);

            // Assert
            response.Result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)response.Result).Value.Should().Be(governances);
        }
    }
}
