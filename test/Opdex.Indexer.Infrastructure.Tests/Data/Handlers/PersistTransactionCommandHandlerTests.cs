using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Core.Domain.Models.TransactionReceipt;
using Opdex.Core.Infrastructure.Abstractions.Data;
using Opdex.Indexer.Infrastructure.Abstractions.Data.Commands;
using Opdex.Indexer.Infrastructure.Data.Handlers;
using Xunit;

namespace Opdex.Indexer.Infrastructure.Tests.Data.Handlers
{
    public class PersistTransactionCommandHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly PersistTransactionCommandHandler _handler;
        
        public PersistTransactionCommandHandlerTests()
        {
            _dbContext = new Mock<IDbContext>();
            var mapper = new MapperConfiguration(config => config.AddProfile(new IndexerInfrastructureMapperProfile())).CreateMapper();
            _handler = new PersistTransactionCommandHandler(_dbContext.Object, mapper);
        }

        [Fact]
        public async Task PersistsTransaction_Success()
        {
            var transaction = new TransactionReceipt("txHash", ulong.MaxValue, 1, "from", "to", true, new dynamic[] { new object() });
            var command = new PersistTransactionCommand(transaction);
        
            _dbContext.Setup(db => db.ExecuteScalarAsync<long>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(1234L));
            
            var result = await _handler.Handle(command, CancellationToken.None);
        
            result.Should().Be(1234);
        }
        
        [Fact]
        public async Task PersistsTransaction_Fail()
        {
            var transaction = new TransactionReceipt("txHash", ulong.MaxValue, 1, "from", "to", true, new dynamic[] { new object() });
            var command = new PersistTransactionCommand(transaction);
        
            _dbContext.Setup(db => db.ExecuteScalarAsync<long>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(0L));
        
            var result = await _handler.Handle(command, CancellationToken.None);
        
            result.Should().Be(0);
        }
    }
}