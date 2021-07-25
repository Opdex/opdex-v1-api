using System.Collections.Generic;

namespace Opdex.Platform.WebApi.Models.Responses
{
    public abstract class PaginatedResponseModel<TItem>
    {
        public IEnumerable<TItem> Results { get; set; }
        public CursorResponseModel Paging { get; set; }
    }
}
