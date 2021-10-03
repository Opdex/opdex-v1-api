using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Markets;
using System;

namespace Opdex.Platform.Application.Abstractions.Queries.Markets
{
    /// <summary>
    /// Retrieve select properties from a market smart contract based on the provided block height.
    /// </summary>
    public class RetrieveMarketContractSummaryQuery : IRequest<MarketContractSummary>
    {
        /// <summary>
        /// Constructor to create a retrieve market contract summary by address query.
        /// </summary>
        /// <param name="market">The address of the market contract.</param>
        /// <param name="blockHeight">The block height to query the contract's state at.</param>
        /// <param name="includePendingOwner">Flag to include the market pending owner property, default is false.</param>
        /// <param name="includeOwner">Flag to include the market owner property, default is false.</param>
        public RetrieveMarketContractSummaryQuery(Address market, ulong blockHeight, bool includePendingOwner = false, bool includeOwner = false)
        {
            if (market == Address.Empty)
            {
                throw new ArgumentNullException(nameof(market), "Market address must be provided.");
            }

            if (blockHeight == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
            }

            Market = market;
            BlockHeight = blockHeight;
            IncludePendingOwner = includePendingOwner;
            IncludeOwner = includeOwner;
        }

        public Address Market { get; }
        public ulong BlockHeight { get; }
        public bool IncludePendingOwner { get; }
        public bool IncludeOwner { get; }
    }
}
