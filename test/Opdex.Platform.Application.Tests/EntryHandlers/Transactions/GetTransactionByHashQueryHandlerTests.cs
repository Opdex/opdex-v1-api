using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.Transactions;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Application.Abstractions.Queries.Transactions;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Application.EntryHandlers.Transactions;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Domain.Models.Transactions;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Transactions
{
    public class GetTransactionByHashQueryHandlerTests
    {
        private readonly Mock<IModelAssembler<Transaction, TransactionDto>> _transactionDtoAssembler;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly GetTransactionByHashQueryHandler _handler;

        public GetTransactionByHashQueryHandlerTests()
        {
            _transactionDtoAssembler = new Mock<IModelAssembler<Transaction, TransactionDto>>();
            _mediatorMock = new Mock<IMediator>();

            _handler = new GetTransactionByHashQueryHandler(_mediatorMock.Object, _transactionDtoAssembler.Object);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void GetTransaction_ThrowsArgumentNullException_InvalidHash(string hash)
        {
            // Arrange

            // Act
            void Act() => new GetTransactionByHashQuery(hash);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Hash must be provided.");
        }

        [Fact]
        public async Task GetTransaction_Sends_RetrieveTransactionByHashQuery()
        {
            // Arrange
            var request = new GetTransactionByHashQuery("hash");

            // Act
            try
            {
                await _handler.Handle(request, It.IsAny<CancellationToken>());
            }
            catch { }

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveTransactionByHashQuery>(q => q.Hash == request.Hash),
                                                       It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetTransaction_Sends_AssembleTransaction()
        {
            // Arrange
            var request = new GetTransactionByHashQuery("hash");
            var transaction = new Transaction(1, "txHash", 2, 3, "PFrSHgtz2khDuciJdLAZtR2uKwgyXryMjM", "PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh", true, "PNvzq4pxJ5v3pp9kDaZyifKNspGD79E4qM");

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTransactionByHashQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(transaction);

            // Act
            try
            {
                await _handler.Handle(request, It.IsAny<CancellationToken>());
            }
            catch { }

            // Assert
            _transactionDtoAssembler.Verify(callTo => callTo.Assemble(transaction), Times.Once);
        }
    }
}
