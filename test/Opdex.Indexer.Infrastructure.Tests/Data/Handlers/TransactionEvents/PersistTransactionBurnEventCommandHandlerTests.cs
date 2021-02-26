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

namespace Opdex.Indexer.Infrastructure.Tests.Data.Handlers.TransactionEvents
{
    public class PersistTransactionBurnEventCommandHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly PersistTransactionBurnEventCommandHandler _handler;
        
        public PersistTransactionBurnEventCommandHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new IndexerInfrastructureMapperProfile())).CreateMapper();
            var logger = new NullLogger<PersistTransactionBurnEventCommandHandler>();
            
            _dbContext = new Mock<IDbContext>();
            _handler = new PersistTransactionBurnEventCommandHandler(_dbContext.Object, mapper, logger);
        }
        
        [Fact]
        public async Task PersistsBurnEvent_Success()
        {
            const string address = "SomeAddress";
            const int sortOrder = 0;
            
            dynamic txLogEvent = new ExpandoObject();
            txLogEvent.Sender = "Sender";
            txLogEvent.To = "To";
            txLogEvent.AmountCrs = 123ul;
            txLogEvent.AmountSrc = "12344";

            var transactionLog = new BurnEvent(txLogEvent, address, sortOrder);
            var command = new PersistTransactionBurnEventCommand(transactionLog);
        
            _dbContext.Setup(db => db.ExecuteCommandAsync(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(1));
            
            var result = await _handler.Handle(command, CancellationToken.None);
        
            result.Should().BeTrue();
        }
        
        [Fact]
        public async Task PersistsBurnEvent_Fail()
        {
            const string address = "SomeAddress";
            const int sortOrder = 0;
            
            dynamic txLogEvent = new ExpandoObject();
            txLogEvent.Sender = "Sender";
            txLogEvent.To = "To";
            txLogEvent.AmountCrs = 123ul;
            txLogEvent.AmountSrc = "12344";

            var transactionLog = new BurnEvent(txLogEvent, address, sortOrder);
            var command = new PersistTransactionBurnEventCommand(transactionLog);
        
            _dbContext.Setup(db => db.ExecuteCommandAsync(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(0));
            
            var result = await _handler.Handle(command, CancellationToken.None);
        
            result.Should().BeFalse();
        }
    }
}