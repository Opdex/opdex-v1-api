using AutoMapper;
using Moq;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses;
using Opdex.Platform.Infrastructure.Data.Handlers.Addresses;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Addresses
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
            const string wallet = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            const SortDirectionType direction = SortDirectionType.ASC;
            const uint limit = 10;

            var cursor = new AddressBalancesCursor(Enumerable.Empty<string>(), false, false, direction, limit, PagingDirection.Backward, 0);

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
            const string wallet = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            var tokens = new[] { "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj" };
            const uint limit = 10;

            var cursor = new AddressBalancesCursor(tokens, false, false, direction, limit, PagingDirection.Backward, 0);

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
        public async Task SelectAddressBalancesWithFilter_ExcludeLpTokens()
        {
            // Arrange
            const SortDirectionType direction = SortDirectionType.ASC;
            const string wallet = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            const bool includeLpTokens = false;
            const uint limit = 10;

            var cursor = new AddressBalancesCursor(Enumerable.Empty<string>(), includeLpTokens, false, direction, limit, PagingDirection.Backward, 0);

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
        public async Task SelectAddressBalancesWithFilter_ExcludeZeroBalanceTokens()
        {
            // Arrange
            const SortDirectionType direction = SortDirectionType.ASC;
            const string wallet = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            const bool includeZeroBalance = false;
            const uint limit = 10;

            var cursor = new AddressBalancesCursor(Enumerable.Empty<string>(), false, includeZeroBalance, direction, limit, PagingDirection.Backward, 0);

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
            const string wallet = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            const SortDirectionType direction = SortDirectionType.ASC;
            const uint limit = 10;

            var cursor = new AddressBalancesCursor(Enumerable.Empty<string>(), false, false, direction, limit, PagingDirection.Forward, 50);

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
            const string wallet = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            const SortDirectionType direction = SortDirectionType.DESC;
            const uint limit = 10;

            var cursor = new AddressBalancesCursor(Enumerable.Empty<string>(), false, false, direction, limit, PagingDirection.Forward, 50);

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
            const string wallet = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            const SortDirectionType requestDirection = SortDirectionType.DESC;
            const uint limit = 10;

            var cursor = new AddressBalancesCursor(Enumerable.Empty<string>(), false, false, requestDirection, limit, PagingDirection.Backward, 50);

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
            const string wallet = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            const SortDirectionType requestDirection = SortDirectionType.ASC;
            const uint limit = 10;

            var cursor = new AddressBalancesCursor(Enumerable.Empty<string>(), false, false, requestDirection, limit, PagingDirection.Backward, 50);

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