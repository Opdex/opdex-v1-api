using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Requests.WalletTransactions
{
    public class QuoteReplayRequest
    {
        /// <summary>
        /// The base64 encoded string representation of a previously quoted transaction request.
        /// </summary>
        [Required]
        public string Quote { get; set; }
    }
}
