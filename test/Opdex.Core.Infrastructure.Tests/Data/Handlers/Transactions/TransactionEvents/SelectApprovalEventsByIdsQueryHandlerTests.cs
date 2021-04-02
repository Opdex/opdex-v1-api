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
    public class SelectApprovalEventsByIdsQueryHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly SelectApprovalEventsByIdsQueryHandler _handler;
        
        public SelectApprovalEventsByIdsQueryHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new CoreInfrastructureMapperProfile())).CreateMapper();
            
            _dbContext = new Mock<IDbContext>();
            _handler = new SelectApprovalEventsByIdsQueryHandler(_dbContext.Object, mapper);
        }

        [Fact]
        public async Task SelectApprovalEventByTransactionId_Success()
        {
            const long transactionId = 10;
            var approvalEvent = new ApprovalEventEntity
            {
                Id = 23423,
                TransactionId = transactionId,
                Address = "SomeAddress",
                SortOrder = 7,
                Owner = "Owner",
                Spender = "Spender",
                Amount = "2346783876847"
            };

            var responseList = new List<ApprovalEventEntity> {approvalEvent}.AsEnumerable();
            
            var command = new SelectApprovalEventsByIdsQuery(new [] { new TransactionEventSummary(1,1,1,1,1,"1") });
        
            _dbContext.Setup(db => db.ExecuteQueryAsync<ApprovalEventEntity>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(responseList));
            
            var results = await _handler.Handle(command, CancellationToken.None);

            foreach (var result in results)
            {
                result.Id.Should().Be(approvalEvent.Id);
                result.TransactionId.Should().Be(approvalEvent.TransactionId);
                result.Address.Should().Be(approvalEvent.Address);
                result.SortOrder.Should().Be(approvalEvent.SortOrder);
                result.Owner.Should().Be(approvalEvent.Owner);
                result.Spender.Should().Be(approvalEvent.Spender);
                result.Amount.Should().Be(approvalEvent.Amount);
            }
        }
    }
}