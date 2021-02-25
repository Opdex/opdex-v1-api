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
    public class PersistTransactionApprovalEventCommandHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly PersistTransactionApprovalEventCommandHandler _handler;
        
        public PersistTransactionApprovalEventCommandHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new IndexerInfrastructureMapperProfile())).CreateMapper();
            var logger = new NullLogger<PersistTransactionApprovalEventCommandHandler>();
            
            _dbContext = new Mock<IDbContext>();
            _handler = new PersistTransactionApprovalEventCommandHandler(_dbContext.Object, mapper, logger);
        }
        
        [Fact]
        public async Task PersistsApprovalEvent_Success()
        {
            dynamic transactionLogObject = new ExpandoObject();
            transactionLogObject.Address = "SomeAddress";
            transactionLogObject.Topics = new [] {"ApprovalEvent"};
            dynamic txLogEvent = new ExpandoObject();
            txLogEvent.Owner = "Owner";
            txLogEvent.Spender = "Spender";
            txLogEvent.Amount = "Amount";
            transactionLogObject.Log = txLogEvent;

            var transactionLog = new TransactionLog(transactionLogObject);
            var command = new PersistTransactionApprovalEventCommand(transactionLog.Event as ApprovalEvent);
        
            _dbContext.Setup(db => db.ExecuteCommandAsync(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(1));
            
            var result = await _handler.Handle(command, CancellationToken.None);
        
            result.Should().BeTrue();
        }
        
        [Fact]
        public async Task PersistsApprovalEvent_Fail()
        {
            dynamic transactionLogObject = new ExpandoObject();
            transactionLogObject.Address = "SomeAddress";
            transactionLogObject.Topics = new [] {"ApprovalEvent"};
            dynamic txLogEvent = new ExpandoObject();
            txLogEvent.Owner = "Owner";
            txLogEvent.Spender = "Spender";
            txLogEvent.Amount = "Amount";
            transactionLogObject.Log = txLogEvent;

            var transactionLog = new TransactionLog(transactionLogObject);
            var command = new PersistTransactionApprovalEventCommand(transactionLog.Event as ApprovalEvent);
        
            _dbContext.Setup(db => db.ExecuteCommandAsync(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(0));
            
            var result = await _handler.Handle(command, CancellationToken.None);
        
            result.Should().BeFalse();
        }
    }
}