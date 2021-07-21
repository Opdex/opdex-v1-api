namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Markets
{
    public class CreateLiquidityPoolEventDto : TransactionEventDto
    {
        public string Token { get; set; }
        public string LiquidityPool { get; set; }
        public override TransactionEventType EventType => TransactionEventType.CreateLiquidityPoolEvent;
    }
}
