namespace Opdex.Platform.WebApi.Models
{
    public interface IApplicationContext
    {
        string Market { get; }
        string Wallet { get; }
    }
}
