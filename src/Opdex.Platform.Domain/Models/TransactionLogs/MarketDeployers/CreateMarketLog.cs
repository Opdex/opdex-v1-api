using System;
using Newtonsoft.Json;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Domain.Models.TransactionLogs.MarketDeployers
{
    public class CreateMarketLog : TransactionLog
    {
        public CreateMarketLog(dynamic log, Address address, int sortOrder)
            : base(TransactionLogType.CreateMarketLog, address, sortOrder)
        {
            Address market = (string)log?.market;
            Address owner = (string)log?.owner;
            Address router = (string)log?.router;
            bool authPoolCreators = log?.authPoolCreators;
            bool authProviders = log?.authProviders;
            bool authTraders = log?.authTraders;
            uint transactionFee = log?.transactionFee;
            Address stakingToken = (string)log?.stakingToken;
            bool enableMarketFee = log?.marketFeeEnabled;

            if (market == Address.Empty)
            {
                throw new ArgumentNullException(nameof(market), "Market address must be set.");
            }

            if (owner == Address.Empty)
            {
                throw new ArgumentNullException(nameof(owner), "Owner address must be set.");
            }

            if (router == Address.Empty)
            {
                throw new ArgumentNullException(nameof(router), "Router address must be set.");
            }

            if (transactionFee > 10)
            {
                throw new ArgumentOutOfRangeException(nameof(transactionFee), "Transaction fee must be between 0-10 inclusive.");
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

        public CreateMarketLog(ulong id, ulong transactionId, Address address, int sortOrder, string details)
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

        public Address Market { get; }
        public Address Owner { get; }
        public Address Router { get; }
        public bool AuthPoolCreators { get; }
        public bool AuthProviders { get; }
        public bool AuthTraders { get; }
        public uint TransactionFee { get; }
        public Address StakingToken { get; }
        public bool EnableMarketFee { get; }

        public struct LogDetails
        {
            public Address Market { get; set; }
            public Address Owner { get; set; }
            public Address Router { get; set; }
            public bool AuthPoolCreators { get; set; }
            public bool AuthProviders { get; set; }
            public bool AuthTraders { get; set; }
            public uint TransactionFee { get; set; }
            public Address StakingToken { get; set; }
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
