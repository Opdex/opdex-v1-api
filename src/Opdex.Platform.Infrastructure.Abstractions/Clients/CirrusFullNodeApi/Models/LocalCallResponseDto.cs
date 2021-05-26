using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models
{
    public class LocalCallResponseDto
    {
        public IList<object> InternalTransfers { get; set; }
        public object GasConsumed { get; set; }
        public bool Revert { get; set; }
        public object ErrorMessage { get; set; }
        public object Return { get; set; }
        public IList<TransactionLogDto> Logs { get; set; }

        public T DeserializeValue<T>()
        {
            if (ErrorMessage != null)
            {
                var error = JsonConvert.SerializeObject(ErrorMessage);
                throw new Exception(error);
            }

            if (Return == null)
            {
                return default;
            }
            
            var value = JsonConvert.SerializeObject(Return);
            return JsonConvert.DeserializeObject<T>(value);
        }
    }
}