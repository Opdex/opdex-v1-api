using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Core.Common.Exceptions;
using Opdex.Core.Infrastructure.Abstractions.Data;
using Opdex.Core.Infrastructure.Abstractions.Data.Models.TransactionEvents;
using Opdex.Core.Infrastructure.Abstractions.Data.Queries.Transactions.TransactionEvents;
using Opdex.Core.Infrastructure.Data.Handlers.Transactions.TransactionEvents;
using Xunit;

namespace Opdex.Core.Infrastructure.Tests.Data.Handlers.Transactions.TransactionEvents
{
    public class SelectSyncEventsByTransactionIdQueryHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly SelectSyncEventsByTransactionIdQueryHandler _handler;
        
        public SelectSyncEventsByTransactionIdQueryHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new CoreInfrastructureMapperProfile())).CreateMapper();
            
            _dbContext = new Mock<IDbContext>();
            _handler = new SelectSyncEventsByTransactionIdQueryHandler(_dbContext.Object, mapper);
        }

        [Fact]
        public async Task SelectSyncEventByTransactionId_Success()
        {
            const long transactionId = 10;
            var expectedResponse = new SyncEventEntity
            {
                Id = 23423,
                TransactionId = transactionId,
                Address = "SomeAddress",
                SortOrder = 7,
                ReserveCrs = 876545678,
                ReserveSrc = "987654567890"
            };
            
            var responseList = new List<SyncEventEntity> {expectedResponse}.AsEnumerable();

            var command = new SelectSyncEventsByTransactionIdQuery(transactionId);
        
            _dbContext.Setup(db => db.ExecuteQueryAsync<SyncEventEntity>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(responseList));
            
            var results = await _handler.Handle(command, CancellationToken.None);

            foreach (var result in results)
            {
                result.Id.Should().Be(expectedResponse.Id);
                result.TransactionId.Should().Be(expectedResponse.TransactionId);
                result.Address.Should().Be(expectedResponse.Address);
                result.SortOrder.Should().Be(expectedResponse.SortOrder);
                result.ReserveCrs.Should().Be(expectedResponse.ReserveCrs);
                result.ReserveSrc.Should().Be(expectedResponse.ReserveSrc);
            }
        }
    }
}