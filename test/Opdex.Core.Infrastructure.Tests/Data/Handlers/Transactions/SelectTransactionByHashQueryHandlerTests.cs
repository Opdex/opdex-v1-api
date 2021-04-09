using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Core.Common.Exceptions;
using Opdex.Core.Infrastructure.Abstractions.Data;
using Opdex.Core.Infrastructure.Abstractions.Data.Models;
using Opdex.Core.Infrastructure.Abstractions.Data.Queries.Transactions;
using Opdex.Core.Infrastructure.Data.Handlers.Transactions;
using Xunit;

namespace Opdex.Core.Infrastructure.Tests.Data.Handlers.Transactions
{
    public class SelectTransactionByHashQueryHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly SelectTransactionByHashQueryHandler _handler;
        
        public SelectTransactionByHashQueryHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new CoreInfrastructureMapperProfile())).CreateMapper();
            
            _dbContext = new Mock<IDbContext>();
            _handler = new SelectTransactionByHashQueryHandler(_dbContext.Object, mapper);
        }

        [Fact]
        public async Task SelectTransactionByHash_Success()
        {
            const string hash = "SomeHash";
            var expectedResponse = new TransactionEntity
            {
                Id = 23423,
                Block = 123432,
                From = "From",
                To = "To",
                GasUsed = 60923,
                Hash = hash,
                CreatedDate = DateTime.Now
            };
            
            var command = new SelectTransactionByHashQuery(hash);
        
            _dbContext.Setup(db => db.ExecuteFindAsync<TransactionEntity>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(expectedResponse));
            
            var result = await _handler.Handle(command, CancellationToken.None);

            result.Id.Should().Be(expectedResponse.Id);
            result.BlockHeight.Should().Be(expectedResponse.Block);
            result.Hash.Should().Be(expectedResponse.Hash);
            result.From.Should().Be(expectedResponse.From);
            result.To.Should().Be(expectedResponse.To);
            result.GasUsed.Should().Be(expectedResponse.GasUsed);
            result.Logs.Should().BeEmpty();
        }
        
        [Fact]
        public void SelectTransactionByHash_Throws_NotFoundException()
        {
            const string hash = "SomeHash";
            
            var command = new SelectTransactionByHashQuery(hash);
        
            _dbContext.Setup(db => db.ExecuteFindAsync<TransactionEntity>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult<TransactionEntity>(null));

            _handler.Invoking(h => h.Handle(command, CancellationToken.None))
                .Should()
                .Throw<NotFoundException>()
                .WithMessage($"{nameof(TransactionEntity)} with hash {hash} was not found.");
        }
    }
}