namespace Veiling.Server.Models
{
    public class Veilingmeester
    {
        public int Id { get; set; }
        public int AantalVeilingenBeheerd { get; set; }
        
        // Foreign key naar Gebruiker
        public int GebruikerId { get; set; }
    
        // Navigation property
        public Gebruiker Gebruiker { get; set; }
        
        public ICollection<VeilingItem> Veilingen { get; set; } = new List<VeilingItem>();  
    }
}