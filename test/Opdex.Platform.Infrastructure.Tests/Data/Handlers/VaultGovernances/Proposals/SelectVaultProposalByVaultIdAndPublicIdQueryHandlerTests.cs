using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Domain.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.VaultGovernances.Proposals;
using Opdex.Platform.Infrastructure.Data.Handlers.VaultGovernances.Proposals;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.VaultGovernances.Proposals;

public class SelectVaultProposalByVaultIdAndPublicIdQueryHandlerTests
{
    private readonly Mock<IDbContext> _dbContext;
    private readonly SelectVaultProposalByVaultIdAndPublicIdQueryHandler _handler;

    public SelectVaultProposalByVaultIdAndPublicIdQueryHandlerTests()
    {
        var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();

        _dbContext = new Mock<IDbContext>();
        _handler = new SelectVaultProposalByVaultIdAndPublicIdQueryHandler(_dbContext.Object, mapper);
    }

    [Fact]
    public async Task SelectVaultProposalByVaultIdAndPublicId_Success()
    {
        const ulong vaultId = 2;
        const ulong publicId = 11;

        var expectedEntity = new VaultProposalEntity
        {
            Id = 10,
            PublicId = 11,
            VaultGovernanceId = 2,
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
            CreatedBlock = 1,
            ModifiedBlock = 2
        };

        var command = new SelectVaultProposalByVaultIdAndPublicIdQuery(vaultId, publicId);

        _dbContext.Setup(db => db.ExecuteFindAsync<VaultProposalEntity>(It.Is<DatabaseQuery>(q => q.Sql.Contains("vault_proposal"))))
            .Returns(() => Task.FromResult(expectedEntity));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Id.Should().Be(expectedEntity.Id);
        result.PublicId.Should().Be(expectedEntity.PublicId);
        result.VaultGovernanceId.Should().Be(expectedEntity.VaultGovernanceId);
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
        result.CreatedBlock.Should().Be(expectedEntity.CreatedBlock);
        result.ModifiedBlock.Should().Be(expectedEntity.ModifiedBlock);
    }

    [Fact]
    public void SelectVaultProposalByVaultIdAndPublicId_Throws_NotFoundException()
    {
        const ulong vaultId = 2;
        const ulong publicId = 11;

        var command = new SelectVaultProposalByVaultIdAndPublicIdQuery(vaultId, publicId);

        _dbContext.Setup(db => db.ExecuteFindAsync<VaultProposalEntity>(It.IsAny<DatabaseQuery>()))
            .Returns(() => Task.FromResult<VaultProposalEntity>(null));

        _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage($"{nameof(VaultProposal)} not found.");
    }

    [Fact]
    public async Task SelectVaultProposalByVaultIdAndPublicId_ReturnsNull()
    {
        const ulong vaultId = 2;
        const ulong publicId = 11;
        const bool findOrThrow = false;

        var command = new SelectVaultProposalByVaultIdAndPublicIdQuery(vaultId, publicId, findOrThrow);

        _dbContext.Setup(db => db.ExecuteFindAsync<VaultProposalEntity>(It.IsAny<DatabaseQuery>()))
            .Returns(() => Task.FromResult<VaultProposalEntity>(null));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeNull();
    }
}
