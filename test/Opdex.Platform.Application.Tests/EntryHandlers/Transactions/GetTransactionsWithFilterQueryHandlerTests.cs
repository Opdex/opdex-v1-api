using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.Transactions;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Application.Abstractions.Queries.Transactions;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Application.EntryHandlers.Transactions;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Domain.Models.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Transactions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Transactions
{
    public class GetTransactionsWithFilterQueryHandlerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IModelAssembler<Transaction, TransactionDto>> _assemblerMock;

        private readonly GetTransactionsWithFilterQueryHandler _handler;

        public GetTransactionsWithFilterQueryHandlerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _assemblerMock = new Mock<IModelAssembler<Transaction, TransactionDto>>();

            _handler = new GetTransactionsWithFilterQueryHandler(_mediatorMock.Object, _assemblerMock.Object);
        }

        [Fact]
        public async Task Handle_RetrieveTransactionsWithFilterQuery_Send()
        {
            // Arrange
            var cursor = new TransactionsCursor(Address.Empty, Enumerable.Empty<TransactionEventType>(), Enumerable.Empty<Address>(), SortDirectionType.ASC, 25, PagingDirection.Backward, 55);
            var request = new GetTransactionsWithFilterQuery(cursor);
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            try
            {
                await _handler.Handle(request, cancellationToken);
            }
            catch (Exception) { }

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveTransactionsWithFilterQuery>(query => query.Cursor == cursor), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task Handle_TransactionsRetrieved_MapResults()
        {
            // Arrange
            var cursor = new TransactionsCursor(Address.Empty, Enumerable.Empty<TransactionEventType>(), Enumerable.Empty<Address>(), SortDirectionType.ASC, 25, PagingDirection.Backward, 55);
            var request = new GetTransactionsWithFilterQuery(cursor);

            var transaction = new Transaction(5, "hash", 500, 10000, "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", true, null);
            var transactions = new Transaction[] { transaction };
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTransactionsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(transactions);

            // Act
            await _handler.Handle(request, CancellationToken.None);

            // Assert
            _assemblerMock.Verify(callTo => callTo.Assemble(transaction), Times.Once);
        }

        [Fact]
        public async Task Handle_LessThanLimitPlusOneResults_RemoveZero()
        {
            // Arrange
            var cursor = new TransactionsCursor(Address.Empty, Enumerable.Empty<TransactionEventType>(), Enumerable.Empty<Address>(), SortDirectionType.ASC, 3, PagingDirection.Backward, 55);
            var request = new GetTransactionsWithFilterQuery(cursor);

            var transactions = new Transaction[]
            {
                new Transaction(5, "hash", 500, 10000, "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", true, null),
                new Transaction(10, "hash", 505, 10000, "PXResSytiRhJwNiD1DS9aZinPjEUvk8BuX", "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", true, null),
                new Transaction(15, "hash", 510, 10000, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", true, null)
            };
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTransactionsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(transactions);
            _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<Transaction>())).ReturnsAsync(new TransactionDto());

            // Act
            var dto = await _handler.Handle(request, CancellationToken.None);

            // Assert
            dto.Transactions.Count().Should().Be(transactions.Length);
        }

        [Fact]
        public async Task Handle_LimitPlusOneResultsPagingBackward_RemoveFirst()
        {
            // Arrange
            var cursor = new TransactionsCursor(Address.Empty, Enumerable.Empty<TransactionEventType>(), Enumerable.Empty<Address>(), SortDirectionType.ASC, 2, PagingDirection.Backward, 55);
            var request = new GetTransactionsWithFilterQuery(cursor);

            var transactions = new Transaction[]
            {
                new Transaction(5, "hash", 500, 10000, "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", true, null),
                new Transaction(10, "hash", 505, 10000, "PXResSytiRhJwNiD1DS9aZinPjEUvk8BuX", "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", true, null),
                new Transaction(15, "hash", 510, 10000, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", true, null)
            };
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTransactionsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(transactions);
            _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<Transaction>())).ReturnsAsync(new TransactionDto());

            // Act
            var dto = await _handler.Handle(request, CancellationToken.None);

            // Assert
            _assemblerMock.Verify(callTo => callTo.Assemble(transactions[0]), Times.Never);
            dto.Transactions.Count().Should().Be(transactions.Length - 1);
        }

        [Fact]
        public async Task Handle_LimitPlusOneResultsPagingForward_RemoveLast()
        {
            // Arrange
            var cursor = new TransactionsCursor(Address.Empty, Enumerable.Empty<TransactionEventType>(), Enumerable.Empty<Address>(), SortDirectionType.ASC, 2, PagingDirection.Forward, 55);
            var request = new GetTransactionsWithFilterQuery(cursor);

            var transactions = new Transaction[]
            {
                new Transaction(5, "hash", 500, 10000, "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", true, null),
                new Transaction(10, "hash", 505, 10000, "PXResSytiRhJwNiD1DS9aZinPjEUvk8BuX", "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", true, null),
                new Transaction(15, "hash", 510, 10000, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", true, null)
            };
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTransactionsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(transactions);
            _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<Transaction>())).ReturnsAsync(new TransactionDto());

            // Act
            var dto = await _handler.Handle(request, CancellationToken.None);

            // Assert
            _assemblerMock.Verify(callTo => callTo.Assemble(transactions[transactions.Length - 1]), Times.Never);
            dto.Transactions.Count().Should().Be(transactions.Length - 1);
        }

        [Fact]
        public async Task Handle_FirstRequestInPagedResults_ReturnCursor()
        {
            // Arrange
            var cursor = new TransactionsCursor(Address.Empty, Enumerable.Empty<TransactionEventType>(), Enumerable.Empty<Address>(), SortDirectionType.ASC, 2, PagingDirection.Forward, 0);
            var request = new GetTransactionsWithFilterQuery(cursor);

            var transactions = new Transaction[]
            {
                new Transaction(5, "hash", 500, 10000, "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", true, null),
                new Transaction(10, "hash", 505, 10000, "PXResSytiRhJwNiD1DS9aZinPjEUvk8BuX", "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", true, null),
                new Transaction(15, "hash", 510, 10000, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", true, null)
            };
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTransactionsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(transactions);
            _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<Transaction>())).ReturnsAsync(new TransactionDto());

            // Act
            var dto = await _handler.Handle(request, CancellationToken.None);

            // Assert
            AssertNext(dto.Cursor, transactions[^2].Id);
            dto.Cursor.Previous.Should().Be(null);
        }

        [Fact]
        public async Task Handle_PagingForwardWithMoreResults_ReturnCursor()
        {
            // Arrange
            var cursor = new TransactionsCursor(Address.Empty, Enumerable.Empty<TransactionEventType>(), Enumerable.Empty<Address>(), SortDirectionType.ASC, 2, PagingDirection.Forward, 50);
            var request = new GetTransactionsWithFilterQuery(cursor);

            var transactions = new Transaction[]
            {
                new Transaction(5, "hash", 500, 10000, "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", true, null),
                new Transaction(10, "hash", 505, 10000, "PXResSytiRhJwNiD1DS9aZinPjEUvk8BuX", "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", true, null),
                new Transaction(15, "hash", 510, 10000, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", true, null)
            };
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTransactionsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(transactions);
            _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<Transaction>())).ReturnsAsync(new TransactionDto());

            // Act
            var dto = await _handler.Handle(request, CancellationToken.None);

            // Assert
            AssertNext(dto.Cursor, transactions[^2].Id);
            AssertPrevious(dto.Cursor, transactions[0].Id);
        }

        [Fact]
        public async Task Handle_PagingBackwardWithMoreResults_ReturnCursor()
        {
            // Arrange
            var cursor = new TransactionsCursor(Address.Empty, Enumerable.Empty<TransactionEventType>(), Enumerable.Empty<Address>(), SortDirectionType.ASC, 2, PagingDirection.Backward, 50);
            var request = new GetTransactionsWithFilterQuery(cursor);

            var transactions = new Transaction[]
            {
                new Transaction(5, "hash", 500, 10000, "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", true, null),
                new Transaction(10, "hash", 505, 10000, "PXResSytiRhJwNiD1DS9aZinPjEUvk8BuX", "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", true, null),
                new Transaction(15, "hash", 510, 10000, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", true, null)
            };
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTransactionsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(transactions);
            _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<Transaction>())).ReturnsAsync(new TransactionDto());

            // Act
            var dto = await _handler.Handle(request, CancellationToken.None);

            // Assert
            AssertNext(dto.Cursor, transactions[^1].Id);
            AssertPrevious(dto.Cursor, transactions[1].Id);
        }

        [Fact]
        public async Task Handle_PagingForwardLastPage_ReturnCursor()
        {
            // Arrange
            var cursor = new TransactionsCursor(Address.Empty, Enumerable.Empty<TransactionEventType>(), Enumerable.Empty<Address>(), SortDirectionType.ASC, 2, PagingDirection.Forward, 50);
            var request = new GetTransactionsWithFilterQuery(cursor);

            var transactions = new Transaction[]
            {
                new Transaction(5, "hash", 500, 10000, "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", true, null),
                new Transaction(10, "hash", 505, 10000, "PXResSytiRhJwNiD1DS9aZinPjEUvk8BuX", "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", true, null)
            };
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTransactionsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(transactions);
            _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<Transaction>())).ReturnsAsync(new TransactionDto());

            // Act
            var dto = await _handler.Handle(request, CancellationToken.None);

            // Assert
            dto.Cursor.Next.Should().Be(null);
            AssertPrevious(dto.Cursor, transactions[0].Id);
        }

        [Fact]
        public async Task Handle_PagingBackwardLastPage_ReturnCursor()
        {
            // Arrange
            var cursor = new TransactionsCursor(Address.Empty, Enumerable.Empty<TransactionEventType>(), Enumerable.Empty<Address>(), SortDirectionType.ASC, 2, PagingDirection.Backward, 50);
            var request = new GetTransactionsWithFilterQuery(cursor);

            var transactions = new Transaction[]
            {
                new Transaction(5, "hash", 500, 10000, "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", true, null),
                new Transaction(10, "hash", 505, 10000, "PXResSytiRhJwNiD1DS9aZinPjEUvk8BuX", "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", true, null)
            };
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTransactionsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(transactions);
            _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<Transaction>())).ReturnsAsync(new TransactionDto());

            // Act
            var dto = await _handler.Handle(request, CancellationToken.None);

            // Assert
            AssertNext(dto.Cursor, transactions[^1].Id);
            dto.Cursor.Previous.Should().Be(null);
        }

        private void AssertNext(CursorDto dto, ulong pointer)
        {
            TransactionsCursor.TryParse(dto.Next.Base64Decode(), out var next).Should().Be(true);
            next.PagingDirection.Should().Be(PagingDirection.Forward);
            next.Pointer.Should().Be(pointer);
        }

        private void AssertPrevious(CursorDto dto, ulong pointer)
        {
            TransactionsCursor.TryParse(dto.Previous.Base64Decode(), out var next).Should().Be(true);
            next.PagingDirection.Should().Be(PagingDirection.Backward);
            next.Pointer.Should().Be(pointer);
        }
    }
}
