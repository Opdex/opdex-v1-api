using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models
{
    public class LocalCallResponseDto
    {
        public IList<InternalTransfer> InternalTransfers { get; set; }
        public GasConsumed GasConsumed { get; set; }
        public bool Revert { get; set; }
        public string ErrorMessage { get; set; }
        public object Return { get; set; }
        public IList<TransactionLogDto> Logs { get; set; }

        public T DeserializeValue<T>()
        {
            if (ErrorMessage.HasValue())
            {
                var error = JsonConvert.SerializeObject(ErrorMessage);
                throw new Exception(error);
            }

            if (Return == null) return default;

            var value = JsonConvert.SerializeObject(Return);
            return JsonConvert.DeserializeObject<T>(value);
        }
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
