using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Opdex.Platform.WebApi.Models.Responses.OHLC
{
    public class OhlcDecimalResponseModel
    {
        [NotNull]
        [Range(0, double.MaxValue)]
        public decimal Open { get; set; }

        [NotNull]
        [Range(0, double.MaxValue)]
        public decimal High { get; set; }

        [NotNull]
        [Range(0, double.MaxValue)]
        public decimal Low { get; set; }

        [NotNull]
        [Range(0, double.MaxValue)]
        public decimal Close { get; set; }
    }
}
