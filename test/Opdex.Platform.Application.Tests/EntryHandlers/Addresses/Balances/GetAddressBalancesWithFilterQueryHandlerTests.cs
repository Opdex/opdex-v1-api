using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.Addresses.Balances;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.Application.Abstractions.Queries.Addresses.Balances;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Application.EntryHandlers.Addresses.Balances;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses.Balances;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Addresses.Balances
{
    public class GetAddressBalancesWithFilterQueryHandlerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IModelAssembler<AddressBalance, AddressBalanceDto>> _assemblerMock;

        private readonly GetAddressBalancesWithFilterQueryHandler _handler;

        public GetAddressBalancesWithFilterQueryHandlerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _assemblerMock = new Mock<IModelAssembler<AddressBalance, AddressBalanceDto>>();

            _handler = new GetAddressBalancesWithFilterQueryHandler(_mediatorMock.Object, _assemblerMock.Object);
        }

        [Fact]
        public async Task Handle_RetrieveAddressBalancesWithFilterQuery_Send()
        {
            // Arrange
            var cursor = new AddressBalancesCursor(Enumerable.Empty<Address>(), false, false, SortDirectionType.ASC, 25, PagingDirection.Backward, 55);
            var request = new GetAddressBalancesWithFilterQuery("PHUzrtkLfffDZMd2v8QULRZvBCY5RwrrQK", cursor);
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            try
            {
                await _handler.Handle(request, cancellationToken);
            }
            catch (Exception) { }

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveAddressBalancesWithFilterQuery>(query => query.Address == request.Address
                                                                                                           && query.Cursor == cursor), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task Handle_BalancesRetrieved_MapResults()
        {
            // Arrange
            var cursor = new AddressBalancesCursor(Enumerable.Empty<Address>(), false, false, SortDirectionType.ASC, 25, PagingDirection.Backward, 55);
            var request = new GetAddressBalancesWithFilterQuery("PHUzrtkLfffDZMd2v8QULRZvBCY5RwrrQK", cursor);

            var balance = new AddressBalance(5, 10, "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", 10000000000, 500, 505);
            var balances = new AddressBalance[] { balance };
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveAddressBalancesWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(balances);

            // Act
            await _handler.Handle(request, CancellationToken.None);

            // Assert
            _assemblerMock.Verify(callTo => callTo.Assemble(balance), Times.Once);
        }

        [Fact]
        public async Task Handle_LessThanLimitPlusOneResults_RemoveZero()
        {
            // Arrange
            var cursor = new AddressBalancesCursor(Enumerable.Empty<Address>(), false, false, SortDirectionType.ASC, 3, PagingDirection.Backward, 55);
            var request = new GetAddressBalancesWithFilterQuery("PHUzrtkLfffDZMd2v8QULRZvBCY5RwrrQK", cursor);

            var balances = new AddressBalance[]
            {
                new AddressBalance(5, 10, "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", 10000000000, 500, 505),
                new AddressBalance(10, 10, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", 10000000000, 500, 505),
                new AddressBalance(15, 10, "PXResSytiRhJwNiD1DS9aZinPjEUvk8BuX", 10000000000, 500, 505)
            };
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveAddressBalancesWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(balances);
            _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<AddressBalance>())).ReturnsAsync(new AddressBalanceDto());

            // Act
            var dto = await _handler.Handle(request, CancellationToken.None);

            // Assert
            dto.Balances.Count().Should().Be(balances.Length);
        }

        [Fact]
        public async Task Handle_LimitPlusOneResultsPagingBackward_RemoveFirst()
        {
            // Arrange
            var cursor = new AddressBalancesCursor(Enumerable.Empty<Address>(), false, false, SortDirectionType.ASC, 2, PagingDirection.Backward, 55);
            var request = new GetAddressBalancesWithFilterQuery("PHUzrtkLfffDZMd2v8QULRZvBCY5RwrrQK", cursor);

            var balances = new AddressBalance[]
            {
                new AddressBalance(5, 10, "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", 10000000000, 500, 505),
                new AddressBalance(10, 10, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", 10000000000, 500, 505),
                new AddressBalance(15, 10, "PXResSytiRhJwNiD1DS9aZinPjEUvk8BuX", 10000000000, 500, 505)
            };
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveAddressBalancesWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(balances);
            _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<AddressBalance>())).ReturnsAsync(new AddressBalanceDto());

            // Act
            var dto = await _handler.Handle(request, CancellationToken.None);

            // Assert
            _assemblerMock.Verify(callTo => callTo.Assemble(balances[0]), Times.Never);
            dto.Balances.Count().Should().Be(balances.Length - 1);
        }

        [Fact]
        public async Task Handle_LimitPlusOneResultsPagingForward_RemoveLast()
        {
            // Arrange
            var cursor = new AddressBalancesCursor(Enumerable.Empty<Address>(), false, false, SortDirectionType.ASC, 2, PagingDirection.Forward, 55);
            var request = new GetAddressBalancesWithFilterQuery("PHUzrtkLfffDZMd2v8QULRZvBCY5RwrrQK", cursor);

            var balances = new AddressBalance[]
            {
                new AddressBalance(5, 10, "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", 10000000000, 500, 505),
                new AddressBalance(10, 10, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", 10000000000, 500, 505),
                new AddressBalance(15, 10, "PXResSytiRhJwNiD1DS9aZinPjEUvk8BuX", 10000000000, 500, 505)
            };
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveAddressBalancesWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(balances);
            _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<AddressBalance>())).ReturnsAsync(new AddressBalanceDto());

            // Act
            var dto = await _handler.Handle(request, CancellationToken.None);

            // Assert
            _assemblerMock.Verify(callTo => callTo.Assemble(balances[balances.Length - 1]), Times.Never);
            dto.Balances.Count().Should().Be(balances.Length - 1);
        }

        [Fact]
        public async Task Handle_FirstRequestInPagedResults_ReturnCursor()
        {
            // Arrange
            var cursor = new AddressBalancesCursor(Enumerable.Empty<Address>(), false, false, SortDirectionType.ASC, 2, PagingDirection.Forward, 0);
            var request = new GetAddressBalancesWithFilterQuery("PHUzrtkLfffDZMd2v8QULRZvBCY5RwrrQK", cursor);

            var balances = new AddressBalance[]
            {
                new AddressBalance(5, 10, "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", 10000000000, 500, 505),
                new AddressBalance(10, 10, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", 10000000000, 500, 505),
                new AddressBalance(15, 10, "PXResSytiRhJwNiD1DS9aZinPjEUvk8BuX", 10000000000, 500, 505)
            };
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveAddressBalancesWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(balances);
            _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<AddressBalance>())).ReturnsAsync(new AddressBalanceDto());

            // Act
            var dto = await _handler.Handle(request, CancellationToken.None);

            // Assert
            AssertNext(dto.Cursor, balances[^2].Id);
            dto.Cursor.Previous.Should().Be(null);
        }

        [Fact]
        public async Task Handle_PagingForwardWithMoreResults_ReturnCursor()
        {
            // Arrange
            var cursor = new AddressBalancesCursor(Enumerable.Empty<Address>(), false, false, SortDirectionType.ASC, 2, PagingDirection.Forward, 50);
            var request = new GetAddressBalancesWithFilterQuery("PHUzrtkLfffDZMd2v8QULRZvBCY5RwrrQK", cursor);

            var balances = new AddressBalance[]
            {
                new AddressBalance(5, 10, "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", 10000000000, 500, 505),
                new AddressBalance(10, 10, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", 10000000000, 500, 505),
                new AddressBalance(15, 10, "PXResSytiRhJwNiD1DS9aZinPjEUvk8BuX", 10000000000, 500, 505)
            };
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveAddressBalancesWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(balances);
            _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<AddressBalance>())).ReturnsAsync(new AddressBalanceDto());

            // Act
            var dto = await _handler.Handle(request, CancellationToken.None);

            // Assert
            AssertNext(dto.Cursor, balances[^2].Id);
            AssertPrevious(dto.Cursor, balances[0].Id);
        }

        [Fact]
        public async Task Handle_PagingBackwardWithMoreResults_ReturnCursor()
        {
            // Arrange
            var cursor = new AddressBalancesCursor(Enumerable.Empty<Address>(), false, false, SortDirectionType.ASC, 2, PagingDirection.Backward, 50);
            var request = new GetAddressBalancesWithFilterQuery("PHUzrtkLfffDZMd2v8QULRZvBCY5RwrrQK", cursor);

            var balances = new AddressBalance[]
            {
                new AddressBalance(5, 10, "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", 10000000000, 500, 505),
                new AddressBalance(10, 10, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", 10000000000, 500, 505),
                new AddressBalance(15, 10, "PXResSytiRhJwNiD1DS9aZinPjEUvk8BuX", 10000000000, 500, 505)
            };
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveAddressBalancesWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(balances);
            _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<AddressBalance>())).ReturnsAsync(new AddressBalanceDto());

            // Act
            var dto = await _handler.Handle(request, CancellationToken.None);

            // Assert
            AssertNext(dto.Cursor, balances[^1].Id);
            AssertPrevious(dto.Cursor, balances[1].Id);
        }

        [Fact]
        public async Task Handle_PagingForwardLastPage_ReturnCursor()
        {
            // Arrange
            var cursor = new AddressBalancesCursor(Enumerable.Empty<Address>(), false, false, SortDirectionType.ASC, 2, PagingDirection.Forward, 50);
            var request = new GetAddressBalancesWithFilterQuery("PHUzrtkLfffDZMd2v8QULRZvBCY5RwrrQK", cursor);

            var balances = new AddressBalance[]
            {
                new AddressBalance(5, 10, "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", 10000000000, 500, 505),
                new AddressBalance(10, 10, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", 10000000000, 500, 505)
            };
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveAddressBalancesWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(balances);
            _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<AddressBalance>())).ReturnsAsync(new AddressBalanceDto());

            // Act
            var dto = await _handler.Handle(request, CancellationToken.None);

            // Assert
            dto.Cursor.Next.Should().Be(null);
            AssertPrevious(dto.Cursor, balances[0].Id);
        }

        [Fact]
        public async Task Handle_PagingBackwardLastPage_ReturnCursor()
        {
            // Arrange
            var cursor = new AddressBalancesCursor(Enumerable.Empty<Address>(), false, false, SortDirectionType.ASC, 2, PagingDirection.Backward, 50);
            var request = new GetAddressBalancesWithFilterQuery("PHUzrtkLfffDZMd2v8QULRZvBCY5RwrrQK", cursor);

            var balances = new AddressBalance[]
            {
                new AddressBalance(5, 10, "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", 10000000000, 500, 505),
                new AddressBalance(10, 10, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", 10000000000, 500, 505)
            };
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveAddressBalancesWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(balances);
            _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<AddressBalance>())).ReturnsAsync(new AddressBalanceDto());

            // Act
            var dto = await _handler.Handle(request, CancellationToken.None);

            // Assert
            AssertNext(dto.Cursor, balances[^1].Id);
            dto.Cursor.Previous.Should().Be(null);
        }

        private void AssertNext(CursorDto dto, ulong pointer)
        {
            AddressBalancesCursor.TryParse(dto.Next.Base64Decode(), out var next).Should().Be(true);
            next.PagingDirection.Should().Be(PagingDirection.Forward);
            next.Pointer.Should().Be(pointer);
        }

        private void AssertPrevious(CursorDto dto, ulong pointer)
        {
            AddressBalancesCursor.TryParse(dto.Previous.Base64Decode(), out var next).Should().Be(true);
            next.PagingDirection.Should().Be(PagingDirection.Backward);
            next.Pointer.Should().Be(pointer);
        }
    }
}
