using MediatR;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Markets;
using System;

namespace Opdex.Platform.Application.Abstractions.Queries.Markets.Permissions
{
    /// <summary>
    /// Retrieve select properties about a wallet's permissions from a market contract based on the provided block height.
    /// </summary>
    public class RetrieveMarketContractPermissionSummaryQuery : IRequest<MarketContractPermissionSummary>
    {
        /// <summary>
        /// Constructor to create a retrieve market contract permission summary by a wallet address query.
        /// </summary>
        /// <param name="market">The address of the market contract.</param>
        /// <param name="wallet">The address of the wallet that permissions are being checked for..</param>
        /// <param name="permissionType">The type of permission being refreshed.</param>
        /// <param name="blockHeight">The block height to query the contract's state at.</param>
        /// <param name="includeAuthorization">Flag to include the permission authorization property, default is false.</param>
        public RetrieveMarketContractPermissionSummaryQuery(Address market, Address wallet, MarketPermissionType permissionType,
                                                            ulong blockHeight, bool includeAuthorization = false)
        {
            // Todo: This should have IncludeBlame however to retrieve this value, we have to search transaction receipts and logs.

            if (market == Address.Empty)
            {
                throw new ArgumentNullException(nameof(market), "Market address must be provided.");
            }

            if (wallet == Address.Empty)
            {
                throw new ArgumentNullException(nameof(wallet), "Wallet address must be provided.");
            }

            if (!permissionType.IsValid())
            {
                throw new ArgumentOutOfRangeException(nameof(permissionType), "Permission type must be valid.");
            }

            if (blockHeight == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
            }

            Market = market;
            Wallet = wallet;
            PermissionType = permissionType;
            BlockHeight = blockHeight;
            IncludeAuthorization = includeAuthorization;
        }

        public Address Market { get; }
        public Address Wallet { get; }
        public MarketPermissionType PermissionType { get; }
        public ulong BlockHeight { get; }
        public bool IncludeAuthorization { get; }
    }
}
