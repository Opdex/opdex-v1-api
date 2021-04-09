using System.Dynamic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Opdex.Core.Domain.Models;
using Opdex.Core.Domain.Models.TransactionLogs;
using Opdex.Core.Infrastructure.Abstractions.Data;
using Opdex.Indexer.Infrastructure.Abstractions.Data.Commands.TransactionLogs;
using Opdex.Indexer.Infrastructure.Data.Handlers.TransactionLogs;
using Xunit;

namespace Opdex.Indexer.Infrastructure.Tests.Data.Handlers.Transactions.TransactionLogs
{
    public class PersistTransactionApprovalLogCommandHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly PersistTransactionApprovalLogCommandHandler _handler;
        
        public PersistTransactionApprovalLogCommandHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new IndexerInfrastructureMapperProfile())).CreateMapper();
            var logger = new NullLogger<PersistTransactionApprovalLogCommandHandler>();
            
            _dbContext = new Mock<IDbContext>();
            _handler = new PersistTransactionApprovalLogCommandHandler(_dbContext.Object, mapper, logger);
        }
        
        [Fact]
        public async Task PersistsApprovalLog_Success()
        {
            const string address = "SomeAddress";
            const int sortOrder = 0;
            
            dynamic txLog = new ExpandoObject();
            txLog.Owner = "Owner";
            txLog.Spender = "Spender";
            txLog.Amount = "Amount";

            var transactionLog = new ApprovalLog(txLog, address, sortOrder);
            var command = new PersistTransactionApprovalLogCommand(transactionLog);
        
            _dbContext.Setup(db => db.ExecuteCommandAsync(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(1));
            
            var result = await _handler.Handle(command, CancellationToken.None);
        
            result.Should().BeGreaterThan(0);
        }
        
        [Fact]
        public async Task PersistsApprovalLog_Fail()
        {
            const string address = "SomeAddress";
            const int sortOrder = 0;
            
            dynamic txLog = new ExpandoObject();
            txLog.Owner = "Owner";
            txLog.Spender = "Spender";
            txLog.Amount = "Amount";

            var transactionLog = new ApprovalLog(txLog, address, sortOrder);
            var command = new PersistTransactionApprovalLogCommand(transactionLog);
        
            _dbContext.Setup(db => db.ExecuteCommandAsync(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(0));
            
            var result = await _handler.Handle(command, CancellationToken.None);
        
            result.Should().Be(0);
        }
    }
}