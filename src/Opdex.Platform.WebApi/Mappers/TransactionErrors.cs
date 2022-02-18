using System;
using System.Text.RegularExpressions;

namespace Opdex.Platform.WebApi.Mappers;

public static class TransactionErrors
{
    private static readonly Regex MethodDefinitionRegex = new(@"(?<=at\s).+?(\(.*?\))", RegexOptions.Compiled, TimeSpan.FromMilliseconds(500));

    /// <summary>
    /// Attempts to match the last method definition in a transaction error.
    /// </summary>
    /// <param name="input">Raw transaction error.</param>
    /// <param name="result">Method definition in the form of ClassName.MethodName(string param1, int param2)</param>
    /// <returns>True if a match was found, otherwise false.</returns>
    private static bool TryMatchLastMethodDefintion(string input, out string result)
    {
        result = null;
        var any = TryMatchMethodDefinitions(input, out var matches);
        if (!any) return false;

        result = matches[^1].Value;
        return true;
    }

    /// <summary>
    /// Attempts to match all method definitions in a transaction error.
    /// </summary>
    /// <param name="input">Raw transaction error.</param>
    /// <param name="results">Method definition matches.</param>
    /// <returns>True if any matches were found, otherwise false.</returns>
    private static bool TryMatchMethodDefinitions(string input, out MatchCollection results)
    {
        var matches = MethodDefinitionRegex.Matches(input);
        results = matches;
        return matches.Count > 0;
    }

    /// <summary>
    /// General smart contract transaction errors.
    /// </summary>
    private static class General
    {
        /// <summary>
        /// Attempts to parse known smart contract transaction errors.
        /// </summary>
        /// <param name="input">The raw transaction error.</param>
        /// <param name="friendlyErrorMessage">Friendly error message that can be displayed to a user.</param>
        /// <returns>True if an error was recognised, otherwise false.</returns>
        internal static bool TryParseFriendlyErrorMessage(string input, out string friendlyErrorMessage)
        {
            friendlyErrorMessage = null;
            if (!IsOverflowException(input))
            {
                return false;
            }

            friendlyErrorMessage = "Value overflow.";
            return true;
        }

        /// <summary>
        /// Attempts to match an overflow exception message.
        /// </summary>
        private static bool IsOverflowException(string input)
        {
            return input.StartsWith(typeof(OverflowException).FullName!);
        }
    }

    /// <summary>
    /// Opdex smart contract transaction errors.
    /// </summary>
    public static class Opdex
    {
        private const string InvalidMarket = "INVALID_MARKET";
        private const string InvalidRouter = "INVALID_ROUTER";
        private const string Unauthorized = "UNAUTHORIZED";
        private const string FailedToSetMarketOwner = "SET_OWNER_FAILURE";
        private const string FailedToAuthorizeRouter = "AUTH_ROUTER_FAILURE";
        private const string InvalidStakingToken = "INVALID_STAKING_TOKEN";
        private const string InvalidToken = "INVALID_TOKEN";
        private const string PoolAlreadyExistsOnMarket = "POOL_EXISTS";
        private const string InvalidPool = "INVALID_POOL";
        private const string PermissionOutsideOfValidRange = "INVALID_PERMISSION";
        private const string MintFailed = "INVALID_MINT_RESPONSE";
        private const string InsufficientCrsAmount = "INSUFFICIENT_CRS_AMOUNT";
        private const string InsufficientSrcAmount = "INSUFFICIENT_SRC_AMOUNT";
        private const string ExcessiveInputAmount = "EXCESSIVE_INPUT_AMOUNT";
        private const string InsufficientAmount = "INSUFFICIENT_AMOUNT";
        private const string InsufficientInputAmount = "INSUFFICIENT_INPUT_AMOUNT";
        private const string InsufficientOutputAmount = "INSUFFICIENT_OUTPUT_AMOUNT";
        private const string InsufficientLiquidity = "INSUFFICIENT_LIQUIDITY";
        private const string InsufficientPrimaryAmount = "INSUFFICIENT_A_AMOUNT";
        private const string InsufficientSecondaryAmount = "INSUFFICIENT_B_AMOUNT";
        private const string TransferFailed = "INVALID_TRANSFER";
        private const string TransferFromFailed = "INVALID_TRANSFER_FROM";
        private const string TransferToFailed = "INVALID_TRANSFER_TO";
        private const string InvalidSwapAttempt = "INVALID_SWAP_ATTEMPT";
        private const string ExpiredDeadline = "EXPIRED_DEADLINE";
        private const string Locked = "LOCKED";
        private const string InvalidBalance = "INVALID_BALANCE";
        private const string InsufficientLiquidityBurned = "INSUFFICIENT_LIQUIDITY_BURNED";
        private const string InvalidOutputAmount = "INVALID_OUTPUT_AMOUNT";
        private const string InvalidTo = "INVALID_TO";
        private const string ZeroInputAmount = "ZERO_INPUT_AMOUNT";
        private const string StakingUnavailable = "STAKING_UNAVAILABLE";
        private const string CannotStakeZero = "CANNOT_STAKE_ZERO";
        private const string InvalidAmount = "INVALID_AMOUNT";
        private const string ProvidedRewardTooHigh = "PROVIDED_REWARD_TOO_HIGH";
        private const string InvalidSender = "INVALID_SENDER";
        private const string InvalidDistributionPeriod = "INVALID_DISTRIBUTION_PERIOD";
        private const string InvalidNomination = "INVALID_NOMINATION";
        private const string DuplicateNomination = "DUPLICATE_NOMINATION";
        private const string DistributionNotReady = "DISTRIBUTION_NOT_READY";
        private const string FailedGovernanceDistribution = "FAILED_GOVERNANCE_DISTRIBUTION";
        private const string FailedVaultDistribution = "FAILED_VAULT_DISTRIBUTION";
        private const string NominationPeriodActive = "NOMINATION_PERIOD_ACTIVE";
        private const string TokenDistributionRequired = "TOKEN_DISTRIBUTION_REQUIRED";
        private const string CertificateExists = "CERTIFICATE_EXISTS";
        private const string InsufficientVaultSupply = "INSUFFICIENT_VAULT_SUPPLY";
        private const string InvalidCertificate = "INVALID_CERTIFICATE";
        private const string InsufficientPledgeAmount = "INSUFFICIENT_PLEDGE_AMOUNT";
        private const string InvalidStatus = "INVALID_STATUS";
        private const string ProposalExpired = "PROPOSAL_EXPIRED";
        private const string InsufficientVoteAmount = "INSUFFICIENT_VOTE_AMOUNT";
        private const string AlreadyVotedNotInFavor = "ALREADY_VOTED_NOT_IN_FAVOR";
        private const string AlreadyVotedInFavor = "ALREADY_VOTED_IN_FAVOR";
        private const string InvalidProposal = "INVALID_PROPOSAL";
        private const string AlreadyComplete = "ALREADY_COMPLETE";
        private const string ProposalInProgress = "PROPOSAL_IN_PROGRESS";
        private const string CertificateNotFound = "CERTIFICATE_NOT_FOUND";
        private const string CertificateVesting = "CERTIFICATE_VESTING";
        private const string RecipientProposalInProgress = "RECIPIENT_PROPOSAL_IN_PROGRESS";
        private const string ExcessiveAmount = "EXCESSIVE_AMOUNT";
        private const string InvalidDescription = "INVALID_DESCRIPTION";
        private const string NotPayable = "NOT_PAYABLE";
        private const string InsufficientDeposit = "INSUFFICIENT_DEPOSIT";
        private const string InvalidCreator = "INVALID_CREATOR";
        private const string InsufficientWithdrawAmount = "INSUFFICIENT_WITHDRAW_AMOUNT";
        private const string InsufficientFunds = "INSUFFICIENT_FUNDS";


        // Smart contract methods
        private const string DeployerSetOwnership = "OpdexMarketDeployer.SetPendingOwnership(Address pendingOwner)";
        private const string DeployerClaimOwnership = "OpdexMarketDeployer.ClaimPendingOwnership()";
        private const string DeployerCreateStandardMarket = "OpdexMarketDeployer.CreateStandardMarket(Address marketOwner, UInt32 transactionFee, Boolean authPoolCreators, Boolean authProviders, Boolean authTraders, Boolean enableMarketFee)";
        private const string DeployerCreateStakingMarket = "OpdexMarketDeployer.CreateStakingMarket(Address stakingToken)";

        private const string StandardMarketAuthorize = "OpdexStandardMarket.Authorize(Address address, byte permission, Boolean authorize)";
        private const string StandardMarketSetOwnership = "OpdexStandardMarket.SetPendingOwnership(Address pendingOwner)";
        private const string StandardMarketClaimOwnership = "OpdexStandardMarket.ClaimPendingOwnership()";
        private const string StandardMarketCreatePool = "OpdexStandardMarket.CreatePool(Address token)";
        private const string StandardMarketCollectFees = "OpdexStandardMarket.CollectMarketFees(Address token, UInt256 amount)";

        private const string StakingMarketCreatePool = "OpdexStakingMarket.CreatePool(Address token)";

        private const string RouterAddLiquidity = "OpdexRouter.AddLiquidity(Address token, UInt256 amountSrcDesired, UInt64 amountCrsMin, UInt256 amountSrcMin, Address to, UInt64 deadline)";
        private const string RouterRemoveLiquidity = "OpdexRouter.RemoveLiquidity(Address token, UInt256 liquidity, UInt64 amountCrsMin, UInt256 amountSrcMin, Address to, UInt64 deadline)";
        private const string RouterSwapExactCrsForSrc = "OpdexRouter.SwapExactCrsForSrc(UInt256 amountSrcOutMin, Address token, Address to, UInt64 deadline)";
        private const string RouterSwapSrcForExactCrs = "OpdexRouter.SwapSrcForExactCrs(UInt64 amountCrsOut, UInt256 amountSrcInMax, Address token, Address to, UInt64 deadline)";
        private const string RouterSwapExactSrcForCrs = "OpdexRouter.SwapExactSrcForCrs(UInt256 amountSrcIn, UInt64 amountCrsOutMin, Address token, Address to, UInt64 deadline)";
        private const string RouterSwapCrsForExactSrc = "OpdexRouter.SwapCrsForExactSrc(UInt256 amountSrcOut, Address token, Address to, UInt64 deadline)";
        private const string RouterSwapSrcForExactSrc = "OpdexRouter.SwapSrcForExactSrc(UInt256 amountSrcInMax, Address tokenIn, UInt256 amountSrcOut, Address tokenOut, Address to, UInt64 deadline)";
        private const string RouterSwapExactSrcForSrc = "OpdexRouter.SwapExactSrcForSrc(UInt256 amountSrcIn, Address tokenIn, UInt256 amountSrcOutMin, Address tokenOut, Address to, UInt64 deadline)";
        private const string RouterGetLiquidityQuote = "OpdexRouter.GetLiquidityQuote(UInt256 amountA, UInt256 reserveA, UInt256 reserveB)";
        private const string RouterGetAmountOutOneHop = "OpdexRouter.GetAmountOut(UInt256 amountIn, UInt256 reserveIn, UInt256 reserveOut)";
        private const string RouterGetAmountOutMultiHop = "OpdexRouter.GetAmountOut(UInt256 tokenInAmount, UInt256 tokenInReserveCrs, UInt256 tokenInReserveSrc, UInt256 tokenOutReserveCrs, UInt256 tokenOutReserveSrc)";
        private const string RouterGetAmountInOneHop = "OpdexRouter.GetAmountIn(UInt256 amountOut, UInt256 reserveIn, UInt256 reserveOut)";
        private const string RouterGetAmountInMultiHop = "OpdexRouter.GetAmountIn(UInt256 tokenOutAmount, UInt256 tokenOutReserveCrs, UInt256 tokenOutReserveSrc, UInt256 tokenInReserveCrs, UInt256 tokenInReserveSrc)";

        private const string StandardPoolMint = "OpdexStandardPool.Mint(Address to)";
        private const string StandardPoolBurn = "OpdexStandardPool.Burn(Address to)";
        private const string StandardPoolSwap = "OpdexStandardPool.Swap(UInt64 amountCrsOut, UInt256 amountSrcOut, Address to, Byte[] data)";
        private const string StandardPoolSkim = "OpdexStandardPool.Skim(Address to)";
        private const string StandardPoolSync = "OpdexStandardPool.Sync()";

        private const string StakingPoolStartStaking = "OpdexStakingPool.StartStaking(UInt256 amount)";
        private const string StakingPoolCollectStakingRewards = "OpdexStakingPool.CollectStakingRewards(Boolean liquidate)";
        private const string StakingPoolStopStaking = "OpdexStakingPool.StopStaking(UInt256 amount, Boolean liquidate)";
        private const string StakingPoolMint = "OpdexStakingPool.Mint(Address to)";
        private const string StakingPoolBurn = "OpdexStakingPool.Burn(Address to)";
        private const string StakingPoolSwap = "OpdexStakingPool.Swap(UInt64 amountCrsOut, UInt256 amountSrcOut, Address to, Byte[] data)";
        private const string StakingPoolSkim = "OpdexStakingPool.Skim(Address to)";
        private const string StakingPoolSync = "OpdexStakingPool.Sync()";

        private const string MiningPoolStartMining = "OpdexMiningPool.StartMining(UInt256 amount)";
        private const string MiningPoolCollectMiningRewards = "OpdexMiningPool.CollectMiningRewards()";
        private const string MiningPoolStopMining = "OpdexMiningPool.StopMining(UInt256 amount)";
        private const string MiningPoolNotifyRewardAmount = "OpdexMiningPool.NotifyRewardAmount(UInt256 reward)";

        private const string MinedTokenNominateLiquidityPool = "OpdexMinedToken.NominateLiquidityPool()";
        private const string MinedTokenDistribute = "OpdexMinedToken.Distribute()";
        private const string MinedTokenDistributeGenesis = "OpdexMinedToken.DistributeGenesis(Address firstNomination, Address secondNomination, Address thirdNomination, Address fourthNomination)";
        private const string MinedTokenDistributeExecute = "OpdexMinedToken.DistributeExecute(UInt32 periodIndex, Address[] nominations)";

        private const string MiningGovernanceNotifyDistribution = "OpdexMiningGovernance.NotifyDistribution(Address firstNomination, Address secondNomination, Address thirdNomination, Address fourthNomination)";
        private const string MiningGovernanceNominateLiquidityPool = "OpdexMiningGovernance.NominateLiquidityPool(Address stakingPool, UInt256 weight)";
        private const string MiningGovernanceRewardMiningPools = "OpdexMiningGovernance.RewardMiningPools()";
        private const string MiningGovernanceRewardMiningPool = "OpdexMiningGovernance.RewardMiningPool()";

        private const string VaultNotifyDistribution = "OpdexVault.NotifyDistribution(UInt256 amount)";
        private const string VaultCreateNewCertificateProposal = "OpdexVault.CreateNewCertificateProposal(UInt256 amount, Address recipient, String description)";
        private const string VaultCreateRevokeCertificateProposal = "OpdexVault.CreateRevokeCertificateProposal(Address recipient, String description)";
        private const string VaultCreateTotalPledgeMinimumProposal = "OpdexVault.CreateTotalPledgeMinimumProposal(UInt256 amount, String description)";
        private const string VaultCreateTotalVoteMinimumProposal = "OpdexVault.CreateTotalVoteMinimumProposal(UInt256 amount, String description)";
        private const string VaultPledge = "OpdexVault.Pledge(UInt64 proposalId)";
        private const string VaultVote = "OpdexVault.Vote(UInt64 proposalId, Boolean inFavor)";
        private const string VaultWithdrawPledge = "OpdexVault.WithdrawPledge(UInt64 proposalId, UInt64 withdrawAmount)";
        private const string VaultWithdrawVote = "OpdexVault.WithdrawVote(UInt64 proposalId, UInt64 withdrawAmount)";
        private const string VaultCompleteProposal = "OpdexVault.CompleteProposal(UInt64 proposalId)";
        private const string VaultRedeemCertificate = "OpdexVault.RedeemCertificate()";


        // compiled Regex is much faster, should be initialize on startup
        private static readonly Regex OpdexErrorRegex = new(@$"(?<=OPDEX:\s).+?(?={Environment.NewLine})", RegexOptions.Compiled, TimeSpan.FromMilliseconds(500));

        /// <summary>
        /// Attempts to parse known errors that can occur during Opdex smart contract method calls.
        /// </summary>
        /// <param name="input">The raw transaction error.</param>
        /// <param name="friendlyErrorMessage">Friendly error message that can be displayed to a user.</param>
        /// <returns>True if an error was recognised, otherwise false.</returns>
        internal static bool TryParseFriendlyErrorMessage(string input, out string friendlyErrorMessage)
        {
            return TryMatchOpdexError(input, out var error)
                ? TryParseFriendlyOpdexErrorMessage(input, error, out friendlyErrorMessage)
                : General.TryParseFriendlyErrorMessage(input, out friendlyErrorMessage);
        }

        private static bool TryParseFriendlyOpdexErrorMessage(string input, string error, out string friendlyErrorMessage)
        {
            friendlyErrorMessage = null;
            // last method in the error determines which
            if (!TryMatchLastMethodDefintion(input, out var method)) return false;
            friendlyErrorMessage = (method, error) switch
            {
                // --- Deployer ---
                (DeployerSetOwnership, Unauthorized) => "Unable to set deployer ownership, unauthorized.",

                (DeployerClaimOwnership, Unauthorized) => "Unable to claim deployer ownership, unauthorized.",

                (DeployerCreateStandardMarket, Unauthorized) => "Unable to create market, unauthorized.",
                (DeployerCreateStandardMarket, InvalidMarket) => "Unable to create market, invalid transaction fee.",
                (DeployerCreateStandardMarket, InvalidRouter) => "Unable to create market, something unexpected happened.", // should never be raised
                (DeployerCreateStandardMarket, FailedToAuthorizeRouter) => "Unable to create market, something unexpected happened.", // should never be raised
                (DeployerCreateStandardMarket, FailedToSetMarketOwner) => "Unable to create market, something unexpected happened.", // should never be raised

                (DeployerCreateStakingMarket, Unauthorized) => "Unable to create market, unauthorized.",
                (DeployerCreateStakingMarket, InvalidStakingToken) => "Unable to create market, invalid staking token.",
                (DeployerCreateStakingMarket, InvalidMarket) => "Unable to create market, something unexpected happened.", // should never be raised
                (DeployerCreateStakingMarket, InvalidRouter) => "Unable to create market, something unexpected happened.", // should never be raised

                // --- Standard Market ---
                (StandardMarketAuthorize, Unauthorized) => "Unable to modify permissions, unauthorized.",
                (StandardMarketAuthorize, PermissionOutsideOfValidRange) => "Unable to modify permissions, unknown permission.",

                (StandardMarketSetOwnership, Unauthorized) => "Unable to set market ownership, unauthorized.",

                (StandardMarketClaimOwnership, Unauthorized) => "Unable to claim market ownership, unauthorized.",

                (StandardMarketCreatePool, Unauthorized) => "Unable to create pool, unauthorized.",
                (StandardMarketCreatePool, InvalidToken) => "Unable to create pool, invalid token.",
                (StandardMarketCreatePool, PoolAlreadyExistsOnMarket) => "Unable to create pool, already exists.",
                (StandardMarketCreatePool, InvalidPool) => "Unable to create pool, something unexpected happened.", // should never be raised

                (StandardMarketCollectFees, Unauthorized) => "Could not collect fees, unauthorized.",
                (StandardMarketCollectFees, InvalidPool) => "Could not collect fees, no pool exists for token.",
                (StandardMarketCollectFees, TransferToFailed) => "Could not collect fees, not enough funds.",

                // --- Staking Market ---
                (StakingMarketCreatePool, InvalidToken) => "Unable to create pool, invalid token.",
                (StakingMarketCreatePool, PoolAlreadyExistsOnMarket) => "Unable to create pool, already exists.",
                (StakingMarketCreatePool, InvalidPool) => "Unable to create pool, something unexpected happened.", // should never be raised

                // --- Router ---
                (RouterAddLiquidity, ExpiredDeadline) => "Unable to add liquidity, deadline expired.",
                (RouterAddLiquidity, Unauthorized) => "Unable to add liquidity, unauthorized.",
                (RouterAddLiquidity, InvalidPool) => "Unable to add liquidity, invalid pool.",
                (RouterAddLiquidity, InsufficientAmount) => "Unable to add liquidity, input must be greater than zero.",
                (RouterAddLiquidity, InsufficientLiquidity) => "Unable to add liquidity, something unexpected happened.", // should never be raised
                (RouterAddLiquidity, InsufficientPrimaryAmount) => "Unable to add liquidity, insufficient CRS amount.",
                (RouterAddLiquidity, InsufficientSecondaryAmount) => "Unable to add liquidity, insufficient SRC amount.",
                (RouterAddLiquidity, TransferFailed) => "Unable to add liquidity, returning CRS change failed.", // should never be raised
                (RouterAddLiquidity, TransferFromFailed) => "Unable to add liquidity, returning SRC change failed.", // should never be raised
                (RouterAddLiquidity, MintFailed) => "Unable to add liquidity, LP token mint failed.", // should never be raised

                (RouterRemoveLiquidity, ExpiredDeadline) => "Unable to remove liquidity, deadline expired.",
                (RouterRemoveLiquidity, Unauthorized) => "Unable to remove liquidity, unauthorized.",
                (RouterRemoveLiquidity, InvalidPool) => "Unable to remove liquidity, invalid pool.",
                (RouterRemoveLiquidity, TransferFromFailed) => "Unable to remove liquidity, transferring LP tokens failed.",
                (RouterRemoveLiquidity, InsufficientCrsAmount) => "Unable to remove liquidity, insufficient CRS redeemed.",
                (RouterRemoveLiquidity, InsufficientSrcAmount) => "Unable to remove liquidity, insufficient SRC redeemed.",

                (RouterSwapExactCrsForSrc, ExpiredDeadline) => "Unable to process swap, deadline expired.",
                (RouterSwapExactCrsForSrc, Unauthorized) => "Unable to process swap, unauthorized.",
                (RouterSwapExactCrsForSrc, InvalidPool) => "Unable to process swap, invalid pool.",
                (RouterSwapExactCrsForSrc, InsufficientInputAmount) => "Unable to process swap, insufficient CRS.",
                (RouterSwapExactCrsForSrc, InsufficientLiquidity) => "Unable to process swap, insufficient liquidity.",
                (RouterSwapExactCrsForSrc, InsufficientOutputAmount) => "Unable to process swap, output too low.",
                (RouterSwapExactCrsForSrc, InvalidSwapAttempt) => "Unable to process swap, something unexpected happened.", // should never be raised

                (RouterSwapSrcForExactCrs, ExpiredDeadline) => "Unable to process swap, deadline expired.",
                (RouterSwapSrcForExactCrs, Unauthorized) => "Unable to process swap, unauthorized.",
                (RouterSwapSrcForExactCrs, InvalidPool) => "Unable to process swap, invalid pool.",
                (RouterSwapSrcForExactCrs, ExcessiveInputAmount) => "Unable to process swap, additional input required.",
                (RouterSwapSrcForExactCrs, InsufficientLiquidity) => "Unable to process swap, insufficient liquidity.",
                (RouterSwapSrcForExactCrs, InsufficientOutputAmount) => "Unable to process swap, insufficient output.",
                (RouterSwapSrcForExactCrs, TransferFromFailed) => "Unable to process swap, SRC transfer failed.",
                (RouterSwapSrcForExactCrs, InvalidSwapAttempt) => "Unable to process swap, something unexpected happened.", // should never be raised

                (RouterSwapExactSrcForCrs, ExpiredDeadline) => "Unable to process swap, deadline expired.",
                (RouterSwapExactSrcForCrs, Unauthorized) => "Unable to process swap, unauthorized.",
                (RouterSwapExactSrcForCrs, InvalidPool) => "Unable to process swap, invalid pool.",
                (RouterSwapExactSrcForCrs, InsufficientInputAmount) => "Unable to process swap, insufficient SRC.",
                (RouterSwapExactSrcForCrs, InsufficientLiquidity) => "Unable to process swap, insufficient liquidity.",
                (RouterSwapExactSrcForCrs, InsufficientOutputAmount) => "Unable to process swap, output too low.",
                (RouterSwapExactSrcForCrs, TransferFromFailed) => "Unable to process swap, SRC transfer failed.",
                (RouterSwapExactSrcForCrs, InvalidSwapAttempt) => "Unable to process swap, something unexpected happened.", // should never be raised

                (RouterSwapCrsForExactSrc, ExpiredDeadline) => "Unable to process swap, deadline expired.",
                (RouterSwapCrsForExactSrc, Unauthorized) => "Unable to process swap, unauthorized.",
                (RouterSwapCrsForExactSrc, InvalidPool) => "Unable to process swap, invalid pool.",
                (RouterSwapCrsForExactSrc, ExcessiveInputAmount) => "Unable to process swap, CRS input too low.",
                (RouterSwapCrsForExactSrc, InsufficientLiquidity) => "Unable to process swap, insufficient liquidity.",
                (RouterSwapCrsForExactSrc, InsufficientOutputAmount) => "Unable to process swap, output must be greater than zero.",
                (RouterSwapCrsForExactSrc, TransferFailed) => "Unable to process swap, could not return change.", // should never be raised
                (RouterSwapCrsForExactSrc, InvalidSwapAttempt) => "Unable to process swap, something unexpected happened.", // should never be raised

                (RouterSwapSrcForExactSrc, ExpiredDeadline) => "Unable to process swap, deadline expired.",
                (RouterSwapSrcForExactSrc, Unauthorized) => "Unable to process swap, unauthorized.",
                (RouterSwapSrcForExactSrc, InvalidPool) => "Unable to process swap, invalid pool(s).",
                (RouterSwapSrcForExactSrc, ExcessiveInputAmount) => "Unable to process swap, additional input required.",
                (RouterSwapSrcForExactSrc, InsufficientLiquidity) => "Unable to process swap, insufficient liquidity.",
                (RouterSwapSrcForExactSrc, InsufficientOutputAmount) => "Unable to process swap, output must be greater than zero.",
                (RouterSwapSrcForExactSrc, TransferFromFailed) => "Unable to process swap, SRC transfer failed.", // should never be raised
                (RouterSwapSrcForExactSrc, InvalidSwapAttempt) => "Unable to process swap, something unexpected happened.", // should never be raised

                (RouterSwapExactSrcForSrc, ExpiredDeadline) => "Unable to process swap, deadline expired.",
                (RouterSwapExactSrcForSrc, Unauthorized) => "Unable to process swap, unauthorized.",
                (RouterSwapExactSrcForSrc, InvalidPool) => "Unable to process swap, invalid pool(s).",
                (RouterSwapExactSrcForSrc, InsufficientInputAmount) => "Unable to process swap, insufficient SRC input.",
                (RouterSwapExactSrcForSrc, InsufficientLiquidity) => "Unable to process swap, insufficient liquidity.",
                (RouterSwapExactSrcForSrc, InsufficientOutputAmount) => "Unable to process swap, output too low.",
                (RouterSwapExactSrcForSrc, TransferFromFailed) => "Unable to process swap, SRC transfer failed.", // should never be raised
                (RouterSwapExactSrcForSrc, InvalidSwapAttempt) => "Unable to process swap, something unexpected happened.", // should never be raised

                (RouterGetLiquidityQuote, InsufficientAmount) => "Unable to get quote, input must be greater than zero.",
                (RouterGetLiquidityQuote, InsufficientLiquidity) => "Unable to get quote, insufficient liquidity.",

                (RouterGetAmountOutOneHop, InsufficientAmount) => "Unable to get quote, input must be greater than zero.",
                (RouterGetAmountOutOneHop, InsufficientLiquidity) => "Unable to get quote, insufficient liquidity.",

                (RouterGetAmountOutMultiHop, InsufficientAmount) => "Unable to get quote, input must be greater than zero.",
                (RouterGetAmountOutMultiHop, InsufficientLiquidity) => "Unable to get quote, insufficient liquidity.",

                (RouterGetAmountInOneHop, InsufficientAmount) => "Unable to get quote, output must be greater than zero.",
                (RouterGetAmountInOneHop, InsufficientLiquidity) => "Unable to get quote, insufficient liquidity.",

                (RouterGetAmountInMultiHop, InsufficientAmount) => "Unable to get quote, output must be greater than zero.",
                (RouterGetAmountInMultiHop, InsufficientLiquidity) => "Unable to get quote, insufficient liquidity.",

                // --- Standard Pool ---
                (StandardPoolMint, Locked) => "Unable to mint, locked.",
                (StandardPoolMint, Unauthorized) => "Unable to mint, unauthorized.",
                (StandardPoolMint, InvalidBalance) => "Unable to mint, could not get SRC balance.",
                (StandardPoolMint, InsufficientLiquidity) => "Unable to mint, insufficient liquidity.",

                (StandardPoolBurn, Locked) => "Unable to burn, locked.",
                (StandardPoolBurn, Unauthorized) => "Unable to burn, unauthorized.",
                (StandardPoolBurn, InvalidBalance) => "Unable to burn, could not get SRC balance.",
                (StandardPoolBurn, InsufficientLiquidityBurned) => "Unable to burn, insufficient liquidity.",
                (StandardPoolBurn, TransferFailed) => "Unable to burn, CRS transfer failed.",
                (StandardPoolBurn, TransferToFailed) => "Unable to burn, SRC transfer failed.",

                (StandardPoolSwap, Locked) => "Unable to swap, locked.",
                (StandardPoolSwap, Unauthorized) => "Unable to swap, unauthorized.",
                (StandardPoolSwap, InvalidOutputAmount) => "Unable to swap, CRS or SRC output must be greater than zero.",
                (StandardPoolSwap, InsufficientLiquidity) => "Unable to swap, insufficient liquidity.",
                (StandardPoolSwap, InvalidTo) => "Unable to swap, invalid output address.",
                (StandardPoolSwap, TransferFailed) => "Unable to swap, CRS transfer failed.",
                (StandardPoolSwap, TransferToFailed) => "Unable to swap, SRC transfer failed.",
                (StandardPoolSwap, InvalidBalance) => "Unable to swap, could not get SRC balance.",
                (StandardPoolSwap, ZeroInputAmount) => "Unable to swap, reserves cannot drop to zero.",
                (StandardPoolSwap, InsufficientInputAmount) => "Unable to swap, constant product cannot change.",

                (StandardPoolSkim, Locked) => "Unable to skim, locked.",
                (StandardPoolSkim, Unauthorized) => "Unable to skim, unauthorized.",
                (StandardPoolSkim, InvalidBalance) => "Unable to skim, could not get SRC balance.",
                (StandardPoolSkim, TransferFailed) => "Unable to skim, CRS transfer failed.",
                (StandardPoolSkim, TransferToFailed) => "Unable to skim, SRC transfer failed.",

                (StandardPoolSync, Locked) => "Unable to sync, locked.",
                (StandardPoolSync, Unauthorized) => "Unable to sync, unauthorized.",
                (StandardPoolSync, InvalidBalance) => "Unable to sync, could not get SRC balance.",

                // --- Staking Pool ---
                (StakingPoolStartStaking, Locked) => "Unable to start staking, locked.",
                (StakingPoolStartStaking, StakingUnavailable) => "Unable to start staking, unavailable.",
                (StakingPoolStartStaking, CannotStakeZero) => "Unable to start staking, amount must be greater than zero.",
                (StakingPoolStartStaking, TransferFromFailed) => "Unable to start staking, staking token transfer failed.",

                (StakingPoolCollectStakingRewards, Locked) => "Unable to collect rewards, locked.",
                (StakingPoolCollectStakingRewards, StakingUnavailable) => "Unable to collect rewards, unavailable.",
                (StakingPoolCollectStakingRewards, InvalidBalance) => "Unable to collect rewards, could not get SRC balance.",
                (StakingPoolCollectStakingRewards, InsufficientLiquidityBurned) => "Unable to collect rewards, insufficient liquidity.",
                (StakingPoolCollectStakingRewards, TransferFailed) => "Unable to collect rewards, CRS transfer failed.",
                (StakingPoolCollectStakingRewards, TransferToFailed) => "Unable to collect rewards, SRC transfer failed.",

                (StakingPoolStopStaking, Locked) => "Unable to stop staking, locked.",
                (StakingPoolStopStaking, StakingUnavailable) => "Unable to stop staking, unavailable.",
                (StakingPoolStopStaking, InvalidAmount) => "Unable to stop staking, invalid amount.",
                (StakingPoolStopStaking, InvalidBalance) => "Unable to stop staking, could not get SRC balance.",
                (StakingPoolStopStaking, InsufficientLiquidityBurned) => "Unable to stop staking, insufficient liquidity.",
                (StakingPoolStopStaking, TransferFailed) => "Unable to stop staking, CRS transfer failed.",
                (StakingPoolStopStaking, TransferToFailed) => "Unable to stop staking, SRC transfer failed.",

                (StakingPoolMint, Locked) => "Unable to mint, locked.",
                (StakingPoolMint, InvalidBalance) => "Unable to mint, could not get SRC balance.",
                (StakingPoolMint, InsufficientLiquidity) => "Unable to mint, insufficient liquidity.",

                (StakingPoolBurn, Locked) => "Unable to burn, locked.",
                (StakingPoolBurn, InvalidBalance) => "Unable to burn, could not get SRC balance.",
                (StakingPoolBurn, InsufficientLiquidityBurned) => "Unable to burn, insufficient liquidity.",
                (StakingPoolBurn, TransferFailed) => "Unable to burn, CRS transfer failed.",
                (StakingPoolBurn, TransferToFailed) => "Unable to burn, SRC transfer failed.",

                (StakingPoolSwap, Locked) => "Unable to swap, locked.",
                (StakingPoolSwap, InvalidOutputAmount) => "Unable to swap, CRS or SRC output must be greater than zero.",
                (StakingPoolSwap, InsufficientLiquidity) => "Unable to swap, insufficient liquidity.",
                (StakingPoolSwap, InvalidTo) => "Unable to swap, invalid output address.",
                (StakingPoolSwap, TransferFailed) => "Unable to swap, CRS transfer failed.",
                (StakingPoolSwap, TransferToFailed) => "Unable to swap, SRC transfer failed.",
                (StakingPoolSwap, InvalidBalance) => "Unable to swap, could not get SRC balance.",
                (StakingPoolSwap, ZeroInputAmount) => "Unable to swap, reserves cannot drop to zero.",
                (StakingPoolSwap, InsufficientInputAmount) => "Unable to swap, constant product cannot change.",

                (StakingPoolSkim, Locked) => "Unable to skim, locked.",
                (StakingPoolSkim, InvalidBalance) => "Unable to skim, could not get SRC balance.",
                (StakingPoolSkim, TransferFailed) => "Unable to skim, CRS transfer failed.",
                (StakingPoolSkim, TransferToFailed) => "Unable to skim, SRC transfer failed.",

                (StakingPoolSync, Locked) => "Unable to sync, locked.",
                (StakingPoolSync, InvalidBalance) => "Unable to sync, could not get SRC balance.",

                // --- Mining Pool ---
                (MiningPoolStartMining, Locked) => "Unable to start mining, locked.",
                (MiningPoolStartMining, InvalidAmount) => "Unable to start mining, amount must be greater than zero.",
                (MiningPoolStartMining, TransferFromFailed) => "Unable to start mining, mining token transfer failed.",

                (MiningPoolCollectMiningRewards, Locked) => "Unable to collect rewards, locked.",
                (MiningPoolCollectMiningRewards, TransferToFailed) => "Unable to collect rewards, SRC transfer failed.",

                (MiningPoolStopMining, Locked) => "Unable to stop mining, locked.",
                (MiningPoolStopMining, InvalidAmount) => "Unable to stop mining, invalid amount.",
                (MiningPoolStopMining, TransferToFailed) => "Unable to stop mining, SRC transfer failed.",

                (MiningPoolNotifyRewardAmount, Locked) => "Unable to notify, locked.",
                (MiningPoolNotifyRewardAmount, Unauthorized) => "Unable to notify, unauthorized.",
                (MiningPoolNotifyRewardAmount, InvalidBalance) => "Unable to notify, could not get SRC balance.",
                (MiningPoolNotifyRewardAmount, ProvidedRewardTooHigh) => "Unable to notify, reward too high.",

                // --- Mined Token ---
                (MinedTokenNominateLiquidityPool, InvalidSender) => "Unable to nominate liquidity pool, invalid sender.",

                (MinedTokenDistributeGenesis, InvalidDistributionPeriod) => "Unable to distribute genesis, already distributed.",
                (MinedTokenDistributeGenesis, Unauthorized) => "Unable to distribute genesis, unauthorized.",
                (MinedTokenDistributeGenesis, InvalidNomination) => "Unable to distribute genesis, nomination(s) invalid.",
                (MinedTokenDistributeGenesis, DuplicateNomination) => "Unable to distribute genesis, duplicate nomination.",

                (MinedTokenDistribute, InvalidDistributionPeriod) => "Unable to distribute, genesis not yet distributed.",

                (MinedTokenDistributeExecute, DistributionNotReady) => "Unable to distribute, not yet ready.",
                (MinedTokenDistributeExecute, FailedGovernanceDistribution) => "Unable to distribute, mining governance distribution failed.",
                (MinedTokenDistributeExecute, FailedVaultDistribution) => "Unable to distribute, vault distribution failed.",

                // --- Mining Governance ---
                (MiningGovernanceNotifyDistribution, InvalidSender) => "Unable to notify distribution, invalid sender.",

                (MiningGovernanceNominateLiquidityPool, InvalidSender) => "Unable to nominate liquidity pool, invalid sender.",

                (MiningGovernanceRewardMiningPools, Locked) => "Unable to reward mining pools, locked.",
                (MiningGovernanceRewardMiningPools, NominationPeriodActive) => "Unable to reward mining pools, nomination period not yet ended.",
                (MiningGovernanceRewardMiningPools, TransferToFailed) => "Unable to reward mining pools, mined token transfer failed.",
                (MiningGovernanceRewardMiningPools, TokenDistributionRequired) => "Unable to reward mining pools, something unexpected happened.",
                (MiningGovernanceRewardMiningPools, InvalidBalance) => "Unable to reward mining pools, something unexpected happened.",

                (MiningGovernanceRewardMiningPool, Locked) => "Unable to reward mining pools, locked.",
                (MiningGovernanceRewardMiningPool, NominationPeriodActive) => "Unable to reward mining pools, nomination period not yet ended.",
                (MiningGovernanceRewardMiningPool, TransferToFailed) => "Unable to reward mining pools, mined token transfer failed.",
                (MiningGovernanceRewardMiningPool, TokenDistributionRequired) => "Unable to reward mining pools, something unexpected happened.",
                (MiningGovernanceRewardMiningPool, InvalidBalance) => "Unable to reward mining pools, something unexpected happened.",

                // --- Vault ---
                (VaultNotifyDistribution, Unauthorized) => "Unable to notify distribution, unauthorized.",
                (VaultCreateNewCertificateProposal, InvalidCreator) => "Unable to create new certificate proposal, proposal creator must not be a smart contract.",
                (VaultCreateNewCertificateProposal, InsufficientDeposit) => "Unable to create new certificate proposal, proposal deposit is required.",
                (VaultCreateNewCertificateProposal, InvalidDescription) => "Unable to create new certificate proposal, invalid description.",
                (VaultCreateNewCertificateProposal, InvalidAmount) => "Unable to create new certificate proposal, amount must be greater than zero.",
                (VaultCreateNewCertificateProposal, CertificateExists) => "Unable to create new certificate proposal, recipient already has a certificate.",
                (VaultCreateNewCertificateProposal, InsufficientVaultSupply) => "Unable to create new certificate proposal, requested value exceeds available vault supply.",
                (VaultCreateNewCertificateProposal, RecipientProposalInProgress) => "Unable to create new certificate proposal, recipient already has an active proposal.",

                (VaultCreateRevokeCertificateProposal, InvalidCreator) => "Unable to create revoke certificate proposal, proposal creator must not be a smart contract.",
                (VaultCreateRevokeCertificateProposal, InsufficientDeposit) => "Unable to create revoke certificate proposal, proposal deposit is required.",
                (VaultCreateRevokeCertificateProposal, InvalidDescription) => "Unable to create revoke certificate proposal, invalid description.",
                (VaultCreateRevokeCertificateProposal, InvalidAmount) => "Unable to create revoke certificate proposal, amount must be greater than zero.",
                (VaultCreateRevokeCertificateProposal, InvalidCertificate) => "Unable to create revoke certificate proposal, certificate cannot be revoked.",
                (VaultCreateRevokeCertificateProposal, RecipientProposalInProgress) => "Unable to create revoke certificate proposal, recipient already has an active proposal.",

                (VaultCreateTotalPledgeMinimumProposal, InvalidCreator) => "Unable to create total pledge minimum proposal, proposal creator must not be a smart contract.",
                (VaultCreateTotalPledgeMinimumProposal, InsufficientDeposit) => "Unable to create total pledge minimum proposal, proposal deposit is required.",
                (VaultCreateTotalPledgeMinimumProposal, InvalidDescription) => "Unable to create total pledge minimum proposal, invalid description.",
                (VaultCreateTotalPledgeMinimumProposal, InvalidAmount) => "Unable to create total pledge minimum proposal, amount must be greater than zero.",
                (VaultCreateTotalPledgeMinimumProposal, ExcessiveAmount) => "Unable to create total pledge minimum proposal, proposed amount too high.",

                (VaultCreateTotalVoteMinimumProposal, InvalidCreator) => "Unable to create total vote minimum proposal, proposal creator must not be a smart contract.",
                (VaultCreateTotalVoteMinimumProposal, InsufficientDeposit) => "Unable to create total vote minimum proposal, proposal deposit is required.",
                (VaultCreateTotalVoteMinimumProposal, InvalidDescription) => "Unable to create total vote minimum proposal, invalid description.",
                (VaultCreateTotalVoteMinimumProposal, InvalidAmount) => "Unable to create total vote minimum proposal, amount must be greater than zero.",
                (VaultCreateTotalVoteMinimumProposal, ExcessiveAmount) => "Unable to create total vote minimum proposal, proposed amount too high.",

                (VaultPledge, InsufficientPledgeAmount) => "Unable to pledge, message valid must be greater than zero.",
                (VaultPledge, InvalidStatus) => "Unable to pledge, invalid status.",
                (VaultPledge, ProposalExpired) => "Unable to pledge, proposal expired.",

                (VaultVote, InsufficientVoteAmount) => "Unable to vote, message value must be greater than zero.",
                (VaultVote, InvalidStatus) => "Unable to vote, invalid status.",
                (VaultVote, ProposalExpired) => "Unable to vote, proposal expired.",
                (VaultVote, AlreadyVotedNotInFavor) => "Unable to vote, currently actively voting not in favor.",
                (VaultVote, AlreadyVotedInFavor) => "Unable to vote, currently actively voting in favor.",

                (VaultWithdrawPledge, NotPayable) => "Unable to withdraw pledge, message value expected to be zero.",
                (VaultWithdrawPledge, InsufficientWithdrawAmount) => "Unable to withdraw pledge, amount must be greater than zero.",
                (VaultWithdrawPledge, InsufficientFunds) => "Unable to withdraw pledge, requested amount exceeds balance.",
                (VaultWithdrawPledge, TransferFailed) => "Unable to withdraw pledge, CRS transfer failed.",

                (VaultWithdrawVote, NotPayable) => "Unable to withdraw vote, message value expected to be zero.",
                (VaultWithdrawVote, InsufficientWithdrawAmount) => "Unable to withdraw vote, amount must be greater than zero.",
                (VaultWithdrawVote, InsufficientFunds) => "Unable to withdraw vote, requested amount exceeds balance.",
                (VaultWithdrawVote, TransferFailed) => "Unable to withdraw vote, CRS transfer failed.",

                (VaultCompleteProposal, NotPayable) => "Unable to complete proposal, message value expected to be zero.",
                (VaultCompleteProposal, InvalidProposal) => "Unable to complete proposal, not found.",
                (VaultCompleteProposal, AlreadyComplete) => "Unable to complete proposal, already complete.",
                (VaultCompleteProposal, ProposalInProgress) => "Unable to complete proposal, not yet expired.",

                (VaultRedeemCertificate, NotPayable) => "Unable to redeem certificate, message value expected to be zero.",
                (VaultRedeemCertificate, CertificateNotFound) => "Unable to redeem certificate, not found.",
                (VaultRedeemCertificate, CertificateVesting) => "Unable to redeem certificate, vesting period still active.",
                (VaultRedeemCertificate, TransferToFailed) => "Unable to redeem certificate, SRC transfer failed.",

                _ => null
            };

            return friendlyErrorMessage is not null;
        }

        /// <summary>
        /// Attempts to match a custom Opdex error message.
        /// </summary>
        /// <param name="input">Raw transaction error.</param>
        /// <param name="result">Opdex error message.</param>
        /// <returns>True if a match was found, otherwise false.</returns>
        private static bool TryMatchOpdexError(string input, out string result)
        {
            var match = OpdexErrorRegex.Match(input.ReplaceLineEndings());
            result = match.Value;
            return match.Success;
        }
    }
}
