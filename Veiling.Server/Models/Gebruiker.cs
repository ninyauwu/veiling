using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace Veiling.Server.Models
{
    public class Gebruiker : IdentityUser
    {
        public string Name { get; set; } = string.Empty;
        public bool Bedrijfsbeheerder { get; set; }
        public bool Geverifieerd { get; set; }
        
        // Foreign key naar Bedrijf 
        public int? BedrijfId { get; set; }
    
        // Navigation property: verwijzing naar het bedrijf
        public Bedrijf? Bedrijf { get; set; }
        
        // Relaties
        [JsonIgnore]
        public ICollection<Bod> Boden { get; set; } = new List<Bod>();
    }
}
