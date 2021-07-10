namespace Opdex.Platform.Application.Abstractions.Models.Vaults
{
    public class VaultDto
    {
        public string Address { get; set; }
        public string Owner { get; set; }
        public ulong Genesis { get; set; }
        public string TokensLocked { get; set; }
        public string TokensUnassigned { get; set; }
        public string LockedToken { get; set; }
    }
}
