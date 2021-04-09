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
    public class SelectMintLogsByIdsQueryHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly SelectMintLogsByIdsQueryHandler _handler;
        
        public SelectMintLogsByIdsQueryHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new CoreInfrastructureMapperProfile())).CreateMapper();
            
            _dbContext = new Mock<IDbContext>();
            _handler = new SelectMintLogsByIdsQueryHandler(_dbContext.Object, mapper);
        }

        [Fact]
        public async Task SelectMintLogByTransactionId_Success()
        {
            const long transactionId = 10;
            var expectedResponse = new MintLogEntity
            {
                Id = 23423,
                TransactionId = transactionId,
                Address = "SomeAddress",
                SortOrder = 7,
                Sender = "SomeSender",
                AmountCrs = 23453847,
                AmountSrc = "2346783876847"
            };

            var responseList = new List<MintLogEntity> {expectedResponse}.AsEnumerable();
            
            var command = new SelectMintLogsByIdsQuery(new [] { new TransactionLogSummary(1,1,1,1,1,"1") });
        
            _dbContext.Setup(db => db.ExecuteQueryAsync<MintLogEntity>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(responseList));
            
            var results = await _handler.Handle(command, CancellationToken.None);

            foreach (var result in results)
            {
                result.Id.Should().Be(expectedResponse.Id);
                result.TransactionId.Should().Be(expectedResponse.TransactionId);
                result.Address.Should().Be(expectedResponse.Address);
                result.SortOrder.Should().Be(expectedResponse.SortOrder);
                result.Sender.Should().Be(expectedResponse.Sender);
                result.AmountCrs.Should().Be(expectedResponse.AmountCrs);
                result.AmountSrc.Should().Be(expectedResponse.AmountSrc);
            }
        }
    }
}