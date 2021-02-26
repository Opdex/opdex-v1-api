using System.Dynamic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Opdex.Core.Domain.Models.Transaction;
using Opdex.Core.Domain.Models.Transaction.TransactionEvents;
using Opdex.Core.Infrastructure.Abstractions.Data;
using Opdex.Indexer.Infrastructure.Abstractions.Data.Commands.TransactionEvents;
using Opdex.Indexer.Infrastructure.Data.Handlers.TransactionEvents;
using Xunit;

namespace Opdex.Indexer.Infrastructure.Tests.Data.Handlers.TransactionEvents
{
    public class PersistTransactionSwapEventCommandHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly PersistTransactionSwapEventCommandHandler _handler;
        
        public PersistTransactionSwapEventCommandHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new IndexerInfrastructureMapperProfile())).CreateMapper();
            var logger = new NullLogger<PersistTransactionSwapEventCommandHandler>();
            
            _dbContext = new Mock<IDbContext>();
            _handler = new PersistTransactionSwapEventCommandHandler(_dbContext.Object, mapper, logger);
        }
        
        [Fact]
        public async Task PersistsSwapEvent_Success()
        {
            const string address = "SomeAddress";
            const int sortOrder = 0;
            
            dynamic txLogEvent = new ExpandoObject();
            txLogEvent.To = "To";
            txLogEvent.Sender = "Sender";
            txLogEvent.AmountCrsIn = 234ul;
            txLogEvent.AmountCrsOut = 0ul;
            txLogEvent.AmountSrcIn = "0";
            txLogEvent.AmountSrcOut = "1234543";

            var transactionLog = new SwapEvent(txLogEvent, address, sortOrder);
            var command = new PersistTransactionSwapEventCommand(transactionLog);
        
            _dbContext.Setup(db => db.ExecuteCommandAsync(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(1));
            
            var result = await _handler.Handle(command, CancellationToken.None);
        
            result.Should().BeTrue();
        }
        
        [Fact]
        public async Task PersistsSwapEvent_Fail()
        {
            const string address = "SomeAddress";
            const int sortOrder = 0;
            
            dynamic txLogEvent = new ExpandoObject();
            txLogEvent.To = "To";
            txLogEvent.Sender = "Sender";
            txLogEvent.AmountCrsIn = 234ul;
            txLogEvent.AmountCrsOut = 0ul;
            txLogEvent.AmountSrcIn = "0";
            txLogEvent.AmountSrcOut = "1234543";

            var transactionLog = new SwapEvent(txLogEvent, address, sortOrder);
            var command = new PersistTransactionSwapEventCommand(transactionLog);
        
            _dbContext.Setup(db => db.ExecuteCommandAsync(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(0));
            
            var result = await _handler.Handle(command, CancellationToken.None);
        
            result.Should().BeFalse();
        }
    }
}