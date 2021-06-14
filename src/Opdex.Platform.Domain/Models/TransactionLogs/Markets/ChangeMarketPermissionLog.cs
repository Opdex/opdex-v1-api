using System;
using Newtonsoft.Json;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Markets;

namespace Opdex.Platform.Domain.Models.TransactionLogs.Markets
{
    public class ChangeMarketPermissionLog : TransactionLog
    {
        public ChangeMarketPermissionLog(dynamic log, string address, int sortOrder)
            : base(TransactionLogType.ChangeMarketPermissionLog, address, sortOrder)
        {
            string fromAddress = log?.address;
            Permissions permission = (Permissions)log?.permission;
            bool isAuthorized = log?.isAuthorized;

            if (!fromAddress.HasValue())
            {
                throw new ArgumentNullException(nameof(fromAddress), "Address must be set.");
            }

            Address = fromAddress;
            Permission = permission;
            IsAuthorized = isAuthorized;
        }

        public ChangeMarketPermissionLog(long id, long transactionId, string address, int sortOrder, string details)
            : base(TransactionLogType.ChangeMarketPermissionLog, id, transactionId, address, sortOrder)
        {
            var logDetails = DeserializeLogDetails(details);
            Address = logDetails.Address;
            Permission = logDetails.Permission;
            IsAuthorized = logDetails.IsAuthorized;
        }

        public string Address { get; }
        public Permissions Permission { get; }
        public bool IsAuthorized { get; }

        public struct LogDetails
        {
            public string Address { get; set; }
            public Permissions Permission { get; set; }
            public bool IsAuthorized { get; set; }
        }

        private static LogDetails DeserializeLogDetails(string details)
        {
            return JsonConvert.DeserializeObject<LogDetails>(details);
        }

        public override string SerializeLogDetails()
        {
            return JsonConvert.SerializeObject(new LogDetails
            {
                Address = Address,
                Permission = Permission,
                IsAuthorized = IsAuthorized
            });
        }
    }
}