namespace Opdex.Platform.Domain.Models
{
    public class Deployer
    {
        public Deployer(long id, string address)
        {
            Id = id;
            Address = address;
        }
        
        public long Id { get; }
        public string Address { get; }
    }
}