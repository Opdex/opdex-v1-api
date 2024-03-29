using AutoMapper;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.Application.Abstractions.Models.Blocks;
using Opdex.Platform.Application.Abstractions.Models.Index;
using Opdex.Platform.Application.Abstractions.Models.MiningGovernances;
using Opdex.Platform.Application.Abstractions.Models.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Models.LiquidityPools.Snapshots;
using Opdex.Platform.Application.Abstractions.Models.Markets;
using Opdex.Platform.Application.Abstractions.Models.MarketTokens;
using Opdex.Platform.Application.Abstractions.Models.MiningPools;
using Opdex.Platform.Application.Abstractions.Models.Tokens;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Deployers;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.MiningGovernances;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Markets;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.MiningPools;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Tokens;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Vault;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Vaults;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Application.Abstractions.Models.Vaults;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.WebApi.Models;
using Opdex.Platform.WebApi.Models.Responses;
using Opdex.Platform.WebApi.Models.Responses.Blocks;
using Opdex.Platform.WebApi.Models.Responses.Index;
using Opdex.Platform.WebApi.Models.Responses.MiningGovernances;
using Opdex.Platform.WebApi.Models.Responses.LiquidityPools;
using Opdex.Platform.WebApi.Models.Responses.LiquidityPools.Snapshots;
using Opdex.Platform.WebApi.Models.Responses.LiquidityPools.Summary;
using Opdex.Platform.WebApi.Models.Responses.Markets;
using Opdex.Platform.WebApi.Models.Responses.MarketTokens;
using Opdex.Platform.WebApi.Models.Responses.MiningPools;
using Opdex.Platform.WebApi.Models.Responses.OHLC;
using Opdex.Platform.WebApi.Models.Responses.Tokens;
using Opdex.Platform.WebApi.Models.Responses.Transactions;
using Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents;
using Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.Deployers;
using Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.MiningGovernances;
using Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.LiquidityPools;
using Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.Markets;
using Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.MiningPools;
using Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.Tokens;
using Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.Vault;
using Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.Vaults;
using Opdex.Platform.WebApi.Models.Responses.Vaults;
using Opdex.Platform.WebApi.Models.Responses.Wallet;

namespace Opdex.Platform.WebApi.Mappers;

public class PlatformWebApiMapperProfile : Profile
{
    public PlatformWebApiMapperProfile()
    {
        CreateMap<WrappedTokenDetailsDto, WrappedTokenDetailsResponseModel>()
            .ForMember(dest => dest.Custodian, opt => opt.MapFrom(src => src.Custodian))
            .ForMember(dest => dest.Chain, opt => opt.MapFrom(src => src.Chain))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.Validated, opt => opt.MapFrom(src => src.Validated))
            .ForMember(dest => dest.Trusted, opt => opt.MapFrom(src => src.Trusted))
            .ForMember(dest => dest.CreatedBlock, opt => opt.MapFrom(src => src.CreatedBlock))
            .ForMember(dest => dest.ModifiedBlock, opt => opt.MapFrom(src => src.ModifiedBlock))
            .ValidateMemberList(MemberList.None);

        CreateMap<MinedTokenDistributionScheduleDto, MinedTokenDistributionScheduleResponseModel>()
            .ForMember(dest => dest.Vault, opt => opt.MapFrom(src => src.Vault))
            .ForMember(dest => dest.MiningGovernance, opt => opt.MapFrom(src => src.MiningGovernance))
            .ForMember(dest => dest.NextDistributionBlock, opt => opt.MapFrom(src => src.NextDistributionBlock))
            .ForMember(dest => dest.History, opt => opt.MapFrom(src => src.History))
            .ValidateMemberList(MemberList.None);

        CreateMap<MinedTokenDistributionItemDto, MinedTokenDistributionItemResponseModel>()
            .ForMember(dest => dest.Vault, opt => opt.MapFrom(src => src.Vault))
            .ForMember(dest => dest.MiningGovernance, opt => opt.MapFrom(src => src.MiningGovernance))
            .ForMember(dest => dest.Block, opt => opt.MapFrom(src => src.Block))
            .ValidateMemberList(MemberList.None);

        CreateMap<TokenDto, TokenResponseModel>()
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Symbol, opt => opt.MapFrom(src => src.Symbol))
            .ForMember(dest => dest.Decimals, opt => opt.MapFrom(src => src.Decimals))
            .ForMember(dest => dest.Sats, opt => opt.MapFrom(src => src.Sats.ToString()))
            .ForMember(dest => dest.TotalSupply, opt => opt.MapFrom(src => src.TotalSupply))
            .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src.Attributes))
            .ForMember(dest => dest.WrappedToken, opt => opt.MapFrom(src => src.WrappedToken))
            .ForMember(dest => dest.Distribution, opt => opt.MapFrom(src => src.Distribution))
            .ForMember(dest => dest.CreatedBlock, opt => opt.MapFrom(src => src.CreatedBlock))
            .ForMember(dest => dest.ModifiedBlock, opt => opt.MapFrom(src => src.ModifiedBlock))
            .ForMember(dest => dest.Summary, opt => opt.MapFrom(src => src.Summary))
            .ValidateMemberList(MemberList.None);

        CreateMap<TokensDto, TokensResponseModel>()
            .ForMember(dest => dest.Results, opt => opt.MapFrom(src => src.Tokens))
            .ForMember(dest => dest.Paging, opt => opt.MapFrom(src => src.Cursor))
            .ValidateMemberList(MemberList.None);

        CreateMap<MarketTokenDto, MarketTokenResponseModel>()
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Symbol, opt => opt.MapFrom(src => src.Symbol))
            .ForMember(dest => dest.Decimals, opt => opt.MapFrom(src => src.Decimals))
            .ForMember(dest => dest.Sats, opt => opt.MapFrom(src => src.Sats))
            .ForMember(dest => dest.TotalSupply, opt => opt.MapFrom(src => src.TotalSupply))
            .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src.Attributes))
            .ForMember(dest => dest.WrappedToken, opt => opt.MapFrom(src => src.WrappedToken))
            .ForMember(dest => dest.Distribution, opt => opt.MapFrom(src => src.Distribution))
            .ForMember(dest => dest.Summary, opt => opt.MapFrom(src => src.Summary))
            .ForMember(dest => dest.LiquidityPool, opt => opt.MapFrom(src => src.LiquidityPool))
            .ForMember(dest => dest.Market, opt => opt.MapFrom(src => src.Market))
            .ValidateMemberList(MemberList.None);

        CreateMap<MarketTokensDto, MarketTokensResponseModel>()
            .ForMember(dest => dest.Results, opt => opt.MapFrom(src => src.Tokens))
            .ForMember(dest => dest.Paging, opt => opt.MapFrom(src => src.Cursor))
            .ValidateMemberList(MemberList.None);

        CreateMap<MarketTokenSnapshotsDto, MarketTokenSnapshotsResponseModel>()
            .ForMember(dest => dest.Results, opt => opt.MapFrom(src => src.Snapshots))
            .ForMember(dest => dest.Paging, opt => opt.MapFrom(src => src.Cursor))
            .ValidateMemberList(MemberList.None);

        CreateMap<TokenSummaryDto, TokenSummaryResponseModel>()
            .ForMember(dest => dest.PriceUsd, opt => opt.MapFrom(src => src.PriceUsd))
            .ForMember(dest => dest.DailyPriceChangePercent, opt => opt.MapFrom(src => src.DailyPriceChangePercent))
            .ForMember(dest => dest.CreatedBlock, opt => opt.MapFrom(src => src.CreatedBlock))
            .ForMember(dest => dest.ModifiedBlock, opt => opt.MapFrom(src => src.ModifiedBlock))
            .ValidateMemberList(MemberList.None);

        CreateMap<TokenSnapshotDto, TokenSnapshotResponseModel>()
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
            .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.Timestamp))
            .ValidateMemberList(MemberList.None);

        CreateMap<TokenSnapshotsDto, TokenSnapshotsResponseModel>()
            .ForMember(dest => dest.Results, opt => opt.MapFrom(src => src.Snapshots))
            .ForMember(dest => dest.Paging, opt => opt.MapFrom(src => src.Cursor))
            .ValidateMemberList(MemberList.None);

        CreateMap<LiquidityPoolDto, LiquidityPoolResponseModel>()
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.TransactionFeePercent, opt => opt.MapFrom(src => src.TransactionFee))
            .ForMember(dest => dest.CreatedBlock, opt => opt.MapFrom(src => src.CreatedBlock))
            .ForMember(dest => dest.ModifiedBlock, opt => opt.MapFrom(src => src.ModifiedBlock))
            .ForMember(dest => dest.Market, opt => opt.MapFrom(src => src.Market))
            .ForMember(dest => dest.Tokens, opt => opt.MapFrom(src => src))
            .ForMember(dest => dest.MiningPool, opt => opt.MapFrom(src => src.MiningPool))
            .ForMember(dest => dest.Summary, opt => opt.MapFrom(src => src.Summary))
            .ValidateMemberList(MemberList.None);

        CreateMap<LiquidityPoolDto, LiquidityPoolTokenGroupResponseModel>()
            .ForMember(dest => dest.Crs, opt => opt.MapFrom(src => src.CrsToken))
            .ForMember(dest => dest.Src, opt => opt.MapFrom(src => src.SrcToken))
            .ForMember(dest => dest.Lp, opt => opt.MapFrom(src => src.LpToken))
            .ForMember(dest => dest.Staking, opt => opt.MapFrom(src => src.StakingToken))
            .ValidateMemberList(MemberList.None);

        CreateMap<LiquidityPoolsDto, LiquidityPoolsResponseModel>()
            .ForMember(dest => dest.Results, opt => opt.MapFrom(src => src.LiquidityPools))
            .ForMember(dest => dest.Paging, opt => opt.MapFrom(src => src.Cursor))
            .ValidateMemberList(MemberList.None);

        CreateMap<LiquidityPoolSummaryDto, LiquidityPoolSummaryResponseModel>()
            .ForMember(dest => dest.Reserves, opt => opt.MapFrom(src => src.Reserves))
            .ForMember(dest => dest.Rewards, opt => opt.MapFrom(src => src.Rewards))
            .ForMember(dest => dest.Staking, opt => opt.MapFrom(src => src.Staking))
            .ForMember(dest => dest.Volume, opt => opt.MapFrom(src => src.Volume))
            .ForMember(dest => dest.Cost, opt => opt.MapFrom(src => src.Cost))
            .ForMember(dest => dest.CreatedBlock, opt => opt.MapFrom(src => src.CreatedBlock))
            .ForMember(dest => dest.ModifiedBlock, opt => opt.MapFrom(src => src.ModifiedBlock))
            .ValidateMemberList(MemberList.None);

        CreateMap<LiquidityPoolSnapshotDto, LiquidityPoolSnapshotResponseModel>()
            .ForMember(dest => dest.TransactionCount, opt => opt.MapFrom(src => src.TransactionCount))
            .ForMember(dest => dest.Reserves, opt => opt.MapFrom(src => src.Reserves))
            .ForMember(dest => dest.Rewards, opt => opt.MapFrom(src => src.Rewards))
            .ForMember(dest => dest.Staking, opt => opt.MapFrom(src => src.Staking))
            .ForMember(dest => dest.Volume, opt => opt.MapFrom(src => src.Volume))
            .ForMember(dest => dest.Cost, opt => opt.MapFrom(src => src.Cost))
            .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.Timestamp))
            .ValidateMemberList(MemberList.None);

        CreateMap<ReservesDto, ReservesResponseModel>()
            .ForMember(dest => dest.Crs, opt => opt.MapFrom(src => src.Crs))
            .ForMember(dest => dest.Src, opt => opt.MapFrom(src => src.Src))
            .ForMember(dest => dest.Usd, opt => opt.MapFrom(src => src.Usd))
            .ForMember(dest => dest.DailyUsdChangePercent, opt => opt.MapFrom(src => src.DailyUsdChangePercent))
            .ValidateMemberList(MemberList.None);

        CreateMap<RewardsDto, RewardsResponseModel>()
            .ForMember(dest => dest.ProviderDailyUsd, opt => opt.MapFrom(src => src.ProviderDailyUsd))
            .ForMember(dest => dest.MarketDailyUsd, opt => opt.MapFrom(src => src.MarketDailyUsd))
            .ForMember(dest => dest.TotalDailyUsd, opt => opt.MapFrom(src => src.ProviderDailyUsd + src.MarketDailyUsd))
            .ValidateMemberList(MemberList.None);

        CreateMap<VolumeDto, VolumeResponseModel>()
            .ForMember(dest => dest.DailyUsd, opt => opt.MapFrom(src => src.DailyUsd))
            .ValidateMemberList(MemberList.None);

        CreateMap<StakingDto, StakingResponseModel>()
            .ForMember(dest => dest.Weight, opt => opt.MapFrom(src => src.Weight))
            .ForMember(dest => dest.Usd, opt => opt.MapFrom(src => src.Usd))
            .ForMember(dest => dest.DailyWeightChangePercent, opt => opt.MapFrom(src => src.DailyWeightChangePercent))
            .ForMember(dest => dest.Nominated, opt => opt.MapFrom(src => src.Nominated))
            .ValidateMemberList(MemberList.None);

        CreateMap<CostDto, CostResponseModel>()
            .ForMember(dest => dest.CrsPerSrc, opt => opt.MapFrom(src => src.CrsPerSrc))
            .ForMember(dest => dest.SrcPerCrs, opt => opt.MapFrom(src => src.SrcPerCrs))
            .ValidateMemberList(MemberList.None);

        CreateMap<ReservesSnapshotDto, ReservesSnapshotResponseModel>()
            .ForMember(dest => dest.Crs, opt => opt.MapFrom(src => src.Crs))
            .ForMember(dest => dest.Src, opt => opt.MapFrom(src => src.Src))
            .ForMember(dest => dest.Usd, opt => opt.MapFrom(src => src.Usd))
            .ValidateMemberList(MemberList.None);

        CreateMap<RewardsSnapshotDto, RewardsSnapshotResponseModel>()
            .ForMember(dest => dest.ProviderUsd, opt => opt.MapFrom(src => src.ProviderUsd))
            .ForMember(dest => dest.MarketUsd, opt => opt.MapFrom(src => src.MarketUsd))
            .ForMember(dest => dest.TotalUsd, opt => opt.MapFrom(src => src.MarketUsd + src.ProviderUsd))
            .ValidateMemberList(MemberList.None);

        CreateMap<VolumeSnapshotDto, VolumeSnapshotResponseModel>()
            .ForMember(dest => dest.Crs, opt => opt.MapFrom(src => src.Crs))
            .ForMember(dest => dest.Src, opt => opt.MapFrom(src => src.Src))
            .ForMember(dest => dest.Usd, opt => opt.MapFrom(src => src.Usd))
            .ValidateMemberList(MemberList.None);

        CreateMap<StakingSnapshotDto, StakingSnapshotResponseModel>()
            .ForMember(dest => dest.Weight, opt => opt.MapFrom(src => src.Weight))
            .ForMember(dest => dest.Usd, opt => opt.MapFrom(src => src.Usd))
            .ValidateMemberList(MemberList.None);

        CreateMap<CostSnapshotDto, CostSnapshotResponseModel>()
            .ForMember(dest => dest.CrsPerSrc, opt => opt.MapFrom(src => src.CrsPerSrc))
            .ForMember(dest => dest.SrcPerCrs, opt => opt.MapFrom(src => src.SrcPerCrs))
            .ValidateMemberList(MemberList.None);

        CreateMap<LiquidityPoolSnapshotsDto, LiquidityPoolSnapshotsResponseModel>()
            .ForMember(dest => dest.Results, opt => opt.MapFrom(src => src.Snapshots))
            .ForMember(dest => dest.Paging, opt => opt.MapFrom(src => src.Cursor))
            .ValidateMemberList(MemberList.None);

        CreateMap<MiningPoolDto, MiningPoolResponseModel>()
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.LiquidityPool, opt => opt.MapFrom(src => src.LiquidityPool))
            .ForMember(dest => dest.MiningPeriodEndBlock, opt => opt.MapFrom(src => src.MiningPeriodEndBlock))
            .ForMember(dest => dest.RewardPerBlock, opt => opt.MapFrom(src => src.RewardPerBlock.ToDecimal(TokenConstants.Opdex.Decimals)))
            .ForMember(dest => dest.RewardPerLpt, opt => opt.MapFrom(src => src.RewardPerLpt.ToDecimal(TokenConstants.Opdex.Decimals)))
            .ForMember(dest => dest.TokensMining, opt => opt.MapFrom(src => src.TokensMining.ToDecimal(TokenConstants.LiquidityPoolToken.Decimals)))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
            .ForMember(dest => dest.CreatedBlock, opt => opt.MapFrom(src => src.CreatedBlock))
            .ForMember(dest => dest.ModifiedBlock, opt => opt.MapFrom(src => src.ModifiedBlock))
            .ValidateMemberList(MemberList.None);

        CreateMap<MiningPoolsDto, MiningPoolsResponseModel>()
            .ForMember(dest => dest.Results, opt => opt.MapFrom(src => src.MiningPools))
            .ForMember(dest => dest.Paging, opt => opt.MapFrom(src => src.Cursor));

        CreateMap<OhlcDto<FixedDecimal>, OhlcFixedDecimalResponseModel>()
            .ForMember(dest => dest.Open, opt => opt.MapFrom(src => src.Open))
            .ForMember(dest => dest.High, opt => opt.MapFrom(src => src.High))
            .ForMember(dest => dest.Low, opt => opt.MapFrom(src => src.Low))
            .ForMember(dest => dest.Close, opt => opt.MapFrom(src => src.Close))
            .ValidateMemberList(MemberList.None);

        CreateMap<OhlcDto<decimal>, OhlcDecimalResponseModel>()
            .ForMember(dest => dest.Open, opt => opt.MapFrom(src => src.Open))
            .ForMember(dest => dest.High, opt => opt.MapFrom(src => src.High))
            .ForMember(dest => dest.Low, opt => opt.MapFrom(src => src.Low))
            .ForMember(dest => dest.Close, opt => opt.MapFrom(src => src.Close))
            .ValidateMemberList(MemberList.None);

        CreateMap<MarketSnapshotDto, MarketSnapshotResponseModel>()
            .ForMember(dest => dest.LiquidityUsd, opt => opt.MapFrom(src => src.LiquidityUsd))
            .ForMember(dest => dest.VolumeUsd, opt => opt.MapFrom(src => src.VolumeUsd))
            .ForMember(dest => dest.Staking, opt => opt.MapFrom(src => src.Staking))
            .ForMember(dest => dest.Rewards, opt => opt.MapFrom(src => src.Rewards))
            .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.Timestamp))
            .ValidateMemberList(MemberList.None);

        CreateMap<MarketDto, MarketResponseModel>()
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.PendingOwner, opt => opt.MapFrom(src => src.PendingOwner))
            .ForMember(dest => dest.Owner, opt => opt.MapFrom(src => src.Owner))
            .ForMember(dest => dest.Tokens, opt => opt.MapFrom(src => src))
            .ForMember(dest => dest.AuthPoolCreators, opt => opt.MapFrom(src => src.AuthPoolCreators))
            .ForMember(dest => dest.AuthProviders, opt => opt.MapFrom(src => src.AuthProviders))
            .ForMember(dest => dest.AuthTraders, opt => opt.MapFrom(src => src.AuthTraders))
            .ForMember(dest => dest.MarketFeeEnabled, opt => opt.MapFrom(src => src.MarketFeeEnabled))
            .ForMember(dest => dest.TransactionFeePercent, opt => opt.MapFrom(src => src.TransactionFeePercent))
            .ForMember(dest => dest.CreatedBlock, opt => opt.MapFrom(src => src.CreatedBlock))
            .ForMember(dest => dest.ModifiedBlock, opt => opt.MapFrom(src => src.ModifiedBlock))
            .ForMember(dest => dest.Summary, opt => opt.MapFrom(src => src.Summary))
            .ValidateMemberList(MemberList.None);

        CreateMap<MarketDto, MarketTokenGroupResponseModel>()
            .ForMember(dest => dest.Crs, opt => opt.MapFrom(src => src.CrsToken))
            .ForMember(dest => dest.Staking, opt => opt.MapFrom(src => src.StakingToken))
            .ValidateMemberList(MemberList.None);

        CreateMap<MarketsDto, MarketsResponseModel>()
            .ForMember(dest => dest.Results, opt => opt.MapFrom(src => src.Markets))
            .ForMember(dest => dest.Paging, opt => opt.MapFrom(src => src.Cursor))
            .ValidateMemberList(MemberList.None);

        CreateMap<MarketSummaryDto, MarketSummaryResponseModel>()
            .ForMember(dest => dest.LiquidityUsd, opt => opt.MapFrom(src => src.LiquidityUsd))
            .ForMember(dest => dest.DailyLiquidityUsdChangePercent, opt => opt.MapFrom(src => src.DailyLiquidityUsdChangePercent))
            .ForMember(dest => dest.VolumeUsd, opt => opt.MapFrom(src => src.VolumeUsd))
            .ForMember(dest => dest.Staking, opt => opt.MapFrom(src => src.Staking))
            .ForMember(dest => dest.Rewards, opt => opt.MapFrom(src => src.Rewards))
            .ForMember(dest => dest.LiquidityPoolCount, opt => opt.MapFrom(src => src.LiquidityPoolCount))
            .ForMember(dest => dest.CreatedBlock, opt => opt.MapFrom(src => src.CreatedBlock))
            .ForMember(dest => dest.ModifiedBlock, opt => opt.MapFrom(src => src.ModifiedBlock))
            .ValidateMemberList(MemberList.None);

        CreateMap<MarketStakingDto, MarketStakingResponseModel>()
            .ForMember(dest => dest.StakingWeight, opt => opt.MapFrom(src => src.StakingWeight))
            .ForMember(dest => dest.DailyStakingWeightChangePercent, opt => opt.MapFrom(src => src.DailyStakingWeightChangePercent))
            .ForMember(dest => dest.StakingUsd, opt => opt.MapFrom(src => src.StakingUsd))
            .ForMember(dest => dest.DailyStakingUsdChangePercent, opt => opt.MapFrom(src => src.DailyStakingUsdChangePercent))
            .ValidateMemberList(MemberList.None);

        CreateMap<MarketSnapshotsDto, MarketSnapshotsResponseModel>()
            .ForMember(dest => dest.Results, opt => opt.MapFrom(src => src.Snapshots))
            .ForMember(dest => dest.Paging, opt => opt.MapFrom(src => src.Cursor))
            .ValidateMemberList(MemberList.None);

        CreateMap<AddressAllowanceDto, ApprovedAllowanceResponseModel>()
            .ForMember(dest => dest.Allowance, opt => opt.MapFrom(src => src.Allowance))
            .ForMember(dest => dest.Owner, opt => opt.MapFrom(src => src.Owner))
            .ForMember(dest => dest.Spender, opt => opt.MapFrom(src => src.Spender))
            .ForMember(dest => dest.Token, opt => opt.MapFrom(src => src.Token));

        CreateMap<AddressBalanceDto, AddressBalanceResponseModel>()
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.Balance, opt => opt.MapFrom(src => src.Balance))
            .ForMember(dest => dest.Token, opt => opt.MapFrom(src => src.Token))
            .ForMember(dest => dest.CreatedBlock, opt => opt.MapFrom(src => src.CreatedBlock))
            .ForMember(dest => dest.ModifiedBlock, opt => opt.MapFrom(src => src.ModifiedBlock));

        CreateMap<AddressBalancesDto, AddressBalancesResponseModel>()
            .ForMember(dest => dest.Results, opt => opt.MapFrom(src => src.Balances))
            .ForMember(dest => dest.Paging, opt => opt.MapFrom(src => src.Cursor))
            .ValidateMemberList(MemberList.None);

        CreateMap<StakingPositionDto, StakingPositionResponseModel>()
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
            .ForMember(dest => dest.StakingToken, opt => opt.MapFrom(src => src.StakingToken))
            .ForMember(dest => dest.LiquidityPool, opt => opt.MapFrom(src => src.LiquidityPool))
            .ForMember(dest => dest.CreatedBlock, opt => opt.MapFrom(src => src.CreatedBlock))
            .ForMember(dest => dest.ModifiedBlock, opt => opt.MapFrom(src => src.ModifiedBlock));

        CreateMap<StakingPositionsDto, StakingPositionsResponseModel>()
            .ForMember(dest => dest.Results, opt => opt.MapFrom(src => src.Positions))
            .ForMember(dest => dest.Paging, opt => opt.MapFrom(src => src.Cursor));

        CreateMap<MiningPositionDto, MiningPositionResponseModel>()
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
            .ForMember(dest => dest.MiningToken, opt => opt.MapFrom(src => src.MiningToken))
            .ForMember(dest => dest.MiningPool, opt => opt.MapFrom(src => src.MiningPool))
            .ForMember(dest => dest.CreatedBlock, opt => opt.MapFrom(src => src.CreatedBlock))
            .ForMember(dest => dest.ModifiedBlock, opt => opt.MapFrom(src => src.ModifiedBlock));

        CreateMap<MiningPositionsDto, MiningPositionsResponseModel>()
            .ForMember(dest => dest.Results, opt => opt.MapFrom(src => src.Positions))
            .ForMember(dest => dest.Paging, opt => opt.MapFrom(src => src.Cursor));

        CreateMap<MiningGovernanceDto, MiningGovernanceResponseModel>()
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.MinedToken, opt => opt.MapFrom(src => src.MinedToken))
            .ForMember(dest => dest.PeriodEndBlock, opt => opt.MapFrom(src => src.PeriodEndBlock))
            .ForMember(dest => dest.PeriodRemainingBlocks, opt => opt.MapFrom(src => src.PeriodRemainingBlocks))
            .ForMember(dest => dest.PeriodBlockDuration, opt => opt.MapFrom(src => src.PeriodBlockDuration))
            .ForMember(dest => dest.MiningPoolRewardPerPeriod, opt => opt.MapFrom(src => src.MiningPoolRewardPerPeriod))
            .ForMember(dest => dest.PeriodsUntilRewardReset, opt => opt.MapFrom(src => src.PeriodsUntilRewardReset))
            .ForMember(dest => dest.TotalRewardsPerPeriod, opt => opt.MapFrom(src => src.TotalRewardsPerPeriod))
            .ForMember(dest => dest.CreatedBlock, opt => opt.MapFrom(src => src.CreatedBlock))
            .ForMember(dest => dest.ModifiedBlock, opt => opt.MapFrom(src => src.ModifiedBlock))
            .ValidateMemberList(MemberList.None);

        CreateMap<MiningGovernancesDto, MiningGovernancesResponseModel>()
            .ForMember(dest => dest.Results, opt => opt.MapFrom(src => src.MiningGovernances))
            .ForMember(dest => dest.Paging, opt => opt.MapFrom(src => src.Cursor));

        CreateMap<BlockDto, BlockResponseModel>()
            .ForMember(dest => dest.Height, opt => opt.MapFrom(src => src.Height))
            .ForMember(dest => dest.Hash, opt => opt.MapFrom(src => src.Hash))
            .ForMember(dest => dest.Time, opt => opt.MapFrom(src => src.Time))
            .ForMember(dest => dest.MedianTime, opt => opt.MapFrom(src => src.MedianTime))
            .ValidateMemberList(MemberList.None);

        CreateMap<BlocksDto, BlocksResponseModel>()
            .ForMember(dest => dest.Results, opt => opt.MapFrom(src => src.Blocks))
            .ForMember(dest => dest.Paging, opt => opt.MapFrom(src => src.Cursor));

        CreateMap<IndexerStatusDto, IndexerStatusResponseModel>()
            .ForMember(dest => dest.Available, opt => opt.MapFrom(src => src.Available))
            .ForMember(dest => dest.LatestBlock, opt => opt.MapFrom(src => src.LatestBlock))
            .ForMember(dest => dest.Locked, opt => opt.MapFrom(src => src.Locked))
            .ForMember(dest => dest.InstanceId, opt => opt.MapFrom(src => src.InstanceId))
            .ForMember(dest => dest.Reason, opt => opt.MapFrom(src => src.Reason.IsValid() ? src.Reason : (IndexLockReason?)null))
            .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.ModifiedDate))
            .ValidateMemberList(MemberList.None);

        CreateMap<CursorDto, CursorResponseModel>()
            .ForMember(dest => dest.Next, opt => opt.MapFrom(src => src.Next))
            .ForMember(dest => dest.Previous, opt => opt.MapFrom(src => src.Previous))
            .ValidateMemberList(MemberList.None);

        CreateMap<VaultCertificateDto, VaultCertificateResponseModel>()
            .ForMember(dest => dest.Owner, opt => opt.MapFrom(src => src.Owner))
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
            .ForMember(dest => dest.VestingStartBlock, opt => opt.MapFrom(src => src.VestingStartBlock))
            .ForMember(dest => dest.VestingEndBlock, opt => opt.MapFrom(src => src.VestingEndBlock))
            .ForMember(dest => dest.Redeemed, opt => opt.MapFrom(src => src.Redeemed))
            .ForMember(dest => dest.Revoked, opt => opt.MapFrom(src => src.Revoked))
            .ForMember(dest => dest.CreatedBlock, opt => opt.MapFrom(src => src.CreatedBlock))
            .ForMember(dest => dest.ModifiedBlock, opt => opt.MapFrom(src => src.ModifiedBlock))
            .ForMember(dest => dest.Proposals, opt => opt.MapFrom(src => src.Proposals));

        CreateMap<VaultCertificatesDto, VaultCertificatesResponseModel>()
            .ForMember(dest => dest.Results, opt => opt.MapFrom(src => src.Certificates))
            .ForMember(dest => dest.Paging, opt => opt.MapFrom(src => src.Cursor));

        CreateMap<VaultDto, VaultResponseModel>()
            .ForMember(dest => dest.Vault, opt => opt.MapFrom(src => src.Vault))
            .ForMember(dest => dest.Token, opt => opt.MapFrom(src => src.Token))
            .ForMember(dest => dest.TokensUnassigned, opt => opt.MapFrom(src => src.TokensUnassigned))
            .ForMember(dest => dest.TokensProposed, opt => opt.MapFrom(src => src.TokensProposed))
            .ForMember(dest => dest.TokensLocked, opt => opt.MapFrom(src => src.TokensLocked))
            .ForMember(dest => dest.TotalPledgeMinimum, opt => opt.MapFrom(src => src.TotalPledgeMinimum))
            .ForMember(dest => dest.TotalVoteMinimum, opt => opt.MapFrom(src => src.TotalVoteMinimum))
            .ForMember(dest => dest.VestingDuration, opt => opt.MapFrom(src => src.VestingDuration))
            .ForMember(dest => dest.CreatedBlock, opt => opt.MapFrom(src => src.CreatedBlock))
            .ForMember(dest => dest.ModifiedBlock, opt => opt.MapFrom(src => src.ModifiedBlock));

        CreateMap<VaultsDto, VaultsResponseModel>()
            .ForMember(dest => dest.Results, opt => opt.MapFrom(src => src.Vaults))
            .ForMember(dest => dest.Paging, opt => opt.MapFrom(src => src.Cursor));

        CreateMap<VaultProposalDto, VaultProposalResponseModel>()
            .ForMember(dest => dest.Vault, opt => opt.MapFrom(src => src.Vault))
            .ForMember(dest => dest.Token, opt => opt.MapFrom(src => src.Token))
            .ForMember(dest => dest.ProposalId, opt => opt.MapFrom(src => src.ProposalId))
            .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator))
            .ForMember(dest => dest.Wallet, opt => opt.MapFrom(src => src.Wallet))
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.Expiration, opt => opt.MapFrom(src => src.Expiration))
            .ForMember(dest => dest.YesAmount, opt => opt.MapFrom(src => src.YesAmount))
            .ForMember(dest => dest.NoAmount, opt => opt.MapFrom(src => src.NoAmount))
            .ForMember(dest => dest.PledgeAmount, opt => opt.MapFrom(src => src.PledgeAmount))
            .ForMember(dest => dest.Approved, opt => opt.MapFrom(src => src.Approved))
            .ForMember(dest => dest.CreatedBlock, opt => opt.MapFrom(src => src.CreatedBlock))
            .ForMember(dest => dest.ModifiedBlock, opt => opt.MapFrom(src => src.ModifiedBlock))
            .ForMember(dest => dest.Certificate, opt => opt.MapFrom(src => src.Certificate));

        CreateMap<VaultProposalsDto, VaultProposalsResponseModel>()
            .ForMember(dest => dest.Results, opt => opt.MapFrom(src => src.Proposals))
            .ForMember(dest => dest.Paging, opt => opt.MapFrom(src => src.Cursor));

        CreateMap<VaultProposalPledgeDto, VaultProposalPledgeResponseModel>()
            .ForMember(dest => dest.Vault, opt => opt.MapFrom(src => src.Vault))
            .ForMember(dest => dest.ProposalId, opt => opt.MapFrom(src => src.ProposalId))
            .ForMember(dest => dest.Pledger, opt => opt.MapFrom(src => src.Pledger))
            .ForMember(dest => dest.Pledge, opt => opt.MapFrom(src => src.Pledge))
            .ForMember(dest => dest.Balance, opt => opt.MapFrom(src => src.Balance))
            .ForMember(dest => dest.CreatedBlock, opt => opt.MapFrom(src => src.CreatedBlock))
            .ForMember(dest => dest.ModifiedBlock, opt => opt.MapFrom(src => src.ModifiedBlock));

        CreateMap<VaultProposalPledgesDto, VaultProposalPledgesResponseModel>()
            .ForMember(dest => dest.Results, opt => opt.MapFrom(src => src.Pledges))
            .ForMember(dest => dest.Paging, opt => opt.MapFrom(src => src.Cursor));

        CreateMap<VaultProposalVoteDto, VaultProposalVoteResponseModel>()
            .ForMember(dest => dest.Vault, opt => opt.MapFrom(src => src.Vault))
            .ForMember(dest => dest.ProposalId, opt => opt.MapFrom(src => src.ProposalId))
            .ForMember(dest => dest.Voter, opt => opt.MapFrom(src => src.Voter))
            .ForMember(dest => dest.Vote, opt => opt.MapFrom(src => src.Vote))
            .ForMember(dest => dest.Balance, opt => opt.MapFrom(src => src.Balance))
            .ForMember(dest => dest.CreatedBlock, opt => opt.MapFrom(src => src.CreatedBlock))
            .ForMember(dest => dest.ModifiedBlock, opt => opt.MapFrom(src => src.ModifiedBlock));

        CreateMap<VaultProposalVotesDto, VaultProposalVotesResponseModel>()
            .ForMember(dest => dest.Results, opt => opt.MapFrom(src => src.Votes))
            .ForMember(dest => dest.Paging, opt => opt.MapFrom(src => src.Cursor));

        // Transactions
        CreateMap<TransactionsDto, TransactionsResponseModel>()
            .ForMember(dest => dest.Results, opt => opt.MapFrom(src => src.Transactions))
            .ForMember(dest => dest.Paging, opt => opt.MapFrom(src => src.Cursor))
            .ValidateMemberList(MemberList.None);

        // Transaction
        CreateMap<TransactionDto, TransactionResponseModel>()
            .ForMember(dest => dest.Hash, opt => opt.MapFrom(src => src.Hash))
            .ForMember(dest => dest.Success, opt => opt.MapFrom(src => src.Success))
            .ForMember(dest => dest.NewContractAddress, opt => opt.MapFrom(src => src.NewContractAddress))
            .ForMember(dest => dest.Block, opt => opt.MapFrom(src => src.BlockDto))
            .ForMember(dest => dest.GasUsed, opt => opt.MapFrom(src => src.GasUsed))
            .ForMember(dest => dest.From, opt => opt.MapFrom(src => src.From))
            .ForMember(dest => dest.To, opt => opt.MapFrom(src => src.To))
            .ForMember(dest => dest.Events, opt => opt.MapFrom(src => src.Events))
            .ForMember(dest => dest.Error, opt => opt.Ignore())
            .AfterMap<TransactionErrorMappingAction>()
            .ValidateMemberList(MemberList.None);

        // Transaction Events
        CreateMap<TransactionEventDto, TransactionEvent>()
            .ForMember(dest => dest.EventType, opt => opt.MapFrom(src => src.EventType))
            .ForMember(dest => dest.Contract, opt => opt.MapFrom(src => src.Contract))
            .ForMember(dest => dest.SortOrder, opt => opt.MapFrom(src => src.SortOrder));

        CreateMap<OwnershipEventDto, OwnershipEvent>()
            .IncludeBase<TransactionEventDto, TransactionEvent>()
            .ForMember(dest => dest.From, opt => opt.MapFrom(src => src.From))
            .ForMember(dest => dest.To, opt => opt.MapFrom(src => src.To));

        // Deployer Transaction Events
        CreateMap<ClaimPendingDeployerOwnershipEventDto, ClaimPendingDeployerOwnershipEvent>()
            .IncludeBase<OwnershipEventDto, OwnershipEvent>();

        CreateMap<SetPendingDeployerOwnershipEventDto, SetPendingDeployerOwnershipEvent>()
            .IncludeBase<OwnershipEventDto, OwnershipEvent>();

        CreateMap<CreateMarketEventDto, CreateMarketEvent>()
            .IncludeBase<TransactionEventDto, TransactionEvent>()
            .ForMember(dest => dest.Market, opt => opt.MapFrom(src => src.Market))
            .ForMember(dest => dest.Owner, opt => opt.MapFrom(src => src.Owner))
            .ForMember(dest => dest.Router, opt => opt.MapFrom(src => src.Router))
            .ForMember(dest => dest.AuthPoolCreators, opt => opt.MapFrom(src => src.AuthPoolCreators))
            .ForMember(dest => dest.AuthProviders, opt => opt.MapFrom(src => src.AuthProviders))
            .ForMember(dest => dest.AuthTraders, opt => opt.MapFrom(src => src.AuthTraders))
            .ForMember(dest => dest.TransactionFeePercent, opt => opt.MapFrom(src => src.TransactionFeePercent))
            .ForMember(dest => dest.StakingToken, opt => opt.MapFrom(src => src.StakingToken))
            .ForMember(dest => dest.EnableMarketFee, opt => opt.MapFrom(src => src.EnableMarketFee));

        // Market Transaction Events
        CreateMap<CreateLiquidityPoolEventDto, CreateLiquidityPoolEvent>()
            .IncludeBase<TransactionEventDto, TransactionEvent>()
            .ForMember(dest => dest.LiquidityPool, opt => opt.MapFrom(src => src.LiquidityPool))
            .ForMember(dest => dest.Token, opt => opt.MapFrom(src => src.Token));

        CreateMap<ClaimPendingMarketOwnershipEventDto, ClaimPendingMarketOwnershipEvent>()
            .IncludeBase<OwnershipEventDto, OwnershipEvent>();

        CreateMap<SetPendingMarketOwnershipEventDto, SetPendingMarketOwnershipEvent>()
            .IncludeBase<OwnershipEventDto, OwnershipEvent>();

        CreateMap<ChangeMarketPermissionEventDto, ChangeMarketPermissionEvent>()
            .IncludeBase<TransactionEventDto, TransactionEvent>()
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.Permission, opt => opt.MapFrom(src => src.Permission))
            .ForMember(dest => dest.IsAuthorized, opt => opt.MapFrom(src => src.IsAuthorized));

        // Liquidity Pool Transaction Events
        CreateMap<ProvideEventDto, ProvideEvent>()
            .IncludeBase<TransactionEventDto, TransactionEvent>()
            .ForMember(dest => dest.AmountCrs, opt => opt.MapFrom(src => src.AmountCrs))
            .ForMember(dest => dest.AmountSrc, opt => opt.MapFrom(src => src.AmountSrc))
            .ForMember(dest => dest.AmountLpt, opt => opt.MapFrom(src => src.AmountLpt))
            .ForMember(dest => dest.TokenSrc, opt => opt.MapFrom(src => src.TokenSrc))
            .ForMember(dest => dest.TokenLp, opt => opt.MapFrom(src => src.TokenLp))
            .ForMember(dest => dest.TokenLpTotalSupply, opt => opt.MapFrom(src => src.TokenLpTotalSupply));

        CreateMap<AddLiquidityEventDto, AddLiquidityEvent>()
            .IncludeBase<ProvideEventDto, ProvideEvent>();

        CreateMap<RemoveLiquidityEventDto, RemoveLiquidityEvent>()
            .IncludeBase<ProvideEventDto, ProvideEvent>();

        CreateMap<ReservesChangeEventDto, ReservesChangeEvent>()
            .IncludeBase<TransactionEventDto, TransactionEvent>()
            .ForMember(dest => dest.Crs, opt => opt.MapFrom(src => src.Crs))
            .ForMember(dest => dest.Src, opt => opt.MapFrom(src => src.Src));

        CreateMap<SwapEventDto, SwapEvent>()
            .IncludeBase<TransactionEventDto, TransactionEvent>()
            .ForMember(dest => dest.Sender, opt => opt.MapFrom(src => src.Sender))
            .ForMember(dest => dest.To, opt => opt.MapFrom(src => src.To))
            .ForMember(dest => dest.AmountCrsIn, opt => opt.MapFrom(src => src.AmountCrsIn))
            .ForMember(dest => dest.AmountCrsOut, opt => opt.MapFrom(src => src.AmountCrsOut))
            .ForMember(dest => dest.AmountSrcIn, opt => opt.MapFrom(src => src.AmountSrcIn))
            .ForMember(dest => dest.AmountSrcOut, opt => opt.MapFrom(src => src.AmountSrcOut));

        CreateMap<CollectStakingRewardsEventDto, CollectStakingRewardsEvent>()
            .IncludeBase<TransactionEventDto, TransactionEvent>()
            .ForMember(dest => dest.Staker, opt => opt.MapFrom(src => src.Staker))
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount));

        CreateMap<StakeEventDto, StakeEvent>()
            .IncludeBase<TransactionEventDto, TransactionEvent>()
            .ForMember(dest => dest.Staker, opt => opt.MapFrom(src => src.Staker))
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
            .ForMember(dest => dest.TotalStaked, opt => opt.MapFrom(src => src.TotalStaked))
            .ForMember(dest => dest.StakerBalance, opt => opt.MapFrom(src => src.StakerBalance));

        CreateMap<StartStakingEventDto, StartStakingEvent>()
            .IncludeBase<StakeEventDto, StakeEvent>();

        CreateMap<StopStakingEventDto, StopStakingEvent>()
            .IncludeBase<StakeEventDto, StakeEvent>();

        // Mining Pool Transaction Events
        CreateMap<CollectMiningRewardsEventDto, CollectMiningRewardsEvent>()
            .IncludeBase<TransactionEventDto, TransactionEvent>()
            .ForMember(dest => dest.Miner, opt => opt.MapFrom(src => src.Miner))
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount));

        CreateMap<MineEventDto, MineEvent>()
            .IncludeBase<TransactionEventDto, TransactionEvent>()
            .ForMember(dest => dest.Miner, opt => opt.MapFrom(src => src.Miner))
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
            .ForMember(dest => dest.TotalSupply, opt => opt.MapFrom(src => src.TotalSupply))
            .ForMember(dest => dest.MinerBalance, opt => opt.MapFrom(src => src.MinerBalance));

        CreateMap<StartMiningEventDto, StartMiningEvent>()
            .IncludeBase<MineEventDto, MineEvent>();

        CreateMap<StopMiningEventDto, StopMiningEvent>()
            .IncludeBase<MineEventDto, MineEvent>();

        CreateMap<EnableMiningEventDto, EnableMiningEvent>()
            .IncludeBase<TransactionEventDto, TransactionEvent>()
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
            .ForMember(dest => dest.RewardRate, opt => opt.MapFrom(src => src.RewardRate))
            .ForMember(dest => dest.MiningPeriodEndBlock, opt => opt.MapFrom(src => src.MiningPeriodEndBlock));

        // Token Transaction Events
        CreateMap<TransferEventDto, TransferEvent>()
            .IncludeBase<TransactionEventDto, TransactionEvent>()
            .ForMember(dest => dest.From, opt => opt.MapFrom(src => src.From))
            .ForMember(dest => dest.To, opt => opt.MapFrom(src => src.To))
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount));

        CreateMap<ApprovalEventDto, ApprovalEvent>()
            .IncludeBase<TransactionEventDto, TransactionEvent>()
            .ForMember(dest => dest.Owner, opt => opt.MapFrom(src => src.Owner))
            .ForMember(dest => dest.Spender, opt => opt.MapFrom(src => src.Spender))
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount));

        CreateMap<DistributionEventDto, DistributionEvent>()
            .IncludeBase<TransactionEventDto, TransactionEvent>()
            .ForMember(dest => dest.VaultAmount, opt => opt.MapFrom(src => src.VaultAmount))
            .ForMember(dest => dest.MiningGovernanceAmount, opt => opt.MapFrom(src => src.MiningGovernanceAmount))
            .ForMember(dest => dest.PeriodIndex, opt => opt.MapFrom(src => src.PeriodIndex))
            .ForMember(dest => dest.TotalSupply, opt => opt.MapFrom(src => src.TotalSupply))
            .ForMember(dest => dest.NextDistributionBlock, opt => opt.MapFrom(src => src.NextDistributionBlock));

        // Interflux token transaction events
        CreateMap<SetInterfluxCustodianEventDto, SetInterfluxCustodianEvent>()
            .IncludeBase<OwnershipEventDto, OwnershipEvent>();

        CreateMap<SupplyChangeEventDto, SupplyChangeEvent>()
            .IncludeBase<TransactionEventDto, TransactionEvent>()
            .ForMember(dest => dest.TotalSupply, opt => opt.MapFrom(src => src.TotalSupply));

        // Mining governance Transaction Events
        CreateMap<RewardMiningPoolEventDto, RewardMiningPoolEvent>()
            .IncludeBase<TransactionEventDto, TransactionEvent>()
            .ForMember(dest => dest.StakingPool, opt => opt.MapFrom(src => src.StakingPool))
            .ForMember(dest => dest.MiningPool, opt => opt.MapFrom(src => src.MiningPool))
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount));

        CreateMap<NominationEventDto, NominationEvent>()
            .IncludeBase<TransactionEventDto, TransactionEvent>()
            .ForMember(dest => dest.StakingPool, opt => opt.MapFrom(src => src.StakingPool))
            .ForMember(dest => dest.MiningPool, opt => opt.MapFrom(src => src.MiningPool))
            .ForMember(dest => dest.Weight, opt => opt.MapFrom(src => src.Weight));

        // Vaults Transaction Events
        CreateMap<CreateVaultCertificateEventDto, CreateVaultCertificateEvent>()
            .IncludeBase<TransactionEventDto, TransactionEvent>()
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
            .ForMember(dest => dest.Holder, opt => opt.MapFrom(src => src.Holder))
            .ForMember(dest => dest.VestedBlock, opt => opt.MapFrom(src => src.VestedBlock));

        CreateMap<RevokeVaultCertificateEventDto, RevokeVaultCertificateEvent>()
            .IncludeBase<TransactionEventDto, TransactionEvent>()
            .ForMember(dest => dest.OldAmount, opt => opt.MapFrom(src => src.OldAmount))
            .ForMember(dest => dest.NewAmount, opt => opt.MapFrom(src => src.NewAmount))
            .ForMember(dest => dest.Holder, opt => opt.MapFrom(src => src.Holder))
            .ForMember(dest => dest.VestedBlock, opt => opt.MapFrom(src => src.VestedBlock));

        CreateMap<RedeemVaultCertificateEventDto, RedeemVaultCertificateEvent>()
            .IncludeBase<TransactionEventDto, TransactionEvent>()
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
            .ForMember(dest => dest.Holder, opt => opt.MapFrom(src => src.Holder))
            .ForMember(dest => dest.VestedBlock, opt => opt.MapFrom(src => src.VestedBlock));

        CreateMap<TransactionQuoteDto, TransactionQuoteResponseModel>()
            .ForMember(dest => dest.Result, opt => opt.MapFrom(src => src.Result))
            .ForMember(dest => dest.GasUsed, opt => opt.MapFrom(src => src.GasUsed))
            .ForMember(dest => dest.Events, opt => opt.MapFrom(src => src.Events))
            .ForMember(dest => dest.Request, opt => opt.MapFrom(src => src.Request))
            .ForMember(dest => dest.Error, opt => opt.Ignore())
            .AfterMap<TransactionErrorMappingAction>()
            .ValidateMemberList(MemberList.None);

        CreateMap<TransactionQuoteRequestDto, QuotedTransactionModel>()
            .ForMember(dest => dest.Sender, opt => opt.MapFrom(src => src.Sender))
            .ForMember(dest => dest.To, opt => opt.MapFrom(src => src.To))
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
            .ForMember(dest => dest.Method, opt => opt.MapFrom(src => src.Method))
            .ForMember(dest => dest.Parameters, opt => opt.MapFrom(src => src.Parameters))
            .ForMember(dest => dest.Callback, opt => opt.MapFrom(src => src.Callback))
            .ValidateMemberList(MemberList.None);

        CreateMap<TransactionQuoteRequestParameterDto, TransactionParameterModel>()
            .ForMember(dest => dest.Label, opt => opt.MapFrom(src => src.Label))
            .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Value))
            .ValidateMemberList(MemberList.None);

        // Vaults
        CreateMap<CompleteVaultProposalEventDto, CompleteVaultProposalEvent>()
            .IncludeBase<TransactionEventDto, TransactionEvent>()
            .ForMember(dest => dest.ProposalId, opt => opt.MapFrom(src => src.ProposalId))
            .ForMember(dest => dest.Approved, opt => opt.MapFrom(src => src.Approved));

        CreateMap<CreateVaultProposalEventDto, CreateVaultProposalEvent>()
            .IncludeBase<TransactionEventDto, TransactionEvent>()
            .ForMember(dest => dest.ProposalId, opt => opt.MapFrom(src => src.ProposalId))
            .ForMember(dest => dest.Wallet, opt => opt.MapFrom(src => src.Wallet))
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.Expiration, opt => opt.MapFrom(src => src.Expiration))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));

        CreateMap<VaultProposalPledgeEventDto, VaultProposalPledgeEvent>()
            .IncludeBase<TransactionEventDto, TransactionEvent>()
            .ForMember(dest => dest.ProposalId, opt => opt.MapFrom(src => src.ProposalId))
            .ForMember(dest => dest.Pledger, opt => opt.MapFrom(src => src.Pledger))
            .ForMember(dest => dest.PledgeAmount, opt => opt.MapFrom(src => src.PledgeAmount))
            .ForMember(dest => dest.PledgerAmount, opt => opt.MapFrom(src => src.PledgerAmount))
            .ForMember(dest => dest.ProposalPledgeAmount, opt => opt.MapFrom(src => src.ProposalPledgeAmount))
            .ForMember(dest => dest.TotalPledgeMinimumMet, opt => opt.MapFrom(src => src.TotalPledgeMinimumMet));

        CreateMap<VaultProposalWithdrawPledgeEventDto, VaultProposalWithdrawPledgeEvent>()
            .IncludeBase<TransactionEventDto, TransactionEvent>()
            .ForMember(dest => dest.ProposalId, opt => opt.MapFrom(src => src.ProposalId))
            .ForMember(dest => dest.Pledger, opt => opt.MapFrom(src => src.Pledger))
            .ForMember(dest => dest.WithdrawAmount, opt => opt.MapFrom(src => src.WithdrawAmount))
            .ForMember(dest => dest.PledgerAmount, opt => opt.MapFrom(src => src.PledgerAmount))
            .ForMember(dest => dest.ProposalPledgeAmount, opt => opt.MapFrom(src => src.ProposalPledgeAmount))
            .ForMember(dest => dest.PledgeWithdrawn, opt => opt.MapFrom(src => src.PledgeWithdrawn));

        CreateMap<VaultProposalVoteEventDto, VaultProposalVoteEvent>()
            .IncludeBase<TransactionEventDto, TransactionEvent>()
            .ForMember(dest => dest.ProposalId, opt => opt.MapFrom(src => src.ProposalId))
            .ForMember(dest => dest.Voter, opt => opt.MapFrom(src => src.Voter))
            .ForMember(dest => dest.VoteAmount, opt => opt.MapFrom(src => src.VoteAmount))
            .ForMember(dest => dest.VoterAmount, opt => opt.MapFrom(src => src.VoterAmount))
            .ForMember(dest => dest.ProposalYesAmount, opt => opt.MapFrom(src => src.ProposalYesAmount))
            .ForMember(dest => dest.ProposalNoAmount, opt => opt.MapFrom(src => src.ProposalNoAmount))
            .ForMember(dest => dest.InFavor, opt => opt.MapFrom(src => src.InFavor));

        CreateMap<VaultProposalWithdrawVoteEventDto, VaultProposalWithdrawVoteEvent>()
            .IncludeBase<TransactionEventDto, TransactionEvent>()
            .ForMember(dest => dest.ProposalId, opt => opt.MapFrom(src => src.ProposalId))
            .ForMember(dest => dest.Voter, opt => opt.MapFrom(src => src.Voter))
            .ForMember(dest => dest.WithdrawAmount, opt => opt.MapFrom(src => src.WithdrawAmount))
            .ForMember(dest => dest.VoterAmount, opt => opt.MapFrom(src => src.VoterAmount))
            .ForMember(dest => dest.ProposalYesAmount, opt => opt.MapFrom(src => src.ProposalYesAmount))
            .ForMember(dest => dest.ProposalNoAmount, opt => opt.MapFrom(src => src.ProposalNoAmount))
            .ForMember(dest => dest.VoteWithdrawn, opt => opt.MapFrom(src => src.VoteWithdrawn));
    }
}
