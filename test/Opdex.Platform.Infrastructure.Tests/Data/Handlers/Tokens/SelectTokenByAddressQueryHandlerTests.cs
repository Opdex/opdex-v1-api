using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens;
using Opdex.Platform.Infrastructure.Data.Handlers.Tokens;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Tokens;

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
    public async Task Handle_Query_Limit1()
    {
        // Arrange
        var query = new SelectTokenByAddressQuery("PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", false);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _dbContext.Verify(callTo => callTo.ExecuteFindAsync<It.IsAnyType>(
            It.Is<DatabaseQuery>(q => q.Sql.EndsWith("LIMIT 1;"))), Times.Once);
    }

    [Fact]
    public async Task SelectTokenByAddress_Success()
    {
        Address address = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u";

        var expectedEntity = new TokenEntity
        {
            Id = 123454,
            Address = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u",
            Name = "SomeName",
            Symbol = "SomeSymbol",
            Sats = 987689076,
            Decimals = 18,
            TotalSupply = 98765434567898765,
            CreatedBlock = 1,
            ModifiedBlock = 2
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
    public async Task SelectTokenByAddress_Throws_NotFoundException()
    {
        Address address = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u";

        var command = new SelectTokenByAddressQuery(address);

        _dbContext.Setup(db => db.ExecuteFindAsync<TokenEntity>(It.IsAny<DatabaseQuery>()))
            .Returns(() => Task.FromResult<TokenEntity>(null));

        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage($"{nameof(Token)} not found.");
    }

    [Fact]
    public async Task SelectTokenByAddress_ReturnsNull()
    {
        Address address = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u";
        const bool findOrThrow = false;

        var command = new SelectTokenByAddressQuery(address, findOrThrow);

        _dbContext.Setup(db => db.ExecuteFindAsync<TokenEntity>(It.IsAny<DatabaseQuery>()))
            .Returns(() => Task.FromResult<TokenEntity>(null));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeNull();
    }
}
