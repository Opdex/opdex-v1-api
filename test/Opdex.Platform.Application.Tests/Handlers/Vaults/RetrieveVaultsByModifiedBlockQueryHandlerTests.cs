using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Application.Handlers.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Handlers.Vaults
{
    public class RetrieveVaultsByModifiedBlockQueryHandlerTests
    {
        private readonly Mock<IMediator> _mediator;
        private readonly RetrieveVaultsByModifiedBlockQueryHandler _handler;

        public RetrieveVaultsByModifiedBlockQueryHandlerTests()
        {
            _mediator = new Mock<IMediator>();
            _handler = new RetrieveVaultsByModifiedBlockQueryHandler(_mediator.Object);
        }

        [Fact]
        public void RetrieveVaultsByModifiedBlockQuery_InvalidBlockHeight_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            // Act
            void Act() => new RetrieveVaultsByModifiedBlockQuery(0);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Contains("Block height must be greater than zero.");
        }

        [Fact]
        public async Task RetrieveVaultsByModifiedBlockQuery_Sends_SelectVaultByModifiedBlockQuery()
        {
            // Arrange
            const ulong blockHeight = 10;

            // Act
            try
            {
                await _handler.Handle(new RetrieveVaultsByModifiedBlockQuery(blockHeight), CancellationToken.None);
            } catch { }

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.Is<SelectVaultsByModifiedBlockQuery>(q => q.BlockHeight == blockHeight),
                                                   It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
