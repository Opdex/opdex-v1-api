using System.Collections.Generic;

namespace Opdex.Platform.WebApi.Models.Responses;

public abstract class PaginatedResponseModel<TItem>
{
    /// <summary>
    /// Current page of results.
    /// </summary>
    public IEnumerable<TItem> Results { get; set; }

    /// <summary>
    /// Page navigation.
    /// </summary>
    public CursorResponseModel Paging { get; set; }
}