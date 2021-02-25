using System.Dynamic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Opdex.Core.Domain.Models.TransactionReceipt;
using Opdex.Core.Domain.Models.TransactionReceipt.LogEvents;
using Opdex.Core.Infrastructure.Abstractions.Data;
using Opdex.Indexer.Infrastructure.Abstractions.Data.Commands.TransactionEvents;
using Opdex.Indexer.Infrastructure.Data.Handlers.TransactionEvents;
using Xunit;

namespace Opdex.Indexer.Infrastructure.Tests.Data.Handlers.TransactionEvents
{
    public class PersistTransactionTransferEventCommandHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly PersistTransactionTransferEventCommandHandler _handler;
        
        public PersistTransactionTransferEventCommandHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new IndexerInfrastructureMapperProfile())).CreateMapper();
            var logger = new NullLogger<PersistTransactionTransferEventCommandHandler>();
            
            _dbContext = new Mock<IDbContext>();
            _handler = new PersistTransactionTransferEventCommandHandler(_dbContext.Object, mapper, logger);
        }
        
        [Fact]
        public async Task PersistsTransferEvent_Success()
        {
            dynamic transactionLogObject = new ExpandoObject();
            transactionLogObject.Address = "SomeAddress";
            transactionLogObject.Topics = new [] {"TransferEvent"};
            dynamic txLogEvent = new ExpandoObject();
            txLogEvent.To = "To";
            txLogEvent.From = "From";
            txLogEvent.Amount = "1234543";
            transactionLogObject.Log = txLogEvent;

            var transactionLog = new TransactionLog(transactionLogObject);
            var command = new PersistTransactionTransferEventCommand(transactionLog.Event as TransferEvent);
        
            _dbContext.Setup(db => db.ExecuteCommandAsync(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(1));
            
            var result = await _handler.Handle(command, CancellationToken.None);
        
            result.Should().BeTrue();
        }
        
        [Fact]
        public async Task PersistsTransferEvent_Fail()
        {
            dynamic transactionLogObject = new ExpandoObject();
            transactionLogObject.Address = "SomeAddress";
            transactionLogObject.Topics = new [] {"TransferEvent"};
            dynamic txLogEvent = new ExpandoObject();
            txLogEvent.To = "To";
            txLogEvent.From = "From";
            txLogEvent.Amount = "1234543";
            transactionLogObject.Log = txLogEvent;

            var transactionLog = new TransactionLog(transactionLogObject);
            var command = new PersistTransactionTransferEventCommand(transactionLog.Event as TransferEvent);
        
            _dbContext.Setup(db => db.ExecuteCommandAsync(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(0));
            
            var result = await _handler.Handle(command, CancellationToken.None);
        
            result.Should().BeFalse();
        }
    }
}