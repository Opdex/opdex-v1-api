using NJsonSchema.Annotations;
using Opdex.Platform.Common.Models;
using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Responses.Tokens
{
    public class TokenResponseModel
    {
        /// <summary>
        /// The token's smart contract address.
        /// </summary>
        [NotNull]
        public Address Address { get; set; }

        /// <summary>
        /// The token's name.
        /// </summary>
        [NotNull]
        public string Name { get; set; }

        /// <summary>
        /// The token's ticker symbol.
        /// </summary>
        [NotNull]
        public string Symbol { get; set; }

        /// <summary>
        /// The total number of decimal places the token has.
        /// </summary>
        [NotNull]
        [Range(0, double.MaxValue)]
        public int Decimals { get; set; }

        /// <summary>
        /// The total number of satoshis per full token.
        /// </summary>
        [NotNull]
        [Range(0, double.MaxValue)]
        public ulong Sats { get; set; }

        /// <summary>
        /// The total supply of the token as stored in contract.
        /// </summary>
        [NotNull]
        public FixedDecimal TotalSupply { get; set; }

        /// <summary>
        /// A summary including the token's USD price and daily price change percentage if exists. Market token's receive
        /// pricing specific to that market.
        /// </summary>
        public TokenSummaryResponseModel Summary { get; set; }
    }
}
