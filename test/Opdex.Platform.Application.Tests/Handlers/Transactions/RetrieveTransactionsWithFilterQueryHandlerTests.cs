using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents;
using Opdex.Platform.Application.Abstractions.Queries.Transactions;
using Opdex.Platform.Application.Handlers.Transactions;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Handlers.Transactions
{
    public class RetrieveTransactionsWithFilterQueryHandlerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly RetrieveTransactionsWithFilterQueryHandler _handler;

        public RetrieveTransactionsWithFilterQueryHandlerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _handler = new RetrieveTransactionsWithFilterQueryHandler(_mediatorMock.Object);
        }

        [Fact]
        public void RetrieveTransactionsWithFilter_ThrowsArgumentOutOfRangeException_InvalidSortDirection()
        {
            // Arrange

            // Act
            void Act() => new RetrieveTransactionsWithFilterQuery(null, new TransactionEventType[0], new string[0], (SortDirectionType)3, 10, 0, 0);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Invalid sort direction");
        }

        [Fact]
        public void RetrieveTransactionsWithFilter_ThrowsArgumentOutOfRangeException_InvalidLimit()
        {
            // Arrange

            // Act
            void Act() => new RetrieveTransactionsWithFilterQuery(null, new TransactionEventType[0], new string[0], SortDirectionType.ASC, 0, 0, 0);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Invalid limit");
        }

        [Fact]
        public async Task RetrieveTransactionsWithFilter_Sends_SelectTransactionsWithFilterQuery()
        {
            // Arrange
            const string wallet = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            var eventTypes = new List<TransactionEventType> {TransactionEventType.ApprovalEvent};
            var contracts = new List<string> {"PAXxsvwuwGrJg1XnNCA8Swtpynzj2G8Eca"};
            const SortDirectionType direction = SortDirectionType.ASC;
            const uint limit = 10;

            var request = new RetrieveTransactionsWithFilterQuery(wallet, eventTypes, contracts, direction, limit, 0, 0);

            // Act
            await _handler.Handle(request, It.IsAny<CancellationToken>());

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<SelectTransactionsWithFilterQuery>(q =>
                                                                                                    q.Wallet == wallet &&
                                                                                                    q.LogTypes.Any() &&
                                                                                                    q.Contracts == contracts &&
                                                                                                    q.Direction == direction &&
                                                                                                    q.Limit == limit),
                                                       It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
