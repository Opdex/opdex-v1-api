using System.Dynamic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Opdex.Core.Domain.Models;
using Opdex.Core.Domain.Models.TransactionEvents;
using Opdex.Core.Infrastructure.Abstractions.Data;
using Opdex.Indexer.Infrastructure.Abstractions.Data.Commands.TransactionEvents;
using Opdex.Indexer.Infrastructure.Data.Handlers.TransactionEvents;
using Xunit;

namespace Opdex.Indexer.Infrastructure.Tests.Data.Handlers.Transactions.TransactionEvents
{
    public class PersistTransactionSyncEventCommandHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly PersistTransactionSyncEventCommandHandler _handler;
        
        public PersistTransactionSyncEventCommandHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new IndexerInfrastructureMapperProfile())).CreateMapper();
            var logger = new NullLogger<PersistTransactionSyncEventCommandHandler>();
            
            _dbContext = new Mock<IDbContext>();
            _handler = new PersistTransactionSyncEventCommandHandler(_dbContext.Object, mapper, logger);
        }
        
        [Fact]
        public async Task PersistsSyncEvent_Success()
        {
            const string address = "SomeAddress";
            const int sortOrder = 0;
            
            dynamic txLogEvent = new ExpandoObject();
            txLogEvent.ReserveCrs = 12345ul;
            txLogEvent.ReserveSrc = "1234543";

            var transactionLog = new SyncEvent(txLogEvent, address, sortOrder);
            var command = new PersistTransactionSyncEventCommand(transactionLog);
        
            _dbContext.Setup(db => db.ExecuteCommandAsync(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(1));
            
            var result = await _handler.Handle(command, CancellationToken.None);
        
            result.Should().BeGreaterThan(0);
        }
        
        [Fact]
        public async Task PersistsSyncEvent_Fail()
        {
            const string address = "SomeAddress";
            const int sortOrder = 0;
            
            dynamic txLogEvent = new ExpandoObject();
            txLogEvent.ReserveCrs = 12345ul;
            txLogEvent.ReserveSrc = "1234543";

            var transactionLog = new SyncEvent(txLogEvent, address, sortOrder);
            var command = new PersistTransactionSyncEventCommand(transactionLog);
        
            _dbContext.Setup(db => db.ExecuteCommandAsync(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(0));
            
            var result = await _handler.Handle(command, CancellationToken.None);
        
            result.Should().Be(0);
        }
    }
}