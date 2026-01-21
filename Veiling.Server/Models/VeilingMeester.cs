namespace Veiling.Server.Models
{
    public class Veilingmeester
    {
        public int Id { get; set; }
        public int AantalVeilingenBeheerd { get; set; }
        
        public string GebruikerId { get; set; }
        public Gebruiker? Gebruiker { get; set; }
        
        public ICollection<Veiling> Veilingen { get; set; } = new List<Veiling>();  
    }
}