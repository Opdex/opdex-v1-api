using System.Diagnostics.CodeAnalysis;

namespace Opdex.Platform.WebApi.Models.Responses.OHLC
{
    public class OhlcDecimalResponseModel
    {
        [NotNull]
        public decimal Open { get; set; }

        [NotNull]
        public decimal High { get; set; }

        [NotNull]
        public decimal Low { get; set; }

        [NotNull]
        public decimal Close { get; set; }
    }
}
