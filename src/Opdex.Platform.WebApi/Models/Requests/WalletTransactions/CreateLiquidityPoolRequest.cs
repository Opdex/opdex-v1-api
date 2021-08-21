using Opdex.Platform.Common.Models;
using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Requests.WalletTransactions
{
    public class CreateLiquidityPoolRequest
    {
        [Required]
        public Address Token { get; set; }
    }
}
