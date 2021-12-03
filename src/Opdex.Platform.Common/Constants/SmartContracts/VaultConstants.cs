namespace Opdex.Platform.Common.Constants.SmartContracts;

public static class VaultConstants
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
        public const string Genesis = nameof(StateKeys.Genesis);
        public const string VestingDuration = nameof(StateKeys.VestingDuration);
        public const string Token = nameof(StateKeys.Token);
        public const string Owner = nameof(StateKeys.Owner);
        public const string PendingOwner = nameof(StateKeys.PendingOwner);
        public const string TotalSupply = nameof(StateKeys.TotalSupply);
        public const string Certificates = nameof(StateKeys.Certificates);
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