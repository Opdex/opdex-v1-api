using AutoMapper;
using Moq;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Transactions;
using Opdex.Platform.Infrastructure.Data.Handlers.Transactions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Transactions
{
    public class SelectTransactionsWithFilterQueryHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly SelectTransactionsWithFilterQueryHandler _handler;

        public SelectTransactionsWithFilterQueryHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();

            _dbContext = new Mock<IDbContext>();
            _handler = new SelectTransactionsWithFilterQueryHandler(_dbContext.Object, mapper);
        }

        [Fact]
        public async Task SelectTransactionWithFilter_ByWallet()
        {
            // Arrange
            const string wallet = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            const SortDirectionType direction = SortDirectionType.ASC;
            const long limit = 10;
            const long next = 0;
            const long previous = 0;

            var command = new SelectTransactionsWithFilterQuery(wallet, new uint[0], new string[0], direction, limit, next, previous);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo =>
                                  callTo.ExecuteQueryAsync<TransactionEntity>(It.Is<DatabaseQuery>(q => q.Sql.Contains("t.`From` = @Wallet") &&
                                                                                                        q.Sql.Contains($"ORDER BY t.Id {direction}") &&
                                                                                                        q.Sql.Contains($"LIMIT {limit + 1}"))), Times.Once);
        }

        [Fact]
        public async Task SelectTransactionWithFilter_ByContracts()
        {
            // Arrange
            const SortDirectionType direction = SortDirectionType.ASC;
            var contracts = new[] {"PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj"};
            const long limit = 10;
            const long next = 0;
            const long previous = 0;

            var command = new SelectTransactionsWithFilterQuery(string.Empty, new uint[0], contracts, direction, limit, next, previous);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo =>
                                  callTo.ExecuteQueryAsync<TransactionEntity>(It.Is<DatabaseQuery>(q => q.Sql.Contains("JOIN transaction_log tl") &&
                                                                                                        q.Sql.Contains("tl.Contract IN @Contracts") &&
                                                                                                        q.Sql.Contains($"ORDER BY t.Id {direction}") &&
                                                                                                        q.Sql.Contains($"LIMIT {limit + 1}"))), Times.Once);
        }

        [Fact]
        public async Task SelectTransactionWithFilter_ByEventTypes()
        {
            // Arrange
            const SortDirectionType direction = SortDirectionType.ASC;
            var eventTypes = new uint[] {1};
            const long limit = 10;
            const long next = 0;
            const long previous = 0;

            var command = new SelectTransactionsWithFilterQuery(string.Empty, eventTypes, new string[0], direction, limit, next, previous);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo =>
                                  callTo.ExecuteQueryAsync<TransactionEntity>(It.Is<DatabaseQuery>(q => q.Sql.Contains("JOIN transaction_log tl") &&
                                                                                                        q.Sql.Contains("tl.LogTypeId IN @LogTypes") &&
                                                                                                        q.Sql.Contains($"ORDER BY t.Id {direction}") &&
                                                                                                        q.Sql.Contains($"LIMIT {limit + 1}"))), Times.Once);
        }

        [Fact]
        public async Task SelectTransactionWithFilter_ByCursor_NextASC()
        {
            // Arrange
            const SortDirectionType direction = SortDirectionType.ASC;
            const long limit = 10;
            const long next = 15;
            const long previous = 0;

            var command = new SelectTransactionsWithFilterQuery(string.Empty, new uint[0], new string[0], direction, limit, next, previous);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo =>
                                  callTo.ExecuteQueryAsync<TransactionEntity>(It.Is<DatabaseQuery>(q => q.Sql.Contains("WHERE t.Id > @TransactionId") &&
                                                                                                        q.Sql.Contains($"ORDER BY t.Id {direction}") &&
                                                                                                        q.Sql.Contains($"LIMIT {limit + 1}"))), Times.Once);
        }

        [Fact]
        public async Task SelectTransactionWithFilter_ByCursor_NextDESC()
        {
            // Arrange
            const SortDirectionType direction = SortDirectionType.DESC;
            const long limit = 10;
            const long next = 15;
            const long previous = 0;

            var command = new SelectTransactionsWithFilterQuery(string.Empty, new uint[0], new string[0], direction, limit, next, previous);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo =>
                                  callTo.ExecuteQueryAsync<TransactionEntity>(It.Is<DatabaseQuery>(q => q.Sql.Contains("WHERE t.Id < @TransactionId") &&
                                                                                                        q.Sql.Contains($"ORDER BY t.Id {direction}") &&
                                                                                                        q.Sql.Contains($"LIMIT {limit + 1}"))), Times.Once);
        }

        [Fact]
        public async Task SelectTransactionWithFilter_ByCursor_PreviousDESC()
        {
            // Arrange
            const SortDirectionType requestDirection = SortDirectionType.DESC;
            const long limit = 10;
            const long next = 0;
            const long previous = 15;
            const SortDirectionType queryDirection = SortDirectionType.ASC;

            var command = new SelectTransactionsWithFilterQuery(string.Empty, new uint[0], new string[0], requestDirection, limit, next, previous);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo =>
                                  callTo.ExecuteQueryAsync<TransactionEntity>(It.Is<DatabaseQuery>(q => q.Sql.Contains("WHERE t.Id > @TransactionId") &&
                                                                                                        q.Sql.Contains($"ORDER BY t.Id {queryDirection}") &&
                                                                                                        q.Sql.Contains($"LIMIT {limit + 1}"))), Times.Once);
        }

        [Fact]
        public async Task SelectTransactionWithFilter_ByCursor_PreviousASC()
        {
            // Arrange
            const SortDirectionType requestDirection = SortDirectionType.ASC;
            const long limit = 10;
            const long next = 0;
            const long previous = 15;
            const SortDirectionType queryDirection = SortDirectionType.DESC;

            var command = new SelectTransactionsWithFilterQuery(string.Empty, new uint[0], new string[0], requestDirection, limit, next, previous);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo =>
                                  callTo.ExecuteQueryAsync<TransactionEntity>(It.Is<DatabaseQuery>(q => q.Sql.Contains("WHERE t.Id < @TransactionId") &&
                                                                                                        q.Sql.Contains($"ORDER BY t.Id {queryDirection}") &&
                                                                                                        q.Sql.Contains($"LIMIT {limit + 1}"))), Times.Once);
        }
    }
}
