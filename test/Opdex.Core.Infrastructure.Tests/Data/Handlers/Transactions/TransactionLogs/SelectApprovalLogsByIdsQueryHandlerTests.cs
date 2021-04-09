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
    public class SelectApprovalLogsByIdsQueryHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly SelectApprovalLogsByIdsQueryHandler _handler;
        
        public SelectApprovalLogsByIdsQueryHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new CoreInfrastructureMapperProfile())).CreateMapper();
            
            _dbContext = new Mock<IDbContext>();
            _handler = new SelectApprovalLogsByIdsQueryHandler(_dbContext.Object, mapper);
        }

        [Fact]
        public async Task SelectApprovalLogByTransactionId_Success()
        {
            const long transactionId = 10;
            var approvalLog = new ApprovalLogEntity
            {
                Id = 23423,
                TransactionId = transactionId,
                Address = "SomeAddress",
                SortOrder = 7,
                Owner = "Owner",
                Spender = "Spender",
                Amount = "2346783876847"
            };

            var responseList = new List<ApprovalLogEntity> {approvalLog}.AsEnumerable();
            
            var command = new SelectApprovalLogsByIdsQuery(new [] { new TransactionLogSummary(1,1,1,1,1,"1") });
        
            _dbContext.Setup(db => db.ExecuteQueryAsync<ApprovalLogEntity>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(responseList));
            
            var results = await _handler.Handle(command, CancellationToken.None);

            foreach (var result in results)
            {
                result.Id.Should().Be(approvalLog.Id);
                result.TransactionId.Should().Be(approvalLog.TransactionId);
                result.Address.Should().Be(approvalLog.Address);
                result.SortOrder.Should().Be(approvalLog.SortOrder);
                result.Owner.Should().Be(approvalLog.Owner);
                result.Spender.Should().Be(approvalLog.Spender);
                result.Amount.Should().Be(approvalLog.Amount);
            }
        }
    }
}