using AutoMapper;
using Moq;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses.Balances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens;
using Opdex.Platform.Infrastructure.Data.Handlers.Addresses.Balances;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Addresses.Balances
{
    public class SelectAddressBalancesWithFilterQueryHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly SelectAddressBalancesWithFilterQueryHandler _handler;

        public SelectAddressBalancesWithFilterQueryHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();

            _dbContext = new Mock<IDbContext>();
            _handler = new SelectAddressBalancesWithFilterQueryHandler(_dbContext.Object, mapper);
        }

        [Fact]
        public async Task SelectAddressBalancesWithFilter_ByWallet()
        {
            // Arrange
            Address wallet = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            const SortDirectionType direction = SortDirectionType.ASC;
            const uint limit = 10;

            var cursor = new AddressBalancesCursor(Enumerable.Empty<Address>(), default, false, direction, limit, PagingDirection.Backward, 50);

            var command = new SelectAddressBalancesWithFilterQuery(wallet, cursor);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo => callTo.ExecuteQueryAsync<AddressBalanceEntity>(
                                  It.Is<DatabaseQuery>(q => q.Sql.Contains("ab.Owner = @Wallet"))), Times.Once);
        }

        [Fact]
        public async Task SelectAddressBalancesWithFilter_ByContracts()
        {
            // Arrange
            const SortDirectionType direction = SortDirectionType.ASC;
            Address wallet = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            var tokens = new Address[] { "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj" };
            const uint limit = 10;

            var cursor = new AddressBalancesCursor(tokens, default, false, direction, limit, PagingDirection.Backward, 50);

            var command = new SelectAddressBalancesWithFilterQuery(wallet, cursor);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo =>
                                  callTo.ExecuteQueryAsync<AddressBalanceEntity>(
                                      It.Is<DatabaseQuery>(q => q.Sql.Contains("JOIN token t") &&
                                                                q.Sql.Contains("t.Address IN @Tokens"))), Times.Once);
        }

        [Fact]
        public async Task SelectAddressBalancesWithFilter_OnlyProvisionalTokens()
        {
            // Arrange
            const SortDirectionType direction = SortDirectionType.ASC;
            Address wallet = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            const TokenProvisionalFilter tokenType = TokenProvisionalFilter.Provisional;
            const uint limit = 10;

            var cursor = new AddressBalancesCursor(Enumerable.Empty<Address>(), tokenType, false, direction, limit, PagingDirection.Backward, 50);

            var command = new SelectAddressBalancesWithFilterQuery(wallet, cursor);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo =>
                                  callTo.ExecuteQueryAsync<AddressBalanceEntity>(
                                      It.Is<DatabaseQuery>(q => q.Sql.Contains("JOIN token t") &&
                                                                q.Sql.Contains("t.IsLpt = @IncludeLpTokens"))), Times.Once);
        }

        [Fact]
        public async Task SelectAddressBalancesWithFilter_OnlyNonProvisionalTokens()
        {
            // Arrange
            const SortDirectionType direction = SortDirectionType.ASC;
            Address wallet = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            const TokenProvisionalFilter tokenType = TokenProvisionalFilter.NonProvisional;
            const uint limit = 10;

            var cursor = new AddressBalancesCursor(Enumerable.Empty<Address>(), tokenType, false, direction, limit, PagingDirection.Backward, 50);

            var command = new SelectAddressBalancesWithFilterQuery(wallet, cursor);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo =>
                                  callTo.ExecuteQueryAsync<AddressBalanceEntity>(
                                      It.Is<DatabaseQuery>(q => q.Sql.Contains("JOIN token t") &&
                                                                q.Sql.Contains("t.IsLpt = @IncludeLpTokens"))), Times.Once);
        }

        [Fact]
        public async Task SelectAddressBalancesWithFilter_AllTokens()
        {
            // Arrange
            const SortDirectionType direction = SortDirectionType.ASC;
            Address wallet = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            const TokenProvisionalFilter tokenType = TokenProvisionalFilter.All;
            const uint limit = 10;

            var cursor = new AddressBalancesCursor(Enumerable.Empty<Address>(), tokenType, false, direction, limit, PagingDirection.Backward, 50);

            var command = new SelectAddressBalancesWithFilterQuery(wallet, cursor);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo =>
                                  callTo.ExecuteQueryAsync<AddressBalanceEntity>(
                                      It.Is<DatabaseQuery>(q => !q.Sql.Contains("JOIN token t") &&
                                                                !q.Sql.Contains("t.IsLpt = @IncludeLpTokens"))), Times.Once);
        }

        [Fact]
        public async Task SelectAddressBalancesWithFilter_ExcludeZeroBalanceTokens()
        {
            // Arrange
            const SortDirectionType direction = SortDirectionType.ASC;
            Address wallet = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            const bool includeZeroBalance = false;
            const uint limit = 10;

            var cursor = new AddressBalancesCursor(Enumerable.Empty<Address>(), default, includeZeroBalance, direction, limit, PagingDirection.Backward, 50);

            var command = new SelectAddressBalancesWithFilterQuery(wallet, cursor);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo =>
                                  callTo.ExecuteQueryAsync<AddressBalanceEntity>(
                                      It.Is<DatabaseQuery>(q => q.Sql.Contains("ab.Balance != '0'"))), Times.Once);
        }

        [Fact]
        public async Task SelectAddressBalancesWithFilter_ByCursor_NextASC()
        {
            // Arrange
            Address wallet = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            const SortDirectionType direction = SortDirectionType.ASC;
            const uint limit = 10;

            var cursor = new AddressBalancesCursor(Enumerable.Empty<Address>(), default, false, direction, limit, PagingDirection.Forward, 50);

            var command = new SelectAddressBalancesWithFilterQuery(wallet, cursor);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo =>
                                  callTo.ExecuteQueryAsync<AddressBalanceEntity>(
                                      It.Is<DatabaseQuery>(q => q.Sql.Contains("ab.Id > @BalanceId") &&
                                                                q.Sql.Contains($"ORDER BY ab.Id {direction}") &&
                                                                q.Sql.Contains($"LIMIT {limit + 1}"))), Times.Once);
        }

        [Fact]
        public async Task SelectAddressBalancesWithFilter_ByCursor_NextDESC()
        {
            // Arrange
            Address wallet = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            const SortDirectionType direction = SortDirectionType.DESC;
            const uint limit = 10;

            var cursor = new AddressBalancesCursor(Enumerable.Empty<Address>(), default, false, direction, limit, PagingDirection.Forward, 50);

            var command = new SelectAddressBalancesWithFilterQuery(wallet, cursor);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo =>
                                  callTo.ExecuteQueryAsync<AddressBalanceEntity>(
                                      It.Is<DatabaseQuery>(q => q.Sql.Contains("ab.Id < @BalanceId") &&
                                                                q.Sql.Contains($"ORDER BY ab.Id {direction}") &&
                                                                q.Sql.Contains($"LIMIT {limit + 1}"))), Times.Once);
        }

        [Fact]
        public async Task SelectAddressBalancesWithFilter_ByCursor_PreviousDESC()
        {
            // Arrange
            Address wallet = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            const SortDirectionType requestDirection = SortDirectionType.DESC;
            const uint limit = 10;

            var cursor = new AddressBalancesCursor(Enumerable.Empty<Address>(), default, false, requestDirection, limit, PagingDirection.Backward, 50);

            var command = new SelectAddressBalancesWithFilterQuery(wallet, cursor);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo =>
                                  callTo.ExecuteQueryAsync<AddressBalanceEntity>(
                                      It.Is<DatabaseQuery>(q => q.Sql.Contains("ab.Id > @BalanceId") &&
                                                                q.Sql.Contains($"ORDER BY ab.Id {SortDirectionType.ASC}") &&
                                                                q.Sql.Contains($"LIMIT {limit + 1}"))), Times.Once);
        }

        [Fact]
        public async Task SelectAddressBalancesWithFilter_ByCursor_PreviousASC()
        {
            // Arrange
            Address wallet = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            const SortDirectionType requestDirection = SortDirectionType.ASC;
            const uint limit = 10;

            var cursor = new AddressBalancesCursor(Enumerable.Empty<Address>(), default, false, requestDirection, limit, PagingDirection.Backward, 50);

            var command = new SelectAddressBalancesWithFilterQuery(wallet, cursor);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo =>
                                  callTo.ExecuteQueryAsync<AddressBalanceEntity>(
                                      It.Is<DatabaseQuery>(q => q.Sql.Contains("ab.Id < @BalanceId") &&
                                                                q.Sql.Contains($"ORDER BY ab.Id {SortDirectionType.DESC}") &&
                                                                q.Sql.Contains($"LIMIT {limit + 1}"))), Times.Once);
        }
    }
}
