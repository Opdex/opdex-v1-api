using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Core.Common.Exceptions;
using Opdex.Core.Infrastructure.Abstractions.Data;
using Opdex.Core.Infrastructure.Abstractions.Data.Models;
using Opdex.Core.Infrastructure.Abstractions.Data.Queries.Pairs;
using Opdex.Core.Infrastructure.Data.Handlers.Pairs;
using Xunit;

namespace Opdex.Core.Infrastructure.Tests.Data.Handlers.Pairs
{
    public class SelectPairByAddressQueryHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly SelectPairByAddressQueryHandler _handler;
        
        public SelectPairByAddressQueryHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new CoreInfrastructureMapperProfile())).CreateMapper();
            
            _dbContext = new Mock<IDbContext>();
            _handler = new SelectPairByAddressQueryHandler(_dbContext.Object, mapper);
        }

        [Fact]
        public async Task SelectPairByAddress_Success()
        {
            const string address = "SomeAddress";
            
            var expectedEntity = new PairEntity
            {
                Id = 123454,
                TokenId = 1235,
                Address = "SomeAddress",
                ReserveCrs = 7654567890,
                ReserveSrc = "8765456789"
            };
                
            var command = new SelectPairByAddressQuery(address);
        
            _dbContext.Setup(db => db.ExecuteFindAsync<PairEntity>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(expectedEntity));
            
            var result = await _handler.Handle(command, CancellationToken.None);

            result.Id.Should().Be(expectedEntity.Id);
            result.TokenId.Should().Be(expectedEntity.TokenId);
            result.Address.Should().Be(expectedEntity.Address);
            result.ReserveCrs.Should().Be(expectedEntity.ReserveCrs);
            result.ReserveSrc.Should().Be(expectedEntity.ReserveSrc);
        }
        
        [Fact]
        public void SelectTransactionByHash_Throws_NotFoundException()
        {
            const string address = "SomeAddress";
            
            var command = new SelectPairByAddressQuery(address);
        
            _dbContext.Setup(db => db.ExecuteFindAsync<PairEntity>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult<PairEntity>(null));

            _handler.Invoking(h => h.Handle(command, CancellationToken.None))
                .Should()
                .Throw<NotFoundException>()
                .WithMessage($"{nameof(PairEntity)} with address {address} was not found.");
        }
    }
}