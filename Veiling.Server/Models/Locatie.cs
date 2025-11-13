namespace Veiling.Server.Models
{
    public class Locatie
    {
        public int Id { get; set; }
        public string Naam { get; set; } = string.Empty; // "Amsterdam", "Rotterdam", "Delft"
        public int KlokId { get; set; } 
        public bool Actief { get; set; }
        
        // Relaties
        public ICollection<Veiling> Veilingen { get; set; } = new List<Veiling>();
    }
}