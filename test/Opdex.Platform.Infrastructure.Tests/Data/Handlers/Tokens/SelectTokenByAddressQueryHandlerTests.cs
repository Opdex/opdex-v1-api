using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens;
using Opdex.Platform.Infrastructure.Data.Handlers.Tokens;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Tokens
{
    public class SelectTokenByAddressQueryHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly SelectTokenByAddressQueryHandler _handler;
        
        public SelectTokenByAddressQueryHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();
            
            _dbContext = new Mock<IDbContext>();
            _handler = new SelectTokenByAddressQueryHandler(_dbContext.Object, mapper);
        }

        [Fact]
        public async Task SelectTokenByAddress_Success()
        {
            const string address = "SomeAddress";
            
            var expectedEntity = new TokenEntity
            {
                Id = 123454,
                Address = "SomeAddress",
                Name = "SomeName",
                Symbol = "SomeSymbol",
                Sats = 987689076,
                Decimals = 18,
                TotalSupply = "98765434567898765"
            };
                
            var command = new SelectTokenByAddressQuery(address);
        
            _dbContext.Setup(db => db.ExecuteFindAsync<TokenEntity>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(expectedEntity));
            
            var result = await _handler.Handle(command, CancellationToken.None);

            result.Id.Should().Be(expectedEntity.Id);
            result.Address.Should().Be(expectedEntity.Address);
            result.Name.Should().Be(expectedEntity.Name);
            result.Symbol.Should().Be(expectedEntity.Symbol);
            result.Sats.Should().Be(expectedEntity.Sats);
            result.Decimals.Should().Be(expectedEntity.Decimals);
            result.TotalSupply.Should().Be(expectedEntity.TotalSupply);
        }
        
        [Fact]
        public void SelectTransactionByHash_Throws_NotFoundException()
        {
            const string address = "SomeAddress";
            
            var command = new SelectTokenByAddressQuery(address);
        
            _dbContext.Setup(db => db.ExecuteFindAsync<TokenEntity>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult<TokenEntity>(null));

            _handler.Invoking(h => h.Handle(command, CancellationToken.None))
                .Should()
                .Throw<NotFoundException>()
                .WithMessage($"{nameof(TokenEntity)} with address {address} was not found.");
        }
    }
}