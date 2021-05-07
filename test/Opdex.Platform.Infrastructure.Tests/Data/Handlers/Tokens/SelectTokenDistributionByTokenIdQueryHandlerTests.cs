using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens.Distribution;
using Opdex.Platform.Infrastructure.Data.Handlers.Tokens.Distribution;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Tokens
{
    public class SelectTokenDistributionByTokenIdQueryHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly SelectTokenDistributionByTokenIdQueryHandler _handler;
        
        public SelectTokenDistributionByTokenIdQueryHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();
            
            _dbContext = new Mock<IDbContext>();
            _handler = new SelectTokenDistributionByTokenIdQueryHandler(_dbContext.Object, mapper);
        }

        [Fact]
        public async Task SelectTokenDistributionByTokenId_Success()
        {
            const long tokenId = 10;
            
            var expectedEntity = new TokenDistributionEntity
            {
                Id = 123454,
                TokenId = tokenId,
                MiningGovernanceId = 999,
                Owner = "SomeOwner",
                Genesis = 10,
                PeriodDuration = 1_971_000,
                PeriodIndex = 1
            };
                
            var command = new SelectTokenDistributionByTokenIdQuery(tokenId);
        
            _dbContext.Setup(db => db.ExecuteFindAsync<TokenDistributionEntity>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(expectedEntity));
            
            var result = await _handler.Handle(command, CancellationToken.None);

            result.Id.Should().Be(expectedEntity.Id);
            result.TokenId.Should().Be(expectedEntity.TokenId);
            result.MiningGovernanceId.Should().Be(expectedEntity.MiningGovernanceId);
            result.Owner.Should().Be(expectedEntity.Owner);
            result.Genesis.Should().Be(expectedEntity.Genesis);
            result.PeriodDuration.Should().Be(expectedEntity.PeriodDuration);
            result.PeriodIndex.Should().Be(expectedEntity.PeriodIndex);
        }
        
        [Fact]
        public void SelectTokenDistributionByTokenId_Throws_NotFoundException()
        {
            const long tokenId = 10;
            
            var command = new SelectTokenDistributionByTokenIdQuery(tokenId);
        
            _dbContext.Setup(db => db.ExecuteFindAsync<TokenDistributionEntity>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult<TokenDistributionEntity>(null));

            _handler.Invoking(h => h.Handle(command, CancellationToken.None))
                .Should()
                .Throw<NotFoundException>()
                .WithMessage($"{nameof(TokenDistributionEntity)} with tokenId {tokenId} was not found.");
        }
    }
}