using System;
using Newtonsoft.Json;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Domain.Models.TransactionLogs
{
    public class LiquidityPoolCreatedLog : TransactionLog
    {
        public LiquidityPoolCreatedLog(dynamic log, string address, int sortOrder) 
            : base(nameof(LiquidityPoolCreatedLog), address, sortOrder)
        {
            string token = log?.token;
            string pool = log?.pool;
            
            if (!token.HasValue())
            {
                throw new ArgumentNullException(nameof(token));
            }
            
            if (!pool.HasValue())
            {
                throw new ArgumentNullException(nameof(pool));
            }

            Token = token;
            Pool = pool;
        }
        
        public LiquidityPoolCreatedLog(long id, long transactionId, string address, int sortOrder, string details)
            : base(nameof(LiquidityPoolCreatedLog), id, transactionId, address, sortOrder)
        {
            var logDetails = DeserializeLogDetails(details);
            Token = logDetails.Token;
            Pool = logDetails.Pool;
        }
        
        public string Token { get; }
        public string Pool { get; }
        
        private struct LogDetails
        {
            public string Token { get; set; }
            public string Pool { get; set; }
        }

        private static LogDetails DeserializeLogDetails(string details)
        {
            return JsonConvert.DeserializeObject<LogDetails>(details);
        }

        public override string SerializeLogDetails()
        {
            return JsonConvert.SerializeObject(new LogDetails
            {
                Token = Token,
                Pool = Pool
            });
        }
    }
}