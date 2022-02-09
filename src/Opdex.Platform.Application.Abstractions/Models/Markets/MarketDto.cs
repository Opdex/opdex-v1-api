using Opdex.Platform.Application.Abstractions.Models.Tokens;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.Models.Markets;

public class MarketDto
{
    public ulong Id { get; set; }
    public Address Address { get; set; }
    public MarketTokenDto StakingToken { get; set; }
    public TokenDto CrsToken { get; set; }
    public Address PendingOwner { get; set; }
    public Address Owner { get; set; }
    public bool AuthPoolCreators { get; set; }
    public bool AuthProviders { get; set; }
    public bool AuthTraders { get; set; }
    public decimal TransactionFeePercent { get; set; }
    public bool MarketFeeEnabled { get; set; }
    public ulong CreatedBlock { get; set; }
    public ulong ModifiedBlock { get; set; }
    public MarketSummaryDto Summary { get; set; }
}
