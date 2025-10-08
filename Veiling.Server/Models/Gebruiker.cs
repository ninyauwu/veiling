namespace Veiling.Server.Models
{
    public class Gebruiker
    {
        public int GebruikerId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? PhoneNumber { get; set; }
        public string EmailAddress { get; set; } = string.Empty;
        public bool Bedrijfsbeheerder { get; set; }
        public bool Geverifieerd { get; set; }
        
        // Relaties
        public ICollection<Bod> Boden { get; set; } = new List<Bod>();
        public ICollection<Kavel> Kavels { get; set; } = new List<Kavel>();
    }
}