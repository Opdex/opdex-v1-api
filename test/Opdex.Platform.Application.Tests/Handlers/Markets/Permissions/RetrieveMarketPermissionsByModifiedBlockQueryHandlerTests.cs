using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Queries.Markets.Permissions;
using Opdex.Platform.Application.Handlers.Markets.Permissions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets.Permissions;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Handlers.Markets.Permissions
{
    public class RetrieveMarketPermissionsByModifiedBlockQueryHandlerTests
    {
        private readonly Mock<IMediator> _mediator;
        private readonly RetrieveMarketPermissionsByModifiedBlockQueryHandler _handler;

        public RetrieveMarketPermissionsByModifiedBlockQueryHandlerTests()
        {
            _mediator = new Mock<IMediator>();
            _handler = new RetrieveMarketPermissionsByModifiedBlockQueryHandler(_mediator.Object);
        }

        [Fact]
        public void RetrieveMarketPermissionsByModifiedBlockQuery_InvalidBlockHeight_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            // Act
            void Act() => new RetrieveMarketPermissionsByModifiedBlockQuery(0);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Contains("Block height must be greater than zero.");
        }

        [Fact]
        public async Task RetrieveMarketPermissionsByModifiedBlockQuery_Sends_SelectMarketPermissionByModifiedBlockQuery()
        {
            // Arrange
            const ulong blockHeight = 10;

            // Act
            try
            {
                await _handler.Handle(new RetrieveMarketPermissionsByModifiedBlockQuery(blockHeight), CancellationToken.None);
            } catch { }

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.Is<SelectMarketPermissionsByModifiedBlockQuery>(q => q.BlockHeight == blockHeight),
                                                   It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
