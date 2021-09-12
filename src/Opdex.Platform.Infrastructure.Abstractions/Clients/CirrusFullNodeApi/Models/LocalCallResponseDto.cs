using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Opdex.Platform.Common;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models
{
    public class LocalCallResponseDto
    {
        public IList<InternalTransfer> InternalTransfers { get; set; } = new List<InternalTransfer>();

        public IList<TransactionLogDto> Logs { get; set; } = new List<TransactionLogDto>();

        public GasConsumed GasConsumed { get; set; }
        public bool Revert { get; set; }
        public Error ErrorMessage { get; set; }
        public object Return { get; set; }

        // Todo: There should be two, Try and Regular
        // This should be done better in general
        public T DeserializeValue<T>()
        {
            if (ErrorMessage?.Value?.HasValue() == true)
            {
                var error = JsonConvert.SerializeObject(ErrorMessage);
                throw new Exception(error);
            }

            if (Return == null) return default;

            var value = JsonConvert.SerializeObject(Return);
            return JsonConvert.DeserializeObject<T>(value);
        }
    }

    public class Error
    {
        public string Value { get; set; }
    }

    public class GasConsumed
    {
        public uint Value { get; set; }
    }

    public class InternalTransfer
    {
        public string From { get; set; }
        public string To { get; set; }
        public ulong Value { get; set; }
    }
}
