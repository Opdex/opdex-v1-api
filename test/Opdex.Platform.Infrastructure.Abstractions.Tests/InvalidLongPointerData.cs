using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using System.Collections;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Abstractions.Tests
{
    /// <summary>
    /// Data for an invalid <see cref="long" /> cursor pointer
    /// </summary>
    public class InvalidLongPointerData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { PagingDirection.Forward, -1 };
            yield return new object[] { PagingDirection.Backward, -1 };
            yield return new object[] { PagingDirection.Backward, 0 }; // zero indicates first request, only possible to page forward
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
