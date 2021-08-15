using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.MiningPools;
using Opdex.Platform.Application.Abstractions.Models.MiningPools;
using Opdex.Platform.WebApi.Models;
using Opdex.Platform.WebApi.Models.Responses.Pools;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Controllers.MiningPoolsController
{
    public class GetMiningPoolTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IApplicationContext> _contextMock;
        private readonly WebApi.Controllers.MiningPoolsController _controller;

        public GetMiningPoolTests()
        {
            _mapperMock = new Mock<IMapper>();
            _mediatorMock = new Mock<IMediator>();
            _contextMock = new Mock<IApplicationContext>();

            _controller = new WebApi.Controllers.MiningPoolsController(_mapperMock.Object, _mediatorMock.Object, _contextMock.Object);
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
