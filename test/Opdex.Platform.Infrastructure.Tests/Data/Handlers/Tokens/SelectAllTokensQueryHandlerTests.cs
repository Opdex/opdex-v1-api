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
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens;
using Opdex.Platform.Infrastructure.Data.Handlers.Tokens;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Tokens
{
    public class SelectAllTokensQueryHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly SelectAllTokensQueryHandler _handler;
        
        public SelectAllTokensQueryHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new CoreInfrastructureMapperProfile())).CreateMapper();
            var logger = new NullLogger<SelectAllTokensQueryHandler>();
            
            _dbContext = new Mock<IDbContext>();
            _handler = new SelectAllTokensQueryHandler(_dbContext.Object, mapper, logger);
        }

        [Fact]
        public async Task SelectAllTokens_Success()
        {
            var expectedEntity = new TokenEntity
            {
                Id = 123454,
                Address = "SomeAddress",
                Name = "SomeAddress",
                Symbol = "SomeSymbol",
                Decimals = 8,
                Sats = 1000000000,
                TotalSupply = "987656789098765"
            };

            var responseList = new List<TokenEntity> {expectedEntity}.AsEnumerable();
                
            var command = new SelectAllTokensQuery();
        
            _dbContext.Setup(db => db.ExecuteQueryAsync<TokenEntity>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(responseList));
            
            var results = await _handler.Handle(command, CancellationToken.None);

            foreach (var result in results)
            {
                result.Id.Should().Be(expectedEntity.Id);
                result.Address.Should().Be(expectedEntity.Address);
                result.Name.Should().Be(expectedEntity.Name);
                result.Symbol.Should().Be(expectedEntity.Symbol);
                result.Decimals.Should().Be(expectedEntity.Decimals);
                result.Sats.Should().Be(expectedEntity.Sats);
                result.TotalSupply.Should().Be(expectedEntity.TotalSupply);
            }
        }
    }
}