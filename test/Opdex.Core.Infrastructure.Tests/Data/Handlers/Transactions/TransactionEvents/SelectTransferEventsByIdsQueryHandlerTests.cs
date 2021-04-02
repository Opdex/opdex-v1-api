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
    public class SelectTransferEventByIdsQueryHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly SelectTransferEventsByIdsQueryHandler _handler;
        
        public SelectTransferEventByIdsQueryHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new CoreInfrastructureMapperProfile())).CreateMapper();
            
            _dbContext = new Mock<IDbContext>();
            _handler = new SelectTransferEventsByIdsQueryHandler(_dbContext.Object, mapper);
        }

        [Fact]
        public async Task SelectTransferEventByTransactionId_Success()
        {
            const long transactionId = 10;
            var expectedResponse = new TransferEventEntity
            {
                Id = 23423,
                TransactionId = transactionId,
                Address = "SomeAddress",
                SortOrder = 7,
                From = "SomeFrom",
                To = "SomeTo",
                Amount = "2398478237"
            };
            
            var responseList = new List<TransferEventEntity> {expectedResponse}.AsEnumerable();

            var command = new SelectTransferEventsByIdsQuery(new [] { new TransactionEventSummary(1,1,1,1,1,"1") });
        
            _dbContext.Setup(db => db.ExecuteQueryAsync<TransferEventEntity>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(responseList));
            
            var results = await _handler.Handle(command, CancellationToken.None);

            foreach (var result in results)
            {
                result.Id.Should().Be(expectedResponse.Id);
                result.TransactionId.Should().Be(expectedResponse.TransactionId);
                result.Address.Should().Be(expectedResponse.Address);
                result.SortOrder.Should().Be(expectedResponse.SortOrder);
                result.From.Should().Be(expectedResponse.From);
                result.To.Should().Be(expectedResponse.To);
                result.Amount.Should().Be(expectedResponse.Amount);
            }
        }
    }
}