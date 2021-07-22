using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Queries.Transactions;
using Opdex.Platform.Application.Handlers.Transactions;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Transactions;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Handlers.Transactions
{
    public class RetrieveTransactionByHashQueryHandlerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly RetrieveTransactionByHashQueryHandler _handler;

        public RetrieveTransactionByHashQueryHandlerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _handler = new RetrieveTransactionByHashQueryHandler(_mediatorMock.Object);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void RetrieveTransaction_ThrowsArgumentNullException_InvalidHash(string hash)
        {
            // Arrange

            // Act
            void Act() => new RetrieveTransactionByHashQuery(hash);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Hash must be provided.");
        }

        [Fact]
        public async Task RetrieveTransaction_Sends_SelectTransactionByHashQuery()
        {
            // Arrange
            var request = new RetrieveTransactionByHashQuery("4d03447aba5e25c5c647a18a175cd36d0c23bcc2c8cb9d0288c119da06f3b2fd");

            // Act
            try
            {
                await _handler.Handle(request, It.IsAny<CancellationToken>());
            } catch { }

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<SelectTransactionByHashQuery>(q => q.Hash == request.Hash),
                                                       It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RetrieveTransaction_Returns()
        {
            // Arrange
            var request = new RetrieveTransactionByHashQuery("4d03447aba5e25c5c647a18a175cd36d0c23bcc2c8cb9d0288c119da06f3b2fd");

            var transaction = new Transaction(1, "4d03447aba5e25c5c647a18a175cd36d0c23bcc2c8cb9d0288c119da06f3b2fd", 2, 3,
                                              "PAXxsvwuwGrJg1XnNCA8Swtpynzj2G8Eca", "PTbuFnGyvo93toQyFdmU1J5WGYBWGUnML3",
                                              true, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj");

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<SelectTransactionByHashQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(transaction);

            // Act
            var response = await _handler.Handle(request, It.IsAny<CancellationToken>());

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<SelectTransactionByHashQuery>(q => q.Hash == request.Hash),
                                                       It.IsAny<CancellationToken>()), Times.Once);

            response.Should().Be(transaction);
        }
    }
}
