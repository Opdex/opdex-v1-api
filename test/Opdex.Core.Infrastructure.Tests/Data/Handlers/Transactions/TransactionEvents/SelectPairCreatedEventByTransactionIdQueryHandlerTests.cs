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
    public class SelectPairCreatedEventByTransactionIdQueryHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly SelectPairCreatedEventByTransactionIdQueryHandler _handler;
        
        public SelectPairCreatedEventByTransactionIdQueryHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new CoreInfrastructureMapperProfile())).CreateMapper();
            
            _dbContext = new Mock<IDbContext>();
            _handler = new SelectPairCreatedEventByTransactionIdQueryHandler(_dbContext.Object, mapper);
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
            
            var command = new SelectPairCreatedEventByTransactionIdQuery(transactionId);
        
            _dbContext.Setup(db => db.ExecuteFindAsync<PairCreatedEventEntity>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(expectedResponse));
            
            var result = await _handler.Handle(command, CancellationToken.None);

            result.Id.Should().Be(expectedResponse.Id);
            result.TransactionId.Should().Be(expectedResponse.TransactionId);
            result.Address.Should().Be(expectedResponse.Address);
            result.SortOrder.Should().Be(expectedResponse.SortOrder);
            result.Token.Should().Be(expectedResponse.Token);
            result.Pair.Should().Be(expectedResponse.Pair);
        }
        
        [Fact]
        public void SelectPairCreatedEventByTransactionId_Throws_NotFoundException()
        {
            const long transactionId = 10;
        
            var command = new SelectPairCreatedEventByTransactionIdQuery(transactionId);
    
            _dbContext.Setup(db => db.ExecuteFindAsync<PairCreatedEventEntity>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult<PairCreatedEventEntity>(null));

            _handler.Invoking(h => h.Handle(command, CancellationToken.None))
                .Should()
                .Throw<NotFoundException>()
                .WithMessage($"{nameof(PairCreatedEventEntity)} with transactionId {transactionId} was not found.");
        }
    }
}