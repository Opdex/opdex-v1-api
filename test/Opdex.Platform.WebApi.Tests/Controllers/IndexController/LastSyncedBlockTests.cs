using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Enums;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Controllers.IndexController
{
    public class LastSyncedBlockTests
    {
        private readonly Mock<IMediator> _mediator;
        private readonly WebApi.Controllers.IndexController _controller;

        public LastSyncedBlockTests()
        {
            _mediator = new Mock<IMediator>();
            var opdexConfiguration = new OpdexConfiguration {Network = NetworkType.DEVNET};

            _controller = new WebApi.Controllers.IndexController(_mediator.Object, opdexConfiguration);
        }

        [Fact]
        public async Task GetLatestBlock_Send_RetrieveLatestBlockQuery()
        {
            // Arrange
            var token = new CancellationTokenSource().Token;

            // Act
            await _controller.GetLastSyncedBlock(token);

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.IsAny<RetrieveLatestBlockQuery>(), token), Times.Once);
        }
    }
}
