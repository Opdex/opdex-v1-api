using MediatR;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Markets
{
    /// <summary>
    /// Determines whether or not an address is authorized with a particular market permission.
    /// </summary>
    public class CallCirrusGetMarketPermissionAuthorizationQuery : IRequest<bool>
    {
        /// <summary>
        /// Creates a request to determine whether an address is authorized with a particular market permission.
        /// </summary>
        /// <param name="market">The address of the market contract.</param>
        /// <param name="wallet">The address to determine permissions.</param>
        /// <param name="permission">The permission type, either: <see cref="MarketPermissionType.CreatePool" />, <see cref="MarketPermissionType.Provide" />, <see cref="MarketPermissionType.Trade" /> or <see cref="MarketPermissionType.SetPermissions" />.</param>
        /// <param name="blockHeight">The block height to search at.</param>
        public CallCirrusGetMarketPermissionAuthorizationQuery(Address market, Address wallet, MarketPermissionType permission, ulong blockHeight)
        {
            if (market == Address.Empty)
            {
                throw new ArgumentNullException(nameof(market), "Market address must be provided.");
            }

            if (wallet == Address.Empty)
            {
                throw new ArgumentNullException(nameof(wallet), "Wallet address must be provided.");
            }

            if (!permission.IsValid())
            {
                throw new ArgumentOutOfRangeException(nameof(permission), "Permission type must be valid.");
            }

            if (blockHeight == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
            }

            Market = market;
            Wallet = wallet;
            Permission = permission;
            BlockHeight = blockHeight;
        }

        public Address Market { get; }
        public Address Wallet { get; }
        public MarketPermissionType Permission { get; }
        public ulong BlockHeight { get; }
    }
}
