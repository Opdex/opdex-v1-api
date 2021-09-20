namespace Opdex.Platform.Common.Constants.SmartContracts
{
    public class VaultConstants
    {

        public static class StateKeys
        {
            public const string Genesis = "VA";
            public const string VestingDuration = "VB";
            public const string Token = "VC";
            public const string Owner = "VD";
            public const string PendingOwner = "VE";
            public const string TotalSupply = "VF";
            public const string Certificates = "VG";
        }

        public static class Properties
        {
            // Insert properties as we would need them for a local call
            // (e.g. get_PropertyName)
        }

        public static class Methods
        {
            public const string CreateCertificate = "CreateCertificate";
            public const string RedeemCertificates = "RedeemCertificates";
            public const string RevokeCertificates = "RevokeCertificates";
            public const string SetPendingOwnership = "SetPendingOwnership";
            public const string ClaimPendingOwnership = "ClaimPendingOwnership";
            public const string GetCertificates = "GetCertificates";
        }
    }
}
