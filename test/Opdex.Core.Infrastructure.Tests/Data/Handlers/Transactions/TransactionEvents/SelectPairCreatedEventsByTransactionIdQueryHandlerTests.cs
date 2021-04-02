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
    public class SelectPairCreatedEventsByTransactionIdQueryHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly SelectPairCreatedEventsByTransactionIdQueryHandler _handler;
        
        public SelectPairCreatedEventsByTransactionIdQueryHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new CoreInfrastructureMapperProfile())).CreateMapper();
            
            _dbContext = new Mock<IDbContext>();
            _handler = new SelectPairCreatedEventsByTransactionIdQueryHandler(_dbContext.Object, mapper);
        }

        [Fact]
        public async Task SelectPairCreatedEventByTransactionId_Success()
        {
            const long transactionId = 10;
            var expectedResponse = new PairCreatedEventEntity
            {
                Id = 23423,
                TransactionId = transactionId,
                Address = "SomeAddress",
                SortOrder = 7,
                Token = "SomeToken",
                Pair = "SomePair"
            };

            var responseList = new List<PairCreatedEventEntity> {expectedResponse}.AsEnumerable();
            
            var command = new SelectPairCreatedEventsByTransactionIdQuery(new [] { new TransactionEventSummary(1,1,1,1,1,"1") });
        
            _dbContext.Setup(db => db.ExecuteQueryAsync<PairCreatedEventEntity>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(responseList));
            
            var results = await _handler.Handle(command, CancellationToken.None);

            foreach (var result in results)
            {
                result.Id.Should().Be(expectedResponse.Id);
                result.TransactionId.Should().Be(expectedResponse.TransactionId);
                result.Address.Should().Be(expectedResponse.Address);
                result.SortOrder.Should().Be(expectedResponse.SortOrder);
                result.Token.Should().Be(expectedResponse.Token);
                result.Pair.Should().Be(expectedResponse.Pair);
            }
        }
    }
}