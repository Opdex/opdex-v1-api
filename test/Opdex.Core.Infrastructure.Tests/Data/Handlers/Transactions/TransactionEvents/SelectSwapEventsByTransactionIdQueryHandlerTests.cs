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
    public class SelectSwapEventsByTransactionIdQueryHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly SelectSwapEventsByTransactionIdQueryHandler _handler;
        
        public SelectSwapEventsByTransactionIdQueryHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new CoreInfrastructureMapperProfile())).CreateMapper();
            
            _dbContext = new Mock<IDbContext>();
            _handler = new SelectSwapEventsByTransactionIdQueryHandler(_dbContext.Object, mapper);
        }

        [Fact]
        public async Task SelectSwapEventByTransactionId_Success()
        {
            const long transactionId = 10;
            var expectedResponse = new SwapEventEntity
            {
                Id = 23423,
                TransactionId = transactionId,
                Address = "SomeAddress",
                SortOrder = 7,
                AmountCrsIn = 876545678,
                AmountCrsOut = 876234256478,
                AmountSrcIn = "987654567890",
                AmountSrcOut = "9870372632390"
            };
            
            var responseList = new List<SwapEventEntity> {expectedResponse}.AsEnumerable();

            var command = new SelectSwapEventsByTransactionIdQuery(new [] { new TransactionEventSummary(1,1,1,1,1,"1") });
        
            _dbContext.Setup(db => db.ExecuteQueryAsync<SwapEventEntity>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(responseList));
            
            var results = await _handler.Handle(command, CancellationToken.None);

            foreach (var result in results)
            {
                result.Id.Should().Be(expectedResponse.Id);
                result.TransactionId.Should().Be(expectedResponse.TransactionId);
                result.Address.Should().Be(expectedResponse.Address);
                result.SortOrder.Should().Be(expectedResponse.SortOrder);
                result.AmountCrsIn.Should().Be(expectedResponse.AmountCrsIn);
                result.AmountCrsOut.Should().Be(expectedResponse.AmountCrsOut);
                result.AmountSrcIn.Should().Be(expectedResponse.AmountSrcIn);
                result.AmountSrcOut.Should().Be(expectedResponse.AmountSrcOut);
            }
        }
    }
}