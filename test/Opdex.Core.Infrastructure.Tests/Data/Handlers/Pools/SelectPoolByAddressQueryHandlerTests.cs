using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Core.Common.Exceptions;
using Opdex.Core.Infrastructure.Abstractions.Data;
using Opdex.Core.Infrastructure.Abstractions.Data.Models;
using Opdex.Core.Infrastructure.Abstractions.Data.Queries.Pools;
using Opdex.Core.Infrastructure.Data.Handlers.Pools;
using Xunit;

namespace Opdex.Core.Infrastructure.Tests.Data.Handlers.Pools
{
    public class SelectPoolByAddressQueryHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly SelectPoolByAddressQueryHandler _handler;
        
        public SelectPoolByAddressQueryHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new CoreInfrastructureMapperProfile())).CreateMapper();
            
            _dbContext = new Mock<IDbContext>();
            _handler = new SelectPoolByAddressQueryHandler(_dbContext.Object, mapper);
        }

        [Fact]
        public async Task SelectPoolByAddress_Success()
        {
            const string address = "SomeAddress";
            
            var expectedEntity = new PoolEntity
            {
                Id = 123454,
                TokenId = 1235,
                Address = "SomeAddress",
                ReserveCrs = 7654567890,
                ReserveSrc = "8765456789"
            };
                
            var command = new SelectPoolByAddressQuery(address);
        
            _dbContext.Setup(db => db.ExecuteFindAsync<PoolEntity>(It.IsAny<DatabaseQuery>()))
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
            
            var command = new SelectPoolByAddressQuery(address);
        
            _dbContext.Setup(db => db.ExecuteFindAsync<PoolEntity>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult<PoolEntity>(null));

            _handler.Invoking(h => h.Handle(command, CancellationToken.None))
                .Should()
                .Throw<NotFoundException>()
                .WithMessage($"{nameof(PoolEntity)} with address {address} was not found.");
        }
    }
}