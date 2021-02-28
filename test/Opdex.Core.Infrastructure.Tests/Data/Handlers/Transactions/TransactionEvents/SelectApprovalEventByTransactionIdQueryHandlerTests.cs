using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Core.Common.Exceptions;
using Opdex.Core.Infrastructure.Abstractions.Data;
using Opdex.Core.Infrastructure.Abstractions.Data.Models.TransactionEvents;
using Opdex.Core.Infrastructure.Abstractions.Data.Queries.Transactions.TransactionEvents;
using Opdex.Core.Infrastructure.Data.Handlers.Transactions.TransactionEvents;
using Xunit;

namespace Opdex.Core.Infrastructure.Tests.Data.Handlers.Transactions.TransactionEvents
{
    public class SelectApprovalEventByTransactionIdQueryHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly SelectApprovalEventByTransactionIdQueryHandler _handler;
        
        public SelectApprovalEventByTransactionIdQueryHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new CoreInfrastructureMapperProfile())).CreateMapper();
            
            _dbContext = new Mock<IDbContext>();
            _handler = new SelectApprovalEventByTransactionIdQueryHandler(_dbContext.Object, mapper);
        }

        [Fact]
        public async Task SelectApprovalEventByTransactionId_Success()
        {
            const long transactionId = 10;
            var expectedResponse = new ApprovalEventEntity
            {
                Id = 23423,
                TransactionId = transactionId,
                Address = "SomeAddress",
                SortOrder = 7,
                Owner = "Owner",
                Spender = "Spender",
                Amount = "2346783876847"
            };
            
            var command = new SelectApprovalEventByTransactionIdQuery(transactionId);
        
            _dbContext.Setup(db => db.ExecuteFindAsync<ApprovalEventEntity>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(expectedResponse));
            
            var result = await _handler.Handle(command, CancellationToken.None);

            result.Id.Should().Be(expectedResponse.Id);
            result.TransactionId.Should().Be(expectedResponse.TransactionId);
            result.Address.Should().Be(expectedResponse.Address);
            result.SortOrder.Should().Be(expectedResponse.SortOrder);
            result.Owner.Should().Be(expectedResponse.Owner);
            result.Spender.Should().Be(expectedResponse.Spender);
            result.Amount.Should().Be(expectedResponse.Amount);
        }
        
        [Fact]
        public void SelectApprovalEventByTransactionId_Throws_NotFoundException()
        {
            const long transactionId = 10;
        
            var command = new SelectApprovalEventByTransactionIdQuery(transactionId);
    
            _dbContext.Setup(db => db.ExecuteFindAsync<ApprovalEventEntity>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult<ApprovalEventEntity>(null));

            _handler.Invoking(h => h.Handle(command, CancellationToken.None))
                .Should()
                .Throw<NotFoundException>()
                .WithMessage($"{nameof(ApprovalEventEntity)} with transactionId {transactionId} was not found.");
        }
    }
}