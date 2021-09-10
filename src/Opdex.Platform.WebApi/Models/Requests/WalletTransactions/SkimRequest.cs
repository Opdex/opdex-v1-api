using Opdex.Platform.Common.Models;
using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Requests.WalletTransactions
{
    public class SkimRequest
    {
        /// <summary>
        /// The recipient of the skimmed tokens.
        /// </summary>
        [Required]
        public Address Recipient { get; set; }
    }
}
