namespace Veiling.Server.Models
{
    public class Gebruiker
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? PhoneNumber { get; set; }
        public string EmailAddress { get; set; } = string.Empty;
        public bool Bedrijfsbeheerder { get; set; }
        public bool Geverifieerd { get; set; }
        
        // Foreign key naar Bedrijf 
        public int? BedrijfId { get; set; }
    
        // Navigation property: verwijzing naar het bedrijf
        public Bedrijf? Bedrijf { get; set; }
        
        // Relaties
        public ICollection<Bod> Boden { get; set; } = new List<Bod>();
    }
}