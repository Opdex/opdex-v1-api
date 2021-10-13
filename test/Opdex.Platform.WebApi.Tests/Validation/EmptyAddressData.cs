using Opdex.Platform.Common.Models;
using System.Collections;
using System.Collections.Generic;

namespace Opdex.Platform.WebApi.Tests.Validation
{
    public class EmptyAddressData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { Address.Empty };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
