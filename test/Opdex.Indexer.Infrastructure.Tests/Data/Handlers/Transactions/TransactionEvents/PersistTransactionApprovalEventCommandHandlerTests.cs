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
            const string address = "SomeAddress";
            const int sortOrder = 0;
            
            dynamic txLogEvent = new ExpandoObject();
            txLogEvent.Owner = "Owner";
            txLogEvent.Spender = "Spender";
            txLogEvent.Amount = "Amount";

            var transactionLog = new ApprovalEvent(txLogEvent, address, sortOrder);
            var command = new PersistTransactionApprovalEventCommand(transactionLog);
        
            _dbContext.Setup(db => db.ExecuteCommandAsync(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(1));
            
            var result = await _handler.Handle(command, CancellationToken.None);
        
            result.Should().BeGreaterThan(0);
        }
        
        [Fact]
        public async Task PersistsApprovalEvent_Fail()
        {
            const string address = "SomeAddress";
            const int sortOrder = 0;
            
            dynamic txLogEvent = new ExpandoObject();
            txLogEvent.Owner = "Owner";
            txLogEvent.Spender = "Spender";
            txLogEvent.Amount = "Amount";

            var transactionLog = new ApprovalEvent(txLogEvent, address, sortOrder);
            var command = new PersistTransactionApprovalEventCommand(transactionLog);
        
            _dbContext.Setup(db => db.ExecuteCommandAsync(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(0));
            
            var result = await _handler.Handle(command, CancellationToken.None);
        
            result.Should().Be(0);
        }
    }
}