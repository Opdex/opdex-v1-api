using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.Addresses;
using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.Application.Abstractions.Queries.Addresses;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Application.EntryHandlers.Addresses;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Addresses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Addresses
{
    public class GetAddressBalancesWithFilterQueryHandlerTests
    {
        private readonly Mock<IModelAssembler<AddressBalance, AddressBalanceDto>> _addressAssemblerMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly GetAddressBalancesWithFilterQueryHandler _handler;

        public GetAddressBalancesWithFilterQueryHandlerTests()
        {
            _addressAssemblerMock = new Mock<IModelAssembler<AddressBalance, AddressBalanceDto>>();
            _mediatorMock = new Mock<IMediator>();

            _handler = new GetAddressBalancesWithFilterQueryHandler(_mediatorMock.Object, _addressAssemblerMock.Object);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(101)]
        public void GetAddressBalancesWithFilterQuery_ThrowsArgumentOutOfRangeException_InvalidLimit(uint limit)
        {
            // Arrange
            const string wallet = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";

            // Act
            void Act() => new GetAddressBalancesWithFilterQuery(wallet, new List<string>(), false, false, SortDirectionType.ASC, limit, null, null);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Limit must be between 0 and");
        }

        [Fact]
        public void GetAddressBalancesWithFilterQuery_ThrowsArgumentException_InvalidSortDirection()
        {
            // Arrange
            const string wallet = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";

            // Act
            void Act() => new GetAddressBalancesWithFilterQuery(wallet, new List<string>(), false, false, (SortDirectionType)3, 10, null, null);

            // Assert
            Assert.Throws<ArgumentException>(Act).Message.Should().Contain("Supplied sort direction must be ASC or DESC.");
        }

        [Fact]
        public void GetAddressBalancesWithFilterQuery_ThrowsArgumentException_PreviousAndNextBothHaveValues()
        {
            // Arrange
            const string wallet = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";

            // Act
            void Act() => new GetAddressBalancesWithFilterQuery(wallet, new List<string>(), false, false, (SortDirectionType)3, 10, "a", "b");

            // Assert
            Assert.Throws<ArgumentException>(Act).Message.Should().Contain("Next and previous cannot both have values.");
        }

        [Fact]
        public async Task GetAddressBalancesWithFilterQuery_Sends_QueryAndAssembler()
        {
            // Arrange
            const string wallet = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            var request = new GetAddressBalancesWithFilterQuery(wallet, new List<string>(), false, false, SortDirectionType.ASC, 10, null, null);

            var transaction = new AddressBalance(1, 2, "owner", "100000000", 1, 1);

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveAddressBalancesWithFilterQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<AddressBalance> { transaction });

            _addressAssemblerMock.Setup(callTo => callTo.Assemble(transaction)).ReturnsAsync(new AddressBalanceDto());

            // Act
            await _handler.Handle(request, It.IsAny<CancellationToken>());

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.IsAny<RetrieveAddressBalancesWithFilterQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            _addressAssemblerMock.Verify(callTo => callTo.Assemble(transaction), Times.Once);
        }

        [Theory]
        [InlineData(SortDirectionType.ASC)]
        [InlineData(SortDirectionType.DESC)]
        public async Task GetAddressBalancesWithFilterQuery_OrderAndRemovePlusOne(SortDirectionType sortDirection)
        {
            // Arrange
            const string wallet = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            const uint limit = 2;
            var request = new GetAddressBalancesWithFilterQuery(wallet, new List<string>(), false, false, sortDirection, limit, null, null);

            var balanceOne = new AddressBalance(1, 2, "owner1", "100000000", 1, 1);
            var balanceTwo = new AddressBalance(2, 2, "owner2", "200000000", 1, 1);
            var balanceThree = new AddressBalance(3, 2, "owner3", "300000000", 1, 1);

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveAddressBalancesWithFilterQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<AddressBalance> { balanceTwo, balanceOne, balanceThree });

            _addressAssemblerMock.Setup(callTo => callTo.Assemble(balanceOne)).ReturnsAsync(new AddressBalanceDto {Id = balanceOne.Id});
            _addressAssemblerMock.Setup(callTo => callTo.Assemble(balanceTwo)).ReturnsAsync(new AddressBalanceDto {Id = balanceTwo.Id});
            _addressAssemblerMock.Setup(callTo => callTo.Assemble(balanceThree)).ReturnsAsync(new AddressBalanceDto {Id = balanceThree.Id});

            // Act
            var transactions = await _handler.Handle(request, It.IsAny<CancellationToken>());

            // Assert
            transactions.Balances.Count().Should().Be((int)limit);
            transactions.Balances.Last().Id.Should().Be(balanceTwo.Id);

            if (sortDirection == SortDirectionType.ASC)
            {
                transactions.Balances.First().Id.Should().Be(balanceOne.Id);
                transactions.Balances.Select(tx => tx.Id).Should().BeInAscendingOrder();
            }
            else
            {
                transactions.Balances.First().Id.Should().Be(balanceThree.Id);
                transactions.Balances.Select(tx => tx.Id).Should().BeInDescendingOrder();
            }
        }

        [Fact]
        public async Task GetAddressBalancesWithFilterQuery_BuildsNextCursor()
        {
            // Arrange
            const string wallet = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            const uint limit = 2;
            var request = new GetAddressBalancesWithFilterQuery(wallet, new List<string>(), false, false,
                                                                SortDirectionType.DESC, limit, null, null);


            var balanceOne = new AddressBalance(1, 2, "owner1", "100000000", 1, 1);
            var balanceTwo = new AddressBalance(2, 2, "owner2", "200000000", 1, 1);
            var balanceThree = new AddressBalance(3, 2, "owner3", "300000000", 1, 1);

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveAddressBalancesWithFilterQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<AddressBalance> { balanceTwo, balanceOne, balanceThree });

            _addressAssemblerMock.Setup(callTo => callTo.Assemble(balanceOne)).ReturnsAsync(new AddressBalanceDto {Id = balanceOne.Id});
            _addressAssemblerMock.Setup(callTo => callTo.Assemble(balanceTwo)).ReturnsAsync(new AddressBalanceDto {Id = balanceTwo.Id});
            _addressAssemblerMock.Setup(callTo => callTo.Assemble(balanceThree)).ReturnsAsync(new AddressBalanceDto {Id = balanceThree.Id});

            // Act
            var balances = await _handler.Handle(request, It.IsAny<CancellationToken>());

            // Assert
            balances.Balances.Count().Should().Be((int)limit);
            balances.Balances.Last().Id.Should().Be(balanceTwo.Id);

            balances.Cursor.Next.Should().NotBeNull();
            balances.Cursor.Next.Base64Decode().Should().Contain($"next:{balanceTwo.Id.ToString().Base64Encode()};");
            balances.Cursor.Previous.Should().BeNull();
        }

        [Fact]
        public async Task GetAddressBalancesWithFilterQuery_BuildsPreviousCursor()
        {
            // Arrange
            const uint limit = 2;
            const string wallet = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            var nextCursor = $"wallet:{wallet};direction:DESC;limit:2;includeLpTokens:false;includeZeroBalnces:false;next:{5.ToString().Base64Encode()};".Base64Encode();
            var request = new GetAddressBalancesWithFilterQuery(wallet, new List<string>(), false, false,
                                                                SortDirectionType.DESC, limit, nextCursor, null);

            var balanceOne = new AddressBalance(3, 2, "owner1", "100000000", 1, 1);
            var balanceTwo = new AddressBalance(4, 2, "owner2", "200000000", 1, 1);
            var balanceThree = new AddressBalance(5, 2, "owner3", "300000000", 1, 1);

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveAddressBalancesWithFilterQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<AddressBalance> { balanceTwo, balanceOne, balanceThree });

            _addressAssemblerMock.Setup(callTo => callTo.Assemble(balanceOne)).ReturnsAsync(new AddressBalanceDto {Id = balanceOne.Id});
            _addressAssemblerMock.Setup(callTo => callTo.Assemble(balanceTwo)).ReturnsAsync(new AddressBalanceDto {Id = balanceTwo.Id});
            _addressAssemblerMock.Setup(callTo => callTo.Assemble(balanceThree)).ReturnsAsync(new AddressBalanceDto {Id = balanceThree.Id});

            // Act
            var balances = await _handler.Handle(request, It.IsAny<CancellationToken>());

            // Assert
            balances.Balances.Count().Should().Be((int)limit);
            balances.Balances.Last().Id.Should().Be(balanceTwo.Id);

            balances.Cursor.Previous.Should().NotBeNull();
            balances.Cursor.Previous.Base64Decode().Should().Contain($"previous:{balanceThree.Id.ToString().Base64Encode()};");
            balances.Cursor.Next.Base64Decode().Should().Contain($"next:{balanceTwo.Id.ToString().Base64Encode()};");
        }
    }
}
