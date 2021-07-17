namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Governances
{
    public class NominationEventDto : TransactionEventDto
    {
        public string StakingPool { get; set; }
        public string MiningPool { get; set; }
        public string Weight { get; set; }
    }
}
