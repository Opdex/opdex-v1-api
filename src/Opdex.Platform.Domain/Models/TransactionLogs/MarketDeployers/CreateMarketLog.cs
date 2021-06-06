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
            string router = log?.router;
            bool authPoolCreators = log?.authPoolCreators;
            bool authProviders = log?.authProviders;
            bool authTraders = log?.authTraders;
            uint transactionFee = log?.transactionFee;
            string stakingToken = log?.stakingToken;
            bool enableMarketFee = log?.marketFeeEnabled;

            if (!market.HasValue())
            {
                throw new ArgumentNullException(nameof(market));
            }
            
            if (!owner.HasValue())
            {
                throw new ArgumentNullException(nameof(owner));
            }
            
            if (!router.HasValue())
            {
                throw new ArgumentNullException(nameof(router));
            }

            Market = market;
            Owner = owner;
            Router = router;
            AuthPoolCreators = authPoolCreators;
            AuthProviders = authProviders;
            AuthTraders = authTraders;
            TransactionFee = transactionFee;
            StakingToken = stakingToken;
            EnableMarketFee = enableMarketFee;
        }

        public CreateMarketLog(long id, long transactionId, string address, int sortOrder, string details)
            : base(TransactionLogType.CreateMarketLog, id, transactionId, address, sortOrder)
        {
            var logDetails = DeserializeLogDetails(details);
            Market = logDetails.Market;
            Owner = logDetails.Owner;
            Router = logDetails.Router;
            AuthPoolCreators = logDetails.AuthPoolCreators;
            AuthProviders = logDetails.AuthProviders;
            AuthTraders = logDetails.AuthTraders;
            TransactionFee = logDetails.TransactionFee;
            StakingToken = logDetails.StakingToken;
            EnableMarketFee = logDetails.EnableMarketFee;
        }
        
        public string Market { get; }
        public string Owner { get; }
        public string Router { get; }
        public bool AuthPoolCreators { get; }
        public bool AuthProviders { get; }
        public bool AuthTraders { get; }
        public uint TransactionFee { get; }
        public string StakingToken { get; }
        public bool EnableMarketFee { get; }

        public struct LogDetails
        {
            public string Market { get; set; }
            public string Owner { get; set; }
            public string Router { get; set; }
            public bool AuthPoolCreators { get; set; }
            public bool AuthProviders { get; set; }
            public bool AuthTraders { get; set; }
            public uint TransactionFee { get; set; }
            public string StakingToken { get; set; }
            public bool EnableMarketFee { get; set; }
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
                Router = Router,
                AuthPoolCreators = AuthPoolCreators,
                AuthProviders = AuthProviders,
                AuthTraders = AuthTraders,
                TransactionFee = TransactionFee,
                StakingToken = StakingToken,
                EnableMarketFee = EnableMarketFee
            });
        }
    }
}