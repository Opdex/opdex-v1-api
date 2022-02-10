using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens.Distribution;
using Opdex.Platform.Infrastructure.Data.Handlers.Tokens.Distribution;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Tokens;

public class SelectLatestTokenDistributionByTokenIdQueryHandlerTests
{
    private readonly Mock<IDbContext> _dbContext;
    private readonly SelectLatestTokenDistributionByTokenIdQueryHandler _handler;

    public SelectLatestTokenDistributionByTokenIdQueryHandlerTests()
    {
        var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();

        _dbContext = new Mock<IDbContext>();
        _handler = new SelectLatestTokenDistributionByTokenIdQueryHandler(_dbContext.Object, mapper);
    }

    [Fact]
    public async Task SelectTokenDistributionByTokenId_Success()
    {
        const ulong tokenId = 99999;
        var expectedEntity = new TokenDistributionEntity
        {
            Id = 123454,
            TokenId = tokenId,
            VaultDistribution = 10000,
            MiningGovernanceDistribution = 10000000,
            DistributionBlock = 87654,
            NextDistributionBlock = 19876543,
            PeriodIndex = 1
        };

        var command = new SelectLatestTokenDistributionByTokenIdQuery(tokenId);

        _dbContext.Setup(db => db.ExecuteFindAsync<TokenDistributionEntity>(It.IsAny<DatabaseQuery>()))
            .Returns(() => Task.FromResult(expectedEntity));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Id.Should().Be(expectedEntity.Id);
        result.TokenId.Should().Be(expectedEntity.TokenId);
        result.VaultDistribution.Should().Be(expectedEntity.VaultDistribution);
        result.MiningGovernanceDistribution.Should().Be(expectedEntity.MiningGovernanceDistribution);
        result.DistributionBlock.Should().Be(expectedEntity.DistributionBlock);
        result.NextDistributionBlock.Should().Be(expectedEntity.NextDistributionBlock);
        result.PeriodIndex.Should().Be(expectedEntity.PeriodIndex);
    }

    [Fact]
    public void SelectTokenDistributionByTokenId_Throws_NotFoundException()
    {
        const ulong tokenId = 99999;
        var command = new SelectLatestTokenDistributionByTokenIdQuery(tokenId);

        _dbContext.Setup(db => db.ExecuteFindAsync<TokenDistributionEntity>(It.IsAny<DatabaseQuery>()))
            .Returns(() => Task.FromResult<TokenDistributionEntity>(null));

        _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage($"{nameof(TokenDistribution)} not found.");
    }
}
