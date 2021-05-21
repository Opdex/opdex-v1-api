using System;
using Newtonsoft.Json;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Domain.Models.TransactionLogs.MarketDeployers
{
    public class CreateMarketLog : TransactionLog
    {
        public CreateMarketLog(dynamic log, string address, int sortOrder) 
            : base(TransactionLogType.CreateMarketLog, address, sortOrder)
        {
            string market = log?.market;
            string owner = log?.owner;
            bool authPoolCreators = log?.authPoolCreators;
            bool authProviders = log?.authProviders;
            bool authTraders = log?.authTraders;
            uint fee = log?.fee;
            string stakingToken = log?.stakingToken;

            if (!market.HasValue())
            {
                throw new ArgumentNullException(nameof(market));
            }

            if (!stakingToken.HasValue())
            {
                throw new ArgumentNullException(nameof(stakingToken));
            }

            Market = market;
            Owner = owner;
            AuthPoolCreators = authPoolCreators;
            AuthProviders = authProviders;
            AuthTraders = authTraders;
            Fee = fee;
            StakingToken = stakingToken;
        }

        public CreateMarketLog(long id, long transactionId, string address, int sortOrder, string details)
            : base(TransactionLogType.CreateMarketLog, id, transactionId, address, sortOrder)
        {
            var logDetails = DeserializeLogDetails(details);
            Market = logDetails.Market;
            Owner = logDetails.Owner;
            AuthPoolCreators = logDetails.AuthPoolCreators;
            AuthProviders = logDetails.AuthProviders;
            AuthTraders = logDetails.AuthTraders;
            Fee = logDetails.Fee;
            StakingToken = logDetails.StakingToken;
        }
        
        public string Market { get; }
        public string Owner { get; }
        public bool AuthPoolCreators { get; }
        public bool AuthProviders { get; }
        public bool AuthTraders { get; }
        public uint Fee { get; }
        public string StakingToken { get; }

        public struct LogDetails
        {
            public string Market { get; set; }
            public string Owner { get; set; }
            public bool AuthPoolCreators { get; set; }
            public bool AuthProviders { get; set; }
            public bool AuthTraders { get; set; }
            public uint Fee { get; set; }
            public string StakingToken { get; set; }
        }
        
        private static LogDetails DeserializeLogDetails(string details)
        {
            return JsonConvert.DeserializeObject<LogDetails>(details);
        }

        public override string SerializeLogDetails()
        {
            return JsonConvert.SerializeObject(new LogDetails
            {
                Market = Market,
                Owner = Owner,
                AuthPoolCreators = AuthPoolCreators,
                AuthProviders = AuthProviders,
                AuthTraders = AuthTraders,
                Fee = Fee,
                StakingToken = StakingToken
            });
        }
    }
}