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
    public class PersistTransactionMintEventCommandHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly PersistTransactionMintEventCommandHandler _handler;
        
        public PersistTransactionMintEventCommandHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new IndexerInfrastructureMapperProfile())).CreateMapper();
            var logger = new NullLogger<PersistTransactionMintEventCommandHandler>();
            
            _dbContext = new Mock<IDbContext>();
            _handler = new PersistTransactionMintEventCommandHandler(_dbContext.Object, mapper, logger);
        }
        
        [Fact]
        public async Task PersistsMintEvent_Success()
        {
            const string address = "SomeAddress";
            const int sortOrder = 0;
            
            dynamic txLogEvent = new ExpandoObject();
            txLogEvent.Sender = "Sender";
            txLogEvent.AmountCrs = 123ul;
            txLogEvent.AmountSrc = "12344";

            var transactionLog = new MintEvent(txLogEvent, address, sortOrder);
            var command = new PersistTransactionMintEventCommand(transactionLog);
        
            _dbContext.Setup(db => db.ExecuteCommandAsync(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(1));
            
            var result = await _handler.Handle(command, CancellationToken.None);
        
            result.Should().BeGreaterThan(0);
        }
        
        [Fact]
        public async Task PersistsMintEvent_Fail()
        {
            const string address = "SomeAddress";
            const int sortOrder = 0;
            
            dynamic txLogEvent = new ExpandoObject();
            txLogEvent.Sender = "Sender";
            txLogEvent.AmountCrs = 123ul;
            txLogEvent.AmountSrc = "12344";

            var transactionLog = new MintEvent(txLogEvent, address, sortOrder);
            var command = new PersistTransactionMintEventCommand(transactionLog);
        
            _dbContext.Setup(db => db.ExecuteCommandAsync(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(0));
            
            var result = await _handler.Handle(command, CancellationToken.None);
        
            result.Should().Be(0);
        }
    }
}