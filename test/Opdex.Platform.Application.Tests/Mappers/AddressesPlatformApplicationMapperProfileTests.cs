using FluentAssertions;
using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.Application.Abstractions.Models.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Models.Markets;
using Opdex.Platform.Application.Abstractions.Models.MarketTokens;
using Opdex.Platform.Application.Abstractions.Models.Tokens;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Application.Abstractions.Models.Vaults;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Domain.Models.LiquidityPools;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Domain.Models.Transactions;
using Opdex.Platform.Domain.Models.Vaults;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Opdex.Platform.Application.Tests.Mappers;

public class AddressesPlatformApplicationMapperProfileTests : PlatformApplicationMapperProfileTests
{
    [Fact]
    public void From_AddressAllowance_To_AddressAllowanceDto()
    {
        // Arrange
        var model = new AddressAllowance(5L, 15L, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", "PQFv8x66vXEQEjw7ZBi8kCavrz15S1ShcG", new UInt256("5000060000"), 500, 1000);

        // Act
        var dto = _mapper.Map<AddressAllowanceDto>(model);

        // Assert
        dto.Owner.Should().Be(model.Owner);
        dto.Spender.Should().Be(model.Spender);
    }

    [Fact]
    public void From_AddressBalance_To_AddressBalanceDto()
    {
        // Arrange
        var model = new AddressBalance(5, 5, "PQFv8x66vXEQEjw7ZBi8kCavrz15S1ShcG", 500000000, 5, 50);

        // Act
        var dto = _mapper.Map<AddressBalanceDto>(model);

        // Assert
        dto.Address.Should().Be(model.Owner);
        dto.CreatedBlock.Should().Be(model.CreatedBlock);
        dto.ModifiedBlock.Should().Be(model.ModifiedBlock);
    }

    [Fact]
    public void From_LiquidityPool_To_LiquidityPoolDto()
    {
        // Arrange
        var model = new LiquidityPool(10, "PX2J4s4UHLfwZbDRJSvPoskKD25xQBHWYi", "BTC-CRS", 5, 15, 25, 500, 505);

        // Act
        var dto = _mapper.Map<LiquidityPoolDto>(model);

        // Assert
        dto.Id.Should().Be(model.Id);
        dto.Address.Should().Be(model.Address);
        dto.Name.Should().Be(model.Name);
        dto.CreatedBlock.Should().Be(model.CreatedBlock);
        dto.ModifiedBlock.Should().Be(model.ModifiedBlock);
    }

    [Fact]
    public void From_Market_To_MarketDto()
    {
        // Arrange
        var model = new Market(10, "t3eYNv5BL2FAC3iS1PEGC4VsovkDgib1MD", 1, 5, Address.Empty, new Address("t7hy4H51KzU6PPCL4QKCdgBGPLV9Jpmf9G"), false, false, false, 3, true, 20, 25);

        // Act
        var dto = _mapper.Map<MarketDto>(model);

        // Assert
        dto.Id.Should().Be(model.Id);
        dto.Address.Should().Be(model.Address);
        dto.PendingOwner.Should().Be(model.PendingOwner);
        dto.Owner.Should().Be(model.Owner);
        dto.AuthProviders.Should().Be(model.AuthProviders);
        dto.AuthTraders.Should().Be(model.AuthTraders);
        dto.AuthPoolCreators.Should().Be(model.AuthPoolCreators);
        dto.MarketFeeEnabled.Should().Be(model.MarketFeeEnabled);
        dto.TransactionFeePercent.Should().Be(model.TransactionFee / 10m);
        dto.CreatedBlock.Should().Be(model.CreatedBlock);
        dto.ModifiedBlock.Should().Be(model.ModifiedBlock);
    }

    [Fact]
    public void From_MarketToken_To_MarketTokenDto()
    {
        // Arrange
        var token = new Token(1, "PBWQ38k7iYnkfGPPGgMkN2kwXwmu3wuFYm", "STRAX", "STRAX", 8, 100_000_000, new UInt256("10000000000000000"), 9, 10);
        var market = new Market(19, "PAcm38V7iYnkfGPPGgCkN2kwXwmu3wuF5f", 2, 3, null, "nkfGPPGgMkN2kwXwmu3wuFYmPBWQ38k7iY", true, true, true, 3, true, 9, 10);
        var model = new MarketToken(market, token);
        model.SetSummary(new TokenSummary(5, market.Id, token.Id, 5.00m, 43.59m, 500, 1000));

        // Act
        var dto = _mapper.Map<MarketTokenDto>(model);

        // Assert
        dto.Id.Should().Be(model.Id);
        dto.Address.Should().Be(model.Address);
        dto.Market.Should().Be(model.Market.Address);
        dto.Decimals.Should().Be(model.Decimals);
        dto.Name.Should().Be(model.Name);
        dto.Sats.Should().Be(model.Sats);
        dto.Symbol.Should().Be(model.Symbol);
        dto.TotalSupply.Should().Be(model.TotalSupply.ToDecimal(model.Decimals));
        dto.CreatedBlock.Should().Be(model.CreatedBlock);
        dto.ModifiedBlock.Should().Be(model.ModifiedBlock);
        dto.Summary.DailyPriceChangePercent.Should().Be(model.Summary.DailyPriceChangePercent);
        dto.Summary.PriceUsd.Should().Be(model.Summary.PriceUsd);
        dto.Summary.CreatedBlock.Should().Be(model.Summary.CreatedBlock);
        dto.Summary.ModifiedBlock.Should().Be(model.Summary.ModifiedBlock);
    }

    [Fact]
    public void From_Token_To_TokenDto()
    {
        // Arrange
        var model = new Token(1, "PBWQ38k7iYnkfGPPGgMkN2kwXwmu3wuFYm", "STRAX", "STRAX", 8, 100_000_000, new UInt256("10000000000000000"), 9, 10);
        model.SetSummary(new TokenSummary(5, 0, model.Id, 5.00m, 43.59m, 500, 1000));

        // Act
        var dto = _mapper.Map<TokenDto>(model);

        // Assert
        dto.Id.Should().Be(model.Id);
        dto.Address.Should().Be(model.Address);
        dto.Decimals.Should().Be(model.Decimals);
        dto.Name.Should().Be(model.Name);
        dto.Sats.Should().Be(model.Sats);
        dto.Symbol.Should().Be(model.Symbol);
        dto.TotalSupply.Should().Be(model.TotalSupply.ToDecimal(model.Decimals));
        dto.CreatedBlock.Should().Be(model.CreatedBlock);
        dto.ModifiedBlock.Should().Be(model.ModifiedBlock);
        dto.Summary.DailyPriceChangePercent.Should().Be(model.Summary.DailyPriceChangePercent);
        dto.Summary.PriceUsd.Should().Be(model.Summary.PriceUsd);
        dto.Summary.CreatedBlock.Should().Be(model.Summary.CreatedBlock);
        dto.Summary.ModifiedBlock.Should().Be(model.Summary.ModifiedBlock);
    }

    [Fact]
    public void From_Transaction_To_TransactionDto()
    {
        // Arrange
        var model = new Transaction(1, new Sha256(5340958239), 2, 3, "PFrSHgtz2khDuciJdLAZtR2uKwgyXryMjM", "PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh", true, null, "PNvzq4pxJ5v3pp9kDaZyifKNspGD79E4qM");

        // Act
        var dto = _mapper.Map<TransactionDto>(model);

        // Assert
        dto.From.Should().Be(model.From);
        dto.To.Should().Be(model.To);
        dto.NewContractAddress.Should().Be(model.NewContractAddress);
        dto.GasUsed.Should().Be(model.GasUsed);
        dto.Success.Should().Be(model.Success);
        dto.Error.Should().Be(model.Error);
        dto.Hash.Should().Be(model.Hash);
    }

    [Fact]
    public void From_TransactionQuote_To_TransactionQuoteDto()
    {
        // Arrange
        Address walletAddress = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
        Address miningPool = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
        FixedDecimal crsToSend = FixedDecimal.Zero;

        var expectedParameters = new List<TransactionQuoteRequestParameter>
        {
            new("Amount", UInt256.Parse("100000000"))
        };

        var expectedRequest = new TransactionQuoteRequest(walletAddress, miningPool, crsToSend, "methodName", "callback", expectedParameters);

        var model = new TransactionQuote("1000", null, 23800, null, expectedRequest);

        // Act
        var dto = _mapper.Map<TransactionQuoteDto>(model);

        // Assert
        dto.Result.Should().Be(model.Result);
        dto.GasUsed.Should().Be(model.GasUsed);
        dto.Error.Should().Be(model.Error);
        dto.Request.To.Should().Be(model.Request.To);
        dto.Request.Amount.Should().Be(model.Request.Amount);
        dto.Request.Callback.Should().Be(model.Request.Callback);
        dto.Request.Method.Should().Be(model.Request.Method);
        dto.Request.Sender.Should().Be(model.Request.Sender);
        var dtoParameters = dto.Request.Parameters.ToList();
        var modelParameters = model.Request.Parameters.ToList();
        for (int i = 0; i < dto.Request.Parameters.Count; i++)
        {
            dtoParameters[i].Label.Should().Be(modelParameters[i].Label);
            dtoParameters[i].Value.Should().Be(modelParameters[i].Value.Serialize());
        }
    }

    [Fact]
    public void From_VaultCertificate_To_VaultCertificateDto()
    {
        // Arrange
        var model = new VaultCertificate(10, 10, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", UInt256.Parse("1000000000"), 10500, false, false, 500, 505);

        // Act
        var dto = _mapper.Map<VaultCertificateDto>(model);

        // Assert
        dto.Owner.Should().Be(model.Owner);
        dto.Redeemed.Should().Be(model.Redeemed);
        dto.Revoked.Should().Be(model.Revoked);
        dto.VestingStartBlock.Should().Be(model.CreatedBlock);
        dto.VestingEndBlock.Should().Be(model.VestedBlock);
        dto.CreatedBlock.Should().Be(model.CreatedBlock);
        dto.ModifiedBlock.Should().Be(model.ModifiedBlock);
    }

    [Fact]
    public void From_Vault_To_VaultDto()
    {
        // Arrange
        var model = new Vault(10, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 10, 10000000000, 100000, 50000000, 10000000, 1000000000, 50, 500);

        // Act
        var dto = _mapper.Map<VaultDto>(model);

        // Assert
        dto.Vault.Should().Be(model.Address);
        dto.VestingDuration.Should().Be(model.VestingDuration);
        dto.TotalPledgeMinimum.Should().Be(model.TotalPledgeMinimum.ToDecimal(TokenConstants.Cirrus.Decimals));
        dto.TotalVoteMinimum.Should().Be(model.TotalVoteMinimum.ToDecimal(TokenConstants.Cirrus.Decimals));
        dto.CreatedBlock.Should().Be(model.CreatedBlock);
        dto.ModifiedBlock.Should().Be(model.ModifiedBlock);
    }

    [Fact]
    public void From_VaultProposal_To_VaultProposalDto()
    {
        // Arrange
        var model = new VaultProposal(2, 5, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 50000000,
                                         "Proposal description", VaultProposalType.Revoke, VaultProposalStatus.Pledge, 100000,
                                         125000000, 40000000, 230000, true, 50, 500);
        // Act
        var dto = _mapper.Map<VaultProposalDto>(model);

        // Assert
        dto.ProposalId.Should().Be(model.PublicId);
        dto.Approved.Should().Be(model.Approved);
        dto.Creator.Should().Be(model.Creator);
        dto.Description.Should().Be(model.Description);
        dto.Expiration.Should().Be(model.Expiration);
        dto.Status.Should().Be(model.Status);
        dto.Type.Should().Be(model.Type);
        dto.Wallet.Should().Be(model.Wallet);
        dto.CreatedBlock.Should().Be(model.CreatedBlock);
        dto.ModifiedBlock.Should().Be(model.ModifiedBlock);
    }

    [Fact]
    public void From_VaultProposalPledge_To_VaultProposalPledgeDto()
    {
        // Arrange
        var model = new VaultProposalPledge(10, 5, 5, "tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm", 500000000000, 100000, 50, 500);

        // Act
        var dto = _mapper.Map<VaultProposalPledgeDto>(model);

        // Assert
        dto.Pledger.Should().Be(model.Pledger);
        dto.CreatedBlock.Should().Be(model.CreatedBlock);
        dto.ModifiedBlock.Should().Be(model.ModifiedBlock);
    }

    [Fact]
    public void From_VaultProposalVote_To_VaultProposalVoteDto()
    {
        // Arrange
        var model = new VaultProposalVote(5, 5, 5, "tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm", 500000000000, 100000, true, 50, 500);

        // Act
        var dto = _mapper.Map<VaultProposalVoteDto>(model);

        // Assert
        dto.Voter.Should().Be(model.Voter);
        dto.InFavor.Should().Be(model.InFavor);
        dto.CreatedBlock.Should().Be(model.CreatedBlock);
        dto.ModifiedBlock.Should().Be(model.ModifiedBlock);
    }
}
