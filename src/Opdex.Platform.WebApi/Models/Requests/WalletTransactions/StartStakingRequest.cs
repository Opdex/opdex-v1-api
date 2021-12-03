using Opdex.Platform.Common.Models;
using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Requests.WalletTransactions;

public class StartStakingRequest
{
    /// <summary>
    /// The amount of tokens to stake.
    /// </summary>
    [Required]
    public FixedDecimal Amount { get; set; }
}