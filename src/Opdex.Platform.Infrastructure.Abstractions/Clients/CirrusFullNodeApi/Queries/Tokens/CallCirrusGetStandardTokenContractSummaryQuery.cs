using System;
using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Tokens;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Tokens
{
    /// <summary>
    /// Create the call to Cirrus to get a token contract summary for any standard token.
    /// </summary>
    public class CallCirrusGetStandardTokenContractSummaryQuery : IRequest<StandardTokenContractSummary>
    {
        /// <summary>
        /// Constructor to create the call to cirrus to make a get a standard token contract summary query.
        /// </summary>
        /// <param name="token">The token contract address.</param>
        /// <param name="blockHeight">The block height to query the properties values at.</param>
        /// <param name="includeBaseProperties">Flag to include base token properties including name, symbol, decimals and sats. Default is false.</param>
        /// <param name="includeTotalSupply">Flag to include the total supply property of the token. Default is false.</param>
        public CallCirrusGetStandardTokenContractSummaryQuery(Address token, ulong blockHeight,
                                                              bool includeBaseProperties = false,
                                                              bool includeTotalSupply = false)
        {
            if (token == Address.Empty)
            {
                throw new ArgumentNullException(nameof(token), "Token address must be provided.");
            }

            if (blockHeight == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
            }

            Token = token;
            BlockHeight = blockHeight;
            IncludeBaseProperties = includeBaseProperties;
            IncludeTotalSupply = includeTotalSupply;
        }

        public Address Token { get; }
        public ulong BlockHeight { get; }
        public bool IncludeBaseProperties { get; }
        public bool IncludeTotalSupply { get; }
    }
}
