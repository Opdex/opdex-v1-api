namespace Opdex.Platform.Domain.Models.Markets
{
    public enum Permissions : byte
    {
        Unknown = 0,
        CreatePool = 1,
        Trade = 2,
        Provide = 3,
        SetPermissions = 4
    }
}