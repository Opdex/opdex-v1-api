using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Transactions;
using Opdex.Platform.Infrastructure.Data.Handlers.Transactions;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Transactions
{
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
        public async Task SelectTransactionByHash_Success()
        {
            const string hash = "SomeHash";
            var expectedResponse = new TransactionEntity
            {
                Id = 23423,
                Block = 123432,
                From = "From",
                To = "To",
                GasUsed = 60923,
                Hash = hash,
                Success = true,
                NewContractAddress = "NewAddress"
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
        public void SelectTransactionByHash_Throws_NotFoundException()
        {
            const string hash = "SomeHash";

            var command = new SelectTransactionByHashQuery(hash);

            _dbContext.Setup(db => db.ExecuteFindAsync<TransactionEntity>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult<TransactionEntity>(null));

            _handler.Invoking(h => h.Handle(command, CancellationToken.None))
                .Should()
                .Throw<NotFoundException>()
                .WithMessage($"{nameof(Transaction)} not found.");
        }
    }
}
