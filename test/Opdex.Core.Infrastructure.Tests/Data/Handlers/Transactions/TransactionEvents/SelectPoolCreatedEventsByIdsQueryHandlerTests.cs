using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Core.Common.Exceptions;
using Opdex.Core.Domain.Models.TransactionEvents;
using Opdex.Core.Infrastructure.Abstractions.Data;
using Opdex.Core.Infrastructure.Abstractions.Data.Models.TransactionEvents;
using Opdex.Core.Infrastructure.Abstractions.Data.Queries.Transactions.TransactionEvents;
using Opdex.Core.Infrastructure.Data.Handlers.Transactions.TransactionEvents;
using Xunit;

namespace Opdex.Core.Infrastructure.Tests.Data.Handlers.Transactions.TransactionEvents
{
    public class SelectPoolCreatedEventsByIdsQueryHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly SelectPoolCreatedEventsByIdsQueryHandler _handler;
        
        public SelectPoolCreatedEventsByIdsQueryHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new CoreInfrastructureMapperProfile())).CreateMapper();
            
            _dbContext = new Mock<IDbContext>();
            _handler = new SelectPoolCreatedEventsByIdsQueryHandler(_dbContext.Object, mapper);
        }

        [Fact]
        public async Task SelectPoolCreatedEventByTransactionId_Success()
        {
            const long transactionId = 10;
            var expectedResponse = new PoolCreatedEventEntity
            {
                Id = 23423,
                TransactionId = transactionId,
                Address = "SomeAddress",
                SortOrder = 7,
                Token = "SomeToken",
                Pool = "SomePool"
            };

            var responseList = new List<PoolCreatedEventEntity> {expectedResponse}.AsEnumerable();
            
            var command = new SelectPoolCreatedEventsByIdsQuery(new [] { new TransactionEventSummary(1,1,1,1,1,"1") });
        
            _dbContext.Setup(db => db.ExecuteQueryAsync<PoolCreatedEventEntity>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(responseList));
            
            var results = await _handler.Handle(command, CancellationToken.None);

            foreach (var result in results)
            {
                result.Id.Should().Be(expectedResponse.Id);
                result.TransactionId.Should().Be(expectedResponse.TransactionId);
                result.Address.Should().Be(expectedResponse.Address);
                result.SortOrder.Should().Be(expectedResponse.SortOrder);
                result.Token.Should().Be(expectedResponse.Token);
                result.Pool.Should().Be(expectedResponse.Pool);
            }
        }
    }
}