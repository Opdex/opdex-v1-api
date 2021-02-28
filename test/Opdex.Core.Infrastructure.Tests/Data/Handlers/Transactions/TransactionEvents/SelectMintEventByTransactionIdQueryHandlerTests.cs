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
    public class SelectMintEventByTransactionIdQueryHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly SelectMintEventByTransactionIdQueryHandler _handler;
        
        public SelectMintEventByTransactionIdQueryHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new CoreInfrastructureMapperProfile())).CreateMapper();
            
            _dbContext = new Mock<IDbContext>();
            _handler = new SelectMintEventByTransactionIdQueryHandler(_dbContext.Object, mapper);
        }

        [Fact]
        public async Task SelectMintEventByTransactionId_Success()
        {
            const long transactionId = 10;
            var expectedResponse = new MintEventEntity
            {
                Id = 23423,
                TransactionId = transactionId,
                Address = "SomeAddress",
                SortOrder = 7,
                Sender = "SomeSender",
                AmountCrs = 23453847,
                AmountSrc = "2346783876847"
            };
            
            var command = new SelectMintEventByTransactionIdQuery(transactionId);
        
            _dbContext.Setup(db => db.ExecuteFindAsync<MintEventEntity>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(expectedResponse));
            
            var result = await _handler.Handle(command, CancellationToken.None);

            result.Id.Should().Be(expectedResponse.Id);
            result.TransactionId.Should().Be(expectedResponse.TransactionId);
            result.Address.Should().Be(expectedResponse.Address);
            result.SortOrder.Should().Be(expectedResponse.SortOrder);
            result.Sender.Should().Be(expectedResponse.Sender);
            result.AmountCrs.Should().Be(expectedResponse.AmountCrs);
            result.AmountSrc.Should().Be(expectedResponse.AmountSrc);
        }
        
        [Fact]
        public void SelectMintEventByTransactionId_Throws_NotFoundException()
        {
            const long transactionId = 10;
        
            var command = new SelectMintEventByTransactionIdQuery(transactionId);
    
            _dbContext.Setup(db => db.ExecuteFindAsync<MintEventEntity>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult<MintEventEntity>(null));

            _handler.Invoking(h => h.Handle(command, CancellationToken.None))
                .Should()
                .Throw<NotFoundException>()
                .WithMessage($"{nameof(MintEventEntity)} with transactionId {transactionId} was not found.");
        }
    }
}