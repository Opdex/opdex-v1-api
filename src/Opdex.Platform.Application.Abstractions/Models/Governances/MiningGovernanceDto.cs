using Opdex.Platform.Application.Abstractions.Models.TokenDtos;

namespace Opdex.Platform.Application.Abstractions.Models.Governances
{
    public class MiningGovernanceDto
    {
        public string Address { get; set; }
        public ulong NominationPeriodEnd { get; set; }
        public ulong MiningDuration { get; set; }
        public uint MiningPoolsFunded { get; set; }
        public string MiningPoolReward { get; set; }
        public TokenDto MinedToken { get; set; }
    }
}
