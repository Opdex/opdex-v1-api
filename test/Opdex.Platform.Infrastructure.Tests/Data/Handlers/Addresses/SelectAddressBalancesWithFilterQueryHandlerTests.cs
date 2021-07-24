using AutoMapper;
using Moq;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses;
using Opdex.Platform.Infrastructure.Data.Handlers.Addresses;
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
            const long limit = 10;
            const long next = 0;
            const long previous = 0;

            var command = new SelectAddressBalancesWithFilterQuery(wallet, new string[0], false, false, direction, limit, next, previous);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo => callTo.ExecuteQueryAsync<AddressBalanceEntity>(
                                  It.Is<DatabaseQuery>(q => q.Sql.Contains("ab.Owner = @Wallet") &&
                                                            q.Sql.Contains($"ORDER BY ab.Id {direction}") &&
                                                            q.Sql.Contains($"LIMIT {limit + 1}"))), Times.Once);
        }

        [Fact]
        public async Task SelectAddressBalancesWithFilter_ByContracts()
        {
            // Arrange
            const SortDirectionType direction = SortDirectionType.ASC;
            const string wallet = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            var tokens = new[] {"PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj"};
            const long limit = 10;
            const long next = 0;
            const long previous = 0;

            var command = new SelectAddressBalancesWithFilterQuery(wallet, tokens, false, false, direction, limit, next, previous);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo =>
                                  callTo.ExecuteQueryAsync<AddressBalanceEntity>(
                                      It.Is<DatabaseQuery>(q => q.Sql.Contains("JOIN token t") &&
                                                                q.Sql.Contains("t.Address IN @Tokens") &&
                                                                q.Sql.Contains($"ORDER BY ab.Id {direction}") &&
                                                                q.Sql.Contains($"LIMIT {limit + 1}"))), Times.Once);
        }

        [Fact]
        public async Task SelectAddressBalancesWithFilter_ExcludeLpTokens()
        {
            // Arrange
            const SortDirectionType direction = SortDirectionType.ASC;
            const string wallet = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            const bool includeLpTokens = false;
            const long limit = 10;
            const long next = 0;
            const long previous = 0;

            var command = new SelectAddressBalancesWithFilterQuery(wallet, new string[0], includeLpTokens, false, direction, limit, next, previous);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo =>
                                  callTo.ExecuteQueryAsync<AddressBalanceEntity>(
                                      It.Is<DatabaseQuery>(q => q.Sql.Contains("JOIN token t") &&
                                                                q.Sql.Contains("t.IsLpt = @IncludeLpTokens") &&
                                                                q.Sql.Contains($"ORDER BY ab.Id {direction}") &&
                                                                q.Sql.Contains($"LIMIT {limit + 1}"))), Times.Once);
        }

        [Fact]
        public async Task SelectAddressBalancesWithFilter_ExcludeZeroBalanceTokens()
        {
            // Arrange
            const SortDirectionType direction = SortDirectionType.ASC;
            const string wallet = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            const bool includeZeroBalance = false;
            const long limit = 10;
            const long next = 0;
            const long previous = 0;

            var command = new SelectAddressBalancesWithFilterQuery(wallet, new string[0], true, includeZeroBalance, direction, limit, next, previous);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo =>
                                  callTo.ExecuteQueryAsync<AddressBalanceEntity>(
                                      It.Is<DatabaseQuery>(q => q.Sql.Contains("ab.Balance != '0'") &&
                                                                q.Sql.Contains($"ORDER BY ab.Id {direction}") &&
                                                                q.Sql.Contains($"LIMIT {limit + 1}"))), Times.Once);
        }

        [Fact]
        public async Task SelectAddressBalancesWithFilter_ByCursor_NextASC()
        {
            // Arrange
            const string wallet = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            const SortDirectionType direction = SortDirectionType.ASC;
            const long limit = 10;
            const long next = 15;
            const long previous = 0;

            var command = new SelectAddressBalancesWithFilterQuery(wallet, new string[0], true, false, direction, limit, next, previous);

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
            const long limit = 10;
            const long next = 15;
            const long previous = 0;

            var command = new SelectAddressBalancesWithFilterQuery(wallet, new string[0], true, false, direction, limit, next, previous);

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
            const long limit = 10;
            const long next = 0;
            const long previous = 15;
            const SortDirectionType queryDirection = SortDirectionType.ASC;

            var command = new SelectAddressBalancesWithFilterQuery(wallet, new string[0], true, false, requestDirection, limit, next, previous);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo =>
                                  callTo.ExecuteQueryAsync<AddressBalanceEntity>(
                                      It.Is<DatabaseQuery>(q => q.Sql.Contains("ab.Id > @BalanceId") &&
                                                                q.Sql.Contains($"ORDER BY ab.Id {queryDirection}") &&
                                                                q.Sql.Contains($"LIMIT {limit + 1}"))), Times.Once);
        }

        [Fact]
        public async Task SelectAddressBalancesWithFilter_ByCursor_PreviousASC()
        {
            // Arrange
            const string wallet = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            const SortDirectionType requestDirection = SortDirectionType.ASC;
            const long limit = 10;
            const long next = 0;
            const long previous = 15;
            const SortDirectionType queryDirection = SortDirectionType.DESC;

            var command = new SelectAddressBalancesWithFilterQuery(wallet, new string[0], true, false, requestDirection, limit, next, previous);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo =>
                                  callTo.ExecuteQueryAsync<AddressBalanceEntity>(
                                      It.Is<DatabaseQuery>(q => q.Sql.Contains("ab.Id < @BalanceId") &&
                                                                q.Sql.Contains($"ORDER BY ab.Id {queryDirection}") &&
                                                                q.Sql.Contains($"LIMIT {limit + 1}"))), Times.Once);
        }
    }
}
