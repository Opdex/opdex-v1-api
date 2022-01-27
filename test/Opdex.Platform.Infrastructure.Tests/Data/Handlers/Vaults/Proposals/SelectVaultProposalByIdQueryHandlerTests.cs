using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Domain.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults.Proposals;
using Opdex.Platform.Infrastructure.Data.Handlers.Vaults.Proposals;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Vaults.Proposals;

public class SelectVaultProposalByIdQueryHandlerTests
{
    private readonly Mock<IDbContext> _dbContext;
    private readonly SelectVaultProposalByIdQueryHandler _handler;

    public SelectVaultProposalByIdQueryHandlerTests()
    {
        var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();

        _dbContext = new Mock<IDbContext>();
        _handler = new SelectVaultProposalByIdQueryHandler(_dbContext.Object, mapper);
    }

    [Fact]
    public async Task SelectVaultProposalById_Success()
    {
        const ulong id = 10;

        var expectedEntity = new VaultProposalEntity
        {
            Id = id,
            PublicId = 11,
            VaultId = 2,
            Creator = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u",
            Wallet = "PGZPZpBXfJ6u1yzNPDw7u4iW4LHVEPMKeh",
            Amount = 233,
            Description = "Description",
            ProposalTypeId = 1,
            ProposalStatusId = 3,
            Expiration = 9876,
            YesAmount = 25,
            NoAmount = 13,
            PledgeAmount = 14,
            Approved = true,
            CreatedBlock = 1,
            ModifiedBlock = 2
        };

        var command = new SelectVaultProposalByIdQuery(id);

        _dbContext.Setup(db => db.ExecuteFindAsync<VaultProposalEntity>(It.Is<DatabaseQuery>(q => q.Sql.Contains("vault_proposal"))))
            .Returns(() => Task.FromResult(expectedEntity));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Id.Should().Be(expectedEntity.Id);
        result.PublicId.Should().Be(expectedEntity.PublicId);
        result.VaultId.Should().Be(expectedEntity.VaultId);
        result.Creator.Should().Be(expectedEntity.Creator);
        result.Wallet.Should().Be(expectedEntity.Wallet);
        result.Amount.Should().Be(expectedEntity.Amount);
        result.Description.Should().Be(expectedEntity.Description);
        result.Type.Should().Be((VaultProposalType)expectedEntity.ProposalTypeId);
        result.Status.Should().Be((VaultProposalStatus)expectedEntity.ProposalStatusId);
        result.Expiration.Should().Be(expectedEntity.Expiration);
        result.YesAmount.Should().Be(expectedEntity.YesAmount);
        result.NoAmount.Should().Be(expectedEntity.NoAmount);
        result.PledgeAmount.Should().Be(expectedEntity.PledgeAmount);
        result.Approved.Should().Be(expectedEntity.Approved);
        result.CreatedBlock.Should().Be(expectedEntity.CreatedBlock);
        result.ModifiedBlock.Should().Be(expectedEntity.ModifiedBlock);
    }

    [Fact]
    public void SelectVaultProposalById_Throws_NotFoundException()
    {
        const ulong id = 10;

        var command = new SelectVaultProposalByIdQuery(id);

        _dbContext.Setup(db => db.ExecuteFindAsync<VaultProposalEntity>(It.IsAny<DatabaseQuery>()))
            .Returns(() => Task.FromResult<VaultProposalEntity>(null));

        _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage("Proposal not found.");
    }

    [Fact]
    public async Task SelectVaultProposalById_ReturnsNull()
    {
        const ulong id = 10;
        const bool findOrThrow = false;

        var command = new SelectVaultProposalByIdQuery(id, findOrThrow);

        _dbContext.Setup(db => db.ExecuteFindAsync<VaultProposalEntity>(It.IsAny<DatabaseQuery>()))
            .Returns(() => Task.FromResult<VaultProposalEntity>(null));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeNull();
    }
}
