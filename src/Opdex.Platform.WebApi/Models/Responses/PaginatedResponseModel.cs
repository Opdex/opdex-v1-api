using NJsonSchema.Annotations;
using System.Collections.Generic;

namespace Opdex.Platform.WebApi.Models.Responses
{
    public abstract class PaginatedResponseModel<TItem>
    {
        [NotNull]
        public IEnumerable<TItem> Results { get; set; }

        [NotNull]
        public CursorResponseModel Paging { get; set; }
    }
}
