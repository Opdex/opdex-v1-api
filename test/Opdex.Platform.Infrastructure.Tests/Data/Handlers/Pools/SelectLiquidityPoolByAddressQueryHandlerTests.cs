using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Pools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Pools;
using Opdex.Platform.Infrastructure.Data.Handlers.Pools;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Pools
{
    public class SelectLiquidityPoolByAddressQueryHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly SelectLiquidityPoolByAddressQueryHandler _handler;
        
        public SelectLiquidityPoolByAddressQueryHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();
            
            _dbContext = new Mock<IDbContext>();
            _handler = new SelectLiquidityPoolByAddressQueryHandler(_dbContext.Object, mapper);
        }

        [Fact]
        public async Task SelectLiquidityPoolByAddress_Success()
        {
            const string address = "SomeAddress";
            
            var expectedEntity = new LiquidityPoolEntity
            {
                Id = 123454,
                TokenId = 1235,
                Address = "SomeAddress",
                ReserveCrs = 7654567890,
                ReserveSrc = "8765456789"
            };
                
            var command = new SelectLiquidityPoolByAddressQuery(address);
        
            _dbContext.Setup(db => db.ExecuteFindAsync<LiquidityPoolEntity>(It.IsAny<DatabaseQuery>()))
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
            
            var command = new SelectLiquidityPoolByAddressQuery(address);
        
            _dbContext.Setup(db => db.ExecuteFindAsync<LiquidityPoolEntity>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult<LiquidityPoolEntity>(null));

            _handler.Invoking(h => h.Handle(command, CancellationToken.None))
                .Should()
                .Throw<NotFoundException>()
                .WithMessage($"{nameof(LiquidityPoolEntity)} with address {address} was not found.");
        }
    }
}