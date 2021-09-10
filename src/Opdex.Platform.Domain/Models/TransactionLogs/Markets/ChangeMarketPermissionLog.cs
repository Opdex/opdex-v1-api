using System;
using Newtonsoft.Json;
using Opdex.Platform.Common;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Markets;

namespace Opdex.Platform.Domain.Models.TransactionLogs.Markets
{
    public class ChangeMarketPermissionLog : TransactionLog
    {
        public ChangeMarketPermissionLog(dynamic log, Address address, int sortOrder)
            : base(TransactionLogType.ChangeMarketPermissionLog, address, sortOrder)
        {
            Address fromAddress = log?.address;
            Permissions permission = (Permissions)log?.permission;
            bool isAuthorized = log?.isAuthorized;

            if (fromAddress == Address.Empty)
            {
                throw new ArgumentNullException(nameof(fromAddress), "Address must be set.");
            }

            Address = fromAddress;
            Permission = permission;
            IsAuthorized = isAuthorized;
        }

        public ChangeMarketPermissionLog(long id, long transactionId, Address address, int sortOrder, string details)
            : base(TransactionLogType.ChangeMarketPermissionLog, id, transactionId, address, sortOrder)
        {
            var logDetails = DeserializeLogDetails(details);
            Address = logDetails.Address;
            Permission = logDetails.Permission;
            IsAuthorized = logDetails.IsAuthorized;
        }

        public Address Address { get; }
        public Permissions Permission { get; }
        public bool IsAuthorized { get; }

        public struct LogDetails
        {
            public Address Address { get; set; }
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
