namespace Opdex.Platform.Application.Abstractions.Models.Tokens;

public class TokenSummaryDto
{
    public decimal PriceUsd { get; set; }
    public decimal DailyPriceChangePercent { get; set; }
    public ulong ModifiedBlock { get; set; }
}