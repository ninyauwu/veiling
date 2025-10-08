namespace Veiling.Server.Models
{
    public class Veilingmeester
    {
        public int Id { get; set; }
        public int AantalVeilingenBeheerd { get; set; }
        
        public ICollection<VeilingItem> Veilingen { get; set; } = new List<VeilingItem>();  // 👈 Update!
    }
}