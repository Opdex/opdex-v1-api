using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.Transactions;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents;
using Opdex.Platform.Application.Abstractions.Queries.Transactions;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Application.EntryHandlers.Transactions;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Transactions
{
    public class GetTransactionsWithFilterQueryHandlerTests
    {
        private readonly Mock<IModelAssembler<Transaction, TransactionDto>> _transactionDtoAssembler;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly GetTransactionsWithFilterQueryHandler _handler;

        public GetTransactionsWithFilterQueryHandlerTests()
        {
            _transactionDtoAssembler = new Mock<IModelAssembler<Transaction, TransactionDto>>();
            _mediatorMock = new Mock<IMediator>();

            _handler = new GetTransactionsWithFilterQueryHandler(_mediatorMock.Object, _transactionDtoAssembler.Object);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(101)]
        public void GetTransactionsWithFilter_ThrowsArgumentOutOfRangeException_InvalidLimit(uint limit)
        {
            // Arrange

            // Act
            void Act() => new GetTransactionsWithFilterQuery("wallet", new List<TransactionEventType>(), new List<string>(), SortDirectionType.ASC, limit, null, null);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Limit must be between 0 and");
        }

        [Fact]
        public void GetTransactionsWithFilter_ThrowsArgumentException_InvalidSortDirection()
        {
            // Arrange

            // Act
            void Act() => new GetTransactionsWithFilterQuery("wallet", new List<TransactionEventType>(), new List<string>(), (SortDirectionType)3, 10, null, null);

            // Assert
            Assert.Throws<ArgumentException>(Act).Message.Should().Contain("Supplied sort direction must be ASC or DESC.");
        }

        [Fact]
        public void GetTransactionsWithFilter_ThrowsArgumentException_PreviousAndNextBothHaveValues()
        {
            // Arrange

            // Act
            void Act() => new GetTransactionsWithFilterQuery("wallet", new List<TransactionEventType>(), new List<string>(), (SortDirectionType)3, 10, "a", "b");

            // Assert
            Assert.Throws<ArgumentException>(Act).Message.Should().Contain("Next and previous cannot both have values.");
        }

        [Fact]
        public async Task GetTransactionsWithFilter_Sends_QueryAndAssembler()
        {
            // Arrange
            var request = new GetTransactionsWithFilterQuery("wallet", new List<TransactionEventType>(), new List<string>(), SortDirectionType.ASC, 10, null, null);

            var transaction = new Transaction(1, "txHash", 2, 3, "from", "to", true, "newContractAddress");

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTransactionsWithFilterQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Transaction> { transaction });

            _transactionDtoAssembler.Setup(callTo => callTo.Assemble(transaction)).ReturnsAsync(new TransactionDto());

            // Act
            await _handler.Handle(request, It.IsAny<CancellationToken>());

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.IsAny<RetrieveTransactionsWithFilterQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            _transactionDtoAssembler.Verify(callTo => callTo.Assemble(transaction), Times.Once);
        }

        [Theory]
        [InlineData(SortDirectionType.ASC)]
        [InlineData(SortDirectionType.DESC)]
        public async Task GetTransactionsWithFilter_OrderAndRemovePlusOne(SortDirectionType sortDirection)
        {
            // Arrange
            const uint limit = 2;
            var request = new GetTransactionsWithFilterQuery("wallet", new List<TransactionEventType>(), new List<string>(), sortDirection, limit, null, null);

            var transactionOne = new Transaction(1, "txHash1", 2, 3, "from1", "1to", true, "newContractAddress1");
            var transactionTwo = new Transaction(2, "txHash2", 2, 3, "from2", "2to", true, "newContractAddress2");
            var transactionThree = new Transaction(3, "txHash3", 2, 3, "from3", "3to", true, "newContractAddress3");

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTransactionsWithFilterQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Transaction> { transactionTwo, transactionOne, transactionThree });

            _transactionDtoAssembler.Setup(callTo => callTo.Assemble(transactionOne)).ReturnsAsync(new TransactionDto { Id = transactionOne.Id });
            _transactionDtoAssembler.Setup(callTo => callTo.Assemble(transactionTwo)).ReturnsAsync(new TransactionDto { Id = transactionTwo.Id });
            _transactionDtoAssembler.Setup(callTo => callTo.Assemble(transactionThree)).ReturnsAsync(new TransactionDto { Id = transactionThree.Id });

            // Act
            var transactions = await _handler.Handle(request, It.IsAny<CancellationToken>());

            // Assert
            transactions.TransactionDtos.Count().Should().Be((int)limit);
            transactions.TransactionDtos.Last().Id.Should().Be(transactionTwo.Id);

            if (sortDirection == SortDirectionType.ASC)
            {
                transactions.TransactionDtos.First().Id.Should().Be(transactionOne.Id);
                transactions.TransactionDtos.Select(tx => tx.Id).Should().BeInAscendingOrder();
            }
            else
            {
                transactions.TransactionDtos.First().Id.Should().Be(transactionThree.Id);
                transactions.TransactionDtos.Select(tx => tx.Id).Should().BeInDescendingOrder();
            }
        }

        [Fact]
        public async Task GetTransactionsWithFilter_BuildsNextCursor()
        {
            // Arrange
            const uint limit = 2;
            var request = new GetTransactionsWithFilterQuery("wallet", new List<TransactionEventType>(), new List<string>(), SortDirectionType.DESC,
                                                             limit, null, null);

            var transactionOne = new Transaction(1, "txHash1", 2, 3, "from1", "1to", true, "newContractAddress1");
            var transactionTwo = new Transaction(2, "txHash2", 2, 3, "from2", "2to", true, "newContractAddress2");
            var transactionThree = new Transaction(3, "txHash3", 2, 3, "from3", "3to", true, "newContractAddress3");

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTransactionsWithFilterQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Transaction> { transactionTwo, transactionOne, transactionThree });

            _transactionDtoAssembler.Setup(callTo => callTo.Assemble(transactionOne)).ReturnsAsync(new TransactionDto { Id = transactionOne.Id });
            _transactionDtoAssembler.Setup(callTo => callTo.Assemble(transactionTwo)).ReturnsAsync(new TransactionDto { Id = transactionTwo.Id });
            _transactionDtoAssembler.Setup(callTo => callTo.Assemble(transactionThree)).ReturnsAsync(new TransactionDto { Id = transactionThree.Id });

            // Act
            var transactions = await _handler.Handle(request, It.IsAny<CancellationToken>());

            // Assert
            transactions.TransactionDtos.Count().Should().Be((int)limit);
            transactions.TransactionDtos.Last().Id.Should().Be(transactionTwo.Id);

            transactions.CursorDto.Next.Should().NotBeNull();
            transactions.CursorDto.Next.Base64Decode().Should().Contain($"next:{transactionTwo.Id.ToString().Base64Encode()};");
            transactions.CursorDto.Previous.Should().BeNull();
        }

        [Fact]
        public async Task GetTransactionsWithFilter_BuildsPreviousCursor()
        {
            // Arrange
            const uint limit = 2;

            var nextCursor = $"limit:2;direction:DESC;next:{5.ToString().Base64Encode()};".Base64Encode();
            var request = new GetTransactionsWithFilterQuery("wallet", new List<TransactionEventType>(), new List<string>(), SortDirectionType.DESC,
                                                             limit, nextCursor, null);

            var transactionOne = new Transaction(3, "txHash1", 2, 3, "from1", "1to", true, "newContractAddress1");
            var transactionTwo = new Transaction(4, "txHash2", 2, 3, "from2", "2to", true, "newContractAddress2");
            var transactionThree = new Transaction(5, "txHash3", 2, 3, "from3", "3to", true, "newContractAddress3");

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTransactionsWithFilterQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Transaction> { transactionTwo, transactionOne, transactionThree });

            _transactionDtoAssembler.Setup(callTo => callTo.Assemble(transactionOne)).ReturnsAsync(new TransactionDto { Id = transactionOne.Id });
            _transactionDtoAssembler.Setup(callTo => callTo.Assemble(transactionTwo)).ReturnsAsync(new TransactionDto { Id = transactionTwo.Id });
            _transactionDtoAssembler.Setup(callTo => callTo.Assemble(transactionThree)).ReturnsAsync(new TransactionDto { Id = transactionThree.Id });

            // Act
            var transactions = await _handler.Handle(request, It.IsAny<CancellationToken>());

            // Assert
            transactions.TransactionDtos.Count().Should().Be((int)limit);
            transactions.TransactionDtos.Last().Id.Should().Be(transactionTwo.Id);

            transactions.CursorDto.Previous.Should().NotBeNull();
            transactions.CursorDto.Previous.Base64Decode().Should().Contain($"previous:{transactionThree.Id.ToString().Base64Encode()};");
            transactions.CursorDto.Next.Base64Decode().Should().Contain($"next:{transactionTwo.Id.ToString().Base64Encode()};");
        }
    }
}
