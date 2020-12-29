using System.Collections.Generic;

namespace Opdex.Core.Infrastructure.Abstractions.Integrations.CirrusFullNodeApi.Models
{
    public class LocalCallResponseDto
    {
        public IList<object> InternalTransfers { get; set; }
        public object GasConsumed { get; set; }
        public bool Revert { get; set; }
        public object ErrorMessage { get; set; }
        public object Return { get; set; }
        public IList<LogDto> Logs { get; set; }
    }
}