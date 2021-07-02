using System;

namespace Opdex.Platform.WebApi.Models.Responses.Tokens
{
    public class TokenSnapshotResponseModel : TokenSummaryResponseModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
