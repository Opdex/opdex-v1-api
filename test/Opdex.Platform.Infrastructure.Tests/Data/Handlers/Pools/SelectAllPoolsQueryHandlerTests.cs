using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Opdex.Core.Infrastructure;
using Opdex.Core.Infrastructure.Abstractions.Data;
using Opdex.Core.Infrastructure.Abstractions.Data.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Pools;
using Opdex.Platform.Infrastructure.Data.Handlers.Pools;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Pools
{
    public class SelectAllPoolsQueryHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly SelectAllPoolsQueryHandler _handler;
        
        public SelectAllPoolsQueryHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new CoreInfrastructureMapperProfile())).CreateMapper();
            var logger = new NullLogger<SelectAllPoolsQueryHandler>();
            
            _dbContext = new Mock<IDbContext>();
            _handler = new SelectAllPoolsQueryHandler(_dbContext.Object, mapper, logger);
        }

        [Fact]
        public async Task SelectAllPools_Success()
        {
            var expectedEntity = new PoolEntity
            {
                Id = 123454,
                TokenId = 2,
                Address = "SomeAddress",
                ReserveCrs = 8765434567890,
                ReserveSrc = "u76543456789076"
            };

            var responseList = new List<PoolEntity> {expectedEntity}.AsEnumerable();
                
            var command = new SelectAllPoolsQuery();
        
            _dbContext.Setup(db => db.ExecuteQueryAsync<PoolEntity>(It.IsAny<DatabaseQuery>()))
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