using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Transactions;
using Opdex.Platform.Infrastructure.Data.Handlers.Transactions;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Transactions;

public class SelectTransactionByHashQueryHandlerTests
{
    private readonly Mock<IDbContext> _dbContext;
    private readonly SelectTransactionByHashQueryHandler _handler;

    public SelectTransactionByHashQueryHandlerTests()
    {
        var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();

        _dbContext = new Mock<IDbContext>();
        _handler = new SelectTransactionByHashQueryHandler(_dbContext.Object, mapper);
    }

    [Fact]
    public async Task Handle_Query_Limit1()
    {
        // Arrange
        var query = new SelectTransactionByHashQuery(new Sha256(0), false);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _dbContext.Verify(callTo => callTo.ExecuteFindAsync<It.IsAnyType>(
            It.Is<DatabaseQuery>(q => q.Sql.EndsWith("LIMIT 1;"))), Times.Once);
    }

    [Fact]
    public async Task SelectTransactionByHash_Success()
    {
        Sha256 hash = new(95840954890);
        var expectedResponse = new TransactionEntity
        {
            Id = 23423,
            Block = 123432,
            From = "PJpR65NLUpTFgs8mJxdSC7bbwgyadJEVgT",
            To = "PSxx8BBVDpB5qHKmm7RGLDVaEL8p9NWbZW",
            GasUsed = 60923,
            Hash = hash,
            Success = true,
            NewContractAddress = "PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh"
        };

        var command = new SelectTransactionByHashQuery(hash);

        _dbContext.Setup(db => db.ExecuteFindAsync<TransactionEntity>(It.IsAny<DatabaseQuery>()))
            .Returns(() => Task.FromResult(expectedResponse));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Id.Should().Be(expectedResponse.Id);
        result.BlockHeight.Should().Be(expectedResponse.Block);
        result.Hash.Should().Be(expectedResponse.Hash);
        result.From.Should().Be(expectedResponse.From);
        result.To.Should().Be(expectedResponse.To);
        result.GasUsed.Should().Be(expectedResponse.GasUsed);
        result.Success.Should().Be(expectedResponse.Success);
        result.NewContractAddress.Should().Be(expectedResponse.NewContractAddress);
        result.Logs.Should().BeEmpty();
    }

    [Fact]
    public async Task SelectTransactionByHash_Throws_NotFoundException()
    {
        Sha256 hash = new Sha256(95840954890);

        var command = new SelectTransactionByHashQuery(hash);

        _dbContext.Setup(db => db.ExecuteFindAsync<TransactionEntity>(It.IsAny<DatabaseQuery>()))
            .Returns(() => Task.FromResult<TransactionEntity>(null));

        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage($"{nameof(Transaction)} not found.");
    }
}
