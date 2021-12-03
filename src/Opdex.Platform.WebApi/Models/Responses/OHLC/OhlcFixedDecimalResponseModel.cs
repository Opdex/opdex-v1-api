using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.OHLC;

public class OhlcFixedDecimalResponseModel
{
    public FixedDecimal Open { get; set; }
    public FixedDecimal High { get; set; }
    public FixedDecimal Low { get; set; }
    public FixedDecimal Close { get; set; }
}