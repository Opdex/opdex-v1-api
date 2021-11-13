using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.MiningPools;
using Opdex.Platform.Application.Abstractions.Models.MiningPools;
using Opdex.Platform.WebApi.Models;
using Opdex.Platform.WebApi.Models.Requests.MiningPools;
using Opdex.Platform.WebApi.Models.Responses.MiningPools;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Controllers.MiningPoolsControllerTests
{
    public class GetMiningPoolsTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IApplicationContext> _contextMock;
        private readonly WebApi.Controllers.MiningPoolsController _controller;

        public GetMiningPoolsTests()
        {
            _mapperMock = new Mock<IMapper>();
            _mediatorMock = new Mock<IMediator>();
            _contextMock = new Mock<IApplicationContext>();

            _controller = new WebApi.Controllers.MiningPoolsController(_mapperMock.Object, _mediatorMock.Object, _contextMock.Object);
        }

        [Fact]
        public async Task GetMiningPools_GetMiningPoolsWithFilterQuery_Send()
        {
            // Arrange
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _controller.GetMiningPools(new MiningPoolFilterParameters(), cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<GetMiningPoolsWithFilterQuery>(query => query.Cursor != null), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task GetMiningPools_Result_ReturnOk()
        {
            // Arrange
            var miningPools = new MiningPoolsResponseModel();

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<GetMiningPoolsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new MiningPoolsDto());
            _mapperMock.Setup(callTo => callTo.Map<MiningPoolsResponseModel>(It.IsAny<MiningPoolsDto>())).Returns(miningPools);

            // Act
            var response = await _controller.GetMiningPools(new MiningPoolFilterParameters(), CancellationToken.None);

            // Assert
            response.Result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)response.Result).Value.Should().Be(miningPools);
        }
    }
}
