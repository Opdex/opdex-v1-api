using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens;
using Opdex.Platform.Infrastructure.Data.Handlers.Tokens;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Tokens
{
    public class SelectTokensWithFilterQueryHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly SelectTokensWithFilterQueryHandler _handler;

        public SelectTokensWithFilterQueryHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();

            _dbContext = new Mock<IDbContext>();
            _handler = new SelectTokensWithFilterQueryHandler(_dbContext.Object, mapper);
        }

        [Fact]
        public async Task SelectAllTokens_Success()
        {
            const long marketId = 1;

            var expectedEntity = new TokenEntity
            {
                Id = 123454,
                Address = "SomeAddress",
                IsLpt = true,
                Name = "SomeAddress",
                Symbol = "SomeSymbol",
                Decimals = 8,
                Sats = 1000000000,
                TotalSupply = "987656789098765",
                CreatedBlock = 1,
                ModifiedBlock = 1
            };

            var responseList = new List<TokenEntity> {expectedEntity}.AsEnumerable();

            var command = new SelectTokensWithFilterQuery(marketId);

            _dbContext.Setup(db => db.ExecuteQueryAsync<TokenEntity>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(responseList));

            var results = await _handler.Handle(command, CancellationToken.None);

            foreach (var result in results)
            {
                result.Id.Should().Be(expectedEntity.Id);
                result.Address.Should().Be(expectedEntity.Address);
                result.IsLpt.Should().Be(expectedEntity.IsLpt);
                result.Name.Should().Be(expectedEntity.Name);
                result.Symbol.Should().Be(expectedEntity.Symbol);
                result.Decimals.Should().Be(expectedEntity.Decimals);
                result.Sats.Should().Be(expectedEntity.Sats);
                result.TotalSupply.Should().Be(expectedEntity.TotalSupply);
            }
        }
    }
}
