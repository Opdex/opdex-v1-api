using AutoMapper;
using Moq;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Transactions;
using Opdex.Platform.Infrastructure.Data.Handlers.Transactions;
using System.Linq;
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
            const uint limit = 10;

            var cursor = new TransactionsCursor(wallet, Enumerable.Empty<TransactionEventType>(),
                                                Enumerable.Empty<string>(), direction, limit, PagingDirection.Forward, 0);

            var command = new SelectTransactionsWithFilterQuery(cursor);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo =>
                                  callTo.ExecuteQueryAsync<TransactionEntity>(It.Is<DatabaseQuery>(q => q.Sql.Contains("t.`From` = @Wallet") &&
                                                                                                        q.Sql.Contains($"ORDER BY t.Id {direction}") &&
                                                                                                        q.Sql.Contains($"LIMIT {limit + 1}"))), Times.Once);
        }

        [Fact]
        public async Task SelectTransactionsWithFilter_ByContracts()
        {
            // Arrange
            var contracts = new[] { "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj" };
            const SortDirectionType direction = SortDirectionType.ASC;
            const uint limit = 10;

            var cursor = new TransactionsCursor("", Enumerable.Empty<TransactionEventType>(),
                                                contracts, direction, limit, PagingDirection.Forward, 0);

            var command = new SelectTransactionsWithFilterQuery(cursor);

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
        public async Task SelectTransactionsWithFilter_ByEventTypes()
        {
            // Arrange
            var eventTypes = new TransactionEventType[] { TransactionEventType.CreateMarketEvent };
            const SortDirectionType direction = SortDirectionType.ASC;
            const uint limit = 10;

            var cursor = new TransactionsCursor("", eventTypes, Enumerable.Empty<string>(), direction, limit, PagingDirection.Forward, 0);

            var command = new SelectTransactionsWithFilterQuery(cursor);

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
        public async Task SelectTransactionsWithFilter_ByCursor_NextASC()
        {
            // Arrange
            const SortDirectionType direction = SortDirectionType.ASC;
            const PagingDirection pagingDirection = PagingDirection.Forward;
            const uint limit = 10;

            var cursor = new TransactionsCursor("", Enumerable.Empty<TransactionEventType>(),
                                                Enumerable.Empty<string>(), direction, limit, pagingDirection, 50);

            var command = new SelectTransactionsWithFilterQuery(cursor);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo =>
                                  callTo.ExecuteQueryAsync<TransactionEntity>(It.Is<DatabaseQuery>(q => q.Sql.Contains("WHERE t.Id > @TransactionId") &&
                                                                                                        q.Sql.Contains($"ORDER BY t.Id {direction}") &&
                                                                                                        q.Sql.Contains($"LIMIT {limit + 1}"))), Times.Once);
        }

        [Fact]
        public async Task SelectTransactionsWithFilter_ByCursor_NextDESC()
        {
            // Arrange
            const SortDirectionType direction = SortDirectionType.DESC;
            const PagingDirection pagingDirection = PagingDirection.Forward;
            const uint limit = 10;

            var cursor = new TransactionsCursor("", Enumerable.Empty<TransactionEventType>(),
                                                Enumerable.Empty<string>(), direction, limit, pagingDirection, 50);

            var command = new SelectTransactionsWithFilterQuery(cursor);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo =>
                                  callTo.ExecuteQueryAsync<TransactionEntity>(It.Is<DatabaseQuery>(q => q.Sql.Contains("WHERE t.Id < @TransactionId") &&
                                                                                                        q.Sql.Contains($"ORDER BY t.Id {direction}") &&
                                                                                                        q.Sql.Contains($"LIMIT {limit + 1}"))), Times.Once);
        }

        [Fact]
        public async Task SelectTransactionsWithFilter_ByCursor_PreviousDESC()
        {
            // Arrange
            const SortDirectionType direction = SortDirectionType.DESC;
            const PagingDirection pagingDirection = PagingDirection.Backward;
            const uint limit = 10;

            var cursor = new TransactionsCursor("", Enumerable.Empty<TransactionEventType>(),
                                                Enumerable.Empty<string>(), direction, limit, pagingDirection, 50);

            var command = new SelectTransactionsWithFilterQuery(cursor);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo =>
                                  callTo.ExecuteQueryAsync<TransactionEntity>(It.Is<DatabaseQuery>(q => q.Sql.Contains("WHERE t.Id > @TransactionId") &&
                                                                                                        q.Sql.Contains($"ORDER BY t.Id ASC") &&
                                                                                                        q.Sql.Contains($"LIMIT {limit + 1}"))), Times.Once);
        }

        [Fact]
        public async Task SelectTransactionsWithFilter_ByCursor_PreviousASC()
        {
            // Arrange
            const SortDirectionType direction = SortDirectionType.ASC;
            const PagingDirection pagingDirection = PagingDirection.Backward;
            const uint limit = 10;

            var cursor = new TransactionsCursor("", Enumerable.Empty<TransactionEventType>(),
                                                Enumerable.Empty<string>(), direction, limit, pagingDirection, 50);

            var command = new SelectTransactionsWithFilterQuery(cursor);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo =>
                                  callTo.ExecuteQueryAsync<TransactionEntity>(It.Is<DatabaseQuery>(q => q.Sql.Contains("WHERE t.Id < @TransactionId") &&
                                                                                                        q.Sql.Contains($"ORDER BY t.Id DESC") &&
                                                                                                        q.Sql.Contains($"LIMIT {limit + 1}"))), Times.Once);
        }
    }
}
