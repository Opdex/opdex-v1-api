using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Platform.Domain.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.VaultGovernances.Proposals;
using Opdex.Platform.Infrastructure.Data.Handlers.VaultGovernances.Proposals;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.VaultGovernances.Proposals;

public class SelectVaultProposalsByModifiedBlockQueryHandlerTests
{
    private readonly Mock<IDbContext> _dbContext;
    private readonly SelectVaultProposalsByModifiedBlockQueryHandler _handler;

    public SelectVaultProposalsByModifiedBlockQueryHandlerTests()
    {
        var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();

        _dbContext = new Mock<IDbContext>();
        _handler = new SelectVaultProposalsByModifiedBlockQueryHandler(_dbContext.Object, mapper);
    }

    [Fact]
    public async Task SelectVaultProposalById_Success()
    {
        const ulong modifiedBlock = 2;

        var expectedList = new List<VaultProposalEntity> {
            new()
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
                Approved = true,
                CreatedBlock = 1,
                ModifiedBlock = modifiedBlock
            }
        };

        var command = new SelectVaultProposalsByModifiedBlockQuery(modifiedBlock);

        _dbContext.Setup(db => db.ExecuteQueryAsync<VaultProposalEntity>(It.Is<DatabaseQuery>(q => q.Sql.Contains("vault_proposal") &&
                                                                                                         q.Sql.Contains("ModifiedBlock = @ModifiedBlock"))))
            .ReturnsAsync(expectedList);

        var results = await _handler.Handle(command, CancellationToken.None);
        var resultsList = results.ToList();

        for (var i = 0; i < resultsList.Count; i++)
        {
            resultsList[i].Id.Should().Be(expectedList[i].Id);
            resultsList[i].PublicId.Should().Be(expectedList[i].PublicId);
            resultsList[i].VaultGovernanceId.Should().Be(expectedList[i].VaultGovernanceId);
            resultsList[i].Creator.Should().Be(expectedList[i].Creator);
            resultsList[i].Wallet.Should().Be(expectedList[i].Wallet);
            resultsList[i].Amount.Should().Be(expectedList[i].Amount);
            resultsList[i].Description.Should().Be(expectedList[i].Description);
            resultsList[i].Type.Should().Be((VaultProposalType)expectedList[i].ProposalTypeId);
            resultsList[i].Status.Should().Be((VaultProposalStatus)expectedList[i].ProposalStatusId);
            resultsList[i].Expiration.Should().Be(expectedList[i].Expiration);
            resultsList[i].YesAmount.Should().Be(expectedList[i].YesAmount);
            resultsList[i].NoAmount.Should().Be(expectedList[i].NoAmount);
            resultsList[i].PledgeAmount.Should().Be(expectedList[i].PledgeAmount);
            resultsList[i].Approved.Should().Be(expectedList[i].Approved);
            resultsList[i].CreatedBlock.Should().Be(expectedList[i].CreatedBlock);
            resultsList[i].ModifiedBlock.Should().Be(expectedList[i].ModifiedBlock);
        }
    }

    [Fact]
    public async Task SelectVaultProposalsByModifiedBlockQuery_ReturnsEmpty()
    {
        const ulong modifiedBlock = 2;
        var command = new SelectVaultProposalsByModifiedBlockQuery(modifiedBlock);

        _dbContext.Setup(db => db.ExecuteQueryAsync<VaultProposalEntity>(It.IsAny<DatabaseQuery>()))
            .ReturnsAsync(Enumerable.Empty<VaultProposalEntity>());

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeEmpty();
    }
}
