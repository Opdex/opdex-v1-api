using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Markets.Permissions;
using Opdex.Platform.Application.EntryHandlers.Markets;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Markets;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Markets
{
    public class GetMarketPermissionsForAddressQueryHandlerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly GetMarketPermissionsForAddressQueryHandler _handler;

        public GetMarketPermissionsForAddressQueryHandlerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _handler = new GetMarketPermissionsForAddressQueryHandler(_mediatorMock.Object);
        }

        [Fact]
        public async Task Handle_RetrieveMarketByAddressQuery_FindOrThrow()
        {
            // Arrange
            Address marketAddress = "PEkFDLUw1aLjYCWoJ1jRehNfTXjgWuZsX3";
            var query = new GetMarketPermissionsForAddressQuery(marketAddress, new Address("PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV"));
            var cancellationToken = new CancellationTokenSource().Token;

            var standardMarket = new Market(5, new Address("PEkFDLUw1aLjYCWoJ1jRehNfTXjgWuZsX3"), 10, 0, Address.Empty, new Address("PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV"),
                                            false, false, false, 3, true, 5, 50);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(standardMarket);

            // Act
            await _handler.Handle(query, cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveMarketByAddressQuery>(query => query.Address == marketAddress
                                                                                                 && query.FindOrThrow), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task Handle_MarketIsStakingMarket_ThrowNotFoundException()
        {
            // Arrange
            Address marketAddress = "PEkFDLUw1aLjYCWoJ1jRehNfTXjgWuZsX3";
            var query = new GetMarketPermissionsForAddressQuery(marketAddress, new Address("PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV"));
            var cancellationToken = new CancellationTokenSource().Token;

            var stakingMarket = new Market(5, new Address("PEkFDLUw1aLjYCWoJ1jRehNfTXjgWuZsX3"), 10, 15, Address.Empty, new Address("PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV"),
                                           false, false, false, 3, true, 5, 50);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(stakingMarket);

            // Act
            Task Act() => _handler.Handle(query, cancellationToken);

            // Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(Act);
            exception.Message.Should().Be("Market address must represent a standard market.");
        }

        [Fact]
        public async Task Handle_RetrieveMarketPermissionsByUserQuery_Send()
        {
            // Arrange
            Address marketAddress = new Address("PEkFDLUw1aLjYCWoJ1jRehNfTXjgWuZsX3");
            Address wallet = new Address("PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV");
            var query = new GetMarketPermissionsForAddressQuery(marketAddress, wallet);
            var cancellationToken = new CancellationTokenSource().Token;

            var standardMarket = new Market(5, new Address("PEkFDLUw1aLjYCWoJ1jRehNfTXjgWuZsX3"), 10, 0, Address.Empty, new Address("PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV"),
                                            false, false, false, 3, true, 5, 50);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(standardMarket);

            // Act
            await _handler.Handle(query, cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveMarketPermissionsByUserQuery>(query => query.MarketId == standardMarket.Id
                                                                                                         && query.User == wallet), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task Handle_MarketOwnerIsWallet_ReturnAll()
        {
            // Arrange
            Address marketAddress = new Address("PEkFDLUw1aLjYCWoJ1jRehNfTXjgWuZsX3");
            Address wallet = new Address("PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV");
            var query = new GetMarketPermissionsForAddressQuery(marketAddress, wallet);
            var cancellationToken = new CancellationTokenSource().Token;

            var standardMarket = new Market(5, new Address("PEkFDLUw1aLjYCWoJ1jRehNfTXjgWuZsX3"), 10, 0, Address.Empty, wallet, false, false, false, 3, true, 5, 50);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(standardMarket);

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketPermissionsByUserQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(Enumerable.Empty<MarketPermissionType>());

            // Act
            var permissions = await _handler.Handle(query, cancellationToken);

            // Assert
            permissions.Count().Should().Be(4);
            permissions.Should().Contain(MarketPermissionType.CreatePool);
            permissions.Should().Contain(MarketPermissionType.Provide);
            permissions.Should().Contain(MarketPermissionType.Trade);
            permissions.Should().Contain(MarketPermissionType.SetPermissions);
        }

        [Fact]
        public async Task Handle_DoesNotAuthPoolCreation_AlwaysReturnCreatePool()
        {
            // Arrange
            Address marketAddress = new Address("PEkFDLUw1aLjYCWoJ1jRehNfTXjgWuZsX3");
            Address wallet = new Address("PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV");
            var query = new GetMarketPermissionsForAddressQuery(marketAddress, wallet);
            var cancellationToken = new CancellationTokenSource().Token;

            var standardMarket = new Market(5, new Address("PEkFDLUw1aLjYCWoJ1jRehNfTXjgWuZsX3"), 10, 0, Address.Empty, new Address("PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh"),
                                            false, true, true, 3, true, 5, 50);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(standardMarket);

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketPermissionsByUserQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(Enumerable.Empty<MarketPermissionType>());

            // Act
            var permissions = await _handler.Handle(query, cancellationToken);

            // Assert
            permissions.Count().Should().Be(1);
            permissions.Should().Contain(MarketPermissionType.CreatePool);
        }

        [Fact]
        public async Task Handle_DoesNotAuthLiquidityProviding_AlwaysReturnProvide()
        {
            // Arrange
            Address marketAddress = new Address("PEkFDLUw1aLjYCWoJ1jRehNfTXjgWuZsX3");
            Address wallet = new Address("PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV");
            var query = new GetMarketPermissionsForAddressQuery(marketAddress, wallet);
            var cancellationToken = new CancellationTokenSource().Token;

            var standardMarket = new Market(5, new Address("PEkFDLUw1aLjYCWoJ1jRehNfTXjgWuZsX3"), 10, 0, Address.Empty, new Address("PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh"),
                                            true, false, true, 3, true, 5, 50);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(standardMarket);

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketPermissionsByUserQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(Enumerable.Empty<MarketPermissionType>());

            // Act
            var permissions = await _handler.Handle(query, cancellationToken);

            // Assert
            permissions.Count().Should().Be(1);
            permissions.Should().Contain(MarketPermissionType.Provide);
        }

        [Fact]
        public async Task Handle_DoesNotAuthTrading_AlwaysReturnTrade()
        {
            // Arrange
            Address marketAddress = new Address("PEkFDLUw1aLjYCWoJ1jRehNfTXjgWuZsX3");
            Address wallet = new Address("PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV");
            var query = new GetMarketPermissionsForAddressQuery(marketAddress, wallet);
            var cancellationToken = new CancellationTokenSource().Token;

            var standardMarket = new Market(5, new Address("PEkFDLUw1aLjYCWoJ1jRehNfTXjgWuZsX3"), 10, 0, Address.Empty, new Address("PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh"),
                                            true, true, false, 3, true, 5, 50);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(standardMarket);

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketPermissionsByUserQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(Enumerable.Empty<MarketPermissionType>());

            // Act
            var permissions = await _handler.Handle(query, cancellationToken);

            // Assert
            permissions.Count().Should().Be(1);
            permissions.Should().Contain(MarketPermissionType.Trade);
        }

        [Fact]
        public async Task Handle_AssignedPermissions_Return()
        {
            // Arrange
            Address marketAddress = new Address("PEkFDLUw1aLjYCWoJ1jRehNfTXjgWuZsX3");
            Address wallet = new Address("PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV");
            var query = new GetMarketPermissionsForAddressQuery(marketAddress, wallet);
            var cancellationToken = new CancellationTokenSource().Token;

            var standardMarket = new Market(5, new Address("PEkFDLUw1aLjYCWoJ1jRehNfTXjgWuZsX3"), 10, 0, Address.Empty, new Address("PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh"),
                                            false, true, false, 3, true, 5, 50);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(standardMarket);

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketPermissionsByUserQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new MarketPermissionType[]
            {
                MarketPermissionType.CreatePool, MarketPermissionType.SetPermissions
            });

            // Act
            var permissions = await _handler.Handle(query, cancellationToken);

            // Assert
            permissions.Count().Should().Be(3);
            permissions.Should().Contain(MarketPermissionType.CreatePool);
            permissions.Should().Contain(MarketPermissionType.SetPermissions);
            permissions.Should().Contain(MarketPermissionType.Trade);
        }
    }
}