using System;
using Opdex.Platform.Application.Abstractions.Models.OHLC;

namespace Opdex.Platform.WebApi.Models.Responses.Tokens
{
    public class TokenSummaryResponseModel
    {
        public OhlcDecimalDto Price { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}