using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Opdex.Platform.Infrastructure;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Pools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Pools;
using Opdex.Platform.Infrastructure.Data.Handlers.Pools;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Pools
{
    public class SelectAllLiquidityPoolsQueryHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly SelectAllLiquidityPoolsQueryHandler _handler;
        
        public SelectAllLiquidityPoolsQueryHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();
            var logger = new NullLogger<SelectAllLiquidityPoolsQueryHandler>();
            
            _dbContext = new Mock<IDbContext>();
            _handler = new SelectAllLiquidityPoolsQueryHandler(_dbContext.Object, mapper, logger);
        }

        [Fact]
        public async Task SelectAllPools_Success()
        {
            var expectedEntity = new LiquidityPoolEntity
            {
                Id = 123454,
                TokenId = 2,
                Address = "SomeAddress",
                ReserveCrs = 8765434567890,
                ReserveSrc = "u76543456789076"
            };

            var responseList = new [] { expectedEntity }.AsEnumerable();
                
            var command = new SelectAllLiquidityPoolsQuery();
        
            _dbContext.Setup(db => db.ExecuteQueryAsync<LiquidityPoolEntity>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(responseList));
            
            var results = await _handler.Handle(command, CancellationToken.None);

            foreach (var result in results)
            {
                result.Id.Should().Be(expectedEntity.Id);
                result.TokenId.Should().Be(expectedEntity.TokenId);
                result.Address.Should().Be(expectedEntity.Address);
                result.ReserveCrs.Should().Be(expectedEntity.ReserveCrs);
                result.ReserveSrc.Should().Be(expectedEntity.ReserveSrc);
            }
        }
    }
}