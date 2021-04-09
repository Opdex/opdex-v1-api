using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Core.Common.Exceptions;
using Opdex.Core.Domain.Models.TransactionLogs;
using Opdex.Core.Infrastructure.Abstractions.Data;
using Opdex.Core.Infrastructure.Abstractions.Data.Models.TransactionLogs;
using Opdex.Core.Infrastructure.Abstractions.Data.Queries.Transactions.TransactionLogs;
using Opdex.Core.Infrastructure.Data.Handlers.Transactions.TransactionLogs;
using Xunit;

namespace Opdex.Core.Infrastructure.Tests.Data.Handlers.Transactions.TransactionLogs
{
    public class SelectReservesLogsByIdsQueryHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly SelectReservesLogsByIdsQueryHandler _handler;
        
        public SelectReservesLogsByIdsQueryHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new CoreInfrastructureMapperProfile())).CreateMapper();
            
            _dbContext = new Mock<IDbContext>();
            _handler = new SelectReservesLogsByIdsQueryHandler(_dbContext.Object, mapper);
        }

        [Fact]
        public async Task SelectReservesLogByTransactionId_Success()
        {
            const long transactionId = 10;
            var expectedResponse = new ReservesLogEntity
            {
                Id = 23423,
                TransactionId = transactionId,
                Address = "SomeAddress",
                SortOrder = 7,
                ReserveCrs = 876545678,
                ReserveSrc = "987654567890"
            };
            
            var responseList = new List<ReservesLogEntity> {expectedResponse}.AsEnumerable();

            var command = new SelectReservesLogsByIdsQuery(new [] { new TransactionLogSummary(1,1,1,1,1,"1") });
        
            _dbContext.Setup(db => db.ExecuteQueryAsync<ReservesLogEntity>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(responseList));
            
            var results = await _handler.Handle(command, CancellationToken.None);

            foreach (var result in results)
            {
                result.Id.Should().Be(expectedResponse.Id);
                result.TransactionId.Should().Be(expectedResponse.TransactionId);
                result.Address.Should().Be(expectedResponse.Address);
                result.SortOrder.Should().Be(expectedResponse.SortOrder);
                result.ReserveCrs.Should().Be(expectedResponse.ReserveCrs);
                result.ReserveSrc.Should().Be(expectedResponse.ReserveSrc);
            }
        }
    }
}