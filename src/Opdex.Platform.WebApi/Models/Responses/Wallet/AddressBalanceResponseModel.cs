using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.Wallet
{
    /// <summary>
    /// Details of a token balance for an address.
    /// </summary>
    public class AddressBalanceResponseModel
    {
        /// <summary>
        /// Amount of tokens.
        /// </summary>
        /// <value>10000.00000000</value>
        public FixedDecimal Balance { get; set; }

        /// <summary>
        /// Address of the token balance holder.
        /// </summary>
        /// <value>tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm</value>
        public Address Address { get; set; }

        /// <summary>
        /// Address of the SRC token.
        /// </summary>
        /// <value>t8WntmWKiLs1BdzoqPGXmPAYzUTpPb3DBw</value>
        public Address Token { get; set; }
    }
}
