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
    public class SelectSyncEventByTransactionIdQueryHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly SelectSyncEventByTransactionIdQueryHandler _handler;
        
        public SelectSyncEventByTransactionIdQueryHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new CoreInfrastructureMapperProfile())).CreateMapper();
            
            _dbContext = new Mock<IDbContext>();
            _handler = new SelectSyncEventByTransactionIdQueryHandler(_dbContext.Object, mapper);
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
            
            var command = new SelectSyncEventByTransactionIdQuery(transactionId);
        
            _dbContext.Setup(db => db.ExecuteFindAsync<SyncEventEntity>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(expectedResponse));
            
            var result = await _handler.Handle(command, CancellationToken.None);

            result.Id.Should().Be(expectedResponse.Id);
            result.TransactionId.Should().Be(expectedResponse.TransactionId);
            result.Address.Should().Be(expectedResponse.Address);
            result.SortOrder.Should().Be(expectedResponse.SortOrder);
            result.ReserveCrs.Should().Be(expectedResponse.ReserveCrs);
            result.ReserveSrc.Should().Be(expectedResponse.ReserveSrc);
        }
        
        [Fact]
        public void SelectSyncEventByTransactionId_Throws_NotFoundException()
        {
            const long transactionId = 10;
        
            var command = new SelectSyncEventByTransactionIdQuery(transactionId);
    
            _dbContext.Setup(db => db.ExecuteFindAsync<SyncEventEntity>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult<SyncEventEntity>(null));

            _handler.Invoking(h => h.Handle(command, CancellationToken.None))
                .Should()
                .Throw<NotFoundException>()
                .WithMessage($"{nameof(SyncEventEntity)} with transactionId {transactionId} was not found.");
        }
    }
}