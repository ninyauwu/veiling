using System.Text.Json.Serialization;

namespace Veiling.Server.Models
{
    public class Bedrijf
    {
        public int Bedrijfscode { get; set; }
        public string Bedrijfsnaam { get; set; } = string.Empty;
        public int KVKnummer { get; set; }
        
        // Relaties
        [JsonIgnore]
        public ICollection<Gebruiker> Gebruikers { get; set; } = new List<Gebruiker>();
    }
}
